﻿using System;
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
    public class CatalogoCuentasController : Controller
    {
        private readonly ProyAnfContext _context;
        private CuentasController cuentasController;

        public CatalogoCuentasController(ProyAnfContext context)
        {
            _context = context;
            cuentasController = new CuentasController(context);
        }

        // GET: CatalogoCuentas
        public async Task<IActionResult> Index()
        {
            var proyAnfContext = _context.Catalogodecuenta.Include(c => c.IdcuentaNavigation).Include(c => c.IdempresaNavigation);
            return View(await proyAnfContext.ToListAsync());
        }

        // GET: CatalogoCuentas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var catalogodecuenta = await _context.Catalogodecuenta
                .Include(c => c.IdcuentaNavigation)
                .Include(c => c.IdempresaNavigation)
                .FirstOrDefaultAsync(m => m.Idempresa == id);
            if (catalogodecuenta == null)
            {
                return NotFound();
            }

            return View(catalogodecuenta);
        }

        [HttpPost]
        /*GuardarCuentas: obtiene las cuentas provinientes del archivo de excel, les asigna su tipo 
        de cuenta y arma el catalogo de la empresa en el sistema*/
        public async Task<string> GuardarCuentas(int IdEmpresa, string celdaCod, string celdaNom, IFormFile files)
        {
            int contador = int.Parse(celdaCod.Substring(1, 1));
            if (files == null || files.Length <= 0)
            {
                return "El archivo subido es inválido, intentelo de nuevo.";
            }

            if (!(Path.GetExtension(files.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase)) && !(Path.GetExtension(files.FileName).Equals(".xls", StringComparison.OrdinalIgnoreCase)))
            {
                return "Solo se aceptan archivos de tipo Excel.";
            }
            var listCuentas = new List<CuentaViewModel>();

            using (var stream = new MemoryStream())
            {
                await files.CopyToAsync(stream);

                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;

                    for (int row = 1; row <= rowCount; row++)
                    {
                        if (!(worksheet.Cells[celdaCod].Value == null) || !(worksheet.Cells[celdaNom].Value == null))
                        {
                            listCuentas.Add(new CuentaViewModel
                            {
                                codigo = worksheet.Cells[celdaCod].Value.ToString().Trim(),
                                nombre = worksheet.Cells[celdaNom].Value.ToString().Trim(),
                            });
                            contador++;
                            celdaCod = celdaCod.Substring(0, 1) + contador;
                            celdaNom = celdaNom.Substring(0, 1) + contador;
                        }
                    }
                }
            }
            await cuentasController.InsertarCuentasDeTotal();
            await cuentasController.InsertarNuevasCuentas(listCuentas);
            List<Catalogodecuenta> catalogodecuentas = await ArmarCatalago(IdEmpresa, listCuentas);
            foreach (var catalogo in catalogodecuentas)
            {
                _context.Add(catalogo);
                await _context.SaveChangesAsync();
            }
            return "Archivo subido con éxito.";
        }

        /*ArmarCatalago: Devuelve una lista del catalogo de la empresa armado*/
        public async Task<List<Catalogodecuenta>> ArmarCatalago(int idEmpresa, List<CuentaViewModel> lstCV)
        {
            List<Cuenta> lstCB = await cuentasController.ConsultarCuentas();
            List<Catalogodecuenta> catalogodecuentas = new List<Catalogodecuenta>();
            foreach (var cuentaV in lstCV)
            {
                for (int i = 0; i < lstCB.Count; i++)
                {
                    if (lstCB[i].Nomcuenta.Equals(cuentaV.nombre))
                    {
                        Catalogodecuenta cc = new Catalogodecuenta
                        {
                            Idcuenta = lstCB[i].Idcuenta,
                            Idempresa = idEmpresa,
                            Codcuentacatalogo = cuentaV.codigo
                        };
                        var catalogoBase = _context.Catalogodecuenta.Count(a => a.Idcuenta == cc.Idcuenta && a.Idempresa == cc.Idempresa);
                        if (!(catalogoBase > 0))
                        {
                            catalogodecuentas.Add(cc);
                        }
                        break;
                    }
                }
            }
            catalogodecuentas = catalogodecuentas.Concat(InsertarCuentasDeTotalCatalogo(idEmpresa)).ToList();
            return catalogodecuentas;
        }

        public List<Catalogodecuenta> InsertarCuentasDeTotalCatalogo(int idEmpresa)
        {
            List<Catalogodecuenta> catalogodecuentas = new List<Catalogodecuenta>();
            var cuentasBase = from s in _context.Cuenta select s;
            cuentasBase = cuentasBase.Where(s => s.Nomcuenta.Equals("TOTAL ACTIVOS CORRIENTES") || s.Nomcuenta.Equals("TOTAL ACTIVOS NO CORRIENTES") ||
            s.Nomcuenta.Equals("TOTAL PASIVOS CORRIENTES") || s.Nomcuenta.Equals("TOTAL PASIVOS NO CORRIENTES") || s.Nomcuenta.Equals("TOTAL PATRIMONIO")
            || s.Nomcuenta.Equals("TOTAL ACTIVO") || s.Nomcuenta.Equals("TOTAL PASIVO MAS PATRIMONIO") || s.Nomcuenta.Equals("VENTAS NETAS")
            || s.Nomcuenta.Equals("COSTO DE VENTAS") || s.Nomcuenta.Equals("UTILIDAD BRUTA") || s.Nomcuenta.Equals("GASTOS ADMINISTRATIVOS")
            || s.Nomcuenta.Equals("UTILIDAD OPERATIVA") || s.Nomcuenta.Equals("GASTOS FINANCIEROS") || s.Nomcuenta.Equals("UTILIDAD ANTES DE IMPUESTOS")
            || s.Nomcuenta.Equals("IMPUESTOS") || s.Nomcuenta.Equals("UTILIDAD NETA") || s.Nomcuenta.Equals("PAGO DE DIVIDENDOS") || s.Nomcuenta.Equals("UTILIDADES RETENIDAS"));
            foreach (var cuentaB in cuentasBase)
            {
                Catalogodecuenta cc = new Catalogodecuenta
                {
                    Idcuenta = cuentaB.Idcuenta,
                    Idempresa = idEmpresa,
                    Codcuentacatalogo = "0"//Para identificar las cuentas de totales el codcatalogo = 0
                };
                var catalogoBase = _context.Catalogodecuenta.Count(a => a.Idcuenta == cc.Idcuenta && a.Idempresa == cc.Idempresa);
                if (!(catalogoBase > 0))
                {
                    catalogodecuentas.Add(cc);
                }                
            }
            return catalogodecuentas;
        }

        // GET: CatalogoCuentas/Create
        public IActionResult Create()
        {
            ViewData["Idcuenta"] = new SelectList(_context.Cuenta, "Idcuenta", "Nomcuenta");
            ViewData["Idempresa"] = new SelectList(_context.Empresa, "Idempresa", "Nomempresa");
            return View();
        }

        // POST: CatalogoCuentas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Idempresa,Idcuenta,Codcuentacatalogo")] Catalogodecuenta catalogodecuenta)
        {
            if (ModelState.IsValid)
            {
                _context.Add(catalogodecuenta);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Idcuenta"] = new SelectList(_context.Cuenta, "Idcuenta", "Nomcuenta", catalogodecuenta.Idcuenta);
            ViewData["Idempresa"] = new SelectList(_context.Empresa, "Idempresa", "Nomempresa", catalogodecuenta.Idempresa);
            return View(catalogodecuenta);
        }

        // GET: CatalogoCuentas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var catalogodecuenta = await _context.Catalogodecuenta.FindAsync(id);
            if (catalogodecuenta == null)
            {
                return NotFound();
            }
            ViewData["Idcuenta"] = new SelectList(_context.Cuenta, "Idcuenta", "Nomcuenta", catalogodecuenta.Idcuenta);
            ViewData["Idempresa"] = new SelectList(_context.Empresa, "Idempresa", "Nomempresa", catalogodecuenta.Idempresa);
            return View(catalogodecuenta);
        }

        // POST: CatalogoCuentas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Idempresa,Idcuenta,Codcuentacatalogo")] Catalogodecuenta catalogodecuenta)
        {
            if (id != catalogodecuenta.Idempresa)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(catalogodecuenta);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CatalogodecuentaExists(catalogodecuenta.Idempresa))
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
            ViewData["Idcuenta"] = new SelectList(_context.Cuenta, "Idcuenta", "Nomcuenta", catalogodecuenta.Idcuenta);
            ViewData["Idempresa"] = new SelectList(_context.Empresa, "Idempresa", "Nomempresa", catalogodecuenta.Idempresa);
            return View(catalogodecuenta);
        }

        // GET: CatalogoCuentas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var catalogodecuenta = await _context.Catalogodecuenta
                .Include(c => c.IdcuentaNavigation)
                .Include(c => c.IdempresaNavigation)
                .FirstOrDefaultAsync(m => m.Idempresa == id);
            if (catalogodecuenta == null)
            {
                return NotFound();
            }

            return View(catalogodecuenta);
        }

        // POST: CatalogoCuentas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var catalogodecuenta = await _context.Catalogodecuenta.FindAsync(id);
            _context.Catalogodecuenta.Remove(catalogodecuenta);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CatalogodecuentaExists(int id)
        {
            return _context.Catalogodecuenta.Any(e => e.Idempresa == id);
        }
    }
}
