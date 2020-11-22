using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using OfficeOpenXml;
using Sistema_de_Informes_de_Analisis_Financieros.Models;
using Sistema_de_Informes_de_Analisis_Financieros.ViewModels;

namespace Sistema_de_Informes_de_Analisis_Financieros.Controllers
{
    [Authorize]
    public class ValoresBalanceController : Controller
    {
        private readonly ProyAnfContext _context;
        private readonly UserManager<Usuario> userManager;
        //Declaracion de constantes
        const int COD_FORMATO_INVALIDO = 1; //Cuando las ctas de total en el balance no tienen el nombre estandar
        const int COD_BALANCE_DESCUADRADO = 2; //Cuando el balance está descuadrado
        const int COD_VALORES_EXITO = 3; //Cuando todos los valores se subieron con éxito

        public ValoresBalanceController(ProyAnfContext context, UserManager<Usuario> user)
        {
            _context = context;
            this.userManager = user;
        }        

        public async Task<string> GuardarBalance(int IdEmpresa, SubirBalance subirBalance, IFormFile files)
        {

            List<BalanceViewModel> listFilasBalance = new List<BalanceViewModel>();
            List<BalanceViewModel> listFilasBalance2 = new List<BalanceViewModel>();
            string mensaje = "Archivo subido con éxito.";            

            if (files == null || files.Length <= 0)
            {
                return "El archivo subido es inválido, intentelo de nuevo.";
            }

            if (!(Path.GetExtension(files.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase)))
            {
                return "Solo se aceptan archivos de tipo Excel con extensión .xlsx";
            }
            //Verificar que la empresa tiene un catalago subido
            var catalogo = _context.Catalogodecuenta.Count(a => a.Idempresa == IdEmpresa);
            if (catalogo > 0)
            {
                foreach (var anio in subirBalance.anios)
                {
                    /*Llamar método para obtener la letra de columna y número de fila de las celdas*/
                    IEnumerable<string> s = SplitAlpha(subirBalance.celdaCuenta);
                    int numCeldaCuenta = int.Parse(s.Last()); //Obtengo # de fila de las cuentas

                    string celValA1 = anio.celdaAnio;

                    s = SplitAlpha(celValA1);
                    int numCeldaAnio = int.Parse(s.Last()); //Obtengo # de fila de los valores
                    if (!(numCeldaAnio == numCeldaCuenta))
                    {
                        return "Los nombres de cuenta y los valores de las mismas deben estar en la misma fila";
                    }

                    //Verificando que no existan datos para ese año
                    if (!(_context.Valoresdebalance.Any(a => a.Anio == anio.anio && a.Idempresa == IdEmpresa)))
                    {
                        listFilasBalance = await LeerExcel(files, subirBalance.hoja, subirBalance.celdaCuenta, anio.celdaAnio, anio.anio);

                        if (listFilasBalance.Count == 0)
                        {
                            return "El archivo de excel está vacio";
                        }

                        if ((await VerificarBalance(IdEmpresa, listFilasBalance)) == COD_BALANCE_DESCUADRADO)
                        {
                            return "Balance para el año " + subirBalance.anios[0].anio + "descuadrado";
                        }
                        if ((await VerificarBalance(IdEmpresa, listFilasBalance)) == COD_FORMATO_INVALIDO)
                        {
                            return "El archivo no presenta los nombres de Totales en formato estándar";
                        }
                    }
                    else
                    {
                        mensaje = "Ya existen datos para el año " + anio.anio;
                    }
                }
            }
            else
            {
                return "No se ha subido ningún catalogo de cuenta.";
            }
            return mensaje;
        }

        public static IEnumerable<string> SplitAlpha(string input)
        {
            var words = new List<string> { string.Empty };
            for (var i = 0; i < input.Length; i++)
            {
                words[words.Count - 1] += input[i];
                if (i + 1 < input.Length && char.IsLetter(input[i]) != char.IsLetter(input[i + 1]))
                {
                    words.Add(string.Empty);
                }
            }
            return words;
        }

