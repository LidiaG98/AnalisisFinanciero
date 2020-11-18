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
                if(_context.Valoresdebalance.Select(t => t.Anio).Distinct().Count() >= 2)
                {
                    return "Capacidad agotada: Ya se han registrado 2 años";
                }
                else
                {
                    //Verificando que no existan datos para ese año
                    if (!(_context.Valoresdebalance.Any(a => a.Anio == subirBalance.anios[0].anio)))
                    {
                        listFilasBalance = await LeerExcel(files, subirBalance.hoja, subirBalance.celdaCuenta, subirBalance.anios[0].celdaAnio, subirBalance.anios[0].anio);
                        if (!(await VerificarBalance(IdEmpresa, listFilasBalance)))
                        {
                            return "Balance para el año " +subirBalance.anios[0].anio+ "descuadrado";
                        }
                    }
                    else
                    {
                        mensaje = "Ya existen datos para el año " + subirBalance.anios[0].anio;
                    }
                    //Verificando si existe año 2 y obteniendo sus datos
                    if (subirBalance.anios.Count == 2)
                    {
                        if (!(_context.Valoresdebalance.Any(a => a.Anio == subirBalance.anios[1].anio)))
                        {
                            listFilasBalance2 = await LeerExcel(files, subirBalance.hoja, subirBalance.celdaCuenta, subirBalance.anios[1].celdaAnio, subirBalance.anios[1].anio);
                            if (!(await VerificarBalance(IdEmpresa, listFilasBalance2)))
                            {
                                return "Balance para el año " + subirBalance.anios[1].anio + "descuadrado";
                            }
                        }
                        else { mensaje = "Ya existen datos para el año " + subirBalance.anios[1].anio; }
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
        public async Task<bool> VerificarBalance(int IdEmpresa, List<BalanceViewModel> balanceV)
        {
            double tActivo = 0, tPmasPat = 0, tAC = 0, tANC = 0, tPC = 0, tPNC = 0, tPat = 0;
            string nomTipoCuenta = "";
            int a1 = 0;
            double maxVal = balanceV.Max(x => x.valor);            
            //var totales = balanceV.Where(x => x.valor == maxVal);
            bool resultado = true;
            var cuentasCatalogo = (from cuenta in _context.Catalogodecuenta
                                   where cuenta.Idempresa == IdEmpresa
                                   select new
                                   {
                                       nomCuenta = cuenta.IdcuentaNavigation.Nomcuenta,
                                       tipoCuenta = cuenta.IdcuentaNavigation.IdtipocuentaNavigation.Nomtipocuenta,
                                       idCuenta = cuenta.IdcuentaNavigation.Idcuenta
                                   }).ToList();
            foreach (var filaBV in balanceV)
            {
                for (int i = 0; i < cuentasCatalogo.Count(); i++)
                {
                    if (filaBV.nomCuenta.Equals(cuentasCatalogo[i].nomCuenta))
                    {
                        if (!(filaBV.nomCuenta.Contains("TOTAL")) && !(filaBV.nomCuenta.Contains("ACTIVO"))
                            && !(filaBV.nomCuenta.Contains("PATRIMONIO")) && !(filaBV.nomCuenta.Contains("PASIVO CORRIENTE"))
                            && !(filaBV.nomCuenta.Contains("PASIVO NO CORRIENTE")))
                        {
                            nomTipoCuenta = cuentasCatalogo[i].tipoCuenta;
                            switch (nomTipoCuenta)
                            {
                                case "ACTIVO CORRIENTE":
                                    tActivo += filaBV.valor;
                                    tAC += filaBV.valor;
                                    break;
                                case "ACTIVO NO CORRIENTE":
                                    tActivo += filaBV.valor;
                                    tANC += filaBV.valor;
                                    break;
                                case "PASIVO CORRIENTE":
                                    tPmasPat += filaBV.valor;
                                    tPC += filaBV.valor;
                                    break;
                                case "PASIVO NO CORRIENTE":
                                    tPmasPat += filaBV.valor;
                                    tPNC += filaBV.valor;
                                    break;
                                case "PATRIMONIO":
                                    tPmasPat += filaBV.valor;
                                    tPat += filaBV.valor;
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                    }
                }
            }
            //if (maxVal == tPmasPat && maxVal == tActivo)
            //{
                foreach (var filaBV in balanceV)
                {
                    a1 = filaBV.anioFila;
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
                        var vc = _context.Valoresdebalance.Where(x => x.Idcuenta == vB.Idcuenta && x.Idempresa == vB.Idempresa
                             && x.Valorcuenta == vB.Valorcuenta && x.Anio == vB.Anio).ToList();
                        if (vc.Count == 0)
                        {
                            _context.Add(vB);                            
                        }
                    }
                    
                }
                var totales = cuentasCatalogo.Where(z => z.nomCuenta.Contains("TOTAL")).ToList();
                foreach (var total in totales)
                {
                    var vt = _context.Valoresdebalance.Where(x => x.Idcuenta == total.idCuenta && x.Idempresa == IdEmpresa
                             && x.Anio == a1).ToList();
                    if (vt.Count == 0)
                    {
                        string tipo = total.tipoCuenta;
                        switch (tipo)
                        {
                            case "ACTIVO CORRIENTE":
                                _context.Add(new Valoresdebalance
                                {
                                    Idempresa = IdEmpresa,
                                    Idcuenta = total.idCuenta,
                                    Valorcuenta = tAC,
                                    Anio = a1
                                });
                                break;
                            case "ACTIVO NO CORRIENTE":
                                _context.Add(new Valoresdebalance
                                {
                                    Idempresa = IdEmpresa,
                                    Idcuenta = total.idCuenta,
                                    Valorcuenta = tANC,
                                    Anio = a1
                                });
                                break;
                            case "PASIVO CORRIENTE":
                                _context.Add(new Valoresdebalance
                                {
                                    Idempresa = IdEmpresa,
                                    Idcuenta = total.idCuenta,
                                    Valorcuenta = tPC,
                                    Anio = a1
                                });
                                break;
                            case "PASIVO NO CORRIENTE":
                                _context.Add(new Valoresdebalance
                                {
                                    Idempresa = IdEmpresa,
                                    Idcuenta = total.idCuenta,
                                    Valorcuenta = tPNC,
                                    Anio = a1
                                });
                                break;
                            case "PASIVO":
                                _context.Add(new Valoresdebalance
                                {
                                    Idempresa = IdEmpresa,
                                    Idcuenta = total.idCuenta,
                                    Valorcuenta = tPmasPat,
                                    Anio = a1
                                });
                                break;
                            case "PATRIMONIO":
                                _context.Add(new Valoresdebalance
                                {
                                    Idempresa = IdEmpresa,
                                    Idcuenta = total.idCuenta,
                                    Valorcuenta = tPat,
                                    Anio = a1
                                });
                                break;
                            case "ACTIVO":
                                _context.Add(new Valoresdebalance
                                {
                                    Idempresa = IdEmpresa,
                                    Idcuenta = total.idCuenta,
                                    Valorcuenta = tActivo,
                                    Anio = a1
                                });
                                break;
                            default:
                                break;
                        } 
                    }
                }
                await _context.SaveChangesAsync();
                resultado = true;
            //}
            return resultado;
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
            
            double activoTotal =0 /*balance.Where(m => m.Idcuenta == c).FirstOrDefault().Valorcuenta*/;

            int an = proyAnfContext.FirstOrDefault().Anio;
            int an2 = an - 1;



            var AH1 = proyAnfContext.Where(s => s.Anio == an);
            foreach( var awa in AH1)
            {
                Vanios1.Add(awa.Valorcuenta);
            }
            var AH2 = proyAnfContext.Where(s => s.Anio == an2);
            foreach (var aw in AH2)
            {
                Vanios2.Add(aw.Valorcuenta);
            }
            ViewData["anio1"] = Vanios1;
            ViewBag.activo = activoTotal;

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
