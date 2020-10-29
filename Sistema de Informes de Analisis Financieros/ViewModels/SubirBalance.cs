using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace Sistema_de_Informes_de_Analisis_Financieros.ViewModels
{
    public class SubirBalance
    {
        public string celdaCuenta { get; set; }
        public string hoja { get; set; }
        public List<AniosBalance> anios { get; set; }

    }

    public class AniosBalance
    {
        public int anio { get; set; }
        public string celdaAnio { get; set; }
    }
}
