using Microsoft.AspNetCore.Identity;
using Sistema_de_Informes_de_Analisis_Financieros.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sistema_de_Informes_de_Analisis_Financieros.Data
{
    public class DbInit
    {
        private readonly UserManager<Usuario> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        public static void Inicio(ProyAnfContext context)
        {
            context.Database.EnsureCreated();            
            if (!context.Razon.Any())
            {
                var razon = new Razon[]
                {
                    new Razon{
                        nombreRazon = "RAZON DE CIRCULANTE",
                        numerador = "ACTIVOS CORRIENTES",
                        denominador = "PASIVOS CORRIENTES",
                        tipo = "FINANCIERA"
                    },
                    new Razon{
                        nombreRazon = "PRUEBA ACIDA",
                        numerador = "ACTIVOS CORRIENTES-INVENTARIO",
                        denominador = "PASIVOS CORRIENTES",
                        tipo = "FINANCIERA"
                    },
                    new Razon{
                        nombreRazon = "RAZON DE CAPITAL DE TRABAJO",
                        numerador = "ACTIVOS CORRIENTES-PASIVOS CORRIENTES",
                        denominador = "ACTIVOS TOTALES",
                        tipo = "FINANCIERA"
                    },
                    new Razon{
                        nombreRazon = "RAZON DE EFECTIVO",
                        numerador = "EFECTIVO + VALORES DE CORTO PLAZO",
                        denominador = "PASIVOS CORRIENTES",
                        tipo = "FINANCIERA"
                    },
                    new Razon{
                        nombreRazon = "RAZON DE ROTACION DE INVENTARIO",
                        numerador = "COSTO DE LAS VENTAS",
                        denominador = "INVENTARIO PROMEDIO",
                        tipo = "EFICIENCIA O ACTIVIDAD"
                    },
                    new Razon{
                        nombreRazon = "RAZON DE DIAS DE INVENTARIO",
                        numerador = "INVENTARIO PROMEDIO",
                        denominador = "COSTO DE LAS VENTAS/365",
                        tipo = "EFICIENCIA O ACTIVIDAD"
                    },
                    new Razon{
                        nombreRazon = "RAZON DE ROTACION DE CUENTAS POR COBRAR",
                        numerador = "VENTAS NETAS",
                        denominador = "PROMEDIO CUENTAS POR COBRAR COMERCIALES",
                        tipo = "EFICIENCIA O ACTIVIDAD"
                    },
                    new Razon{
                        nombreRazon = "RAZON DE PERIODO MEDIO DE COBRANZA",
                        numerador = "PROMEDIO CUENTAS POR COBRAR COMERCIALES*365",
                        denominador = "VENTAS NETAS",
                        tipo = "EFICIENCIA O ACTIVIDAD"
                    },
                    new Razon{
                        nombreRazon = "RAZON DE ROTACION DE CUENTAS POR PAGAR",
                        numerador = "COMPRAS",
                        denominador = "PROMEDIO CUENTAS POR COBRAR COMERCIALES",
                        tipo = "EFICIENCIA O ACTIVIDAD"
                    },
                    new Razon{
                        nombreRazon = "PERIODO MEDIO DE PAGO",
                        numerador = "PROMEDIO CUENTAS POR COBRAR COMERCIALES*365",
                        denominador = "COMPRAS",
                        tipo = "EFICIENCIA O ACTIVIDAD"
                    },
                    new Razon{
                        nombreRazon = "INDICE DE ROTACION DE ACTIVOS TOTALES",
                        numerador = "VENTAS NETAS",
                        denominador = "ACTIVO TOTAL PROMEDIO",
                        tipo = "EFICIENCIA O ACTIVIDAD"
                    },
                    new Razon{
                        nombreRazon = "INDICE DE ROTACION DE ACTIVOS FIJOS",
                        numerador = "VENTAS NETAS",
                        denominador = "ACTIVO FIJO NETO PROMEDIO",
                        tipo = "EFICIENCIA O ACTIVIDAD"
                    },
                    new Razon{
                        nombreRazon = "INDICE DE MARGEN BRUTO",
                        numerador = "UTILIDAD BRUTA",
                        denominador = "VENTAS NETAS",
                        tipo = "EFICIENCIA O ACTIVIDAD"
                    },
                    new Razon{
                        nombreRazon = "INDICE DE MARGEN OPERATIVO",
                        numerador = "UTILIDAD OPERATIVA",
                        denominador = "VENTAS NETAS",
                        tipo = "EFICIENCIA O ACTIVIDAD"
                    },
                    new Razon{
                        nombreRazon = "GRADO DE ENDEUDAMIENTO",
                        numerador = "PASIVO TOTAL",
                        denominador = "ACTIVO TOTAL",
                        tipo = "APALANCAMIENTO"
                    },
                    new Razon{
                        nombreRazon = "GRADO DE PROPIEDAD",
                        numerador = "PATRIMONIO",
                        denominador = "ACTIVO TOTAL",
                        tipo = "APALANCAMIENTO"
                    },
                    new Razon{
                        nombreRazon = "RAZON DE ENDEUDAMIENTO PATRIMONIAL",
                        numerador = "PASIVO TOTAL",
                        denominador = "PATRIMONIO TOTAL",
                        tipo = "APALANCAMIENTO"
                    },
                    new Razon{
                        nombreRazon = "RAZON DE COBERTURA DE GASTOS FINANCIEROS",
                        numerador = "UTILIDAD ANTES DE IMPUESTO",
                        denominador = "GASTOS FINANCIEROS",
                        tipo = "APALANCAMIENTO"
                    },
                    new Razon{
                        nombreRazon = "RENTABILIDAD NETA DEL PATRIMONIO",
                        numerador = "UTILIDAD NETA",
                        denominador = "PATRIMONIO PROMEDIO",
                        tipo = "RENTABILIDAD"
                    },
                    new Razon{
                        nombreRazon = "RENTABILIDAD POR ACCION",
                        numerador = "UTILIDAD NETA",
                        denominador = "NUMERO DE ACCIONES",
                        tipo = "RENTABILIDAD"
                    },
                    new Razon{
                        nombreRazon = "RENTABILIDAD DEL ACTIVO",
                        numerador = "UTILIDAD NETA",
                        denominador = "ACTIVO TOTAL PROMEDIO",
                        tipo = "RENTABILIDAD"
                    },
                    new Razon{
                        nombreRazon = "RENTABILIDAD SOBRE VENTAS",
                        numerador = "UTILIDAD NETA",
                        denominador = "VENTAS NETAS",
                        tipo = "RENTABILIDAD"
                    },
                    new Razon{
                        nombreRazon = "RENTABILIDAD SOBRE INVERSION",
                        numerador = "INGRESOS-INVERSION",
                        denominador = "INVERSION",
                        tipo = "RENTABILIDAD"
                    }
                };
                foreach (Razon r in razon)
                {
                    context.Add(r);
                }                
            }
            if (!context.Ratio.Any())
            {
                var ratio = new Ratio[]
                {
                        new Ratio
                        {
                            Nombreratiob = "RAZON DE CIRCULANTE"
                        },
                        new Ratio
                        {
                            Nombreratiob = "RENTABILIDAD DEL ACTIVO"
                        },
                        new Ratio
                        {
                            Nombreratiob = "RENTABILIDAD POR ACCION"
                        },
                        new Ratio
                        {
                            Nombreratiob = "RENTABILIDAD NETA DEL PATRIMONIO"
                        },
                        new Ratio
                        {
                            Nombreratiob = "RAZON DE COBERTURA DE GASTOS FINANCIEROS"
                        },
                        new Ratio
                        {
                            Nombreratiob = "RAZON DE ENDEUDAMIENTO PATRIMONIAL"
                        },
                        new Ratio
                        {
                            Nombreratiob = "GRADO DE PROPIEDAD"
                        },
                        new Ratio
                        {
                            Nombreratiob = "GRADO DE ENDEUDAMIENTO"
                        },
                        new Ratio
                        {
                            Nombreratiob = "INDICE DE MARGEN OPERATIVO"
                        },
                        new Ratio
                        {
                            Nombreratiob = "INDICE DE MARGEN BRUTO"
                        },
                        new Ratio
                        {
                            Nombreratiob = "RENTABILIDAD SOBRE VENTAS"
                        },
                        new Ratio
                        {
                            Nombreratiob = "INDICE DE ROTACION DE ACTIVOS FIJOS"
                        },
                        new Ratio
                        {
                            Nombreratiob = "PERIODO MEDIO DE PAGO"
                        },
                        new Ratio
                        {
                            Nombreratiob = "RAZON DE ROTACION DE CUENTAS POR PAGAR"
                        },
                        new Ratio
                        {
                            Nombreratiob = "RAZON DE PERIODO MEDIO DE COBRANZA"
                        },
                        new Ratio
                        {
                            Nombreratiob = "RAZON DE ROTACION DE CUENTAS POR COBRAR"
                        },
                        new Ratio
                        {
                            Nombreratiob = "RAZON DE DIAS DE INVENTARIO"
                        },
                        new Ratio
                        {
                            Nombreratiob = "RAZON DE ROTACION DE INVENTARIO"
                        },
                        new Ratio
                        {
                            Nombreratiob = "RAZON DE EFECTIVO"
                        },
                        new Ratio
                        {
                            Nombreratiob = "RAZON DE CAPITAL DE TRABAJO"
                        },
                        new Ratio
                        {
                            Nombreratiob = "PRUEBA ACIDA"
                        },
                        new Ratio
                        {
                            Nombreratiob = "INDICE DE ROTACION DE ACTIVOS TOTALES"
                        },
                        new Ratio
                        {
                            Nombreratiob = "RENTABILIDAD SOBRE INVERSION"
                        },
                };
                foreach (Ratio r in ratio)
                {
                    context.Add(r);
                }
            }            
            context.SaveChanges();
        }
    }
}
