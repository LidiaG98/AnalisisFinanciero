using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sistema_de_Informes_de_Analisis_Financieros.ViewModels
{
    public class AnalisisRazonViewModel
    {
        public int idRazon { get; set; }
        public string nombreRazon { get; set; }
        public List<int> anio { get; set; }
        public List<string> numerador { get; set; }
        public List<string> denominador { get; set; }
        public string tipo { get; set; }
        public string signoNumerador { get; set; }
        public string signoDenominador { get; set; }
        public double valorNumA1 { get; set; }
        public double promNum1 { get; set; }
        public double valorDenA1 { get; set; }
        public double promDen1 { get; set; }
        public double valorNum2A1 { get; set; }
        public double promNum2 { get; set; }
        public double valorDen2A1 { get; set; }
        public double promDen2 { get; set; }
        public int anio1 { get; set; }
        public double valorNumA2 { get; set; }
        public double valorDenA2 { get; set; }
        public double valorNum2A2 { get; set; }
        public double valorDen2A2 { get; set; }
        public int anio2 { get; set; }
        //resultados
        public double resA1 { get; set; }
        public double resA2 { get; set; }
        public double resProm { get; set; }
        public double valorSector { get; set; }
        public double valorEmpresa1 { get; set; }
        public double valorEmpresa2 { get; set; }
        //Mensajes resultados
        public string mensajeBase1 { get; set; }
        public string mensajeBase2 { get; set; }
        public string mensajeEmp1 { get; set; }
        public string mensajeEmp2 { get; set; }        
    }
}
