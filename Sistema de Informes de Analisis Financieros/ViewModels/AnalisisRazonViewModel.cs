using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sistema_de_Informes_de_Analisis_Financieros.ViewModels
{
    public class AnalisisRazonViewModel
    {
        public string nombreRazon { get; set; }
        public List<string> numerador { get; set; }
        public List<string> denominador { get; set; }
        public string tipo { get; set; }
        public string signoNumerador { get; set; }
        public string signoDenominador { get; set; }
    }
}
