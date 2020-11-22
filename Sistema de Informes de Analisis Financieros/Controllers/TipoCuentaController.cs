using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Sistema_de_Informes_de_Analisis_Financieros.Models;

namespace Sistema_de_Informes_de_Analisis_Financieros.Controllers
{    
    [Authorize(Roles = "Administrator")]
    public class TipoCuentaController : Controller
    {
        private readonly ProyAnfContext _context;

        public TipoCuentaController(ProyAnfContext context)
        {
            _context = context;
        }

        // GET: TipoCuenta
        public async Task<List<Tipocuenta>> ConsultarTipoCuenta()
        {
            return await _context.Tipocuenta.ToListAsync();
        }

        // GET: TipoCuenta/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipocuenta = await _context.Tipocuenta
                .FirstOrDefaultAsync(m => m.Idtipocuenta == id);
            if (tipocuenta == null)
            {
                return NotFound();
            }

            return View(tipocuenta);
        }

        // GET: TipoCuenta/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TipoCuenta/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Idtipocuenta,Nomtipocuenta")] Tipocuenta tipocuenta)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tipocuenta);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tipocuenta);
        }

        // GET: TipoCuenta/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipocuenta = await _context.Tipocuenta.FindAsync(id);
            if (tipocuenta == null)
            {
                return NotFound();
            }
            return View(tipocuenta);
        }

        // POST: TipoCuenta/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Idtipocuenta,Nomtipocuenta")] Tipocuenta tipocuenta)
        {
            if (id != tipocuenta.Idtipocuenta)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tipocuenta);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipocuentaExists(tipocuenta.Idtipocuenta))
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
            return View(tipocuenta);
        }

        // GET: TipoCuenta/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipocuenta = await _context.Tipocuenta
                .FirstOrDefaultAsync(m => m.Idtipocuenta == id);
            if (tipocuenta == null)
            {
                return NotFound();
            }

            return View(tipocuenta);
        }

        // POST: TipoCuenta/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tipocuenta = await _context.Tipocuenta.FindAsync(id);
            _context.Tipocuenta.Remove(tipocuenta);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TipocuentaExists(int id)
        {
            return _context.Tipocuenta.Any(e => e.Idtipocuenta == id);
        }
    }
}
