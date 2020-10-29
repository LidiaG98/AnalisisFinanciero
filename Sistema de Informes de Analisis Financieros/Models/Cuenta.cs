using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_de_Informes_de_Analisis_Financieros.Models
{
    public partial class Cuenta
    {
        public Cuenta()
        {
            Catalogodecuenta = new HashSet<Catalogodecuenta>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Idcuenta { get; set; }
        public int Idtipocuenta { get; set; }
        public string Nomcuenta { get; set; }

        public virtual Tipocuenta IdtipocuentaNavigation { get; set; }
        public virtual ICollection<Catalogodecuenta> Catalogodecuenta { get; set; }
    }
}
