using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_de_Informes_de_Analisis_Financieros.Models
{
    public partial class Tipocuenta
    {
        public Tipocuenta()
        {
            Cuenta = new HashSet<Cuenta>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Idtipocuenta { get; set; }
        public string Nomtipocuenta { get; set; }

        public virtual ICollection<Cuenta> Cuenta { get; set; }
    }
}
