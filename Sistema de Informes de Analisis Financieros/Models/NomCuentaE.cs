using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Sistema_de_Informes_de_Analisis_Financieros.Models
{
    public class NomCuentaE
    {
        [Key]
        [Required(ErrorMessage = "Este campo es obligatorio")]
        [Display(Name = "Código")]
        public int nomCuentaEID { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        [Display(Name = "Nombre")]
        [MaxLength(100)]
        public string nomCuentaE { get; set; }

        public ICollection<Catalogodecuenta> Catalogodecuenta { get; set; }
    }
}
