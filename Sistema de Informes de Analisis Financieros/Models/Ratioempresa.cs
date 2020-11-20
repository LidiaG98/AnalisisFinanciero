using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_de_Informes_de_Analisis_Financieros.Models
{
    public partial class Ratioempresa
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Idratioempresa { get; set; }
        public int Idratio { get; set; }
        public int Idempresa { get; set; }
        public double Valorratioempresa { get; set; }
        public int anio { get; set; }

        public virtual Empresa IdempresaNavigation { get; set; }
        public virtual Ratio IdratioNavigation { get; set; }
    }
}
