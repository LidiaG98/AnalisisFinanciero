using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Sistema_de_Informes_de_Analisis_Financieros.Models;
using Sistema_de_Informes_de_Analisis_Financieros.ViewModels;

namespace Sistema_de_Informes_de_Analisis_Financieros.Controllers
{    
    [Authorize]
    public class NomCuentaEsController : Controller
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly ProyAnfContext _context;

        public NomCuentaEsController(ProyAnfContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: NomCuentaEs
        public IActionResult ActualizarCatalogoCuenta()
        {
            var user = this.User;
            List<Usuario> u = _context.Users.Include(e=>e.Idempresa).Where(e=>e.UserName == user.Identity.Name).ToList();
            List<Catalogodecuenta> cc = new List<Catalogodecuenta>();
            cc = _context.Catalogodecuenta.Include(e=>e.IdcuentaNavigation).Where(e=>e.Codcuentacatalogo!="0" && e.Idempresa==u[0].Idempresa.Idempresa && e.Codcuentacatalogo != "D").ToList();
            List<SelectListItem> items = cc.ConvertAll(d =>
            {
                return new SelectListItem()
                {
                    Text = d.IdcuentaNavigation.Nomcuenta,
                    Value = d.Codcuentacatalogo,
                    Selected = false
                };
            });
            ViewData["Idcuenta"] = items;
            return View();
        }

        //POST
        public async Task<IActionResult> ActualizarCCuenta(List<ListCViewModel> listCs)
        {
            var user = this.User;
            List<Usuario> u = _context.Users.Include(e => e.Idempresa).Where(e => e.UserName == user.Identity.Name).ToList();
            List<NomCuentaE> nom;
            List<Catalogodecuenta> cc;

            for (int i =0; i<listCs.Count;i++)
            {
                for(int x=0; x < listCs[i].codCuenta.Count; x++)
                {
                    if(listCs[i].codCuenta[x] != null)
                    {
                        listCs[i].codCuenta[x] = listCs[i].codCuenta[x].Replace(".", "");
                    }
                }
                cc = _context.Catalogodecuenta.Where(e => e.Codcuentacatalogo != "0" && e.Idempresa == u[0].Idempresa.Idempresa && e.Codcuentacatalogo != "D").ToList();
                nom = _context.NomCuentaE.Where(e => e.nomCuentaE == listCs[i].nombre).ToList();

                for (int j = 0; j < cc.Count; j++)
                {
                    switch (listCs[i].nombre)
                    {
                        case "ACTIVOS CORRIENTES":
                            foreach(string cod in listCs[i].codCuenta)
                            {
                                if (cc[j].Codcuentacatalogo.StartsWith(cod))
                                {
                                    cc[j].Codcuentacatalogo = cc[j].Codcuentacatalogo.Replace(".", "");
                                    cc[j].nomCuentaEID = nom[0].nomCuentaEID;
                                    _context.Update(cc[j]);
                                    await _context.SaveChangesAsync();
                                }
                            }
                        break;
                        case "PASIVOS CORRIENTES":
                            foreach (string cod in listCs[i].codCuenta)
                            {
                                if (cc[j].Codcuentacatalogo.StartsWith(cod))
                                {
                                    cc[j].Codcuentacatalogo = cc[j].Codcuentacatalogo.Replace(".", "");
                                    cc[j].nomCuentaEID = nom[0].nomCuentaEID;
                                    _context.Update(cc[j]);
                                    await _context.SaveChangesAsync();
                                }
                            }
                         break;
                         case "INGRESOS":
                            foreach (string cod in listCs[i].codCuenta)
                            {
                                if (cc[j].Codcuentacatalogo.StartsWith(cod))
                                {
                                    cc[j].Codcuentacatalogo = cc[j].Codcuentacatalogo.Replace(".", "");
                                    cc[j].nomCuentaEID = nom[0].nomCuentaEID;
                                    _context.Update(cc[j]);
                                    await _context.SaveChangesAsync();
                                }
                            }
                        break;
                        case "VENTAS":
                            foreach (string cod in listCs[i].codCuenta)
                            {
                                if (cc[j].Codcuentacatalogo.StartsWith(cod))
                                {
                                    if (cc[j].nomCuentaEID == null)
                                    {
                                        cc[j].Codcuentacatalogo = cc[j].Codcuentacatalogo.Replace(".", "");
                                        cc[j].nomCuentaEID = nom[0].nomCuentaEID;
                                        _context.Update(cc[j]);
                                        await _context.SaveChangesAsync();
                                    }
                                }
                            }
                        break;
                        case "INVENTARIO":
                            foreach (string cod in listCs[i].codCuenta)
                            {
                                if (cc[j].Codcuentacatalogo.StartsWith(cod))
                                {
                                    cc[j].Codcuentacatalogo = cc[j].Codcuentacatalogo.Replace(".", "");
                                    cc[j].nomCuentaEID = nom[0].nomCuentaEID;
                                    _context.Update(cc[j]);
                                    await _context.SaveChangesAsync();
                                }
                            }
                        break;
                        case "EFECTIVO + VALORES DE CORTO PLAZO":
                            foreach (string cod in listCs[i].codCuenta)
                            {
                                if (cc[j].Codcuentacatalogo.StartsWith(cod))
                                {
                                    cc[j].Codcuentacatalogo = cc[j].Codcuentacatalogo.Replace(".", "");
                                    cc[j].nomCuentaEID = nom[0].nomCuentaEID;
                                    _context.Update(cc[j]);
                                    await _context.SaveChangesAsync();
                                }
                            }
                        break;
                        case "COSTO DE LAS VENTAS":
                            foreach (string cod in listCs[i].codCuenta)
                            {
                                if (cc[j].Codcuentacatalogo.StartsWith(cod))
                                {
                                    cc[j].Codcuentacatalogo = cc[j].Codcuentacatalogo.Replace(".", "");
                                    cc[j].nomCuentaEID = nom[0].nomCuentaEID;
                                    _context.Update(cc[j]);
                                    await _context.SaveChangesAsync();
                                }
                            }
                         break;
                         case "VENTAS NETAS":
                            foreach (string cod in listCs[i].codCuenta)
                            {
                                if (cc[j].Codcuentacatalogo.StartsWith(cod))
                                {
                                    if (cc[j].nomCuentaEID == null)
                                    {
                                        cc[j].Codcuentacatalogo = cc[j].Codcuentacatalogo.Replace(".", "");
                                        cc[j].nomCuentaEID = nom[0].nomCuentaEID;
                                        _context.Update(cc[j]);
                                        await _context.SaveChangesAsync();
                                    }
                                }
                            }
                          break;
                          case "CUENTAS POR COBRAR COMERCIALES":
                            foreach (string cod in listCs[i].codCuenta)
                            {
                                if (cc[j].Codcuentacatalogo.StartsWith(cod))
                                {
                                    cc[j].Codcuentacatalogo = cc[j].Codcuentacatalogo.Replace(".", "");
                                    cc[j].nomCuentaEID = nom[0].nomCuentaEID;
                                    _context.Update(cc[j]);
                                    await _context.SaveChangesAsync();
                                }
                            }
                          break;
                          case "COMPRAS":
                            foreach (string cod in listCs[i].codCuenta)
                            {
                                if (cc[j].Codcuentacatalogo.StartsWith(cod))
                                {
                                    if (cc[j].nomCuentaEID == null)
                                    {
                                        cc[j].Codcuentacatalogo = cc[j].Codcuentacatalogo.Replace(".", "");
                                        cc[j].nomCuentaEID = nom[0].nomCuentaEID;
                                        _context.Update(cc[j]);
                                        await _context.SaveChangesAsync();
                                    }
                                }
                            }
                          break;
                          case "CUENTAS POR PAGAR COMERCIALES":
                            foreach (string cod in listCs[i].codCuenta)
                            {
                                if (cc[j].Codcuentacatalogo.StartsWith(cod))
                                {
                                    cc[j].Codcuentacatalogo = cc[j].Codcuentacatalogo.Replace(".", "");
                                    cc[j].nomCuentaEID = nom[0].nomCuentaEID;
                                    _context.Update(cc[j]);
                                    await _context.SaveChangesAsync();
                                }
                            }
                          break;
                          case "ACTIVO FIJO NETO":
                            foreach (string cod in listCs[i].codCuenta)
                            {
                                if (cc[j].Codcuentacatalogo.StartsWith(cod))
                                {
                                    cc[j].Codcuentacatalogo = cc[j].Codcuentacatalogo.Replace(".", "");
                                    cc[j].nomCuentaEID = nom[0].nomCuentaEID;
                                    _context.Update(cc[j]);
                                    await _context.SaveChangesAsync();
                                }
                            }
                          break;
                          case "GASTOS FINANCIEROS":
                            foreach (string cod in listCs[i].codCuenta)
                            {
                                if (cc[j].Codcuentacatalogo.StartsWith(cod))
                                {
                                    cc[j].Codcuentacatalogo = cc[j].Codcuentacatalogo.Replace(".", "");
                                    cc[j].nomCuentaEID = nom[0].nomCuentaEID;
                                    _context.Update(cc[j]);
                                    await _context.SaveChangesAsync();
                                }
                            }
                          break;
                          case "INVERSION":
                            foreach (string cod in listCs[i].codCuenta)
                            {
                                if (cc[j].Codcuentacatalogo.StartsWith(cod))
                                {
                                    if (cc[j].nomCuentaEID == null)
                                    {
                                        cc[j].Codcuentacatalogo = cc[j].Codcuentacatalogo.Replace(".", "");
                                        cc[j].nomCuentaEID = nom[0].nomCuentaEID;
                                        _context.Update(cc[j]);
                                        await _context.SaveChangesAsync();
                                    }
                                }
                            }
                          break;
                          case "OTROS INGRESOS":
                            foreach (string cod in listCs[i].codCuenta)
                            {
                                if (cc[j].Codcuentacatalogo.StartsWith(cod))
                                {
                                    if (cc[j].nomCuentaEID == null)
                                    {
                                        cc[j].Codcuentacatalogo = cc[j].Codcuentacatalogo.Replace(".", "");
                                        cc[j].nomCuentaEID = nom[0].nomCuentaEID;
                                        _context.Update(cc[j]);
                                        await _context.SaveChangesAsync();
                                    }
                                }
                            }
                          break;
                          case "IMPUESTOS":
                            foreach (string cod in listCs[i].codCuenta)
                            {
                                if (cc[j].Codcuentacatalogo.StartsWith(cod))
                                {
                                    cc[j].Codcuentacatalogo = cc[j].Codcuentacatalogo.Replace(".", "");
                                    cc[j].nomCuentaEID = nom[0].nomCuentaEID;
                                    _context.Update(cc[j]);
                                    await _context.SaveChangesAsync();
                                }
                            }
                          break;
                          case "INTERESES":
                            foreach (string cod in listCs[i].codCuenta)
                            {
                                if (cc[j].Codcuentacatalogo.StartsWith(cod))
                                {
                                    cc[j].Codcuentacatalogo = cc[j].Codcuentacatalogo.Replace(".", "");
                                    cc[j].nomCuentaEID = nom[0].nomCuentaEID;
                                    _context.Update(cc[j]);
                                    await _context.SaveChangesAsync();
                                }
                            }
                            break;
                        case "NUMERO DE ACCIONES":
                            foreach (string cod in listCs[i].codCuenta)
                            {
                                if(cod != null)
                                {
                                    if (cc[j].Codcuentacatalogo.StartsWith(cod))
                                    {
                                        cc[j].Codcuentacatalogo = cc[j].Codcuentacatalogo.Replace(".", "");
                                        cc[j].nomCuentaEID = nom[0].nomCuentaEID;
                                        _context.Update(cc[j]);
                                        await _context.SaveChangesAsync();
                                    }
                                }
                            }
                            break;
                    }

                }
            }
            return RedirectToAction("Index", "CatalogoCuentas");
        }

        // GET: NomCuentaEs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nomCuentaE = await _context.NomCuentaE
                .FirstOrDefaultAsync(m => m.nomCuentaEID == id);
            if (nomCuentaE == null)
            {
                return NotFound();
            }

            return View(nomCuentaE);
        }

