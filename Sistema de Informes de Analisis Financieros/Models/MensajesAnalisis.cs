using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Sistema_de_Informes_de_Analisis_Financieros.Models
{
    public class MensajesAnalisis
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int idMensaje { get; set; }        
        public string mensajeMayorBase { get; set; }
        public string mensajeMenorBase { get; set; }
        public string mensajeMayorEmp { get; set; }
        public string mensajeMenorEmp { get; set; }
        public string mensajeIgualBase { get; set; }
        public string mensajeIgualEmp { get; set; }

        [ForeignKey("idRatio")]
        public int idRatio { get; set; }
        public Ratio Ratio { get; set; }
    }
}
