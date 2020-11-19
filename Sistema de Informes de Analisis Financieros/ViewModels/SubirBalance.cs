using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace Sistema_de_Informes_de_Analisis_Financieros.ViewModels
{
    public class SubirBalance
    {
        [Required(ErrorMessage = "Debe especificar una celda")]
        public string celdaCuenta { get; set; }
        [Required(ErrorMessage = "Debe especificar el nombre de la hoja")]
        public string hoja { get; set; }
        public List<AniosBalance> anios { get; set; }

    }

    public class AniosBalance
    {
        [Required(ErrorMessage = "Debe especificar el año de los datos")]
        public int anio { get; set; }
        [Required(ErrorMessage = "Debe especificar una celda")]
        public string celdaAnio { get; set; }
    }
}
