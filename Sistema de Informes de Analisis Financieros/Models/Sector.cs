using System;
using System.Collections.Generic;

namespace Sistema_de_Informes_de_Analisis_Financieros.Models
{
    public partial class Sector
    {
        public Sector()
        {
            Empresa = new HashSet<Empresa>();
            Ratiobasesector = new HashSet<Ratiobasesector>();
        }

        public int Idsector { get; set; }
        public string Nomsector { get; set; }

        public virtual ICollection<Empresa> Empresa { get; set; }
        public virtual ICollection<Ratiobasesector> Ratiobasesector { get; set; }
    }
}
