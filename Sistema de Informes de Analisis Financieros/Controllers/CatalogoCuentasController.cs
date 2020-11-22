using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Sistema_de_Informes_de_Analisis_Financieros.Models;
using Sistema_de_Informes_de_Analisis_Financieros.ViewModels;

namespace Sistema_de_Informes_de_Analisis_Financieros.Controllers
{
    [Authorize]    
    public class CatalogoCuentasController : Controller
    {
        private readonly ProyAnfContext _context;
        private CuentasController cuentasController;
        private readonly UserManager<Usuario> userManager;

        public CatalogoCuentasController(ProyAnfContext context, UserManager<Usuario> user)
        {
            _context = context;
            cuentasController = new CuentasController(context);
            this.userManager = user;
        }

        // GET: CatalogoCuentas
        public async Task<IActionResult> Index(int? id)
        {
            var usuario = this.User;
            Usuario u = _context.Users.Include(l => l.Idempresa).Where(l => l.UserName == usuario.Identity.Name).FirstOrDefault();
            List<Catalogodecuenta> proyAnfContext;
            if (await userManager.IsInRoleAsync(u, "Administrator"))
            {
                ViewBag.nomEmpresa = u.Idempresa.Nomempresa;
                ViewBag.idEmpresa = u.Idempresa.Idempresa;
                proyAnfContext = _context.Catalogodecuenta.Include(c => c.IdcuentaNavigation).Include(c => c.IdempresaNavigation).
                    Where(c => c.Idempresa == id).ToList();
                return View(proyAnfContext);
            }
            proyAnfContext = _context.Catalogodecuenta.Include(c => c.IdcuentaNavigation).Include(c => c.IdempresaNavigation)
                .Where(p => p.Idempresa == u.Idempresa.Idempresa).ToList();
            ViewBag.nomEmpresa = u.Idempresa.Nomempresa;
            ViewBag.idEmpresa = u.Idempresa.Idempresa;
            return View(proyAnfContext);
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
        public async Task<string> GuardarCuentas(int IdEmpresa, string celdaCod, string celdaNom, string hoja, IFormFile files)
        {
            int contador = int.Parse(celdaCod.Substring(1, 1));
            if (files == null || files.Length <= 0)
            {
                return "El archivo subido es inválido, intentelo de nuevo.";
            }

            if (!(Path.GetExtension(files.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase)))
            {
                return "Solo se aceptan archivos de tipo Excel.";
            }
            if(_context.Catalogodecuenta.Where(cC => cC.Idempresa == IdEmpresa).Any())
            {
                return "Ya existe un catálogo registrado para esta empresa.";
            }

            var listCuentas = new List<CuentaViewModel>();

            using (var stream = new MemoryStream())
            {
                await files.CopyToAsync(stream);

                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[hoja];
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
                List<Cuenta> ctaBase = _context.Cuenta.Where(cB => cB.Nomcuenta.Equals(cuentaV.nombre)).Include(i =>i.IdtipocuentaNavigation).ToList();
                if (ctaBase.Count > 1)
                {
                    string val = "";
                    string cod = cuentaV.codigo.Replace(".", "");
                    int identificadorCuenta = 0;
                   
                    if (cod.Length > 1) { val = cod.Substring(0, 2); }
                    else { val = cod.Substring(0, 1); }

                    int catalogoBase = 0;
                    Catalogodecuenta cc = new Catalogodecuenta();

                    switch (val)
                    {
                        case "1":
                            identificadorCuenta = ctaBase.Where(cta => cta.IdtipocuentaNavigation.Nomtipocuenta.Equals("ACTIVO")).First().Idcuenta;
                            cc = new Catalogodecuenta
                            {
                                Idcuenta = identificadorCuenta,
                                Idempresa = idEmpresa,
                                Codcuentacatalogo = cuentaV.codigo
                            };
                            catalogoBase = _context.Catalogodecuenta.Count(a => a.Idcuenta == cc.Idcuenta && a.Idempresa == cc.Idempresa);
                            if (!(catalogoBase > 0))
                            {
                                catalogodecuentas.Add(cc);
                            }
                            break;
                        case "11":
                            identificadorCuenta = ctaBase.Where(cta => cta.IdtipocuentaNavigation.Nomtipocuenta.Equals("ACTIVO CORRIENTE")).First().Idcuenta;
                            cc = new Catalogodecuenta
                            {
                                Idcuenta = identificadorCuenta,
                                Idempresa = idEmpresa,
                                Codcuentacatalogo = cuentaV.codigo
                            };
                            catalogoBase = _context.Catalogodecuenta.Count(a => a.Idcuenta == cc.Idcuenta && a.Idempresa == cc.Idempresa);
                            if (!(catalogoBase > 0))
                            {
                                catalogodecuentas.Add(cc);
                            }
                            break;
                        case "12":
                            identificadorCuenta = ctaBase.Where(cta => cta.IdtipocuentaNavigation.Nomtipocuenta.Equals("ACTIVO NO CORRIENTE")).First().Idcuenta;
                            cc = new Catalogodecuenta
                            {
                                Idcuenta = identificadorCuenta,
                                Idempresa = idEmpresa,
                                Codcuentacatalogo = cuentaV.codigo
                            };
                            catalogoBase = _context.Catalogodecuenta.Count(a => a.Idcuenta == cc.Idcuenta && a.Idempresa == cc.Idempresa);
                            if (!(catalogoBase > 0))
                            {
                                catalogodecuentas.Add(cc);
                            }
                            break;
                        case "2":
                            identificadorCuenta = ctaBase.Where(cta => cta.IdtipocuentaNavigation.Nomtipocuenta.Equals("PASIVO")).First().Idcuenta;
                            cc = new Catalogodecuenta
                            {
                                Idcuenta = identificadorCuenta,
                                Idempresa = idEmpresa,
                                Codcuentacatalogo = cuentaV.codigo
                            };
                            catalogoBase = _context.Catalogodecuenta.Count(a => a.Idcuenta == cc.Idcuenta && a.Idempresa == cc.Idempresa);
                            if (!(catalogoBase > 0))
                            {
                                catalogodecuentas.Add(cc);
                            }
                            break;
                        case "21":
                            identificadorCuenta = ctaBase.Where(cta => cta.IdtipocuentaNavigation.Nomtipocuenta.Equals("PASIVO CORRIENTE")).First().Idcuenta;
                            cc = new Catalogodecuenta
                            {
                                Idcuenta = identificadorCuenta,
                                Idempresa = idEmpresa,
                                Codcuentacatalogo = cuentaV.codigo
                            };
                            catalogoBase = _context.Catalogodecuenta.Count(a => a.Idcuenta == cc.Idcuenta && a.Idempresa == cc.Idempresa);
                            if (!(catalogoBase > 0))
                            {
                                catalogodecuentas.Add(cc);
                            }
                            break;
                        case "22":
                            identificadorCuenta = ctaBase.Where(cta => cta.IdtipocuentaNavigation.Nomtipocuenta.Equals("PASIVO NO CORRIENTE")).First().Idcuenta;
                            cc = new Catalogodecuenta
                            {
                                Idcuenta = identificadorCuenta,
                                Idempresa = idEmpresa,
                                Codcuentacatalogo = cuentaV.codigo
                            };
                            catalogoBase = _context.Catalogodecuenta.Count(a => a.Idcuenta == cc.Idcuenta && a.Idempresa == cc.Idempresa);
                            if (!(catalogoBase > 0))
                            {
                                catalogodecuentas.Add(cc);
                            }
                            break;
                        case "3":
                            identificadorCuenta = ctaBase.Where(cta => cta.IdtipocuentaNavigation.Nomtipocuenta.Equals("PATRIMONIO")).First().Idcuenta;
                            cc = new Catalogodecuenta
                            {
                                Idcuenta = identificadorCuenta,
                                Idempresa = idEmpresa,
                                Codcuentacatalogo = cuentaV.codigo
                            };
                            catalogoBase = _context.Catalogodecuenta.Count(a => a.Idcuenta == cc.Idcuenta && a.Idempresa == cc.Idempresa);
                            if (!(catalogoBase > 0))
                            {
                                catalogodecuentas.Add(cc);
                            }
                            break;
                        case "4":
                            identificadorCuenta = ctaBase.Where(cta => cta.IdtipocuentaNavigation.Nomtipocuenta.Equals("GASTOS")).First().Idcuenta;
                            cc = new Catalogodecuenta
                            {
                                Idcuenta = identificadorCuenta,
                                Idempresa = idEmpresa,
                                Codcuentacatalogo = cuentaV.codigo
                            };
                            catalogoBase = _context.Catalogodecuenta.Count(a => a.Idcuenta == cc.Idcuenta && a.Idempresa == cc.Idempresa);
                            if (!(catalogoBase > 0))
                            {
                                catalogodecuentas.Add(cc);
                            }
                            break;
                        case "5":
                            identificadorCuenta = ctaBase.Where(cta => cta.IdtipocuentaNavigation.Nomtipocuenta.Equals("INGRESOS")).First().Idcuenta;
                            cc = new Catalogodecuenta
                            {
                                Idcuenta = identificadorCuenta,
                                Idempresa = idEmpresa,
                                Codcuentacatalogo = cuentaV.codigo
                            };
                            catalogoBase = _context.Catalogodecuenta.Count(a => a.Idcuenta == cc.Idcuenta && a.Idempresa == cc.Idempresa);
                            if (!(catalogoBase > 0))
                            {
                                catalogodecuentas.Add(cc);
                            }
                            break;
                        default:
                            if (cod.StartsWith("3"))
                            {
                                identificadorCuenta = ctaBase.Where(cta => cta.IdtipocuentaNavigation.Nomtipocuenta.Equals("PATRIMONIO")).First().Idcuenta;
                                cc = new Catalogodecuenta
                                {
                                    Idcuenta = identificadorCuenta,
                                    Idempresa = idEmpresa,
                                    Codcuentacatalogo = cuentaV.codigo
                                };
                                catalogoBase = _context.Catalogodecuenta.Count(a => a.Idcuenta == cc.Idcuenta && a.Idempresa == cc.Idempresa);
                                if (!(catalogoBase > 0))
                                {
                                    catalogodecuentas.Add(cc);
                                }
                            }
                            else if (cod.StartsWith("4"))
                            {
                                identificadorCuenta = ctaBase.Where(cta => cta.IdtipocuentaNavigation.Nomtipocuenta.Equals("GASTOS")).First().Idcuenta;
                                cc = new Catalogodecuenta
                                {
                                    Idcuenta = identificadorCuenta,
                                    Idempresa = idEmpresa,
                                    Codcuentacatalogo = cuentaV.codigo
                                };
                                catalogoBase = _context.Catalogodecuenta.Count(a => a.Idcuenta == cc.Idcuenta && a.Idempresa == cc.Idempresa);
                                if (!(catalogoBase > 0))
                                {
                                    catalogodecuentas.Add(cc);
                                }
                            }
                            else if (cod.StartsWith("5"))
                            {
                                identificadorCuenta = ctaBase.Where(cta => cta.IdtipocuentaNavigation.Nomtipocuenta.Equals("INGRESOS")).First().Idcuenta;
                                cc = new Catalogodecuenta
                                {
                                    Idcuenta = identificadorCuenta,
                                    Idempresa = idEmpresa,
                                    Codcuentacatalogo = cuentaV.codigo
                                };
                                catalogoBase = _context.Catalogodecuenta.Count(a => a.Idcuenta == cc.Idcuenta && a.Idempresa == cc.Idempresa);
                                if (!(catalogoBase > 0))
                                {
                                    catalogodecuentas.Add(cc);
                                }
                            }
                            else
                            {
                                identificadorCuenta = ctaBase.Where(cta => cta.IdtipocuentaNavigation.Nomtipocuenta.Equals("OTROS")).First().Idcuenta;
                                cc = new Catalogodecuenta
                                {
                                    Idcuenta = identificadorCuenta,
                                    Idempresa = idEmpresa,
                                    Codcuentacatalogo = cuentaV.codigo
                                };
                                catalogoBase = _context.Catalogodecuenta.Count(a => a.Idcuenta == cc.Idcuenta && a.Idempresa == cc.Idempresa);
                                if (!(catalogoBase > 0))
                                {
                                    catalogodecuentas.Add(cc);
                                }
                            }
                            break;
                    }
                }
                else if (ctaBase.Count == 1)
                {
                    Catalogodecuenta cc = new Catalogodecuenta
                    {
                        Idcuenta = ctaBase.First().Idcuenta,
                        Idempresa = idEmpresa,
                        Codcuentacatalogo = cuentaV.codigo
                    };
                    var catalogoBase = _context.Catalogodecuenta.Count(a => a.Idcuenta == cc.Idcuenta && a.Idempresa == cc.Idempresa);
                    if (!(catalogoBase > 0))
                    {
                        catalogodecuentas.Add(cc);
                    }
                }
                
            }
            int idDefault = _context.Cuenta.Where(d => d.Nomcuenta.Equals("Default")).FirstOrDefault().Idcuenta;
            var cb = _context.Catalogodecuenta.Count(a => a.Idcuenta == idDefault && a.Idempresa == idEmpresa);
            if (!(cb > 0))
            {
                catalogodecuentas.Add(new Catalogodecuenta { 
                    Idcuenta = idDefault,
                    Idempresa = idEmpresa,
                    Codcuentacatalogo = "D"
                });
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
            || s.Nomcuenta.Equals("TOTAL ACTIVO") || s.Nomcuenta.Equals("TOTAL PASIVO MAS PATRIMONIO"));
            foreach (var cuentaB in cuentasBase)
            {
                Catalogodecuenta cc = new Catalogodecuenta
                    {
                        Idcuenta = cuentaB.Idcuenta,
                        Idempresa = idEmpresa,
                        Codcuentacatalogo = "0"//Para identificar las cuentas de totales del balance el codcatalogo = 0
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
