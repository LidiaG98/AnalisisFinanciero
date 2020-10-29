using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sistema_de_Informes_de_Analisis_Financieros.ViewModels
{
    public class BalanceViewModel
    {
        public string nomCuenta { get; set; }
        public double valor { get; set; }
        public int anioFila { get; set; }
    }
}
