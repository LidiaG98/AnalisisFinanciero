﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Sistema_de_Informes_de_Analisis_Financieros.Models;
using Sistema_de_Informes_de_Analisis_Financieros.ViewModels;

namespace Sistema_de_Informes_de_Analisis_Financieros.Controllers
{
    public class EmpresasController : Controller
    {
        private readonly ProyAnfContext _context;
        private CatalogoCuentasController catalogo;
        private ValoresBalanceController valoresController;

        public EmpresasController(ProyAnfContext context)
        {
            _context = context;
            catalogo = new CatalogoCuentasController(context);
            valoresController = new ValoresBalanceController(context);
        }

        // GET: Empresas
        public async Task<IActionResult> Index()
        {
            var proyAnfContext = _context.Empresa.Include(e => e.IdsectorNavigation);
            return View(await proyAnfContext.ToListAsync());
        }

        public async Task<IActionResult> GuardarCuentas(int IdEmpresa,string celdaCod, string celdaNom, IFormFile files)
        {
            celdaCod = celdaCod.ToUpper();
            celdaNom = celdaNom.ToUpper();
            ViewData["Mensaje"] = await catalogo.GuardarCuentas(IdEmpresa,celdaCod,celdaNom,files);
            return RedirectToAction("Index", "CatalogoCuentas");
        }

        public IActionResult SubirBalance()
        {
            return PartialView();
        }

        public async Task GuardarBalance(int IdEmpresa, SubirBalance subirBalance, IFormFile files)
        {
            await valoresController.GuardarBalance(IdEmpresa, subirBalance, files);
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
        public IActionResult Create()
        {
            ViewData["Idsector"] = new SelectList(_context.Sector, "Idsector", "Nomsector");
            return View();
        }

        // POST: Empresas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
    }
}
