﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sistema_de_Informes_de_Analisis_Financieros.Models;
using Sistema_de_Informes_de_Analisis_Financieros.ViewModels;

namespace Sistema_de_Informes_de_Analisis_Financieros
{
    public class RazonsController : Controller
    {
        private readonly ProyAnfContext _context;
        private readonly UserManager<Usuario> _userManager;

        public RazonsController(ProyAnfContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
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
            var usuario = this.User;
            Usuario u = _context.Users.Include(l => l.Idempresa).Where(l => l.UserName == usuario.Identity.Name).FirstOrDefault();
            var razon = _context.Razon.Where(r => r.idRazon == idRazon).FirstOrDefault();
            AnalisisRazonViewModel model = new AnalisisRazonViewModel()
            {
                nombreRazon = razon.nombreRazon,
                tipo = razon.tipo,
                signoDenominador = "",
                signoNumerador = "",
                numerador = new List<string>(),
                denominador = new List<string>(),
                valorDenA1 = 0,
                valorDenA2 = 0,
                valorNumA1 = 0,
                valorNumA2 = 0,
                promDen1 = -1,
                promDen2 = -1,
                promNum1 = -1,
                promNum2 = -1,
                resA1 = 0,
                resA2 = 0,
                resProm = 0,
                mensajeBase1 = "",
                mensajeBase2 = "",
                mensajeEmp1 = "",
                mensajeEmp2 = ""
            };
            model.numerador.Add(razon.numerador);
            model.denominador.Add(razon.denominador);

            //numerador
            if (razon.numerador.Split('+','-','/','*').Length > 1 && !razon.numerador.Equals("EFECTIVO + VALORES DE CORTO PLAZO"))
            {
                model.numerador = razon.numerador.Split('+', '-', '/', '*').ToList();
            }            
            //denominador
            if (razon.denominador.Split('+', '-', '/', '*').Length > 1 && !razon.numerador.Equals("EFECTIVO + VALORES DE CORTO PLAZO"))
            {
                model.denominador = razon.denominador.Split('+', '-', '/', '*').ToList();
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

            //Procesando todas las razones
            //switch (razon.nombreRazon)
            //{
            //    case "PRUEBA ACIDA":
            //        {

            //            break;
            //        }
            //}

            //Obteniendo los datos del numerador
            NomCuentaE numerador1, numerador2;
            int idNum1, idNum2;
            bool isProm1 = false, isProm2 = false, isTotal1 = false, isTotal2 = false;
            for (int i = 0; i < model.numerador.Count; i++)
            {                
                //Obteniendo datos de numerador 1
                if (i == 0)
                {
                    model.numerador[i] = model.numerador[i].Replace("COMPRAS", "COSTO DE LAS VENTAS").Trim();
                    //Verificando si es promedio
                    if (model.numerador[i].Contains("PROMEDIO"))
                    {
                        isProm1 = true;
                        model.numerador[i] = model.numerador[i].Replace("PROMEDIO", "").Trim();
                    }
                    //Verificando si es total
                    if (model.numerador[i].Contains("TOTAL"))
                    {
                        isTotal1 = true;
                        model.numerador[i] = model.numerador[i].Replace("TOTAL", "").Trim();
                        model.numerador[i] = model.numerador[i].Replace("ES", "").Trim();
                        model.numerador[i] = model.numerador[i].Replace("S", "").Trim();
                        model.numerador[i] = model.numerador[i].Replace("PAIVO", "PASIVO").Trim();                        
                    }
                    List<Catalogodecuenta> num1CuentasCatalogo = null;
                    if(isTotal1 || model.numerador[i].Equals("PATRIMONIO"))
                    {                        
                        var num1 = _context.Cuenta
                            .Where(n => n.Nomcuenta == model.numerador[i])
                            .FirstOrDefault();
                        if(num1 != null)
                        {
                            idNum1 = num1.Idcuenta;
                            num1CuentasCatalogo = _context.Catalogodecuenta
                                .Where(l => l.Idcuenta == idNum1)
                                .Where(l => l.Idempresa == u.Idempresa.Idempresa)
                                .ToList();
                        }                        
                    }
                    else
                    {
                        //Obteniendo Id de NomCuentaID para el numerador 1
                        numerador1 = _context.NomCuentaE
                            .Where(n => n.nomCuentaE == model.numerador[i])
                            .FirstOrDefault();
                        if (numerador1 != null)
                        {
                            idNum1 = numerador1.nomCuentaEID;
                            //Obtenemos las cuentas que coincidan con ese id
                            num1CuentasCatalogo = _context.Catalogodecuenta
                                .Where(l => l.nomCuentaEID == idNum1)
                                .Where(l => l.Idempresa == u.Idempresa.Idempresa)
                                .ToList();
                        }                            
                    }                
                    if(num1CuentasCatalogo != null)
                    {
                        //Obtenemos el codigo base de esas cuentas
                        string codigobase = "99999999999999999999";
                        for (int j = 0; j < num1CuentasCatalogo.Count; j++)
                        {
                            if (num1CuentasCatalogo[j].Codcuentacatalogo.Length < codigobase.Length)
                            {
                                codigobase = num1CuentasCatalogo[j].Codcuentacatalogo;
                            }
                        }
                        //consiguiendo todas las cuentas de la empresa
                        var cuentasCatalogo = _context.Catalogodecuenta
                            .Where(l => l.Idempresa == u.Idempresa.Idempresa)
                            .ToList();
                        //consiguiendo los valores de las cuentas necesarias
                        foreach (var cuenta in cuentasCatalogo)
                        {
                            if (cuenta.Codcuentacatalogo.StartsWith(codigobase))
                            {
                                //obtener valores si están en el balance
                                var valoresBalance = _context.Valoresdebalance
                                    .Where(l => l.Idcuenta == cuenta.Idcuenta)
                                    .Where(l => l.Idempresa == u.Idempresa.Idempresa)
                                    .ToList();
                                //Sumar valores al total
                                for (int j = 0; j < valoresBalance.Count; j++)
                                {
                                    if (j == 0)
                                    {
                                        model.anio1 = valoresBalance[j].Anio;
                                        model.valorNumA1 += valoresBalance[j].Valorcuenta;
                                    }
                                    else
                                    {
                                        model.anio2 = valoresBalance[j].Anio;
                                        model.valorNumA2 += valoresBalance[j].Valorcuenta;
                                    }
                                }
                                //obtener valores si están en el estado de resultados
                                var valoresEstado = _context.Valoresestado
                                    .Where(l => l.Idcuenta == cuenta.Idcuenta)
                                    .Where(l => l.Idempresa == u.Idempresa.Idempresa)
                                    .ToList();
                                //Sumar valores al total
                                for (int j = 0; j < valoresEstado.Count; j++)
                                {
                                    if (j == 0)
                                    {
                                        model.anio1 = valoresEstado[j].Anio;
                                        model.valorNumA1 += valoresEstado[j].Valorestado;
                                    }
                                    else
                                    {
                                        model.anio2 = valoresEstado[j].Anio;
                                        model.valorNumA2 += valoresEstado[j].Valorestado;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (int.TryParse(model.numerador[i], out _))
                        {
                            model.valorNumA1 = int.Parse(model.numerador[i]);
                            model.valorNumA2 = int.Parse(model.numerador[i]);
                        }
                        //obtener valores si están en el estado de resultados
                        var valoresEstado = _context.Valoresestado
                            .Where(l => l.Nombrevalore.ToUpper().Contains(model.numerador[i]))
                            .Where(l => l.Idempresa == u.Idempresa.Idempresa)
                            .ToList();
                        if (model.numerador[i].Equals("UTILIDAD OPERATIVA") && valoresEstado.Count == 0)
                        {
                            valoresEstado = _context.Valoresestado
                            .Where(l => l.Nombrevalore.ToUpper().Contains("OPERA"))
                            .Where(l => l.Nombrevalore.ToUpper().Contains("UTILIDAD"))
                            .Where(l => l.Idempresa == u.Idempresa.Idempresa)
                            .ToList();
                        }
                        //Sumar valores al total
                        for (int j = 0; j < valoresEstado.Count; j++)
                        {
                            if (j == 0)
                            {
                                model.anio1 = valoresEstado[j].Anio;
                                model.valorNumA1 += valoresEstado[j].Valorestado;
                            }
                            else
                            {
                                model.anio2 = valoresEstado[j].Anio;
                                model.valorNumA2 += valoresEstado[j].Valorestado;
                            }
                        }                        
                    }                                       
                   if (isProm1)
                   {
                        model.promNum1 = (model.valorNumA1 + model.valorNumA2) / 2;
                   }
                }
                if (i == 1)
                {
                    model.numerador[i] = model.numerador[i].Replace("COMPRAS", "COSTO DE LAS VENTAS").Trim();
                    //Verificando si es promedio
                    if (model.numerador[i].Contains("PROMEDIO"))
                    {
                        isProm2 = true;
                        model.numerador[i] = model.numerador[i].Replace("PROMEDIO", "").Trim();
                    }
                    //Verificando si es total
                    if (model.numerador[i].Contains("TOTAL"))
                    {
                        isTotal1 = true;
                        model.numerador[i] = model.numerador[i].Replace("TOTAL", "").Trim();
                        model.numerador[i] = model.numerador[i].Replace("ES", "").Trim();
                        model.numerador[i] = model.numerador[i].Replace("S", "").Trim();
                        model.numerador[i] = model.numerador[i].Replace("PAIVO", "PASIVO").Trim();
                    }
                    List<Catalogodecuenta> num2CuentasCatalogo = null;
                    if (isTotal1 || model.numerador[i].Equals("PATRIMONIO"))
                    {
                        var num2 = _context.Cuenta
                            .Where(n => n.Nomcuenta == model.numerador[i])
                            .FirstOrDefault();
                        if (num2 != null)
                        {
                            idNum2 = num2.Idcuenta;
                            num2CuentasCatalogo = _context.Catalogodecuenta
                                .Where(l => l.Idcuenta == idNum2)
                                .Where(l => l.Idempresa == u.Idempresa.Idempresa)
                                .ToList();
                        }
                    }
                    else
                    {
                        //Obteniendo Id de NomCuentaID para el numerador 1
                        numerador2 = _context.NomCuentaE
                            .Where(n => n.nomCuentaE == model.numerador[i])
                            .FirstOrDefault();
                        if (numerador2 != null)
                        {
                            idNum2 = numerador2.nomCuentaEID;
                            //Obtenemos las cuentas que coincidan con ese id
                            num2CuentasCatalogo = _context.Catalogodecuenta
                                .Where(l => l.nomCuentaEID == idNum2)
                                .Where(l => l.Idempresa == u.Idempresa.Idempresa)
                                .ToList();
                        }
                    }
                    if(num2CuentasCatalogo != null)
                    {
                        //Obtenemos el codigo base de esas cuentas
                        string codigobase = "99999999999999999999";
                        for (int j = 0; j < num2CuentasCatalogo.Count; j++)
                        {
                            if (num2CuentasCatalogo[j].Codcuentacatalogo.Length < codigobase.Length)
                            {
                                codigobase = num2CuentasCatalogo[j].Codcuentacatalogo;
                            }
                        }
                        //consiguiendo todas las cuentas de la empresa
                        var cuentasCatalogo = _context.Catalogodecuenta
                            .Where(l => l.Idempresa == u.Idempresa.Idempresa)
                            .ToList();
                        //consiguiendo los valores de las cuentas necesarias
                        foreach (var cuenta in cuentasCatalogo)
                        {
                            if (cuenta.Codcuentacatalogo.StartsWith(codigobase))
                            {
                                //obtener valores si están en el balance
                                var valoresBalance = _context.Valoresdebalance
                                    .Where(l => l.Idcuenta == cuenta.Idcuenta)
                                    .Where(l => l.Idempresa == u.Idempresa.Idempresa)
                                    .ToList();
                                //Sumar valores al total
                                for (int j = 0; j < valoresBalance.Count; j++)
                                {
                                    if (j == 0)
                                    {
                                        model.anio1 = valoresBalance[j].Anio;
                                        model.valorNum2A1 += valoresBalance[j].Valorcuenta;
                                    }
                                    else
                                    {
                                        model.anio2 = valoresBalance[j].Anio;
                                        model.valorNum2A2 += valoresBalance[j].Valorcuenta;
                                    }
                                }
                                //obtener valores si están en el estado de resultados
                                var valoresEstado = _context.Valoresestado
                                    .Where(l => l.Idcuenta == cuenta.Idcuenta)
                                    .Where(l => l.Idempresa == u.Idempresa.Idempresa)
                                    .ToList();
                                //Sumar valores al total
                                for (int j = 0; j < valoresEstado.Count; j++)
                                {
                                    if (j == 0)
                                    {
                                        model.anio1 = valoresEstado[j].Anio;
                                        model.valorNum2A1 += valoresEstado[j].Valorestado;
                                    }
                                    else
                                    {
                                        model.anio2 = valoresEstado[j].Anio;
                                        model.valorNum2A2 += valoresEstado[j].Valorestado;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (int.TryParse(model.numerador[i], out _))
                        {
                            model.valorNum2A1 = int.Parse(model.numerador[i]);
                            model.valorNum2A2 = int.Parse(model.numerador[i]);
                        }
                        //obtener valores si están en el estado de resultados
                        var valoresEstado = _context.Valoresestado
                            .Where(l => l.Nombrevalore.ToUpper().Contains(model.numerador[i]))
                            .Where(l => l.Idempresa == u.Idempresa.Idempresa)
                            .ToList();
                        if (model.numerador[i].Equals("UTILIDAD OPERATIVA") && valoresEstado.Count == 0)
                        {
                            valoresEstado = _context.Valoresestado
                            .Where(l => l.Nombrevalore.ToUpper().Contains("OPERA"))
                            .Where(l => l.Nombrevalore.ToUpper().Contains("UTILIDAD"))
                            .Where(l => l.Idempresa == u.Idempresa.Idempresa)
                            .ToList();
                        }
                        //Sumar valores al total
                        for (int j = 0; j < valoresEstado.Count; j++)
                        {
                            if (j == 0)
                            {
                                model.anio1 = valoresEstado[j].Anio;
                                model.valorNum2A1 += valoresEstado[j].Valorestado;
                            }
                            else
                            {
                                model.anio2 = valoresEstado[j].Anio;
                                model.valorNum2A2 += valoresEstado[j].Valorestado;
                            }
                        }                                                
                    }                    
                    if (isProm2)
                    {
                        model.promNum2 = (model.valorNum2A1 + model.valorNum2A2) / 2;
                    }
                }
            }
            //Obteniendo los datos del denominador
            NomCuentaE denominador1, denominador2;
            int idDen1, idDen2;
            isProm1 = false; isProm2 = false; isTotal1 = false; isTotal2 = false;
            for (int i = 0; i < model.denominador.Count; i++)
            {                
                //Obteniendo datos de numerador 1
                if (i == 0)
                {
                    model.denominador[i] = model.denominador[i].Replace("COMPRAS", "COSTO DE LAS VENTAS").Trim();
                    //Verificando si es promedio
                    if (model.denominador[i].Contains("PROMEDIO"))
                    {
                        isProm1 = true;
                        model.denominador[i] = model.denominador[i].Replace("PROMEDIO", "").Trim();
                    }
                    //Verificando si es total
                    if (model.denominador[i].Contains("TOTAL"))
                    {
                        isTotal1 = true;
                        model.denominador[i] = model.denominador[i].Replace("TOTAL", "").Trim();
                        model.denominador[i] = model.denominador[i].Replace("ES", "").Trim();
                        model.denominador[i] = model.denominador[i].Replace("S", "").Trim();
                        model.numerador[i] = model.numerador[i].Replace("PAIVO", "PASIVO").Trim();
                    }
                    List<Catalogodecuenta> num1CuentasCatalogo = null;
                    if (isTotal1 || model.denominador[i].Equals("PATRIMONIO"))
                    {
                        var num1 = _context.Cuenta
                            .Where(n => n.Nomcuenta == model.denominador[i])
                            .FirstOrDefault();
                        if (num1 != null)
                        {
                            idDen1 = num1.Idcuenta;
                            num1CuentasCatalogo = _context.Catalogodecuenta
                                .Where(l => l.Idcuenta == idDen1)
                                .Where(l => l.Idempresa == u.Idempresa.Idempresa)
                                .ToList();
                        }
                    }
                    else
                    {
                        //Obteniendo Id de NomCuentaID para el denominador 1
                        denominador1 = _context.NomCuentaE
                            .Where(n => n.nomCuentaE == model.denominador[i])
                            .FirstOrDefault();
                        if (denominador1 != null)
                        {
                            idDen1 = denominador1.nomCuentaEID;
                            //Obtenemos las cuentas que coincidan con ese id
                            num1CuentasCatalogo = _context.Catalogodecuenta
                                .Where(l => l.nomCuentaEID == idDen1)
                                .Where(l => l.Idempresa == u.Idempresa.Idempresa)
                                .ToList();
                        }
                    }
                    if(num1CuentasCatalogo != null)
                    {
                        //Obtenemos el codigo base de esas cuentas
                        string codigobase = "99999999999999999999";
                        for (int j = 0; j < num1CuentasCatalogo.Count; j++)
                        {
                            if (num1CuentasCatalogo[j].Codcuentacatalogo.Length < codigobase.Length)
                            {
                                codigobase = num1CuentasCatalogo[j].Codcuentacatalogo;
                            }
                        }
                        //consiguiendo todas las cuentas de la empresa
                        var cuentasCatalogo = _context.Catalogodecuenta
                            .Where(l => l.Idempresa == u.Idempresa.Idempresa)
                            .ToList();
                        //consiguiendo los valores de las cuentas necesarias
                        foreach (var cuenta in cuentasCatalogo)
                        {
                            if (cuenta.Codcuentacatalogo.StartsWith(codigobase))
                            {
                                //obtener valores si están en el balance
                                var valoresBalance = _context.Valoresdebalance
                                    .Where(l => l.Idcuenta == cuenta.Idcuenta)
                                    .Where(l => l.Idempresa == u.Idempresa.Idempresa)
                                    .ToList();
                                //Sumar valores al total
                                for (int j = 0; j < valoresBalance.Count; j++)
                                {
                                    if (j == 0)
                                    {
                                        model.anio1 = valoresBalance[j].Anio;
                                        model.valorDenA1 += valoresBalance[j].Valorcuenta;
                                    }
                                    else
                                    {
                                        model.anio2 = valoresBalance[j].Anio;
                                        model.valorDenA2 += valoresBalance[j].Valorcuenta;
                                    }
                                }
                                //obtener valores si están en el estado de resultados
                                var valoresEstado = _context.Valoresestado
                                    .Where(l => l.Idcuenta == cuenta.Idcuenta)
                                    .Where(l => l.Idempresa == u.Idempresa.Idempresa)
                                    .ToList();
                                //Sumar valores al total
                                for (int j = 0; j < valoresEstado.Count; j++)
                                {
                                    if (j == 0)
                                    {
                                        model.anio1 = valoresEstado[j].Anio;
                                        model.valorDenA1 += valoresEstado[j].Valorestado;
                                    }
                                    else
                                    {
                                        model.anio2 = valoresEstado[j].Anio;
                                        model.valorDenA2 += valoresEstado[j].Valorestado;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (int.TryParse(model.denominador[i], out _))
                        {
                            model.valorDenA1 = int.Parse(model.denominador[i]);
                            model.valorDenA2 = int.Parse(model.denominador[i]);
                        }
                        //obtener valores si están en el estado de resultados
                        var valoresEstado = _context.Valoresestado
                            .Where(l => l.Nombrevalore.ToUpper().Contains(model.denominador[i]))
                            .Where(l => l.Idempresa == u.Idempresa.Idempresa)
                            .ToList();
                        if (model.denominador[i].Equals("UTILIDAD OPERATIVA") && valoresEstado.Count == 0)
                        {
                            valoresEstado = _context.Valoresestado
                            .Where(l => l.Nombrevalore.ToUpper().Contains("OPERA"))
                            .Where(l => l.Nombrevalore.ToUpper().Contains("UTILIDAD"))
                            .Where(l => l.Idempresa == u.Idempresa.Idempresa)
                            .ToList();
                        }
                        //Sumar valores al total
                        for (int j = 0; j < valoresEstado.Count; j++)
                        {
                            if (j == 0)
                            {
                                model.anio1 = valoresEstado[j].Anio;
                                model.valorDenA1 += valoresEstado[j].Valorestado;
                            }
                            else
                            {
                                model.anio2 = valoresEstado[j].Anio;
                                model.valorDenA2 += valoresEstado[j].Valorestado;
                            }
                        }
                    }
                    if (isProm1)
                    {
                        model.promDen1 = (model.valorDenA1 + model.valorDenA2) / 2;
                    }
                }
                if (i == 1)
                {
                    model.denominador[i] = model.denominador[i].Replace("COMPRAS", "COSTO DE LAS VENTAS").Trim();
                    //Verificando si es promedio
                    if (model.denominador[i].Contains("PROMEDIO"))
                    {
                        isProm2 = true;
                        model.denominador[i] = model.denominador[i].Replace("PROMEDIO", "").Trim();
                    }
                    //Verificando si es total
                    if (model.denominador[i].Contains("TOTAL"))
                    {
                        isTotal1 = true;
                        model.denominador[i] = model.denominador[i].Replace("TOTAL", "").Trim();
                        model.denominador[i] = model.denominador[i].Replace("ES", "").Trim();
                        model.denominador[i] = model.denominador[i].Replace("S", "").Trim();
                        model.numerador[i] = model.numerador[i].Replace("PAIVO", "PASIVO").Trim();
                    }
                    List<Catalogodecuenta> num2CuentasCatalogo = null;
                    if (isTotal1 || model.denominador[i].Equals("PATRIMONIO"))
                    {
                        var num1 = _context.Cuenta
                            .Where(n => n.Nomcuenta == model.denominador[i])
                            .FirstOrDefault();
                        if (num1 != null)
                        {
                            idDen2 = num1.Idcuenta;
                            num2CuentasCatalogo = _context.Catalogodecuenta
                                .Where(l => l.Idcuenta == idDen2)
                                .Where(l => l.Idempresa == u.Idempresa.Idempresa)
                                .ToList();
                        }
                    }
                    else
                    {
                        //Obteniendo Id de NomCuentaID para el denominador 1
                        denominador2 = _context.NomCuentaE
                            .Where(n => n.nomCuentaE == model.denominador[i])
                            .FirstOrDefault();
                        if (denominador2 != null)
                        {
                            idDen2 = denominador2.nomCuentaEID;
                            //Obtenemos las cuentas que coincidan con ese id
                            num2CuentasCatalogo = _context.Catalogodecuenta
                                .Where(l => l.nomCuentaEID == idDen2)
                                .Where(l => l.Idempresa == u.Idempresa.Idempresa)
                                .ToList();
                        }
                    }
                    if (num2CuentasCatalogo != null)
                    {
                        //Obtenemos el codigo base de esas cuentas
                        string codigobase = "99999999999999999999";
                        for (int j = 0; j < num2CuentasCatalogo.Count; j++)
                        {
                            if (num2CuentasCatalogo[j].Codcuentacatalogo.Length < codigobase.Length)
                            {
                                codigobase = num2CuentasCatalogo[j].Codcuentacatalogo;
                            }
                        }
                        //consiguiendo todas las cuentas de la empresa
                        var cuentasCatalogo = _context.Catalogodecuenta
                            .Where(l => l.Idempresa == u.Idempresa.Idempresa)
                            .ToList();
                        //consiguiendo los valores de las cuentas necesarias
                        foreach (var cuenta in cuentasCatalogo)
                        {
                            if (cuenta.Codcuentacatalogo.StartsWith(codigobase))
                            {
                                //obtener valores si están en el balance
                                var valoresBalance = _context.Valoresdebalance
                                    .Where(l => l.Idcuenta == cuenta.Idcuenta)
                                    .Where(l => l.Idempresa == u.Idempresa.Idempresa)
                                    .ToList();
                                //Sumar valores al total
                                for (int j = 0; j < valoresBalance.Count; j++)
                                {
                                    if (j == 0)
                                    {
                                        model.anio1 = valoresBalance[j].Anio;
                                        model.valorDen2A1 += valoresBalance[j].Valorcuenta;
                                    }
                                    else
                                    {
                                        model.anio2 = valoresBalance[j].Anio;
                                        model.valorDen2A2 += valoresBalance[j].Valorcuenta;
                                    }
                                }
                                //obtener valores si están en el estado de resultados
                                var valoresEstado = _context.Valoresestado
                                    .Where(l => l.Idcuenta == cuenta.Idcuenta)
                                    .Where(l => l.Idempresa == u.Idempresa.Idempresa)
                                    .ToList();
                                //Sumar valores al total
                                for (int j = 0; j < valoresEstado.Count; j++)
                                {
                                    if (j == 0)
                                    {
                                        model.anio1 = valoresEstado[j].Anio;
                                        model.valorDen2A1 += valoresEstado[j].Valorestado;
                                    }
                                    else
                                    {
                                        model.anio2 = valoresEstado[j].Anio;
                                        model.valorDen2A2 += valoresEstado[j].Valorestado;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (int.TryParse(model.denominador[i], out _))
                        {
                            model.valorDen2A1 = int.Parse(model.denominador[i]);
                            model.valorDen2A2 = int.Parse(model.denominador[i]);
                        }
                        //obtener valores si están en el estado de resultados
                        var valoresEstado = _context.Valoresestado
                            .Where(l => l.Nombrevalore.ToUpper().Contains(model.denominador[i]))
                            .Where(l => l.Idempresa == u.Idempresa.Idempresa)
                            .ToList();
                        if (model.denominador[i].Equals("UTILIDAD OPERATIVA") && valoresEstado.Count == 0)
                        {
                            valoresEstado = _context.Valoresestado
                            .Where(l => l.Nombrevalore.ToUpper().Contains("OPERA"))
                            .Where(l => l.Nombrevalore.ToUpper().Contains("UTILIDAD"))
                            .Where(l => l.Idempresa == u.Idempresa.Idempresa)
                            .ToList();
                        }
                        //Sumar valores al total
                        for (int j = 0; j < valoresEstado.Count; j++)
                        {
                            if (j == 0)
                            {
                                model.anio1 = valoresEstado[j].Anio;
                                model.valorDen2A1 += valoresEstado[j].Valorestado;
                            }
                            else
                            {
                                model.anio2 = valoresEstado[j].Anio;
                                model.valorDen2A2 += valoresEstado[j].Valorestado;
                            }
                        }
                    }
                    if (isProm2)
                    {
                        model.promDen2 = (model.valorDen2A1 + model.valorDen2A2) / 2;
                    }
                }
            }

            //Realizando los calculos
            double totalNum1, totalNum2, totalDen1, totalDen2;
            switch (model.signoNumerador)
            {
                case " + ":
                    {
                        if(model.promNum1 == -1 && model.promNum2 == -1)
                        {
                            totalNum1 = model.valorNumA1 + model.valorNum2A1;
                            totalNum2 = model.valorNumA2 + model.valorNum2A2;
                        }                        
                        else
                        {
                            if(model.promNum1 == -1 && model.promNum2 != -1)
                            {
                                totalNum1 = model.valorNumA1 + model.promNum2;
                                totalNum2 = model.valorNumA2 + model.promNum2;
                            }
                            else if (model.promNum2 == -1 && model.promNum1 != -1)
                            {
                                totalNum1 = model.valorNum2A1 + model.promNum1;
                                totalNum2 = model.valorNum2A2 + model.promNum1;
                            }
                            else
                            {
                                totalNum1 = model.promNum1 + model.promNum2;
                                totalNum2 = model.promNum1 + model.promNum2;
                            }
                        }
                        break;
                    }
                case " - ":
                    {
                        if (model.promNum1 == -1 && model.promNum2 == -1)
                        {
                            totalNum1 = model.valorNumA1 - model.valorNum2A1;
                            totalNum2 = model.valorNumA2 - model.valorNum2A2;
                        }
                        else
                        {
                            if (model.promNum1 == -1 && model.promNum2 != -1)
                            {
                                totalNum1 = model.valorNumA1 - model.promNum2;
                                totalNum2 = model.valorNumA2 - model.promNum2;
                            }
                            else if (model.promNum2 == -1 && model.promNum1 != -1)
                            {
                                totalNum1 = model.valorNum2A1 - model.promNum1;
                                totalNum2 = model.valorNum2A2 - model.promNum1;
                            }
                            else
                            {
                                totalNum1 = model.promNum1 - model.promNum2;
                                totalNum2 = model.promNum1 - model.promNum2;
                            }
                        }
                        break;
                    }
                case " X ":
                    {
                        if (model.promNum1 == -1 && model.promNum2 == -1)
                        {
                            totalNum1 = model.valorNumA1 * model.valorNum2A1;
                            totalNum2 = model.valorNumA2 * model.valorNum2A2;
                        }
                        else
                        {
                            if (model.promNum1 == -1 && model.promNum2 != -1)
                            {
                                totalNum1 = model.valorNumA1 * model.promNum2;
                                totalNum2 = model.valorNumA2 * model.promNum2;
                            }
                            else if (model.promNum2 == -1 && model.promNum1 != -1)
                            {
                                totalNum1 = model.valorNum2A1 * model.promNum1;
                                totalNum2 = model.valorNum2A2 * model.promNum1;
                            }
                            else
                            {
                                totalNum1 = model.promNum1 * model.promNum2;
                                totalNum2 = model.promNum1 * model.promNum2;
                            }
                        }
                        break;
                    }
                case " / ":
                    {
                        if (model.promNum1 == -1 && model.promNum2 == -1)
                        {
                            totalNum1 = model.valorNumA1 / model.valorNum2A1;
                            totalNum2 = model.valorNumA2 / model.valorNum2A2;
                        }
                        else
                        {
                            if (model.promNum1 == -1 && model.promNum2 != -1)
                            {
                                totalNum1 = model.valorNumA1 / model.promNum2;
                                totalNum2 = model.valorNumA2 / model.promNum2;
                            }
                            else if (model.promNum2 == -1 && model.promNum1 != -1)
                            {
                                totalNum1 = model.valorNum2A1 / model.promNum1;
                                totalNum2 = model.valorNum2A2 / model.promNum1;
                            }
                            else
                            {
                                totalNum1 = model.promNum1 / model.promNum2;
                                totalNum2 = model.promNum1 / model.promNum2;
                            }
                        }
                        break;
                    }
                default:
                    {
                        if(model.promNum1 != -1)
                        {
                            totalNum1 = model.promNum1;
                            totalNum2 = model.promNum1;
                        }
                        else
                        {
                            totalNum1 = model.valorNumA1;
                            totalNum2 = model.valorNumA2;
                        }                        
                        break;
                    }
            }
            switch (model.signoDenominador)
            {
                case " + ":
                    {
                        if (model.promDen1 == -1 && model.promDen2 == -1)
                        {
                            totalDen1 = model.valorDenA1 + model.valorDen2A2;
                            totalDen2 = model.valorDenA2 + model.valorDen2A2;
                        }
                        else
                        {
                            if (model.promDen1 == -1 && model.promDen2 != -1)
                            {
                                totalDen1 = model.valorDenA1 + model.promDen2;
                                totalDen2 = model.valorDenA2 + model.promDen2;
                            }
                            else if (model.promDen2 == -1 && model.promDen1 != -1)
                            {
                                totalDen1 = model.valorDen2A2 + model.promDen1;
                                totalDen2 = model.valorDen2A2 + model.promDen1;
                            }
                            else
                            {
                                totalDen1 = model.promDen1 + model.promDen2;
                                totalDen2 = model.promDen1 + model.promDen2;
                            }
                        }
                        break;
                    }
                case " - ":
                    {
                        if (model.promDen1 == -1 && model.promDen2 == -1)
                        {
                            totalDen1 = model.valorDenA1 - model.valorDen2A2;
                            totalDen2 = model.valorDenA2 - model.valorDen2A2;
                        }
                        else
                        {
                            if (model.promDen1 == -1 && model.promDen2 != -1)
                            {
                                totalDen1 = model.valorDenA1 - model.promDen2;
                                totalDen2 = model.valorDenA2 - model.promDen2;
                            }
                            else if (model.promDen2 == -1 && model.promDen1 != -1)
                            {
                                totalDen1 = model.valorDen2A2 - model.promDen1;
                                totalDen2 = model.valorDen2A2 - model.promDen1;
                            }
                            else
                            {
                                totalDen1 = model.promDen1 - model.promDen2;
                                totalDen2 = model.promDen1 - model.promDen2;
                            }
                        }
                        break;
                    }
                case " X ":
                    {
                        if (model.promDen1 == -1 && model.promDen2 == -1)
                        {
                            totalDen1 = model.valorDenA1 * model.valorDen2A2;
                            totalDen2 = model.valorDenA2 * model.valorDen2A2;
                        }
                        else
                        {
                            if (model.promDen1 == -1 && model.promDen2 != -1)
                            {
                                totalDen1 = model.valorDenA1 * model.promDen2;
                                totalDen2 = model.valorDenA2 * model.promDen2;
                            }
                            else if (model.promDen2 == -1 && model.promDen1 != -1)
                            {
                                totalDen1 = model.valorDen2A2 * model.promDen1;
                                totalDen2 = model.valorDen2A2 * model.promDen1;
                            }
                            else
                            {
                                totalDen1 = model.promDen1 * model.promDen2;
                                totalDen2 = model.promDen1 * model.promDen2;
                            }
                        }
                        break;
                    }
                case " / ":
                    {
                        if (model.promDen1 == -1 && model.promDen2 == -1)
                        {
                            totalDen1 = model.valorDenA1 / model.valorDen2A2;
                            totalDen2 = model.valorDenA2 / model.valorDen2A2;
                        }
                        else
                        {
                            if (model.promDen1 == -1 && model.promDen2 != -1)
                            {
                                totalDen1 = model.valorDenA1 / model.promDen2;
                                totalDen2 = model.valorDenA2 / model.promDen2;
                            }
                            else if (model.promDen2 == -1 && model.promDen1 != -1)
                            {
                                totalDen1 = model.valorDen2A2 / model.promDen1;
                                totalDen2 = model.valorDen2A2 / model.promDen1;
                            }
                            else
                            {
                                totalDen1 = model.promDen1 / model.promDen2;
                                totalDen2 = model.promDen1 / model.promDen2;
                            }
                        }
                        break;
                    }
                default:
                    {
                        totalDen1 = model.valorDenA1;
                        totalDen2 = model.valorDenA2;
                        break;
                    }
            }

            model.resA1 = totalNum1 / totalDen1;
            model.resA2 = totalNum2 / totalDen2;

            //Obteniendo los ratios base
            var empresa = _context.Empresa
                            .Where(l => l.Idempresa == u.Idempresa.Idempresa)
                            .FirstOrDefault();
            var razonAnalizada = _context.Ratio.Where(l => l.Nombreratiob == model.nombreRazon).FirstOrDefault();
            var razonSector = _context.Ratiobasesector
                .Where(l => l.Idratio == razonAnalizada.Idratio)
                .Where(l => l.Idsector == empresa.Idsector)
                .FirstOrDefault();
            double valorSector = 0;
            if (razonSector == null)
            {
                valorSector = 0;
            }
            else
            {
                valorSector = razonSector.Valorratiob;
            }
            
            model.valorSector = valorSector;
            double menos5 = valorSector - valorSector * 0.05;
            double mas5 = valorSector + valorSector * 0.05;

            //ver cual año es mayor
            double valorMasActual;
            if(model.anio1 > model.anio2)
            {
                valorMasActual = model.resA1;
            }
            else if (model.anio2 > model.anio1)
            {
                valorMasActual = model.resA2;
            }
            else
            {
                valorMasActual = model.resA1;
            }

            //Guardando el valor de la razón de la empresa en la base
            Ratioempresa nuevo = new Ratioempresa
            {
                Idempresa = empresa.Idempresa,
                Idratio = razonAnalizada.Idratio,
                Valorratioempresa = valorMasActual
            };
            var valorEmpresa = _context.Ratioempresa
                                .Where(l => l.Idempresa == empresa.Idempresa)
                                .Where(l => l.Idratio == razonAnalizada.Idratio)
                                .FirstOrDefault();
            if (valorEmpresa == null)
            {
                _context.Add(nuevo);
            }
            else
            {
                valorEmpresa.Valorratioempresa = valorMasActual;
                _context.Update(valorEmpresa);
            }
            _context.SaveChanges();

            //Pasando mensajes
            //Año 1 Base
            if (model.resA1 > menos5 && model.resA1 < mas5)
            {
                var mensaje = _context.MensajesAnalisis
                    .Where(l => l.idRatio == razonAnalizada.Idratio)
                    .FirstOrDefault();
                if(mensaje == null)
                {
                    model.mensajeBase1 = "No hay mensaje en la base para esta razón";
                }
                else
                {
                    model.mensajeBase1 = mensaje.mensajeIgualBase;
                }                
            }
            else if (model.resA1 < valorSector)
            {                
                var mensaje = _context.MensajesAnalisis
                    .Where(l => l.idRatio == razonAnalizada.Idratio)
                    .FirstOrDefault();
                if (mensaje == null)
                {
                    model.mensajeBase1 = "No hay mensaje en la base para esta razón";
                }
                else
                {
                    model.mensajeBase1 = mensaje.mensajeMenorBase;
                }
            }
            else
            {
                var mensaje = _context.MensajesAnalisis
                    .Where(l => l.idRatio == razonAnalizada.Idratio)
                    .FirstOrDefault();
                if (mensaje == null)
                {
                    model.mensajeBase1 = "No hay mensaje en la base para esta razón";
                }
                else
                {
                    model.mensajeBase1 = mensaje.mensajeMayorBase;
                }
            }
            //Año 2
            if (model.resA2 > valorSector - valorSector * 0.05 && model.resA2 < valorSector + valorSector * 0.05)
            {
                var mensaje = _context.MensajesAnalisis
                    .Where(l => l.idRatio == razonAnalizada.Idratio)
                    .FirstOrDefault();
                if (mensaje == null)
                {
                    model.mensajeBase2 = "No hay mensaje en la base para esta razón";
                }
                else
                {
                    model.mensajeBase2 = mensaje.mensajeIgualBase;
                }
            }
            else if (model.resA2 < valorSector)
            {
                var mensaje = _context.MensajesAnalisis
                    .Where(l => l.idRatio == razonAnalizada.Idratio)
                    .FirstOrDefault();
                if (mensaje == null)
                {
                    model.mensajeBase2 = "No hay mensaje en la base para esta razón";
                }
                else
                {
                    model.mensajeBase2 = mensaje.mensajeMenorBase;
                }
            }
            else
            {
                var mensaje = _context.MensajesAnalisis
                    .Where(l => l.idRatio == razonAnalizada.Idratio)
                    .FirstOrDefault();
                if (mensaje == null)
                {
                    model.mensajeBase2 = "No hay mensaje en la base para esta razón";
                }
                else
                {
                    model.mensajeBase2 = mensaje.mensajeMayorBase;
                }
            }

            //Obteniendo promedio de empresas
            var empresasSector = _context.Empresa.Where(l => l.Idsector == empresa.Idsector).ToList();
            List<Ratioempresa> ratioempresas = new List<Ratioempresa>();
            foreach(var empresaSector in empresasSector)
            {                
                var ratioempresa = _context.Ratioempresa
                    .Where(l => l.Idempresa == empresaSector.Idempresa)
                    .Where(l => l.Idratio == razonAnalizada.Idratio)
                    .FirstOrDefault();

                if (ratioempresa != null)
                {
                    ratioempresas.Add(ratioempresa);
                }
            }

            double totalEmpresas = 0;
            int numeroEmpresas = ratioempresas.Count;
            foreach(var ratio in ratioempresas)
            {
                totalEmpresas += ratio.Valorratioempresa;
            }
            model.valorEmpresa = totalEmpresas / numeroEmpresas;
            menos5 = model.valorEmpresa - model.valorEmpresa * 0.05;
            mas5 = model.valorEmpresa + model.valorEmpresa * 0.05;
            //enviando mensajes
            if (model.resA1 > menos5 && model.resA1 < mas5)
            {
                var mensaje = _context.MensajesAnalisis
                    .Where(l => l.idRatio == razonAnalizada.Idratio)
                    .FirstOrDefault();
                if (mensaje == null)
                {
                    model.mensajeEmp1 = "No hay mensaje en la base para esta razón";
                }
                else
                {
                    model.mensajeEmp1 = mensaje.mensajeIgualEmp;
                }
            }
            else if (model.resA1 < model.valorEmpresa)
            {
                var mensaje = _context.MensajesAnalisis
                    .Where(l => l.idRatio == razonAnalizada.Idratio)
                    .FirstOrDefault();
                if (mensaje == null)
                {
                    model.mensajeEmp1 = "No hay mensaje en la base para esta razón";
                }
                else
                {
                    model.mensajeEmp1 = mensaje.mensajeMenorEmp;
                }
            }
            else
            {
                var mensaje = _context.MensajesAnalisis
                    .Where(l => l.idRatio == razonAnalizada.Idratio)
                    .FirstOrDefault();
                if (mensaje == null)
                {
                    model.mensajeEmp1 = "No hay mensaje en la base para esta razón";
                }
                else
                {
                    model.mensajeEmp1 = mensaje.mensajeMayorEmp;
                }
            }
            //Año 2
            if (model.resA2 > menos5 && model.resA2 < mas5)
            {
                var mensaje = _context.MensajesAnalisis
                    .Where(l => l.idRatio == razonAnalizada.Idratio)
                    .FirstOrDefault();
                if (mensaje == null)
                {
                    model.mensajeEmp2 = "No hay mensaje en la base para esta razón";
                }
                else
                {
                    model.mensajeEmp2 = mensaje.mensajeIgualEmp;
                }
            }
            else if (model.resA2 < model.valorEmpresa)
            {
                var mensaje = _context.MensajesAnalisis
                    .Where(l => l.idRatio == razonAnalizada.Idratio)
                    .FirstOrDefault();
                if (mensaje == null)
                {
                    model.mensajeEmp2 = "No hay mensaje en la base para esta razón";
                }
                else
                {
                    model.mensajeEmp2 = mensaje.mensajeMenorEmp;
                }
            }
            else
            {
                var mensaje = _context.MensajesAnalisis
                    .Where(l => l.idRatio == razonAnalizada.Idratio)
                    .FirstOrDefault();
                if (mensaje == null)
                {
                    model.mensajeEmp2 = "No hay mensaje en la base para esta razón";
                }
                else
                {
                    model.mensajeEmp2 = mensaje.mensajeMayorEmp;
                }
            }

            return View(model);
        }

        

    }
}
