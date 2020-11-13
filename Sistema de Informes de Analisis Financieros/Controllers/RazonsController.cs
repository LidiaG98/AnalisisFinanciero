using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Sistema_de_Informes_de_Analisis_Financieros.Models;
using Sistema_de_Informes_de_Analisis_Financieros.ViewModels;

namespace Sistema_de_Informes_de_Analisis_Financieros
{
    public class RazonsController : Controller
    {
        private readonly ProyAnfContext _context;

        public RazonsController(ProyAnfContext context)
        {
            _context = context;
        }

        // GET: Razons
        public async Task<IActionResult> Index()
        {
            return View(await _context.Razon.ToListAsync());
        }

        // GET: Razons/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var razon = await _context.Razon
                .FirstOrDefaultAsync(m => m.idRazon == id);
            if (razon == null)
            {
                return NotFound();
            }

            return View(razon);
        }

        // GET: Razons/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Razons/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("idRazon,nombreRazon,numerador,denominador")] Razon razon)
        {
            if (ModelState.IsValid)
            {
                _context.Add(razon);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(razon);
        }

        // GET: Razons/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var razon = await _context.Razon.FindAsync(id);
            if (razon == null)
            {
                return NotFound();
            }
            return View(razon);
        }

        // POST: Razons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("idRazon,nombreRazon,numerador,denominador")] Razon razon)
        {
            if (id != razon.idRazon)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(razon);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RazonExists(razon.idRazon))
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
            return View(razon);
        }

        // GET: Razons/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var razon = await _context.Razon
                .FirstOrDefaultAsync(m => m.idRazon == id);
            if (razon == null)
            {
                return NotFound();
            }

            return View(razon);
        }

        // POST: Razons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var razon = await _context.Razon.FindAsync(id);
            _context.Razon.Remove(razon);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RazonExists(int id)
        {
            return _context.Razon.Any(e => e.idRazon == id);
        }

        //Método general para las razones
        [HttpGet]
        public async Task<IActionResult> AnalisisRazon(int idRazon)
        {
            var razon = _context.Razon.Where(r => r.idRazon == idRazon).FirstOrDefault();
            AnalisisRazonViewModel model = new AnalisisRazonViewModel()
            {
                nombreRazon = razon.nombreRazon,
                tipo = razon.tipo,
                signoDenominador = "",
                signoNumerador = "",
                numerador = new List<string>(),
                denominador = new List<string>()
            };
            model.numerador.Add(razon.numerador);
            model.denominador.Add(razon.denominador);

            //numerador
            if (razon.numerador.Split('+').Length > 1)
            {
                model.numerador = razon.numerador.Split('+').ToList();
            }
            if (razon.numerador.Split('-').Length > 1)
            {
                model.numerador = razon.numerador.Split('-').ToList();
            }
            if (razon.numerador.Split('*').Length > 1)
            {
                model.numerador = razon.numerador.Split('*').ToList();
            }
            if (razon.numerador.Split('/').Length > 1)
            {
                model.numerador = razon.numerador.Split('/').ToList();
            }
            //denominador
            if (razon.denominador.Split('+').Length > 1)
            {
                model.denominador = razon.denominador.Split('+').ToList();
            }
            if (razon.denominador.Split('-').Length > 1)
            {
                model.denominador = razon.denominador.Split('-').ToList();
            }
            if (razon.denominador.Split('*').Length > 1)
            {
                model.denominador = razon.denominador.Split('*').ToList();
            }
            if (razon.denominador.Split('/').Length > 1)
            {
                model.denominador = razon.denominador.Split('/').ToList();
            }           
            //signo numerador
            if(model.signoNumerador.Equals("") && razon.numerador.Contains('+'))
            {
                model.signoNumerador = " + ";
            }
            if (model.signoNumerador.Equals("") && razon.numerador.Contains('-'))
            {
                model.signoNumerador = " - ";
            }
            if (model.signoNumerador.Equals("") && razon.numerador.Contains('*'))
            {
                model.signoNumerador = " X ";
            }
            if (model.signoNumerador.Equals("") && razon.numerador.Contains('/'))
            {
                model.signoNumerador = " / ";
            }
            //signo denominador
            if (model.signoDenominador.Equals("") && razon.denominador.Contains('+'))
            {
                model.signoDenominador = " + ";
            }
            if (model.signoDenominador.Equals("") && razon.denominador.Contains('-'))
            {
                model.signoDenominador = " - ";
            }
            if (model.signoDenominador.Equals("") && razon.denominador.Contains('*'))
            {
                model.signoDenominador = " X ";
            }
            if (model.signoDenominador.Equals("") && razon.denominador.Contains('/'))
            {
                model.signoDenominador = " / ";
            }



            return View(model);
        }

        

    }
}
