using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Sistema_de_Informes_de_Analisis_Financieros.Models;
using Sistema_de_Informes_de_Analisis_Financieros.ViewModels;

namespace Sistema_de_Informes_de_Analisis_Financieros.Controllers
{
    public class EstadoRController : Controller
    {
        private readonly ProyAnfContext _context;

        public EstadoRController(ProyAnfContext context)
        {
            _context = context;
        }

        public async Task<string> GuardarEstado(int IdEmpresa, SubirBalance subirBalance, IFormFile files)
        {
            List<BalanceViewModel> lstFilasEstado = new List<BalanceViewModel>();
            List<BalanceViewModel> lstFilasEstado2 = new List<BalanceViewModel>();
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
                if (_context.Valoresestado.Select(t => t.Anio).Distinct().Count() >= 2)
                {
                    return "Capacidad agotada: Ya se han registrado 2 años";
                }
                else
                {
                    //Verificando que no existan datos para ese año
                    if (!(_context.Valoresestado.Any(a => a.Anio == subirBalance.anios[0].anio)))
                    {
                        lstFilasEstado = await LeerExcel(files, subirBalance.hoja, subirBalance.celdaCuenta, subirBalance.anios[0].celdaAnio, subirBalance.anios[0].anio);
                        await VerificarYSubirEstado(IdEmpresa, lstFilasEstado);
                        
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
                            lstFilasEstado2 = await LeerExcel(files, subirBalance.hoja, subirBalance.celdaCuenta, subirBalance.anios[1].celdaAnio, subirBalance.anios[1].anio);
                            await VerificarYSubirEstado(IdEmpresa, lstFilasEstado2);
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

        public async Task VerificarYSubirEstado(int IdEmpresa, List<BalanceViewModel> balanceV)
        {                       
            var cuentasCatalogo = (from cuenta in _context.Catalogodecuenta
                                   where cuenta.Idempresa == IdEmpresa
                                   select new
                                   {
                                       nomCuenta = cuenta.IdcuentaNavigation.Nomcuenta,
                                       tipoCuenta = cuenta.IdcuentaNavigation.IdtipocuentaNavigation.Nomtipocuenta,
                                       idCuenta = cuenta.IdcuentaNavigation.Idcuenta,
                                       codCuentaC = cuenta.Codcuentacatalogo
                                   }).ToList();
            int idDefault = _context.Cuenta.Where(d => d.Nomcuenta.Equals("Default")).FirstOrDefault().Idcuenta;
            foreach (var filaEstado in balanceV)
            {                
                var infoCuenta = cuentasCatalogo.Find(w => w.nomCuenta.Equals(filaEstado.nomCuenta));
                if (!(infoCuenta == null))
                {
                    var verificar = _context.Valoresestado.Where(s => s.Idcuenta == infoCuenta.idCuenta && s.Idempresa == IdEmpresa
                    && s.Valorestado == filaEstado.valor && s.Anio == filaEstado.anioFila && s.Nombrevalore.Equals(filaEstado.nomCuenta)).ToList();
                    if (verificar.Count == 0)
                    {
                        _context.Add(new Valoresestado
                        {
                            Idempresa = IdEmpresa,
                            Idcuenta = infoCuenta.idCuenta,
                            Valorestado = filaEstado.valor,
                            Anio = filaEstado.anioFila,
                            Nombrevalore = filaEstado.nomCuenta
                        });
                    }                    
                }
                else
                {
                    var verificar = _context.Valoresestado.Where(s => s.Idcuenta == idDefault && s.Idempresa == IdEmpresa
                    && s.Valorestado == filaEstado.valor && s.Anio == filaEstado.anioFila && s.Nombrevalore.Equals(filaEstado.nomCuenta)).ToList();
                    if (verificar.Count == 0)
                    {
                        _context.Add(new Valoresestado
                        {
                            Idempresa = IdEmpresa,
                            Idcuenta = idDefault,
                            Valorestado = filaEstado.valor,
                            Anio = filaEstado.anioFila,
                            Nombrevalore = filaEstado.nomCuenta
                        });
                    }                        
                }
            }
            await _context.SaveChangesAsync();            
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

        // GET: EstadoR
        public async Task<IActionResult> Index(string mensaje)
        {
            var proyAnfContext = _context.Valoresestado.Include(v => v.Id);
            ViewBag.Mensaje = mensaje;
            return View(await proyAnfContext.ToListAsync());
        }

        // GET: EstadoR/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var valoresestado = await _context.Valoresestado
                .Include(v => v.Id)
                .FirstOrDefaultAsync(m => m.Idvalore == id);
            if (valoresestado == null)
            {
                return NotFound();
            }

            return View(valoresestado);
        }

        // GET: EstadoR/Create
        public IActionResult Create()
        {
            ViewData["Idempresa"] = new SelectList(_context.Catalogodecuenta, "Idempresa", "Codcuentacatalogo");
            return View();
        }

        // POST: EstadoR/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Idvalore,Idempresa,Idcuenta,Nombrevalore,Valorestado,Anio")] Valoresestado valoresestado)
        {
            if (ModelState.IsValid)
            {
                _context.Add(valoresestado);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Idempresa"] = new SelectList(_context.Catalogodecuenta, "Idempresa", "Codcuentacatalogo", valoresestado.Idempresa);
            return View(valoresestado);
        }

        // GET: EstadoR/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var valoresestado = await _context.Valoresestado.FindAsync(id);
            if (valoresestado == null)
            {
                return NotFound();
            }
            ViewData["Idempresa"] = new SelectList(_context.Catalogodecuenta, "Idempresa", "Codcuentacatalogo", valoresestado.Idempresa);
            return View(valoresestado);
        }

        // POST: EstadoR/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Idvalore,Idempresa,Idcuenta,Nombrevalore,Valorestado,Anio")] Valoresestado valoresestado)
        {
            if (id != valoresestado.Idvalore)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(valoresestado);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ValoresestadoExists(valoresestado.Idvalore))
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
            ViewData["Idempresa"] = new SelectList(_context.Catalogodecuenta, "Idempresa", "Codcuentacatalogo", valoresestado.Idempresa);
            return View(valoresestado);
        }

        // GET: EstadoR/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var valoresestado = await _context.Valoresestado
                .Include(v => v.Id)
                .FirstOrDefaultAsync(m => m.Idvalore == id);
            if (valoresestado == null)
            {
                return NotFound();
            }

            return View(valoresestado);
        }

        // POST: EstadoR/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var valoresestado = await _context.Valoresestado.FindAsync(id);
            _context.Valoresestado.Remove(valoresestado);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ValoresestadoExists(int id)
        {
            return _context.Valoresestado.Any(e => e.Idvalore == id);
        }
    }
}
