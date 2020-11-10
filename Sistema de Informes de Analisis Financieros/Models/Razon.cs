using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Sistema_de_Informes_de_Analisis_Financieros.Models
{
    public class Razon
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required(ErrorMessage = "Este campo es obligatorio")]
        [Display(Name = "idRazon")]
        public int idRazon { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        [Display(Name = "Nombre")]
        public string nombreRazon { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        [Display(Name = "Numerador")]
        public string numerador { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        [Display(Name = "Denominador")]
        public string denominador { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        [Display(Name = "Tipo")]
        public string tipo { get; set; }

    }
}
