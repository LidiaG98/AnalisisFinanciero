using Sistema_de_Informes_de_Analisis_Financieros.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sistema_de_Informes_de_Analisis_Financieros.ViewModels
{
    public class AnalisisHorizontalViewModel
    {
        public string nombreCuenta { get; set; }
        public int anio1 { get; set; }
        public int anio2 { get; set; }

        public double valorAnio1 { get; set; }
        public double valorAnio2 { get; set; }

        public double valorhorizontal { get; set; }



    }
}
