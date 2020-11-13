using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
        { // Convierte texto en funciones y puede mostrar el resultado
            Mathos.Parser.MathParser parse = new Mathos.Parser.MathParser();
            
            //contextos que creí necesarios para seguir v,:
            var cliente = from s in _context.Empresa select s;
            var catCuent = from e in _context.Catalogodecuenta select e;
            var cuenta = from x in _context.Cuenta select x;
            var empresa = from y in _context.Empresa select y;
            var razon = from m in _context.Razon select m;
            var tipocuenta = from l in _context.Tipocuenta select l;
            var balance = from n in _context.Valoresdebalance select n;
            //Lista que guarda los resultados de los ratios
            List<double> Resultado = new List<double>();
            //delimitador generalizado para recuperar el nombre de las cuentas
            // string delimitador = "";

           //recupero el usuario que está logeado, asumo explota si no hay nadie logeado
            var user = this.User;
            List<Usuario> u = _context.Users.Include(e => e.Idempresa).Where(e => e.UserName == user.Identity.Name).ToList();
            List<Cuenta> cuex = new List<Cuenta>();
           //recupero el catalogo de la empresa del usuario logeado
            catCuent = catCuent.Where(y => y.Idempresa == u[0].Idempresa.Idempresa);
           /* foreach(Catalogodecuenta cato in catCuent)
            {
                cuex.Add(cuenta.FirstOrDefault(x => x.Idcuenta == cato.Idcuenta));
                
            }*/
           //necesitaba tener las cuentas del catalogo que filtré arriba, y no sa{bia como xd tons hice un join 
           //entre la tabla
            var cuentaCat = _context.Cuenta.Join(_context.Catalogodecuenta,
                cuentax => cuentax.Idcuenta,
                catcuentx => catcuentx.Idcuenta,
                (cuentax, catcuentx) => new
                {
                    cuentaID = cuentax.Idcuenta,
                    Idtipocuenta = cuentax.Idtipocuenta


                }) ;
            foreach (Razon rakata in razon)
            {
               string[] razoncitanum= rakata.numerador.Split('+','-','*');
               string[] razoncitaden = rakata.numerador.Split('+', '-', '*');
                List<Tipocuenta> tipoC = new List<Tipocuenta>();
                List<int> cuentax = new List<int>();
                List<double> suma = new List<double>();
                
               

                //pasar a numero el numerador de los ratios
                for(int i=0;i< razoncitanum.Length;i++)
                {
                    var tipoCC= tipocuenta.FirstOrDefault(l => l.Nomtipocuenta.Equals(razoncitanum[i]));
                    if (tipoCC != null)
                    {


                        var cuentaX = cuentaCat.Where(x => x.Idtipocuenta == tipoCC.Idtipocuenta);
                        foreach (var c in cuentaX)
                        {
                            var banlancin = balance.FirstOrDefault(n => n.Idcuenta == c.cuentaID);
                            //sumo el total de las cuentas del mismo tipo
                            suma[i] = suma[i] + banlancin.Valorcuenta;
                        }
                        rakata.numerador = rakata.numerador.Replace(razoncitanum[i], suma[i].ToString());
                         

                    }
                   
                }
                for (int i = 0; i < razoncitaden.Length; i++)
                {
                    var tipoCC = tipocuenta.FirstOrDefault(l => l.Nomtipocuenta.Equals(razoncitaden[i]));
                    var cuentaX = cuenta.Where(x => x.Idtipocuenta == tipoCC.Idtipocuenta);
                    foreach (Cuenta c in cuentaX)
                    {
                        var banlancin = balance.FirstOrDefault(n => n.Idcuenta == c.Idcuenta);
                        suma[i] = suma[i] + banlancin.Valorcuenta;
                    }
                    rakata.denominador = rakata.denominador.Replace(razoncitanum[i], suma[i].ToString());
                }



                
                Resultado.Add(( parse.Parse(rakata.numerador + "/" + rakata.denominador )));
                
            }
            ViewData["Resultados"] = Resultado;

           
                
                 


            // var empresa = empresa.Where(s => s.clienteID.Contains(Buscar) || s.nombre.Contains(Buscar) || s.apellido.Contains(Buscar));



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
