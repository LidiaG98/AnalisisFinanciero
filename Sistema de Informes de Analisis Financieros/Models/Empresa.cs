using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required(ErrorMessage = "Este campo es obligatorio")]
        [Display(Name = "Código")]
        public int Idempresa { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        [Display(Name = "Sector")]
        public int Idsector { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        [Display(Name = "Nombre")]
        public string Nomempresa { get; set; }        
        [Display(Name = "Descripción")]
        public string Descempresa { get; set; }
        
        [Display(Name = "Razón Social")]
        public string Razonsocial { get; set; }

        [Display(Name = "Sector")]
        public virtual Sector IdsectorNavigation { get; set; }
        //public virtual ICollection<User> AspNetUsers { get; set; }
        public virtual ICollection<Catalogodecuenta> Catalogodecuenta { get; set; }
        public virtual ICollection<Ratioempresa> Ratioempresa { get; set; }
    }
}
