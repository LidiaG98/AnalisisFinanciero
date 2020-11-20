using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Sistema_de_Informes_de_Analisis_Financieros.Models;
using Sistema_de_Informes_de_Analisis_Financieros.ViewModels;

namespace Sistema_de_Informes_de_Analisis_Financieros.Controllers
{    
    [Authorize]
    public class CuentasController : Controller
    {
        private readonly ProyAnfContext _context;
        public TipoCuentaController tipoCuenta;

        public CuentasController(ProyAnfContext context)
        {
            _context = context;
            tipoCuenta = new TipoCuentaController(context);
        }

        // ConsultarCuentas: Devuelve la lista de cuentas ingresadas al sistema.
        public async Task<List<Cuenta>> ConsultarCuentas()
        {
            var proyAnfContext = _context.Cuenta.Include(c => c.IdtipocuentaNavigation);
            List<Cuenta> lstCuentas = await proyAnfContext.ToListAsync();
            return lstCuentas;
        }

        /*AsignarTipo: Recibe la lista de cuentas extraída del archivo de catálogo y de acuerdo a su código de 
        catalogo le asigna un tipo de cuenta y devuelve una lista de cuentas ya con tipo asignado*/
        public async Task<List<Cuenta>> AsignarTipo(List<CuentaViewModel> lstCuentasV)
        {
            List<Tipocuenta> tipos = await tipoCuenta.ConsultarTipoCuenta();
            int AC = 0, A = 0, ANC = 0, PC = 0, P = 0, PNC = 0, Pat = 0, G = 0, I = 0, O = 0;

            foreach (var tipo in tipos)
            {
                if (tipo.Nomtipocuenta.Equals("ACTIVO CORRIENTE")) { AC = tipo.Idtipocuenta; }
                else if (tipo.Nomtipocuenta.Equals("ACTIVO NO CORRIENTE")) { ANC = tipo.Idtipocuenta; }
                else if (tipo.Nomtipocuenta.Equals("PASIVO CORRIENTE")) { PC = tipo.Idtipocuenta; }
                else if (tipo.Nomtipocuenta.Equals("PASIVO NO CORRIENTE")) { PNC = tipo.Idtipocuenta; }
                else if (tipo.Nomtipocuenta.Equals("PATRIMONIO")) { Pat = tipo.Idtipocuenta; }                
                else if (tipo.Nomtipocuenta.Equals("INGRESOS")) { I = tipo.Idtipocuenta; }
                else if (tipo.Nomtipocuenta.Equals("OTROS")) { O = tipo.Idtipocuenta; }                
                else if (tipo.Nomtipocuenta.Equals("ACTIVO")) { A = tipo.Idtipocuenta; }
                else if (tipo.Nomtipocuenta.Equals("PASIVO")) { P = tipo.Idtipocuenta; }
                else if (tipo.Nomtipocuenta.Equals("GASTOS")) { G = tipo.Idtipocuenta; }
            }

            List<Cuenta> lstCTipo = new List<Cuenta>();            
            foreach (var cuenta in lstCuentasV)
            {
                string val = "";                
                //cuenta.codigo=cuenta.codigo.Replace(".", "");
                string cod = cuenta.codigo.Replace(".", "");

                if (cod.Length > 1) { val = cod.Substring(0, 2); }
                else { val = cod.Substring(0, 1); }

                switch (val)
                {
                    case "1":
                        lstCTipo.Add(new Cuenta
                        {
                            Nomcuenta = cuenta.nombre,
                            Idtipocuenta = A,
                        });
                        break;
                    case "11":
                        lstCTipo.Add(new Cuenta
                        {
                            Nomcuenta = cuenta.nombre,
                            Idtipocuenta = AC,
                        });                        
                        break;
                    case "12":
                        lstCTipo.Add(new Cuenta
                        {
                            Nomcuenta = cuenta.nombre,
                            Idtipocuenta = ANC,
                        });
                        break;
                    case "2":
                        lstCTipo.Add(new Cuenta
                        {
                            Nomcuenta = cuenta.nombre,
                            Idtipocuenta = P,
                        });
                        break;
                    case "21":
                        lstCTipo.Add(new Cuenta
                        {
                            Nomcuenta = cuenta.nombre,
                            Idtipocuenta = PC,
                        });
                        break;
                    case "22":
                        lstCTipo.Add(new Cuenta
                        {
                            Nomcuenta = cuenta.nombre,
                            Idtipocuenta = PNC,
                        });
                        break;
                    case "3":
                        lstCTipo.Add(new Cuenta
                        {
                            Nomcuenta = cuenta.nombre,
                            Idtipocuenta = Pat,
                        });
                        break;
                    case "4":
                        lstCTipo.Add(new Cuenta
                        {
                            Nomcuenta = cuenta.nombre,
                            Idtipocuenta = G,
                        });
                        break;                    
                    case "5":
                        lstCTipo.Add(new Cuenta
                        {
                            Nomcuenta = cuenta.nombre,
                            Idtipocuenta = I,
                        });
                        break;
                    default:
                        if (cod.StartsWith("3")) {
                            lstCTipo.Add(new Cuenta
                            {
                                Nomcuenta = cuenta.nombre,
                                Idtipocuenta = Pat,
                            });
                        }
                        else if (cod.StartsWith("4"))
                        {
                            lstCTipo.Add(new Cuenta
                            {
                                Nomcuenta = cuenta.nombre,
                                Idtipocuenta = G,
                            });
                        }
                        else if (cod.StartsWith("5"))
                        {
                            lstCTipo.Add(new Cuenta
                            {
                                Nomcuenta = cuenta.nombre,
                                Idtipocuenta = I,
                            });
                        }
                        else
                        {
                            lstCTipo.Add(new Cuenta
                            {
                                Nomcuenta = cuenta.nombre,
                                Idtipocuenta = O,
                            });                            
                        }
                        break;
                }                
            }
            return lstCTipo; //Lista de cuentas con el tipo añadido
        }

