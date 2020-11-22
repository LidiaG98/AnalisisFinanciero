using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_de_Informes_de_Analisis_Financieros.Models
{
    public partial class Ratiobasesector
    {        
        public int Idratio { get; set; }
        public int Idsector { get; set; }
        public double Valorratiob { get; set; }

        public virtual Ratio IdratioNavigation { get; set; }
        public virtual Sector IdsectorNavigation { get; set; }
    }
}
