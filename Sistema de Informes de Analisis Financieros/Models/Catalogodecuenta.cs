using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sistema_de_Informes_de_Analisis_Financieros.Models
{
    public partial class Catalogodecuenta
    {
        public Catalogodecuenta()
        {
            Valoresdebalance = new HashSet<Valoresdebalance>();
            Valoresestado = new HashSet<Valoresestado>();
        }

        public int Idempresa { get; set; }
        public int Idcuenta { get; set; }
        [Display(Name = "Código")]
        public string Codcuentacatalogo { get; set; }
        public int? nomCuentaEID { get; set; }

        [Display(Name = "Cuenta")]

        public virtual Cuenta IdcuentaNavigation { get; set; }
        public virtual Empresa IdempresaNavigation { get; set; }
        public NomCuentaE nomCuentaE { get; set; }
        public virtual ICollection<Valoresdebalance> Valoresdebalance { get; set; }
        public virtual ICollection<Valoresestado> Valoresestado { get; set; }
    }
}
