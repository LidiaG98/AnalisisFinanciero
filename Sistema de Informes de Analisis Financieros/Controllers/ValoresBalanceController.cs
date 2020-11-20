using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using OfficeOpenXml;
using Sistema_de_Informes_de_Analisis_Financieros.Models;
using Sistema_de_Informes_de_Analisis_Financieros.ViewModels;

namespace Sistema_de_Informes_de_Analisis_Financieros.Controllers
{
    public class ValoresBalanceController : Controller
    {
        private readonly ProyAnfContext _context;
        //Declaracion de constantes
        const int COD_FORMATO_INVALIDO = 1; //Cuando las ctas de total en el balance no tienen el nombre estandar
        const int COD_BALANCE_DESCUADRADO = 2; //Cuando el balance está descuadrado
        const int COD_VALORES_EXITO = 3; //Cuando todos los valores se subieron con éxito

        public ValoresBalanceController(ProyAnfContext context)
        {
            _context = context;
        }

        public async Task<string> GuardarBalance(int IdEmpresa, SubirBalance subirBalance, IFormFile files)
        {

            List<BalanceViewModel> listFilasBalance = new List<BalanceViewModel>();
            List<BalanceViewModel> listFilasBalance2 = new List<BalanceViewModel>();
            string mensaje = "Archivo subido con éxito.";            

            if (files == null || files.Length <= 0)
            {
                return "El archivo subido es inválido, intentelo de nuevo.";
            }

            if (!(Path.GetExtension(files.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase)))
            {
                return "Solo se aceptan archivos de tipo Excel con extensión .xlsx";
            }
            //Verificar que la empresa tiene un catalago subido
            var catalogo = _context.Catalogodecuenta.Count(a => a.Idempresa == IdEmpresa);
            if (catalogo > 0)
            {
                foreach (var anio in subirBalance.anios)
                {
                    /*Llamar método para obtener la letra de columna y número de fila de las celdas*/
                    IEnumerable<string> s = SplitAlpha(subirBalance.celdaCuenta);
                    int numCeldaCuenta = int.Parse(s.Last()); //Obtengo # de fila de las cuentas

                    string celValA1 = anio.celdaAnio;

                    s = SplitAlpha(celValA1);
                    int numCeldaAnio = int.Parse(s.Last()); //Obtengo # de fila de los valores
                    if (!(numCeldaAnio == numCeldaCuenta))
                    {
                        return "Los nombres de cuenta y los valores de las mismas deben estar en la misma fila";
                    }

                    //Verificando que no existan datos para ese año
                    if (!(_context.Valoresdebalance.Any(a => a.Anio == anio.anio)))
                    {
                        listFilasBalance = await LeerExcel(files, subirBalance.hoja, subirBalance.celdaCuenta, anio.celdaAnio, anio.anio);

                        if ((await VerificarBalance(IdEmpresa, listFilasBalance)) == COD_BALANCE_DESCUADRADO)
                        {
                            return "Balance para el año " + subirBalance.anios[0].anio + "descuadrado";
                        }
                        if ((await VerificarBalance(IdEmpresa, listFilasBalance)) == COD_FORMATO_INVALIDO)
                        {
                            return "El archivo no presenta los nombres de Totales en formato estándar";
                        }
                    }
                    else
                    {
                        mensaje = "Ya existen datos para el año " + anio.anio;
                    }
                }
            }
            else
            {
                return "No se ha subido ningún catalogo de cuenta.";
            }
            return mensaje;
        }

        public static IEnumerable<string> SplitAlpha(string input)
        {
            var words = new List<string> { string.Empty };
            for (var i = 0; i < input.Length; i++)
            {
                words[words.Count - 1] += input[i];
                if (i + 1 < input.Length && char.IsLetter(input[i]) != char.IsLetter(input[i + 1]))
                {
                    words.Add(string.Empty);
                }
            }
            return words;
        }