        /*VerificarBalance: verifica que el balance cuadre e inserta los valores en la base*/
        public async Task<int> VerificarBalance(int IdEmpresa, List<BalanceViewModel> balanceV)
        {
            int a1 = 0;
            int resultado = COD_BALANCE_DESCUADRADO; //Inicializo asumiendo que esta descuadrado

            //Obtengo la lista de cuentas en catalogo, excluyendo las cuentas de total que se agregaron en la creación cod=0 y hago una lista personalizada
            var cuentasCatalogo = (from cuenta in _context.Catalogodecuenta
                                   where cuenta.Idempresa == IdEmpresa
                                   && cuenta.Codcuentacatalogo != "D"
                                   && cuenta.Codcuentacatalogo != "0"
                                   select new
                                   {
                                       nomCuenta = cuenta.IdcuentaNavigation.Nomcuenta,
                                       tipoCuenta = cuenta.IdcuentaNavigation.IdtipocuentaNavigation.Nomtipocuenta,
                                       idCuenta = cuenta.IdcuentaNavigation.Idcuenta,
                                       cuentaEstandar = cuenta.nomCuentaE.nomCuentaE,
                                       codCuentaCatalogo = cuenta.Codcuentacatalogo
                                   }).ToList();
            //Recojo las listas del balance que correspondan a los 3 totales principales
            var ctaTotalActivos = balanceV.Where(x => x.nomCuenta.Equals("TOTAL ACTIVOS", StringComparison.OrdinalIgnoreCase)
            || x.nomCuenta.Equals("ACTIVO TOTAL", StringComparison.OrdinalIgnoreCase) || x.nomCuenta.Equals("TOTAL ACTIVO", StringComparison.OrdinalIgnoreCase)
            || x.nomCuenta.Equals("ACTIVOS TOTALES", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

            var ctaTotalPasivo = balanceV.Where(x => x.nomCuenta.Equals("TOTAL PASIVO", StringComparison.OrdinalIgnoreCase)
            || x.nomCuenta.Equals("PASIVO TOTAL", StringComparison.OrdinalIgnoreCase) || x.nomCuenta.Equals("TOTAL PASIVOS", StringComparison.OrdinalIgnoreCase)
            || x.nomCuenta.Equals("PASIVOS TOTALES", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

            var ctaTotalPatrimonio = balanceV.Where(x => x.nomCuenta.Contains("TOTAL CAPITAL SOCIAL", StringComparison.OrdinalIgnoreCase)
            || x.nomCuenta.Contains("TOTAL PATRIMONIO", StringComparison.OrdinalIgnoreCase) || x.nomCuenta.Contains("PATRIMONIO TOTAL", StringComparison.OrdinalIgnoreCase)
            || x.nomCuenta.Contains("PASIVOS TOTALES", StringComparison.OrdinalIgnoreCase) || x.nomCuenta.Contains("TOTAL CAPITAL CONTABLE", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

            if (ctaTotalActivos == null || ctaTotalPasivo == null || ctaTotalPatrimonio == null)
            {
                resultado = COD_FORMATO_INVALIDO; //Si alguna lista esta vacia es porque no se encontró el nombre estándar
            }

            //Almaceno su valor
            double valTotalActivos = ctaTotalActivos.valor;
            double valTotalPasivo = ctaTotalPasivo.valor;
            double valTotalPatrimonio = ctaTotalPatrimonio.valor;
            double valTotalPatMasPasivo = valTotalPasivo + valTotalPatrimonio;

            if (valTotalActivos == valTotalPatMasPasivo) //Comparo que cuadre
            {
                foreach (var filaBV in balanceV)
                {
                    a1 = filaBV.anioFila;
                    //Busco la cuenta en el catalogo para obtener su información
                    var infoCuenta = cuentasCatalogo.Find(z => z.nomCuenta.Equals(filaBV.nomCuenta));
                    if (!(infoCuenta == null))
                    {
                        Valoresdebalance vB = new Valoresdebalance
                        {
                            Idempresa = IdEmpresa,
                            Idcuenta = infoCuenta.idCuenta,
                            Valorcuenta = filaBV.valor,
                            Anio = filaBV.anioFila
                        };
                        //Verifico que no exista este registro en la base
                        var vc = _context.Valoresdebalance.Where(x => x.Idcuenta == vB.Idcuenta && x.Idempresa == vB.Idempresa
                             && x.Valorcuenta == vB.Valorcuenta && x.Anio == vB.Anio).ToList();
                        if (vc.Count == 0)
                        {
                            _context.Add(vB);
                        }
                    }

                }
                //Busco las cuentas de total que yo agregué
                var ctasTotales = (from cuenta in _context.Catalogodecuenta
                                   where cuenta.Idempresa == IdEmpresa
                                   && cuenta.Codcuentacatalogo.Equals("0")
                                   select new
                                   {
                                       nomCuenta = cuenta.IdcuentaNavigation.Nomcuenta,
                                       tipoCuenta = cuenta.IdcuentaNavigation.IdtipocuentaNavigation.Nomtipocuenta,
                                       idCuenta = cuenta.IdcuentaNavigation.Idcuenta,
                                       cuentaEstandar = cuenta.nomCuentaE.nomCuentaE,
                                       codCuentaCatalogo = cuenta.Codcuentacatalogo
                                   }).ToList();
                //Lleno las 3 cuentas de total que se que tengo su valor
                foreach (var total in ctasTotales)
                {
                    var vt = _context.Valoresdebalance.Where(x => x.Idcuenta == total.idCuenta && x.Idempresa == IdEmpresa
                             && x.Anio == a1).ToList();
                    if (vt.Count == 0)
                    {
                        string tipo = total.tipoCuenta;
                        switch (tipo)
                        {
                            case "PASIVO":
                                _context.Add(new Valoresdebalance
                                {
                                    Idempresa = IdEmpresa,
                                    Idcuenta = total.idCuenta,
                                    Valorcuenta = valTotalPasivo,
                                    Anio = a1
                                });
                                break;
                            case "PATRIMONIO":
                                _context.Add(new Valoresdebalance
                                {
                                    Idempresa = IdEmpresa,
                                    Idcuenta = total.idCuenta,
                                    Valorcuenta = valTotalPatrimonio,
                                    Anio = a1
                                });
                                break;
                            case "ACTIVO":
                                _context.Add(new Valoresdebalance
                                {
                                    Idempresa = IdEmpresa,
                                    Idcuenta = total.idCuenta,
                                    Valorcuenta = valTotalActivos,
                                    Anio = a1
                                });
                                break;
                            default:
                                break;
                        }
                    }
                }
                await _context.SaveChangesAsync();
                resultado = COD_VALORES_EXITO; //Si todo se inserto, retorno el codigo de éxito
            }
            return resultado; //Si el if de verificar que el balance cuadre da falso, se retorna el valor inicial de resultado (cod descuadrado)
        }

        /*LeerExcel: lee el archivo de excel que sube el usuario y devuelve sus datos en una lista de Balance View Model*/
        public async Task<List<BalanceViewModel>> LeerExcel(IFormFile files, string hoja, string cCuenta, string cValor, int anio)
        {
            List<BalanceViewModel> lstDatos = new List<BalanceViewModel>();
            string celCuenta = cCuenta;
            /*Llamar método para obtener la letra de columna y número de fila de las celdas*/
            IEnumerable<string> s = SplitAlpha(celCuenta);
            int contador = int.Parse(s.Last());

            string celValA1 = cValor;
            string letraC = s.First();

            s = SplitAlpha(celValA1);
            string letraA = s.First();

            using (var stream = new MemoryStream())
            {
                await files.CopyToAsync(stream);

                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[hoja];
                    var rowCount = worksheet.Dimension.Rows;
                    //Verificar que la hoja no esté vacia
                    if (worksheet.Dimension == null)
                    {
                        return lstDatos;
                    }
                    //Obteniendo datos del año 1
                    for (int row = 1; row <= rowCount; row++)
                    {
                        if (!(worksheet.Cells[celCuenta].Value == null) || !(worksheet.Cells[celValA1].Value == null))
                        {
                            if (worksheet.Cells[celValA1].Value == null)
                            {
                                lstDatos.Add(new BalanceViewModel
                                {
                                    nomCuenta = worksheet.Cells[celCuenta].Value.ToString().Trim(),
                                    valor = 0,
                                    anioFila = anio
                                });
                            }
                            else
                            {
                                lstDatos.Add(new BalanceViewModel
                                {
                                    nomCuenta = worksheet.Cells[celCuenta].Value.ToString().Trim(),
                                    valor = double.Parse(worksheet.Cells[celValA1].Value.ToString().Trim()),
                                    anioFila = anio
                                });

                            }
                        }
                        contador++;
                        celCuenta = letraC + contador;
                        celValA1 = letraA + contador;
                    }
                    return lstDatos;
                }
            }
        }

        // GET: ValoresBalance
        public async Task<IActionResult> Index(string mensaje,int? id)
        {
            var usuario = this.User;
            Usuario u = _context.Users.Include(l => l.Idempresa).Where(l => l.UserName == usuario.Identity.Name).FirstOrDefault();
            List<Valoresdebalance> proyAnfContext;
            if (await userManager.IsInRoleAsync(u, "Administrator"))
            {
                ViewBag.nomEmpresa = u.Idempresa.Nomempresa;
                ViewBag.idEmpresa = u.Idempresa.Idempresa;
                proyAnfContext = _context.Valoresdebalance.Include(v => v.Id).Where(p => p.Idempresa == id).Include(v => v.Id.IdcuentaNavigation).ToList();
                return View(proyAnfContext);
            }
            proyAnfContext = _context.Valoresdebalance.Include(v => v.Id).Include(v => v.Id.IdcuentaNavigation).Where(p => p.Idempresa == u.Idempresa.Idempresa).ToList();
            ViewBag.nomEmpresa = u.Idempresa.Nomempresa;
            ViewBag.idEmpresa = u.Idempresa.Idempresa;
            ViewBag.Mensaje = mensaje;
            return View(proyAnfContext);
        }
        public async Task<IActionResult> AnalsisHorizontal(int? ana1,int? ana2)
            
        {
          
            var user = this.User;
            List<Usuario> u = _context.Users.Include(e => e.Idempresa).Where(e => e.UserName == user.Identity.Name).ToList();
            if (u[0].Idempresa!= null)
            {


                var proyAnfContext = _context.Valoresdebalance.Where(x => x.Idempresa == u[0].Idempresa.Idempresa);
                if (proyAnfContext.Any())
                {
                    var xs = proyAnfContext.GroupBy(x => x.Anio).Select(x => new { Anio = x.Key, Buildings = x.Count() });
                    if (xs.Count() >= 2)
                    {


                        List<Catalogodecuenta> cuentas = _context.Catalogodecuenta.Include(r => r.IdcuentaNavigation).Where(y => y.Idempresa == u[0].Idempresa.Idempresa).ToList();
                        List<Cuenta> cuentasU = _context.Cuenta.ToList();
                        if (ana1 == null || ana2 == null)
                        {
                            ana1 = proyAnfContext.FirstOrDefault().Anio;
                            ana2 = ana1 - 1;
                        }
                        List<Valoresdebalance> AH1 = proyAnfContext.Include(x => x.Id.IdcuentaNavigation).Where(s => s.Anio == ana1).OrderBy(r => r.Id.IdcuentaNavigation.Nomcuenta).ToList();
                        List<Valoresdebalance> AH2 = proyAnfContext.Include(x => x.Id.IdcuentaNavigation).Where(s => s.Anio == ana2).OrderBy(r => r.Id.IdcuentaNavigation.Nomcuenta).ToList();
                        List<Valoresdebalance> vb = new List<Valoresdebalance>();
                        List<Valoresdebalance> pasivo1 = new List<Valoresdebalance>();
                        List<Valoresdebalance> pasivo2 = new List<Valoresdebalance>();
                        List<Valoresdebalance> activo1 = new List<Valoresdebalance>();
                        List<Valoresdebalance> activo2 = new List<Valoresdebalance>();
                        Valoresdebalance chance = new Valoresdebalance();
                        string mensajeVirtud = "";
                        string mensajeNegativo = "";
                        string mensajeSugerencia = "";
                        double suma1 = 0;
                        double suma2 = 0;
                        Cuenta cuentaAct1Max = new Cuenta();
                        double valorAct1Max = 0;
                        Cuenta cuentaPas1Max = new Cuenta();
                        double valorPas1Max = 0;
                        Cuenta cuentaAct2Max = new Cuenta();
                        double valorAct2Max = 0;
                        Cuenta cuentaPas2Max = new Cuenta();
                        double valorPas2Max = 0;
                        Cuenta cuentaAct1Min = new Cuenta();
                        double valorAct1Min = 0;
                        Cuenta cuentaPas1Min = new Cuenta();
                        double valorPas1Min = 0;
                        Cuenta cuentaAct2Min = new Cuenta();
                        double valorAct2Min = 0;
                        Cuenta cuentaPas2Min = new Cuenta();
                        double valorPas2Min = 0;
                        Valoresdebalance lv = new Valoresdebalance();
                        List<Valoresdebalance> vb1 = new List<Valoresdebalance>();
                        Valoresdebalance lv1 = new Valoresdebalance();
                        List<AnalisisHorizontalViewModel> anV = new List<AnalisisHorizontalViewModel>();
                        foreach (Catalogodecuenta cs in cuentas)
                        {
                            lv = proyAnfContext.Where(l => l.Idcuenta == cs.Idcuenta && l.Anio == ana1).FirstOrDefault();
                            if (lv != null)
                            {
                                vb.Add(lv);
                            }

                        }
                        foreach (Catalogodecuenta cs in cuentas)
                        {
                            lv1 = proyAnfContext.Where(l => l.Idcuenta == cs.Idcuenta && l.Anio == ana2).FirstOrDefault();
                            if (lv1 != null)
                            {
                                vb1.Add(lv);
                            }

                        }
                        for (int i = 0; i < vb1.Count(); i++)
                        {
                            AnalisisHorizontalViewModel analiHorizon = new AnalisisHorizontalViewModel()
                            {
                                nombreCuenta = AH1[i].Id.IdcuentaNavigation.Nomcuenta,
                                anio1 = (int)ana1,
                                anio2 = (int)ana2,
                                valorAnio1 = AH1[i].Valorcuenta,
                                valorAnio2 = AH2[i].Valorcuenta,
                                valorhorizontal = AH1[i].Valorcuenta - AH2[i].Valorcuenta
                            };

                            anV.Add(analiHorizon);


                        }

                        //calculo  para opinar
                        foreach (var dato in AH1)
                        {
                            if (dato.Id.Codcuentacatalogo.StartsWith("1"))
                            {
                                activo1.Add(dato);
                            }
                            if (dato.Id.Codcuentacatalogo.StartsWith("2"))
                            {
                                pasivo1.Add(dato);
                            }
                        }
                        foreach (var dato in AH2)
                        {
                            if (dato.Id.Codcuentacatalogo.StartsWith("1"))
                            {
                                activo2.Add(dato);
                            }
                            if (dato.Id.Codcuentacatalogo.StartsWith("2"))
                            {
                                pasivo2.Add(dato);
                            }
                        }
                        activo1 = activo1.OrderBy(r => r.Id.IdcuentaNavigation.Nomcuenta).ToList();
                        activo2 = activo2.OrderBy(r => r.Id.IdcuentaNavigation.Nomcuenta).ToList();
                        List<AV> valor = new List<AV>();
                        for (int i = 0; i < activo1.Count(); i++)
                        {

                            valor.Add(new AV
                            {
                                cuenta1 = activo1[i],
                                cuenta2 = activo2[i],
                                diferencia = activo1[i].Valorcuenta - activo2[i].Valorcuenta

                            }); ;
                        }
                        valor = valor.OrderBy(x => x.diferencia).ToList();




                        suma1 = activo1.Sum(r => r.Valorcuenta);
                        suma2 = activo2.Sum(r => r.Valorcuenta);
                        //activo maximo 1
                        chance = activo1.OrderBy(r => r.Valorcuenta).LastOrDefault();
                        cuentaAct1Max = cuentasU.Where(x => x.Idcuenta == chance.Idcuenta).FirstOrDefault();
                        valorAct1Max = chance.Valorcuenta;
                        //activo maximo 2
                        chance = activo2.OrderBy(r => r.Valorcuenta).LastOrDefault();
                        cuentaAct2Max = cuentasU.Where(x => x.Idcuenta == chance.Idcuenta).FirstOrDefault();
                        valorAct2Max = chance.Valorcuenta;
                        //activo minimo 1

                        chance = activo1.OrderBy(r => r.Valorcuenta).FirstOrDefault();
                        cuentaAct1Min = cuentasU.Where(x => x.Idcuenta == chance.Idcuenta).FirstOrDefault();
                        valorAct1Min = chance.Valorcuenta;
                        //activo minimo 2
                        chance = activo2.OrderBy(r => r.Valorcuenta).FirstOrDefault();
                        cuentaAct2Min = cuentasU.Where(x => x.Idcuenta == chance.Idcuenta).FirstOrDefault();
                        valorAct2Min = chance.Valorcuenta;
                        //pasivo maximo 1
                        chance = pasivo1.OrderBy(r => r.Valorcuenta).LastOrDefault();
                        cuentaPas1Max = cuentasU.Where(x => x.Idcuenta == chance.Idcuenta).FirstOrDefault();
                        valorPas1Max = chance.Valorcuenta;
                        //pasivo maximo 2
                        chance = pasivo2.OrderBy(r => r.Valorcuenta).LastOrDefault();

                        cuentaPas2Max = cuentasU.Where(x => x.Idcuenta == chance.Idcuenta).FirstOrDefault();
                        valorPas2Max = chance.Valorcuenta;
                        //pasivo minimo 1
                        chance = pasivo1.OrderBy(r => r.Valorcuenta).FirstOrDefault();

                        cuentaPas1Min = cuentasU.Where(x => x.Idcuenta == chance.Idcuenta).FirstOrDefault();
                        valorPas1Min = chance.Valorcuenta;
                        //pasivo minimo 2
                        chance = pasivo2.OrderBy(r => r.Valorcuenta).FirstOrDefault();
                        cuentaPas2Min = cuentasU.Where(x => x.Idcuenta == chance.Idcuenta).FirstOrDefault();
                        valorPas2Min = chance.Valorcuenta;
                        //opinando como licenciado
                        int cuen = proyAnfContext.Where(l => l.Id.IdcuentaNavigation.Nomcuenta == "TOTAL ACTIVO" && l.Anio == ana1).FirstOrDefault().Idcuenta;
                        int cuen2 = proyAnfContext.Where(x => x.Id.IdcuentaNavigation.Nomcuenta == "TOTAL ACTIVO" && x.Anio == ana2).FirstOrDefault().Idcuenta;
                        double cuentala = proyAnfContext.Where(x => x.Idcuenta == cuen).FirstOrDefault().Valorcuenta;
                        double cuentala2 = proyAnfContext.Where(x => x.Idcuenta == cuen2).FirstOrDefault().Valorcuenta;
                        double totalpor = Math.Abs(((cuentala - cuentala2) / cuentala) * 100);
                        //Opinión a favor 
                        if (cuentala > cuentala2)
                        {

                            if (totalpor > 10)
                            {
                                mensajeVirtud = mensajeVirtud + "Su Total de activo aumentó un " + totalpor + "% en comparación al año " + ana2;
                            }
                            else
                            {
                                mensajeVirtud = mensajeVirtud + "Su Total de activo tuvo un pequeño aumento del " + totalpor + "% en comparación al año " + ana2;
                            }

                        }
                        else if (cuentala < cuentala2)
                        {
                            if (totalpor >= 20 || totalpor <= 40)
                            {
                                mensajeVirtud = mensajeVirtud + "Su Total activo disminuyó considerablemente un  " + totalpor + "% en comparación al año " + ana2;
                            }
                            else if (totalpor > 40)
                            {
                                mensajeVirtud = mensajeVirtud + "Su Total de activo disminuyó drasticamente un  " + totalpor + "% en comparación al año " + ana2;
                            }
                            else
                            {
                                mensajeVirtud = mensajeVirtud + "Su Total de activo disminuyó un " + totalpor + "% en comparación al año " + ana2;
                            }

                        }
                        else
                        {
                            mensajeVirtud = mensajeVirtud + "Su Total de activo se mantuvo intacto " + totalpor + "% en comparación al año " + ana2;
                        }

                        mensajeVirtud = mensajeVirtud + " El mayor aumento de activos fue con la cuenta " + valor.LastOrDefault().cuenta1.Id.IdcuentaNavigation.Nomcuenta
                            + " con un aumento de " + Math.Abs(Math.Round(valor.LastOrDefault().diferencia, 2)) + " mientras que la mayor disminución en los activos fue " +
                            valor.FirstOrDefault().cuenta1.Id.IdcuentaNavigation.Nomcuenta + " reduciendo $" + Math.Abs(Math.Round(valor.FirstOrDefault().diferencia, 2)) +
                            " del año " + ana2;





                        ViewData["Anios"] = new SelectList(xs, "Anio", "Anio");
                        ViewBag.mensajebueno = mensajeVirtud;
                        ViewBag.mensajemalo = mensajeNegativo;
                        ViewBag.mensajemeh = mensajeSugerencia;
                        ViewData["todo"] = anV;
                        ViewData["an1"] = ana1;
                        ViewData["an2"] = ana2;
                        ViewBag.existe = true;
                    } else
                    {
                        ViewBag.existe = false;
                    }
                }
                else
                {
                    ViewBag.existe = false;
                }
                ViewBag.nocuenta = false;
                return View(await proyAnfContext.ToListAsync());
            } else
            {
                ViewBag.nocuenta = true;
               var proyAnfContext = _context.Valoresdebalance;
               return View(await proyAnfContext.ToListAsync());
              

            }
            
        }
        public async Task<IActionResult> AnalsisVertical(int? Anual)
        {
          
            
            var user = this.User;
            List<Usuario> u = _context.Users.Include(e => e.Idempresa).Where(e => e.UserName == user.Identity.Name).ToList();
            if (u[0].Idempresa != null)
            {

                //recupero el catalogo de la empresa del usuario
                var proyAnfContext = _context.Valoresdebalance.Where(y => y.Idempresa == u[0].Idempresa.Idempresa);
                if (proyAnfContext.Any())
                {


                    if (Anual == null)
                    {
                        Anual = proyAnfContext.FirstOrDefault().Anio;
                    }
                    List<Valoresdebalance> pasivo1 = new List<Valoresdebalance>();

                    List<Valoresdebalance> activo1 = new List<Valoresdebalance>();

                    string mensajeVirtud = "";
                    string mensajeNegativo = "";
                    string mensajeSugerencia = "";
                    double suma1 = 0;
                    double suma2 = 0;
                    Cuenta cuentaAct1Max = new Cuenta();
                    double valorAct1Max = 0;
                    Cuenta cuentaPas1Max = new Cuenta();
                    double valorPas1Max = 0;
                    Cuenta cuentaAct1Min = new Cuenta();
                    double valorAct1Min = 0;
                    Cuenta cuentaPas1Min = new Cuenta();
                    double valorPas1Min = 0;


                    List<Catalogodecuenta> cuentas = _context.Catalogodecuenta.Include(r => r.IdcuentaNavigation).Where(y => y.Idempresa == u[0].Idempresa.Idempresa).ToList();
                    int cuen = proyAnfContext.Where(x => x.Id.IdcuentaNavigation.Nomcuenta == "TOTAL ACTIVO" && x.Anio == Anual).FirstOrDefault().Idcuenta;

                    List<int> Anos = new List<int>();
                    List<Cuenta> cuentasU = _context.Cuenta.ToList();
                    Valoresdebalance chance = new Valoresdebalance();
                    var duplicado = cuentas.Where(l => l.Codcuentacatalogo != "0");
                    Valoresdebalance lv = new Valoresdebalance();
                    List<Valoresdebalance> vb = new List<Valoresdebalance>();
                    foreach (Catalogodecuenta cs in cuentas)
                    {
                        lv = proyAnfContext.Where(l => l.Idcuenta == cs.Idcuenta && l.Anio == Anual).FirstOrDefault();
                        if (lv != null)
                        {
                            vb.Add(lv);
                        }

                    }
                    double cuentala = proyAnfContext.Where(x => x.Idcuenta == cuen).FirstOrDefault().Valorcuenta;
                    List<Valoresdebalance> AH1 = proyAnfContext.Include(x => x.Id.IdcuentaNavigation).Where(s => s.Anio == Anual).OrderBy(r => r.Id.IdcuentaNavigation.Nomcuenta).ToList();

                    /*
                    //recupero usuario
                    var user = this.User;
                    List<Usuario> u = _context.Users.Include(e => e.Idempresa).Where(e => e.UserName == user.Identity.Name).ToList();
                    //recupero el catalogo de la empresa del usuario
                    List<Catalogodecuenta> cuentas = _context.Catalogodecuenta.Include(r => r.IdcuentaNavigation).Where(y => y.Idempresa == u[0].Idempresa.Idempresa).ToList();
                    //busco el catalogo  que no sean totales por su id  diferente de 0
                    List<Catalogodecuenta> cuentis = cuentas.Where(z => z.Codcuentacatalogo != "0").ToList();
                    //para guardar el nombre de las cuentas
                    List<string> xc = new List<string>();
                    //contexto de  cuenta
                    var cuenta = from x in _context.Cuenta select x;
                    // contexto de balance general de la empresa del usuario loggeado
                    var proyAnfContext = _context.Valoresdebalance.Where(y => y.Idempresa == u[0].Idempresa.Idempresa);
                    //recupero el primer año de los balances
                    int an = proyAnfContext.FirstOrDefault().Anio;
                    //busco los balances con el primer año
                    proyAnfContext = proyAnfContext.Where(y => y.Anio == an);
                    //consigo la cuenta de nombre Total activo de  la empresa

                    //Guardo el valor en balance de la cuenta total activo
                    double activoTotal = proyAnfContext.Where(x => x.Idcuenta == cuentala.Idcuenta).FirstOrDefault().Valorcuenta;
                    // filtro las nombres de la cuentas que peretenecen al balance de la empresa
                    foreach (var aw in proyAnfContext)
                    {

                        xc.Add(cuenta.Where(x => x.Idcuenta == aw.Idcuenta).FirstOrDefault().Nomcuenta);
                    }
                    //envìo el total y el nombre a la vista
                    ViewBag.activo = Math.Round( activoTotal);

                    */
                    //òpniones
                    foreach (var dato in AH1)
                    {
                        if (dato.Id.Codcuentacatalogo.StartsWith("1"))
                        {
                            activo1.Add(dato);
                        }
                        if (dato.Id.Codcuentacatalogo.StartsWith("2"))
                        {
                            pasivo1.Add(dato);
                        }
                    }
                    chance = activo1.OrderBy(r => r.Valorcuenta).LastOrDefault();
                    cuentaAct1Max = cuentasU.Where(x => x.Idcuenta == chance.Idcuenta).FirstOrDefault();
                    valorAct1Max = chance.Valorcuenta;
                    chance = activo1.OrderBy(r => r.Valorcuenta).FirstOrDefault();
                    cuentaAct1Min = cuentasU.Where(x => x.Idcuenta == chance.Idcuenta).FirstOrDefault();
                    valorAct1Min = chance.Valorcuenta;
                    //pasivo minimo 1
                    chance = pasivo1.OrderBy(r => r.Valorcuenta).FirstOrDefault();
                    cuentaPas1Min = cuentasU.Where(x => x.Idcuenta == chance.Idcuenta).FirstOrDefault();
                    valorPas1Min = chance.Valorcuenta;
                    //
                    chance = pasivo1.OrderBy(r => r.Valorcuenta).LastOrDefault();
                    cuentaPas1Max = cuentasU.Where(x => x.Idcuenta == chance.Idcuenta).FirstOrDefault();
                    valorPas1Max = chance.Valorcuenta;

                    mensajeVirtud = "Su activo de mayor provecho es la cuenta: " + cuentaAct1Max.Nomcuenta + " siendo la cuenta que produce mayor ganancia con un valor de $" + valorAct1Max +
                        " por su contraparte, la empresa se ve afectada por gastos mayormente ocasiones por: " + cuentaPas1Max.Nomcuenta + ", cuenta que tiene un saldo de: " + valorPas1Max
                       ;

                    ViewBag.activo = cuentala;
                    ViewBag.year = Anual;
                    ViewBag.cuentas = vb;
                    ViewBag.mensajebueno = mensajeVirtud;
                    var xs = proyAnfContext.GroupBy(x => x.Anio).Select(x => new { Anio = x.Key, Buildings = x.Count() });



                    ViewData["Anios"] = new SelectList(xs, "Anio", "Anio");
                    ViewBag.existe = true;
                }
                else
                {
                    ViewBag.existe = false;
                }
                ViewBag.noexiste = false;
                return View(await proyAnfContext.ToListAsync());
            }
            else
            {
                ViewBag.noexiste =true;
                var proyAnfContext = _context.Valoresdebalance;
                return View(await proyAnfContext.ToListAsync());
            }
            
        }

        // GET: ValoresBalance/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var valoresdebalance = await _context.Valoresdebalance
                .Include(v => v.Id)
                .FirstOrDefaultAsync(m => m.Idbalance == id);
            if (valoresdebalance == null)
            {
                return NotFound();
            }

            return View(valoresdebalance);
        }

        // GET: ValoresBalance/Create
        public IActionResult Create(int? id, bool? valR)
        {
            ViewData["idEmpresa"] = id;
            if (valR != null) { ViewData["ctasNoFinalizadas"] = valR; }
            else { ViewData["ctasNoFinalizadas"] = false; }
            ViewData["catalogo"] = _context.Catalogodecuenta.Where(p => p.Idempresa == id).Include(p => p.IdcuentaNavigation).ToList();
            ViewData["ctasCatalogo"] = _context.Catalogodecuenta.Where(p => p.Idempresa==id).Select
                (x => new SelectListItem()
                    {   Text = x.IdcuentaNavigation.Nomcuenta,
                        Value = x.Idcuenta.ToString()});
            return View();
        }

        // POST: ValoresBalance/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Idbalance,Idempresa,Idcuenta,Valorcuenta,Anio")] Valoresdebalance valoresdebalance)
        {
            int numCtasCatalogo = _context.Catalogodecuenta.Where(p => p.Idempresa == valoresdebalance.Idempresa).Count();
            int numCtasIngresadas = _context.Valoresdebalance.Where(p => p.Idempresa == valoresdebalance.Idempresa && p.Anio == valoresdebalance.Anio).Count();
            bool val = false;
            if (ModelState.IsValid)
            {                
                if(!(_context.Valoresdebalance.Where(p => p.Idempresa == valoresdebalance.Idempresa
                    && p.Idcuenta == valoresdebalance.Idcuenta && p.Anio == valoresdebalance.Anio).Any()))
                {                    
                    _context.Add(valoresdebalance);
                    await _context.SaveChangesAsync();
                    if (numCtasCatalogo > numCtasIngresadas)
                    {
                        val = true;
                        ViewData["ctasNoFinalizadas"] = val;
                    }
                    return RedirectToAction(nameof(Create), new { id = valoresdebalance.Idempresa, valR = val });
                }
                else
                {
                    ModelState.AddModelError("Valorcuenta", "Ya se ha ingresado un valor para esta combinación de cuenta y año");
                }                
            }
            ViewData["idEmpresa"] = valoresdebalance.Idempresa;
            //var ctas = _context.Valoresdebalance.Include(p => p.Id).ThenInclude(p => p.IdcuentaNavigation).Where(p => p.Anio == valoresdebalance.Anio && p.Idempresa == valoresdebalance.Idempresa);
            //var ctasCatalogo = _context.Catalogodecuenta.Where(p => p.Idempresa == valoresdebalance.Idempresa).ToList();
            //ctasCatalogo = ctasCatalogo.Remove(ctas.);
            ViewData["ctasCatalogo"] = _context.Catalogodecuenta.Where(p => p.Idempresa == valoresdebalance.Idempresa).Select
                (x => new SelectListItem()
                {
                    Text = x.IdcuentaNavigation.Nomcuenta,
                    Value = x.Idcuenta.ToString()
                });
            if (numCtasCatalogo > numCtasIngresadas)
            {
                ViewData["ctasNoFinalizadas"] = true;
            }            
            return View(valoresdebalance);
        }

        // GET: ValoresBalance/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var valoresdebalance = await _context.Valoresdebalance.FindAsync(id);
            if (valoresdebalance == null)
            {
                return NotFound();
            }
            ViewData["Idempresa"] = new SelectList(_context.Catalogodecuenta, "Idempresa", "Codcuentacatalogo", valoresdebalance.Idempresa);
            return View(valoresdebalance);
        }

        // POST: ValoresBalance/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Idbalance,Idempresa,Idcuenta,Valorcuenta,Anio")] Valoresdebalance valoresdebalance)
        {
            if (id != valoresdebalance.Idbalance)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(valoresdebalance);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ValoresdebalanceExists(valoresdebalance.Idbalance))
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
            ViewData["Idempresa"] = new SelectList(_context.Catalogodecuenta, "Idempresa", "Codcuentacatalogo", valoresdebalance.Idempresa);
            return View(valoresdebalance);
        }

        // GET: ValoresBalance/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var valoresdebalance = await _context.Valoresdebalance
                .Include(v => v.Id)
                .FirstOrDefaultAsync(m => m.Idbalance == id);
            if (valoresdebalance == null)
            {
                return NotFound();
            }

            return View(valoresdebalance);
        }

        // POST: ValoresBalance/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var valoresdebalance = await _context.Valoresdebalance.FindAsync(id);

            _context.Valoresdebalance.Remove(valoresdebalance);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ValoresdebalanceExists(int id)
        {

            return _context.Valoresdebalance.Any(e => e.Idbalance == id);
        }

    }
}
