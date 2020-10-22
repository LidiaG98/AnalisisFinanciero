using System;
using System.Collections.Generic;

namespace Sistema_de_Informes_de_Analisis_Financieros.Models
{
    public partial class Ratioempresa
    {
        public int Idratioempresa { get; set; }
        public int Idratio { get; set; }
        public int Idempresa { get; set; }
        public double Valorratioempresa { get; set; }

        public virtual Empresa IdempresaNavigation { get; set; }
        public virtual Ratio IdratioNavigation { get; set; }
    }
}