        /*VerificarBalance: verifica que el balance cuadre e inserta los valores en la base*/
        public async Task<int> VerificarBalance(int IdEmpresa, List<BalanceViewModel> balanceV)
        {
            int a1 = 0;
            int resultado = COD_BALANCE_DESCUADRADO; //Inicializo asumiendo que esta descuadrado

            //Obtengo la lista de cuentas en catalogo, excluyendo las cuentas de total que se agregaron en la creación cod=0 y hago una lista personalizada
            var cuentasCatalogo = (from cuenta in _context.Catalogodecuenta
                                   where cuenta.Idempresa == IdEmpresa
                                   && cuenta.Codcuentacatalogo != "D"
                                   && cuenta.Codcuentacatalogo != "0"
                                   select new
                                   {
                                       nomCuenta = cuenta.IdcuentaNavigation.Nomcuenta,
                                       tipoCuenta = cuenta.IdcuentaNavigation.IdtipocuentaNavigation.Nomtipocuenta,
                                       idCuenta = cuenta.IdcuentaNavigation.Idcuenta,
                                       cuentaEstandar = cuenta.nomCuentaE.nomCuentaE,
                                       codCuentaCatalogo = cuenta.Codcuentacatalogo
                                   }).ToList();
            //Recojo las listas del balance que correspondan a los 3 totales principales
            var ctaTotalActivos = balanceV.Where(x => x.nomCuenta.Equals("TOTAL ACTIVOS", StringComparison.OrdinalIgnoreCase)
            || x.nomCuenta.Equals("ACTIVO TOTAL", StringComparison.OrdinalIgnoreCase) || x.nomCuenta.Equals("TOTAL ACTIVO", StringComparison.OrdinalIgnoreCase)
            || x.nomCuenta.Equals("ACTIVOS TOTALES", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

            var ctaTotalPasivo = balanceV.Where(x => x.nomCuenta.Equals("TOTAL PASIVO", StringComparison.OrdinalIgnoreCase)
            || x.nomCuenta.Equals("PASIVO TOTAL", StringComparison.OrdinalIgnoreCase) || x.nomCuenta.Equals("TOTAL PASIVOS", StringComparison.OrdinalIgnoreCase)
            || x.nomCuenta.Equals("PASIVOS TOTALES", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

            var ctaTotalPatrimonio = balanceV.Where(x => x.nomCuenta.Contains("TOTAL CAPITAL SOCIAL", StringComparison.OrdinalIgnoreCase)
            || x.nomCuenta.Contains("TOTAL PATRIMONIO", StringComparison.OrdinalIgnoreCase) || x.nomCuenta.Contains("PATRIMONIO TOTAL", StringComparison.OrdinalIgnoreCase)
            || x.nomCuenta.Contains("PASIVOS TOTALES", StringComparison.OrdinalIgnoreCase) || x.nomCuenta.Contains("TOTAL CAPITAL CONTABLE", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

            if (ctaTotalActivos == null || ctaTotalPasivo == null || ctaTotalPatrimonio == null)
            {
                resultado = COD_FORMATO_INVALIDO; //Si alguna lista esta vacia es porque no se encontró el nombre estándar
            }

            //Almaceno su valor
            double valTotalActivos = ctaTotalActivos.valor;
            double valTotalPasivo = ctaTotalPasivo.valor;
            double valTotalPatrimonio = ctaTotalPatrimonio.valor;
            double valTotalPatMasPasivo = valTotalPasivo + valTotalPatrimonio;

            if (valTotalActivos == valTotalPatMasPasivo) //Comparo que cuadre
            {
                foreach (var filaBV in balanceV)
                {
                    a1 = filaBV.anioFila;
                    //Busco la cuenta en el catalogo para obtener su información
                    var infoCuenta = cuentasCatalogo.Find(z => z.nomCuenta.Equals(filaBV.nomCuenta));
                    if (!(infoCuenta == null))
                    {
                        Valoresdebalance vB = new Valoresdebalance
                        {
                            Idempresa = IdEmpresa,
                            Idcuenta = infoCuenta.idCuenta,
                            Valorcuenta = filaBV.valor,
                            Anio = filaBV.anioFila
                        };
                        //Verifico que no exista este registro en la base
                        var vc = _context.Valoresdebalance.Where(x => x.Idcuenta == vB.Idcuenta && x.Idempresa == vB.Idempresa
                             && x.Valorcuenta == vB.Valorcuenta && x.Anio == vB.Anio).ToList();
                        if (vc.Count == 0)
                        {
                            _context.Add(vB);
                        }
                    }

                }
                //Busco las cuentas de total que yo agregué
                var ctasTotales = (from cuenta in _context.Catalogodecuenta
                                   where cuenta.Idempresa == IdEmpresa
                                   && cuenta.Codcuentacatalogo.Equals("0")
                                   select new
                                   {
                                       nomCuenta = cuenta.IdcuentaNavigation.Nomcuenta,
                                       tipoCuenta = cuenta.IdcuentaNavigation.IdtipocuentaNavigation.Nomtipocuenta,
                                       idCuenta = cuenta.IdcuentaNavigation.Idcuenta,
                                       cuentaEstandar = cuenta.nomCuentaE.nomCuentaE,
                                       codCuentaCatalogo = cuenta.Codcuentacatalogo
                                   }).ToList();
                //Lleno las 3 cuentas de total que se que tengo su valor
                foreach (var total in ctasTotales)
                {
                    var vt = _context.Valoresdebalance.Where(x => x.Idcuenta == total.idCuenta && x.Idempresa == IdEmpresa
                             && x.Anio == a1).ToList();
                    if (vt.Count == 0)
                    {
                        string tipo = total.tipoCuenta;
                        switch (tipo)
                        {
                            case "PASIVO":
                                _context.Add(new Valoresdebalance
                                {
                                    Idempresa = IdEmpresa,
                                    Idcuenta = total.idCuenta,
                                    Valorcuenta = valTotalPasivo,
                                    Anio = a1
                                });
                                break;
                            case "PATRIMONIO":
                                _context.Add(new Valoresdebalance
                                {
                                    Idempresa = IdEmpresa,
                                    Idcuenta = total.idCuenta,
                                    Valorcuenta = valTotalPatrimonio,
                                    Anio = a1
                                });
                                break;
                            case "ACTIVO":
                                _context.Add(new Valoresdebalance
                                {
                                    Idempresa = IdEmpresa,
                                    Idcuenta = total.idCuenta,
                                    Valorcuenta = valTotalActivos,
                                    Anio = a1
                                });
                                break;
                            default:
                                break;
                        }
                    }
                }
                await _context.SaveChangesAsync();
                resultado = COD_VALORES_EXITO; //Si todo se inserto, retorno el codigo de éxito
            }
            return resultado; //Si el if de verificar que el balance cuadre da falso, se retorna el valor inicial de resultado (cod descuadrado)
        }

        /*LeerExcel: lee el archivo de excel que sube el usuario y devuelve sus datos en una lista de Balance View Model*/
        public async Task<List<BalanceViewModel>> LeerExcel(IFormFile files, string hoja, string cCuenta, string cValor, int anio)
        {
            List<BalanceViewModel> lstDatos = new List<BalanceViewModel>();
            string celCuenta = cCuenta;
            /*Llamar método para obtener la letra de columna y número de fila de las celdas*/
            IEnumerable<string> s = SplitAlpha(celCuenta);
            int contador = int.Parse(s.Last());

            string celValA1 = cValor;
            string letraC = s.First();

            s = SplitAlpha(celValA1);
            string letraA = s.First();

            using (var stream = new MemoryStream())
            {
                await files.CopyToAsync(stream);

                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[hoja];
                    var rowCount = worksheet.Dimension.Rows;
                    //Verificar que la hoja no esté vacia
                    if (worksheet.Dimension == null)
                    {
                        return lstDatos;
                    }
                    //Obteniendo datos del año 1
                    for (int row = 1; row <= rowCount; row++)
                    {
                        if (!(worksheet.Cells[celCuenta].Value == null) || !(worksheet.Cells[celValA1].Value == null))
                        {
                            if (worksheet.Cells[celValA1].Value == null)
                            {
                                lstDatos.Add(new BalanceViewModel
                                {
                                    nomCuenta = worksheet.Cells[celCuenta].Value.ToString().Trim(),
                                    valor = 0,
                                    anioFila = anio
                                });
                            }
                            else
                            {
                                lstDatos.Add(new BalanceViewModel
                                {
                                    nomCuenta = worksheet.Cells[celCuenta].Value.ToString().Trim(),
                                    valor = double.Parse(worksheet.Cells[celValA1].Value.ToString().Trim()),
                                    anioFila = anio
                                });

                            }
                        }
                        contador++;
                        celCuenta = letraC + contador;
                        celValA1 = letraA + contador;
                    }
                    return lstDatos;
                }
            }
        }

        // GET: ValoresBalance
        public async Task<IActionResult> Index(string mensaje)
        {
            var proyAnfContext = _context.Valoresdebalance.Include(v => v.Id);

            ViewBag.Mensaje = mensaje;

            return View(await proyAnfContext.ToListAsync());
        }
        public async Task<IActionResult> AnalsisHorizontal()
        {
            var user = this.User;
            List<Usuario> u = _context.Users.Include(e => e.Idempresa).Where(e => e.UserName == user.Identity.Name).ToList();
            List<double> Vanios1 = new List<double>();
            List<double> Vanios2 = new List<double>();


            var proyAnfContext = _context.Valoresdebalance.Where(x => x.Idempresa == u[0].Idempresa.Idempresa);
            var catCuent = from e in _context.Catalogodecuenta.Include(r => r.IdcuentaNavigation) select e;
            var balance = from x in _context.Valoresdebalance select x;
            catCuent = catCuent.Where(y => y.Idempresa == u[0].Idempresa.Idempresa).Include(r => r.IdcuentaNavigation);

            //double activoTotal = balance.Where(m => m.Idcuenta == c).FirstOrDefault().Valorcuenta;

            int an = proyAnfContext.FirstOrDefault().Anio;
            int an2 = an - 1;



            var AH1 = proyAnfContext.Where(s => s.Anio == an);
            foreach (var awa in AH1)
            {
                Vanios1.Add(awa.Valorcuenta);
            }
            var AH2 = proyAnfContext.Where(s => s.Anio == an2);
            foreach (var aw in AH2)
            {
                Vanios2.Add(aw.Valorcuenta);
            }
            ViewData["anio1"] = Vanios1;
            //ViewBag.activo = activoTotal;

            ViewData["anio2"] = Vanios2;
            //años 
            ViewData["ani1"] = an;
            ViewData["ani1"] = an2;
            return View(await proyAnfContext.ToListAsync());
        }
        public async Task<IActionResult> AnalsisVertical(string mensaje)
        {
            var user = this.User;
            List<Usuario> u = _context.Users.Include(e => e.Idempresa).Where(e => e.UserName == user.Identity.Name).ToList();
            var catCuent = from e in _context.Catalogodecuenta.Include(r => r.IdcuentaNavigation) select e;
            var pmp = from x in _context.Cuenta select x;
            List<Cuenta> cuentas = pmp.ToList();


            var proyAnfContext = _context.Valoresdebalance.Where(y => y.Idempresa == u[0].Idempresa.Idempresa);

            int an = proyAnfContext.FirstOrDefault().Anio;
            proyAnfContext = _context.Valoresdebalance.Where(y => y.Idempresa == u[0].Idempresa.Idempresa && y.Anio == an);
            catCuent = catCuent.Where(y => y.Idempresa == u[0].Idempresa.Idempresa);
            var cuenta = catCuent.Where(x => x.IdcuentaNavigation.Nomcuenta == "TOTAL ACTIVO").FirstOrDefault();
            double activoTotal = proyAnfContext.Where(x => x.Idcuenta == cuenta.Idcuenta).FirstOrDefault().Valorcuenta;

            ViewBag.activo = activoTotal;
            ViewBag.cuentas = cuentas;

            return View(await proyAnfContext.ToListAsync());
        }

        // GET: ValoresBalance/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var valoresdebalance = await _context.Valoresdebalance
                .Include(v => v.Id)
                .FirstOrDefaultAsync(m => m.Idbalance == id);
            if (valoresdebalance == null)
            {
                return NotFound();
            }

            return View(valoresdebalance);
        }

        // GET: ValoresBalance/Create
        public IActionResult Create()
        {
            ViewData["Idempresa"] = new SelectList(_context.Catalogodecuenta, "Idempresa", "Codcuentacatalogo");
            return View();
        }

        // POST: ValoresBalance/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Idbalance,Idempresa,Idcuenta,Valorcuenta,Anio")] Valoresdebalance valoresdebalance)
        {
            if (ModelState.IsValid)
            {
                _context.Add(valoresdebalance);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Idempresa"] = new SelectList(_context.Catalogodecuenta, "Idempresa", "Codcuentacatalogo", valoresdebalance.Idempresa);
            return View(valoresdebalance);
        }

        // GET: ValoresBalance/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var valoresdebalance = await _context.Valoresdebalance.FindAsync(id);
            if (valoresdebalance == null)
            {
                return NotFound();
            }
            ViewData["Idempresa"] = new SelectList(_context.Catalogodecuenta, "Idempresa", "Codcuentacatalogo", valoresdebalance.Idempresa);
            return View(valoresdebalance);
        }

        // POST: ValoresBalance/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Idbalance,Idempresa,Idcuenta,Valorcuenta,Anio")] Valoresdebalance valoresdebalance)
        {
            if (id != valoresdebalance.Idbalance)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(valoresdebalance);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ValoresdebalanceExists(valoresdebalance.Idbalance))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Idempresa"] = new SelectList(_context.Catalogodecuenta, "Idempresa", "Codcuentacatalogo", valoresdebalance.Idempresa);
            return View(valoresdebalance);
        }

        // GET: ValoresBalance/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var valoresdebalance = await _context.Valoresdebalance
                .Include(v => v.Id)
                .FirstOrDefaultAsync(m => m.Idbalance == id);
            if (valoresdebalance == null)
            {
                return NotFound();
            }

            return View(valoresdebalance);
        }

        // POST: ValoresBalance/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var valoresdebalance = await _context.Valoresdebalance.FindAsync(id);

            _context.Valoresdebalance.Remove(valoresdebalance);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ValoresdebalanceExists(int id)
        {

            return _context.Valoresdebalance.Any(e => e.Idbalance == id);
        }

    }
}