        //InsertarNuevasCuentas: Verifica si la cuenta ya existe en la base, si no es así la inserta
        public async Task InsertarNuevasCuentas(List<CuentaViewModel> lstCuentasV)
        {
            List<Cuenta> lstCBase = await ConsultarCuentas();            
            List<Cuenta> lstCuentas = await AsignarTipo(lstCuentasV);
            bool bandera = true;
            foreach (var cuenta in lstCuentas)
            {
                bandera = true;
                for (int i = 0; i < lstCBase.Count; i++)
                {
                    if (lstCBase[i].Nomcuenta.Equals(cuenta.Nomcuenta))
                    {
                        bandera = false;
                        break;
                    }
                }
                if (bandera)
                {
                    _context.Add(cuenta);
                    await _context.SaveChangesAsync();
                }
            }            
        }

        /*InsertarCuentasDeTotal: Inserta las cuentas destinadas a guardar el total del balance y estado de resultados*/
        public async Task InsertarCuentasDeTotal()
        {
            List<Tipocuenta> tipos = await tipoCuenta.ConsultarTipoCuenta();
            int AC = 0,A = 0, ANC = 0, PC = 0, P = 0, PNC = 0, Pat = 0, GA = 0, G = 0, GF = 0, OG = 0, I = 0, O = 0, IMP = 0;            
            var cuentasBase = from s in _context.Cuenta select s;
            cuentasBase = cuentasBase.Where(s => s.Nomcuenta.Equals("TOTAL ACTIVOS CORRIENTES") || s.Nomcuenta.Equals("TOTAL ACTIVOS NO CORRIENTES") ||
            s.Nomcuenta.Equals("TOTAL PASIVOS CORRIENTES") || s.Nomcuenta.Equals("TOTAL PASIVOS NO CORRIENTES") || s.Nomcuenta.Equals("TOTAL PATRIMONIO")
            || s.Nomcuenta.Equals("TOTAL ACTIVO") || s.Nomcuenta.Equals("TOTAL PASIVO MAS PATRIMONIO") || s.Nomcuenta.Equals("VENTAS NETAS")
            || s.Nomcuenta.Equals("COSTO DE VENTAS") || s.Nomcuenta.Equals("UTILIDAD BRUTA") || s.Nomcuenta.Equals("GASTOS ADMINISTRATIVOS")
            || s.Nomcuenta.Equals("UTILIDAD OPERATIVA") || s.Nomcuenta.Equals("GASTOS FINANCIEROS") || s.Nomcuenta.Equals("UTILIDAD ANTES DE IMPUESTOS")
            || s.Nomcuenta.Equals("IMPUESTOS") || s.Nomcuenta.Equals("UTILIDAD NETA") || s.Nomcuenta.Equals("PAGO DE DIVIDENDOS") || s.Nomcuenta.Equals("UTILIDADES RETENIDAS"));
            int total = cuentasBase.Count();
            if (total == 0)
            {
                foreach (var tipo in tipos)
                {
                    if (tipo.Nomtipocuenta.Equals("ACTIVO CORRIENTE")) { AC = tipo.Idtipocuenta; }
                    else if (tipo.Nomtipocuenta.Equals("ACTIVO NO CORRIENTE")) { ANC = tipo.Idtipocuenta; }
                    else if (tipo.Nomtipocuenta.Equals("PASIVO CORRIENTE")) { PC = tipo.Idtipocuenta; }
                    else if (tipo.Nomtipocuenta.Equals("PASIVO NO CORRIENTE")) { PNC = tipo.Idtipocuenta; }
                    else if (tipo.Nomtipocuenta.Equals("PATRIMONIO")) { Pat = tipo.Idtipocuenta; }
                    else if (tipo.Nomtipocuenta.Equals("INGRESOS")) { I = tipo.Idtipocuenta; }
                    else if (tipo.Nomtipocuenta.Equals("OTROS")) { O = tipo.Idtipocuenta; }                    
                    else if (tipo.Nomtipocuenta.Equals("ACTIVO")) { A = tipo.Idtipocuenta; }
                    else if (tipo.Nomtipocuenta.Equals("PASIVO")) { P = tipo.Idtipocuenta; }
                    else if (tipo.Nomtipocuenta.Equals("GASTOS")) { G = tipo.Idtipocuenta; }
                }
                _context.Cuenta.Add(new Cuenta { 
                    Idtipocuenta = A,
                    Nomcuenta = "TOTAL ACTIVO"
                });
                _context.Cuenta.Add(new Cuenta
                {
                    Idtipocuenta = AC,
                    Nomcuenta = "TOTAL ACTIVOS CORRIENTES"
                });
                _context.Cuenta.Add(new Cuenta
                {
                    Idtipocuenta = ANC,
                    Nomcuenta = "TOTAL ACTIVOS NO CORRIENTES"
                });
                _context.Cuenta.Add(new Cuenta
                {
                    Idtipocuenta = PC,
                    Nomcuenta = "TOTAL PASIVOS CORRIENTES"
                });
                _context.Cuenta.Add(new Cuenta
                {
                    Idtipocuenta = PNC,
                    Nomcuenta = "TOTAL PASIVOS NO CORRIENTES"
                });
                _context.Cuenta.Add(new Cuenta
                {
                    Idtipocuenta = Pat,
                    Nomcuenta = "TOTAL PATRIMONIO"
                });
                _context.Cuenta.Add(new Cuenta
                {
                    Idtipocuenta = P,
                    Nomcuenta = "TOTAL PASIVO MAS PATRIMONIO"
                });
                _context.Cuenta.Add(new Cuenta
                {
                    Idtipocuenta = I,
                    Nomcuenta = "VENTAS NETAS"
                });
                _context.Cuenta.Add(new Cuenta
                {
                    Idtipocuenta = G,
                    Nomcuenta = "COSTO DE VENTAS"
                });
                _context.Cuenta.Add(new Cuenta
                {
                    Idtipocuenta = I,
                    Nomcuenta = "UTILIDAD BRUTA"
                });
                _context.Cuenta.Add(new Cuenta
                {
                    Idtipocuenta = G,
                    Nomcuenta = "GASTOS ADMINISTRATIVOS"
                });
                _context.Cuenta.Add(new Cuenta
                {
                    Idtipocuenta = I,
                    Nomcuenta = "UTILIDAD OPERATIVA"
                });
                _context.Cuenta.Add(new Cuenta
                {
                    Idtipocuenta = G,
                    Nomcuenta = "GASTOS FINANCIEROS"
                });
                _context.Cuenta.Add(new Cuenta
                {
                    Idtipocuenta = I,
                    Nomcuenta = "UTILIDAD ANTES DE IMPUESTOS"
                });
                _context.Cuenta.Add(new Cuenta
                {
                    Idtipocuenta = G,
                    Nomcuenta = "IMPUESTOS"
                });
                _context.Cuenta.Add(new Cuenta
                {
                    Idtipocuenta = I,
                    Nomcuenta = "UTILIDAD NETA"
                });
                _context.Cuenta.Add(new Cuenta
                {
                    Idtipocuenta = G,
                    Nomcuenta = "PAGO DE DIVIDENDOS"
                });
                _context.Cuenta.Add(new Cuenta
                {
                    Idtipocuenta = I,
                    Nomcuenta = "UTILIDADES RETENIDAS"
                });
                var v = _context.Cuenta.Count(s => s.Idtipocuenta == O && s.Nomcuenta.Equals("Default"));
                if (v == 0)
                {
                    _context.Cuenta.Add(new Cuenta
                    {
                        Idtipocuenta = O,
                        Nomcuenta = "Default"
                    });
                }
                await _context.SaveChangesAsync();
            }

        }

