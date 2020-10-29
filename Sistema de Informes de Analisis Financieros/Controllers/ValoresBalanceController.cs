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
    public class ValoresBalanceController : Controller
    {
        private readonly ProyAnfContext _context;

        public ValoresBalanceController(ProyAnfContext context)
        {
            _context = context;
        }

        public async Task<string> GuardarBalance(int IdEmpresa, SubirBalance subirBalance, IFormFile files)
        {            
            string celCuenta = subirBalance.celdaCuenta;
            string celValA1 = subirBalance.anios[0].celdaAnio;
            int contador = int.Parse(subirBalance.celdaCuenta.Substring(1, 1));
            if (files == null || files.Length <= 0)
            {
                return "El archivo subido es inválido, intentelo de nuevo.";
            }

            if (!(Path.GetExtension(files.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase)) && !(Path.GetExtension(files.FileName).Equals(".xls", StringComparison.OrdinalIgnoreCase)))
            {
                return "Solo se aceptan archivos de tipo Excel.";
            }
            var listFilasBalance = new List<BalanceViewModel>();

            using (var stream = new MemoryStream())
            {
                await files.CopyToAsync(stream);

                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[subirBalance.hoja];
                    var rowCount = worksheet.Dimension.Rows;
                    //Obteniendo datos del año 1
                    for (int row = 1; row <= rowCount; row++)
                    {
                        if (!(worksheet.Cells[subirBalance.celdaCuenta].Value == null) || !(worksheet.Cells[subirBalance.anios[0].celdaAnio].Value == null))
                        {
                            listFilasBalance.Add(new BalanceViewModel
                            {
                                nomCuenta = worksheet.Cells[celCuenta].Value.ToString().Trim(),
                                valor = double.Parse(worksheet.Cells[celValA1].Value.ToString().Trim()),
                                anioFila = subirBalance.anios[0].anio
                            });
                            contador++;
                            celCuenta = celCuenta.Substring(0, 1) + contador;
                            celValA1 = celValA1.Substring(0, 1) + contador;
                        }
                    }
                    //Verificando si existe año 2 y obteniendo sus datos
                    if (subirBalance.anios.Count == 2)
                    {
                        string celValA2 = subirBalance.anios[1].celdaAnio;
                        celCuenta = subirBalance.celdaCuenta;
                        contador = int.Parse(subirBalance.celdaCuenta.Substring(1, 1));
                        for (int row = 1; row <= rowCount; row++)
                        {
                            if (!(worksheet.Cells[subirBalance.celdaCuenta].Value == null) || !(worksheet.Cells[subirBalance.anios[1].celdaAnio].Value == null))
                            {
                                listFilasBalance.Add(new BalanceViewModel
                                {
                                    nomCuenta = worksheet.Cells[celCuenta].Value.ToString().Trim(),
                                    valor = double.Parse(worksheet.Cells[celValA1].Value.ToString().Trim()),
                                    anioFila = subirBalance.anios[0].anio
                                });
                                contador++;
                                celCuenta = celCuenta.Substring(0, 1) + contador;
                                celValA2 = celValA2.Substring(0, 1) + contador;
                            }
                        }
                    }
                }
            }            
            return "Archivo subido con éxito.";
        }

        // GET: ValoresBalance
        public async Task<IActionResult> Index()
        {
            var proyAnfContext = _context.Valoresdebalance.Include(v => v.Id);
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
