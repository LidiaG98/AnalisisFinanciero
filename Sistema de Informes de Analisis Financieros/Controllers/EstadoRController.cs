using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Sistema_de_Informes_de_Analisis_Financieros.Models;

namespace Sistema_de_Informes_de_Analisis_Financieros.Controllers
{
    public class EstadoRController : Controller
    {
        private readonly ProyAnfContext _context;

        public EstadoRController(ProyAnfContext context)
        {
            _context = context;
        }

        // GET: EstadoR
        public async Task<IActionResult> Index()
        {
            var proyAnfContext = _context.Valoresestado.Include(v => v.Id);
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
