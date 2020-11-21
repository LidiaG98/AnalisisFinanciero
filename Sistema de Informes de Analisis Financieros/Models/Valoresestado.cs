using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_de_Informes_de_Analisis_Financieros.Models
{
    public partial class Valoresestado
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Idvalore { get; set; }
        public int Idempresa { get; set; }

        [Display(Name = "Cuenta")]
        public int Idcuenta { get; set; }

        [Display(Name = "Nombre de fila")]
        [Required(ErrorMessage = "Ingrese el nombre de la fila")]
        public string Nombrevalore { get; set; }

        [Display(Name = "Valor de la cuenta ($)")]
        [Required(ErrorMessage = "Ingrese el valor de la cuenta")]
        public double Valorestado { get; set; }

        [Display(Name = "Año")]
        [Required(ErrorMessage = "Ingrese el año de este valor")]
        public int Anio { get; set; }

        public virtual Catalogodecuenta Id { get; set; }
    }
}