        // GET: NomCuentaEs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: NomCuentaEs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("nomCuentaEID,nomCuentaE")] NomCuentaE nomCuentaE)
        {
            if (ModelState.IsValid)
            {
                _context.Add(nomCuentaE);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(nomCuentaE);
        }

        // GET: NomCuentaEs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nomCuentaE = await _context.NomCuentaE.FindAsync(id);
            if (nomCuentaE == null)
            {
                return NotFound();
            }
            return View(nomCuentaE);
        }

        // POST: NomCuentaEs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("nomCuentaEID,nomCuentaE")] NomCuentaE nomCuentaE)
        {
            if (id != nomCuentaE.nomCuentaEID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nomCuentaE);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NomCuentaEExists(nomCuentaE.nomCuentaEID))
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
            return View(nomCuentaE);
        }

        // GET: NomCuentaEs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nomCuentaE = await _context.NomCuentaE
                .FirstOrDefaultAsync(m => m.nomCuentaEID == id);
            if (nomCuentaE == null)
            {
                return NotFound();
            }

            return View(nomCuentaE);
        }

        // POST: NomCuentaEs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var nomCuentaE = await _context.NomCuentaE.FindAsync(id);
            _context.NomCuentaE.Remove(nomCuentaE);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<JsonResult> GetCC()
        {
            var user = this.User;
            List<Usuario> u =  _context.Users.Include(e => e.Idempresa).Where(e => e.UserName == user.Identity.Name).ToList();
            var jsonData = _context.Catalogodecuenta.Include(e => e.IdcuentaNavigation).Where(e => e.Codcuentacatalogo != "0" && e.Idempresa == u[0].Idempresa.Idempresa && e.Codcuentacatalogo != "D").ToList();
            return Json(jsonData);
        }

        private bool NomCuentaEExists(int id)
        {
            return _context.NomCuentaE.Any(e => e.nomCuentaEID == id);
        }
    }
}
