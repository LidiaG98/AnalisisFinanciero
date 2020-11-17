﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Sistema_de_Informes_de_Analisis_Financieros.Models;

namespace Sistema_de_Informes_de_Analisis_Financieros.Controllers
{
    public class RatioBaseSectorController : Controller
    {
        private readonly ProyAnfContext _context;

        public RatioBaseSectorController(ProyAnfContext context)
        {
            _context = context;
        }

        // GET: RatioBaseSector
        public async Task<IActionResult> Index()
        {
            var proyAnfContext = _context.Ratiobasesector.Include(r => r.IdratioNavigation).Include(r => r.IdsectorNavigation);
            return View(await proyAnfContext.ToListAsync());
        }

        // GET: RatioBaseSector/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ratiobasesector = await _context.Ratiobasesector
                .Include(r => r.IdratioNavigation)
                .Include(r => r.IdsectorNavigation)
                .FirstOrDefaultAsync(m => m.Idratio == id);
            if (ratiobasesector == null)
            {
                return NotFound();
            }

            return View(ratiobasesector);
        }

        // GET: RatioBaseSector/Create
        public IActionResult Create()
        {
            ViewData["Idratio"] = new SelectList(_context.Ratio, "Idratio", "Nombreratiob");
            ViewData["Idsector"] = new SelectList(_context.Sector, "Idsector", "Nomsector");
            return View();
        }

        // POST: RatioBaseSector/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Idratio,Idsector,Valorratiob")] Ratiobasesector ratiobasesector)
        {
            if (ModelState.IsValid)
            {
                _context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[RATIOBASESECTOR] ON");
                //_context.Database.ExecuteSqlCommand("INSERT INTO RATIOBASESECTOR (Idratio, Idsector, Valorratiob) VALUES (" 
                //    + ratiobasesector.Idratio + ", "
                //    + ratiobasesector.Idsector + ", "
                //    + ratiobasesector.Valorratiob + ");");
                _context.Add(ratiobasesector);
                await _context.SaveChangesAsync();
                _context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[RATIOBASESECTOR] OFF");
                return RedirectToAction(nameof(Index));
            }
            ViewData["Idratio"] = new SelectList(_context.Ratio, "Idratio", "Nombreratiob", ratiobasesector.Idratio);
            ViewData["Idsector"] = new SelectList(_context.Sector, "Idsector", "Nomsector", ratiobasesector.Idsector);
            return View(ratiobasesector);
        }

        // GET: RatioBaseSector/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ratiobasesector = await _context.Ratiobasesector.FindAsync(id);
            if (ratiobasesector == null)
            {
                return NotFound();
            }
            ViewData["Idratio"] = new SelectList(_context.Ratio, "Idratio", "Nombreratiob", ratiobasesector.Idratio);
            ViewData["Idsector"] = new SelectList(_context.Sector, "Idsector", "Nomsector", ratiobasesector.Idsector);
            return View(ratiobasesector);
        }

        // POST: RatioBaseSector/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Idratio,Idsector,Valorratiob")] Ratiobasesector ratiobasesector)
        {
            if (id != ratiobasesector.Idratio)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ratiobasesector);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RatiobasesectorExists(ratiobasesector.Idratio))
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
            ViewData["Idratio"] = new SelectList(_context.Ratio, "Idratio", "Nombreratiob", ratiobasesector.Idratio);
            ViewData["Idsector"] = new SelectList(_context.Sector, "Idsector", "Nomsector", ratiobasesector.Idsector);
            return View(ratiobasesector);
        }

        // GET: RatioBaseSector/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ratiobasesector = await _context.Ratiobasesector
                .Include(r => r.IdratioNavigation)
                .Include(r => r.IdsectorNavigation)
                .FirstOrDefaultAsync(m => m.Idratio == id);
            if (ratiobasesector == null)
            {
                return NotFound();
            }

            return View(ratiobasesector);
        }

        // POST: RatioBaseSector/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ratiobasesector = await _context.Ratiobasesector.FindAsync(id);
            _context.Ratiobasesector.Remove(ratiobasesector);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RatiobasesectorExists(int id)
        {
            return _context.Ratiobasesector.Any(e => e.Idratio == id);
        }
    }
}
