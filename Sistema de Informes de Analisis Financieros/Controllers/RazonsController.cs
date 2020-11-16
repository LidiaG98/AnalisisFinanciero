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
            var catCuent = from e in _context.Catalogodecuenta.Include(r => r.IdcuentaNavigation) select e;
            var cuenta = from x in _context.Cuenta select x;
            var empresa = from y in _context.Empresa select y;
           
            var razon = from m in _context.Razon select m;
            var tipocuenta = from l in _context.NomCuentaE select l;
            var balance = from n in _context.Valoresdebalance select n;
            int an = balance.FirstOrDefault().Anio;
            //Lista que guarda los resultados de los ratios
            List<double> Resultado = new List<double>();
            //delimitador generalizado para recuperar el nombre de las cuentas
            // string delimitador = "";
            
           //recupero el usuario que está logeado, asumo explota si no hay nadie logeado
            var user = this.User;
            List<Usuario> u = _context.Users.Include(e => e.Idempresa).Where(e => e.UserName == user.Identity.Name).ToList();

            List<Cuenta> cuex = new List<Cuenta>();
           //recupero el catalogo de la empresa del usuario logeado
            catCuent = catCuent.Where(y => y.Idempresa == u[0].Idempresa.Idempresa );
           // List<Cuenta> cuenx = _context.Users.Include(e => e.Idempresa).Where(e => e.UserName == user.Identity.Name).ToList();
            /* foreach(Catalogodecuenta cato in catCuent)
             {
                 cuex.Add(cuenta.FirstOrDefault(x => x.Idcuenta == cato.Idcuenta));

             }*/
            //necesitaba tener las cuentas del catalogo que filtré arriba, y no sa{bia como xd tons hice un join 
            //entre la tabla
          /*  var cuentaCat = _context.Cuenta.Join(_context.Catalogodecuenta,
                cuentax => cuentax.Idcuenta,
                catcuentx => catcuentx.Idcuenta,
                (cuentax, catcuentx) => new
                {
                    cuentaID = cuentax.Idcuenta,
                    Idtipocuenta = cuentax.Idtipocuenta


                }) ;*/
            foreach (Razon rakata in razon)
            {
               string[] razoncitanum= rakata.numerador.Split('+','-','*','/');
               string[] razoncitaden = rakata.denominador.Split('+', '-', '*','/');
                List<Tipocuenta> tipoC = new List<Tipocuenta>();
                List<int> cuentax = new List<int>();
                double suma=0;
                double suma2 = 0;
                //pasar a numero el numerador de los ratios
                for (int i=0;i< razoncitanum.Length;i++)
                {
                    suma = 0;
                    suma2 = 0;
                    var tipoCC= tipocuenta.FirstOrDefault(l => l.nomCuentaE.Equals(razoncitanum[i]));
                    if (tipoCC != null)
                    {
                        var cuentaX = catCuent.Where(x => x.nomCuentaEID == tipoCC.nomCuentaEID);
                        foreach (var c in cuentaX)
                        {
                            var banlancin = balance.FirstOrDefault(n => n.Idcuenta == c.Idcuenta && n.Anio == an);
                            //sumo el total de las cuentas del mismo tipo
                            if (banlancin != null)
                            { suma = suma + banlancin.Valorcuenta; }
                            else { suma = suma + 0; }

                        }
                        rakata.numerador = rakata.numerador.Replace(razoncitanum[i], suma.ToString());
                    }
                    else if (razoncitanum[i].Contains("PROMEDIO"))
                    {
                       
                        string divo = razoncitanum[i].Replace("PROMEDIO", "");
                        var tipoP = tipocuenta.FirstOrDefault(l => l.nomCuentaE.Equals(divo));
                       
                        if(tipoP !=null)
                            {
                            var cuentaZ = catCuent.Where(x => x.nomCuentaEID == tipoP.nomCuentaEID);
                            int banAnio = balance.OrderBy(n => n.Anio).FirstOrDefault().Anio;
                        int banAnio2 = balance.Where(n =>n.Anio == banAnio+1).FirstOrDefault().Anio;
                          
                            foreach (var c in cuentaZ)
                            {
                                var banlancin = balance.Where(n => n.Anio == banAnio).FirstOrDefault(n => n.Idcuenta == c.Idcuenta);
                                var banlancin2 = balance.Where(n => n.Anio == banAnio2).FirstOrDefault(n => n.Idcuenta == c.Idcuenta);
                                //sumo el total de las cuentas del mismo tipo
                                if (banlancin != null)
                                { suma = suma + banlancin.Valorcuenta; }
                                else { suma = suma + 0; }
                                if (banlancin2 != null)
                                { suma = suma + banlancin2.Valorcuenta; }
                                else { suma = suma + 0; }
                                suma = (suma2 + suma) / 2;

                                rakata.numerador = rakata.numerador.Replace(razoncitanum[i], suma.ToString());
                            }
                        
                        }
                        else
                        {
                            rakata.numerador = rakata.numerador.Replace(razoncitanum[i], "1+1");
                        }


                    }
                else if(!razoncitanum[i].Equals("365"))
                {
                    rakata.numerador = rakata.numerador.Replace(razoncitanum[i], "1+1");
                }
                   

                }

                //------------------------------------------------ Denominador
                 
                for (int i = 0; i < razoncitaden.Length; i++)
                {
                    var tipoCC = tipocuenta.FirstOrDefault(l => l.nomCuentaE.Equals(razoncitaden[i]));
                    if (tipoCC != null)
                    {
                        var cuentaX = catCuent.Where(x => x.nomCuentaEID == tipoCC.nomCuentaEID);
                        Console.WriteLine(cuentaX);
                        foreach (var c in cuentaX)
                        {  
                            var banlancin = balance.FirstOrDefault(n => n.Idcuenta == c.Idcuenta);
                          //  suma = suma + banlancin.Valorcuenta;
                            
                        }

                        // rakata.denominador = rakata.denominador.Replace(razoncitanum[i], suma.ToString());
                        rakata.denominador = rakata.denominador.Replace(razoncitaden[i], "1+1");

                    }
                    else if (razoncitaden[i].Contains("PROMEDIO"))
                    {

                        string divo = razoncitaden[i].Replace("PROMEDIO", "");
                        var tipoP = tipocuenta.FirstOrDefault(l => l.nomCuentaE.Equals(divo));

                        if (tipoP != null)
                        {
                            var cuentaZ = catCuent.Where(x => x.nomCuentaEID == tipoP.nomCuentaEID);
                            int banAnio = balance.OrderBy(n => n.Anio).FirstOrDefault().Anio;
                            int banAnio2 = balance.Where(n => n.Anio == banAnio + 1).FirstOrDefault().Anio;

                            foreach (var c in cuentaZ)
                            {



                                var banlancin = balance.Where(n => n.Anio == banAnio).FirstOrDefault(n => n.Idcuenta == c.Idcuenta);
                                var banlancin2 = balance.Where(n => n.Anio == banAnio2).FirstOrDefault(n => n.Idcuenta == c.Idcuenta);
                                //sumo el total de las cuentas del mismo tipo
                                if (banlancin != null)
                                { suma = suma + banlancin.Valorcuenta; }
                                else { suma = suma + 0; }
                                if (banlancin2 != null)
                                { suma = suma + banlancin2.Valorcuenta; }
                                else { suma = suma + 0; }
                                suma = (suma2 + suma) / 2;

                                rakata.denominador = rakata.denominador.Replace(razoncitaden[i], suma.ToString());



                            }

                        }
                        else
                        {
                            rakata.denominador = rakata.denominador.Replace(razoncitaden[i], "1+1");
                        }


                    }
                    else if (!razoncitaden[i].Equals("365"))
                    {
                        rakata.denominador = rakata.denominador.Replace(razoncitaden[i], "1+1");
                    }


                }


                double pas = parse.Parse("(" + rakata.numerador + ")" + "/" + "(" + rakata.denominador + ")");

                Resultado.Add(pas);
                
            }
            ViewData["Resultados"] = Resultado;
            var razonI = _context.Razon; 
            return View(await razonI.ToListAsync());
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
