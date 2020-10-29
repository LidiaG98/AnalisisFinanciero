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
        public int Idcuenta { get; set; }
        public string Nombrevalore { get; set; }
        public double Valorestado { get; set; }
        public int Anio { get; set; }

        public virtual Catalogodecuenta Id { get; set; }
    }
}
