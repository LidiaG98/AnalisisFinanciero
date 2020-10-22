using System;
using System.Collections.Generic;

namespace Sistema_de_Informes_de_Analisis_Financieros.Models
{
    public partial class Empresa
    {
        public Empresa()
        {
            //AspNetUsers = new HashSet<AspNetUsers>();
            Catalogodecuenta = new HashSet<Catalogodecuenta>();
            Ratioempresa = new HashSet<Ratioempresa>();
        }

        public int Idempresa { get; set; }
        public int Idsector { get; set; }
        public string Nomempresa { get; set; }
        public string Descempresa { get; set; }
        public string Razonsocial { get; set; }

        public virtual Sector IdsectorNavigation { get; set; }
        //public virtual ICollection<User> AspNetUsers { get; set; }
        public virtual ICollection<Catalogodecuenta> Catalogodecuenta { get; set; }
        public virtual ICollection<Ratioempresa> Ratioempresa { get; set; }
    }
}
