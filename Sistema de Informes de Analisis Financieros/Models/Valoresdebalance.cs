using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_de_Informes_de_Analisis_Financieros.Models
{
    public partial class Valoresdebalance
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Idbalance { get; set; }
        public int Idempresa { get; set; }
        public int Idcuenta { get; set; }
        public double Valorcuenta { get; set; }
        public int Anio { get; set; }

        public virtual Catalogodecuenta Id { get; set; }
    }
}