        // GET: Cuentas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cuenta = await _context.Cuenta
                .Include(c => c.IdtipocuentaNavigation)
                .FirstOrDefaultAsync(m => m.Idcuenta == id);
            if (cuenta == null)
            {
                return NotFound();
            }

            return View(cuenta);
        }

        // GET: Cuentas/Create
        public IActionResult Create()
        {
            ViewData["Idtipocuenta"] = new SelectList(_context.Tipocuenta, "Idtipocuenta", "Nomtipocuenta");
            return View();
        }

        
        // GET: Cuentas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cuenta = await _context.Cuenta.FindAsync(id);
            if (cuenta == null)
            {
                return NotFound();
            }
            ViewData["Idtipocuenta"] = new SelectList(_context.Tipocuenta, "Idtipocuenta", "Nomtipocuenta", cuenta.Idtipocuenta);
            return View(cuenta);
        }

        // POST: Cuentas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Idcuenta,Idtipocuenta,Nomcuenta")] Cuenta cuenta)
        {
            if (id != cuenta.Idcuenta)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cuenta);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CuentaExists(cuenta.Idcuenta))
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
            ViewData["Idtipocuenta"] = new SelectList(_context.Tipocuenta, "Idtipocuenta", "Nomtipocuenta", cuenta.Idtipocuenta);
            return View(cuenta);
        }

        // GET: Cuentas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cuenta = await _context.Cuenta
                .Include(c => c.IdtipocuentaNavigation)
                .FirstOrDefaultAsync(m => m.Idcuenta == id);
            if (cuenta == null)
            {
                return NotFound();
            }

            return View(cuenta);
        }

        // POST: Cuentas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cuenta = await _context.Cuenta.FindAsync(id);
            _context.Cuenta.Remove(cuenta);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CuentaExists(int id)
        {
            return _context.Cuenta.Any(e => e.Idcuenta == id);
        }
    }
}
