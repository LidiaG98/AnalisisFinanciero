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
    public class MensajesAnalisisController : Controller
    {
        private readonly ProyAnfContext _context;

        public MensajesAnalisisController(ProyAnfContext context)
        {
            _context = context;
        }

        // GET: MensajesAnalisis
        public async Task<IActionResult> Index()
        {
            return View(await _context.MensajesAnalisis.ToListAsync());
        }

        // GET: MensajesAnalisis/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mensajesAnalisis = await _context.MensajesAnalisis
                .FirstOrDefaultAsync(m => m.idMensaje == id);
            if (mensajesAnalisis == null)
            {
                return NotFound();
            }

            return View(mensajesAnalisis);
        }

        // GET: MensajesAnalisis/Create
        public IActionResult Create()
        {            
            SelectList listRatios = new SelectList(_context.Ratio.ToList(), "Idratio", "Nombreratiob");
            ViewBag.listRatios = listRatios;
            return View();
        }

        // POST: MensajesAnalisis/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("idMensaje,mensajeMayorBase,mensajeMenorBase,mensajeMayorEmp,mensajeMenorEmp,mensajeIgualBase,mensajeIgualEmp,idRatio")] MensajesAnalisis mensajesAnalisis)
        {
            if (ModelState.IsValid)
            {
                _context.Add(mensajesAnalisis);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(mensajesAnalisis);
        }

        // GET: MensajesAnalisis/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mensajesAnalisis = await _context.MensajesAnalisis.FindAsync(id);
            if (mensajesAnalisis == null)
            {
                return NotFound();
            }
            SelectList listRatios = new SelectList(_context.Ratio.ToList(), "Idratio", "Nombreratiob");
            ViewBag.listRatios = listRatios;
            return View(mensajesAnalisis);
        }

        // POST: MensajesAnalisis/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("idMensaje,mensajeMayorBase,mensajeMenorBase,mensajeMayorEmp,mensajeMenorEmp,mensajeIgualBase,mensajeIgualEmp,idRatio")] MensajesAnalisis mensajesAnalisis)
        {
            if (id != mensajesAnalisis.idMensaje)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mensajesAnalisis);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MensajesAnalisisExists(mensajesAnalisis.idMensaje))
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
            return View(mensajesAnalisis);
        }

        // GET: MensajesAnalisis/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mensajesAnalisis = await _context.MensajesAnalisis
                .FirstOrDefaultAsync(m => m.idMensaje == id);
            if (mensajesAnalisis == null)
            {
                return NotFound();
            }

            return View(mensajesAnalisis);
        }

        // POST: MensajesAnalisis/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mensajesAnalisis = await _context.MensajesAnalisis.FindAsync(id);
            _context.MensajesAnalisis.Remove(mensajesAnalisis);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MensajesAnalisisExists(int id)
        {
            return _context.MensajesAnalisis.Any(e => e.idMensaje == id);
        }
    }
}
