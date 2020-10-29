using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_de_Informes_de_Analisis_Financieros.Models
{
    public partial class Ratio
    {
        public Ratio()
        {
            Ratiobasesector = new HashSet<Ratiobasesector>();
            Ratioempresa = new HashSet<Ratioempresa>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Idratio { get; set; }
        public string Nombreratiob { get; set; }

        public virtual ICollection<Ratiobasesector> Ratiobasesector { get; set; }
        public virtual ICollection<Ratioempresa> Ratioempresa { get; set; }
    }
}
