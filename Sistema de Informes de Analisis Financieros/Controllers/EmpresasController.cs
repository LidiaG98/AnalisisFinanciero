using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Sistema_de_Informes_de_Analisis_Financieros.Models;
using Sistema_de_Informes_de_Analisis_Financieros.ViewModels;

namespace Sistema_de_Informes_de_Analisis_Financieros.Controllers
{
    [Authorize]
    public class EmpresasController : Controller
    {
        private readonly ProyAnfContext _context;
        private CatalogoCuentasController catalogo;
        private ValoresBalanceController valoresController;
        private EstadoRController estadoController;
        private readonly UserManager<Usuario> _userManager;

        public EmpresasController(ProyAnfContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            catalogo = new CatalogoCuentasController(context,userManager);
            valoresController = new ValoresBalanceController(context,userManager);
            estadoController = new EstadoRController(context,userManager);
            _userManager = userManager;
        }

        // GET: Empresas
        public async Task<IActionResult> Index()
        {
            var usuario = this.User;
            Usuario u = _context.Users.Include(l => l.Idempresa).Where(l => l.UserName == usuario.Identity.Name).FirstOrDefault();
            if (usuario.IsInRole("Administrator"))
            {
                var proyAnfContext = _context.Empresa.Include(e => e.IdsectorNavigation);
                return View(await proyAnfContext.ToListAsync());
            }
            else
            {
                var proyAnfContext = _context.Empresa.Include(e => e.IdsectorNavigation).Where(l => l.Idempresa == u.Idempresa.Idempresa);
                return View(await proyAnfContext.ToListAsync());
            }
        }

        public async Task<IActionResult> GuardarCuentas(int IdEmpresa,string celdaCod, string celdaNom ,string hoja, IFormFile files)
        {
            celdaCod = celdaCod.ToUpper();
            celdaNom = celdaNom.ToUpper();
            ViewData["Mensaje"] = await catalogo.GuardarCuentas(IdEmpresa,celdaCod,celdaNom,hoja,files);
            return RedirectToAction("ActualizarCatalogoCuenta", "NomCuentaEs");
        }

        [HttpGet]
        public IActionResult SubirBalance()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<IActionResult> SubirBalance(int IdEmpresa,SubirBalance subirBalance, IFormFile files)
        {
            if (ModelState.IsValid)
            {
                string mensaje = await valoresController.GuardarBalance(IdEmpresa, subirBalance, files);
                return RedirectToAction("Index", "ValoresBalance", new { mensaje = mensaje });
            }
            return PartialView("SubirBalance", subirBalance);
        }

        public IActionResult SubirEstado()
        {
            return PartialView();
        }

        public async Task<IActionResult> GuardarBalance(int IdEmpresa, SubirBalance subirBalance, IFormFile files)
        {
            if (ModelState.IsValid)
            {
                string mensaje = await valoresController.GuardarBalance(IdEmpresa, subirBalance, files);
                return RedirectToAction("Index", "ValoresBalance", new { mensaje = mensaje });
            }
            return PartialView("SubirBalance",subirBalance);
            
        }

        public async Task<IActionResult> GuardarEstado(int IdEmpresa, SubirBalance subirBalance, IFormFile files)
        {
            string msje = await estadoController.GuardarEstado(IdEmpresa, subirBalance, files);
            return RedirectToAction("Index", "EstadoR", new { mensaje = msje });
        }

        // GET: Empresas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empresa = await _context.Empresa
                .Include(e => e.IdsectorNavigation)
                .FirstOrDefaultAsync(m => m.Idempresa == id);
            if (empresa == null)
            {
                return NotFound();
            }

            return View(empresa);
        }

        // GET: Empresas/Create        
        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            ViewData["Idsector"] = new SelectList(_context.Sector, "Idsector", "Nomsector");
            return View();
        }

        // POST: Empresas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.        
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Idempresa,Idsector,Nomempresa,Descempresa,Razonsocial")] Empresa empresa)
        {
            if (ModelState.IsValid)
            {
                _context.Add(empresa);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Idsector"] = new SelectList(_context.Sector, "Idsector", "Nomsector", empresa.Idsector);
            return View(empresa);
        }

        // GET: Empresas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empresa = await _context.Empresa.FindAsync(id);
            if (empresa == null)
            {
                return NotFound();
            }
            ViewData["Idsector"] = new SelectList(_context.Sector, "Idsector", "Nomsector", empresa.Idsector);
            return View(empresa);
        }

        // POST: Empresas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Idempresa,Idsector,Nomempresa,Descempresa,Razonsocial")] Empresa empresa)
        {
            if (id != empresa.Idempresa)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(empresa);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmpresaExists(empresa.Idempresa))
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
            ViewData["Idsector"] = new SelectList(_context.Sector, "Idsector", "Nomsector", empresa.Idsector);
            return View(empresa);
        }

        // GET: Empresas/Delete/5        
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empresa = await _context.Empresa
                .Include(e => e.IdsectorNavigation)
                .FirstOrDefaultAsync(m => m.Idempresa == id);
            if (empresa == null)
            {
                return NotFound();
            }

            return View(empresa);
        }

        // POST: Empresas/Delete/5        
        [Authorize(Roles = "Administrator")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var empresa = await _context.Empresa.FindAsync(id);
            _context.Empresa.Remove(empresa);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmpresaExists(int id)
        {
            return _context.Empresa.Any(e => e.Idempresa == id);
        }


        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<IActionResult> CrearUsuario()
        {
            SelectList listRatios = new SelectList(_context.Empresa.ToList(), "Idempresa", "Nomempresa");
            ViewBag.listRatios = listRatios;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearUsuario([Bind("email,Password,ConfirmPassword,idEmpresa")] CrearUsuarioModel usuario)
        {
            SelectList listRatios = new SelectList(_context.Empresa.ToList(), "Idempresa", "Nomempresa");
            ViewBag.listRatios = listRatios;
            if (ModelState.IsValid)
            {
                Empresa empresa = _context.Empresa.Where(l => l.Idempresa == usuario.idEmpresa).FirstOrDefault();
                Usuario u = new Usuario()
                {
                    UserName = usuario.email,
                    Email = usuario.email,
                    EmailConfirmed = true,
                    Idempresa = empresa
                };                                
                string pass = usuario.Password;
                var result = await _userManager.CreateAsync(u, pass);                
                return RedirectToAction("Index");
            }            
            return View(usuario);
        }
    }
}
