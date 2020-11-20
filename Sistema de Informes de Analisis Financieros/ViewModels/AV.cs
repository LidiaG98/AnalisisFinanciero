using Sistema_de_Informes_de_Analisis_Financieros.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sistema_de_Informes_de_Analisis_Financieros.ViewModels
{
    public class AV
    {
        public Valoresdebalance cuenta1 {get;set;}
        public Valoresdebalance cuenta2 {get;set;}
        public double diferencia { get; set; }
    }
}
