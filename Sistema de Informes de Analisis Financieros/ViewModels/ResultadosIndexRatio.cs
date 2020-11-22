using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sistema_de_Informes_de_Analisis_Financieros.ViewModels
{
    public class ResultadosIndexRatio
    {
        public int idEmpresa { get; set; }
        public string Nombre { get; set; }
        public double ValorRazon { get; set; }
        public int anio { get; set; }
    }
}
