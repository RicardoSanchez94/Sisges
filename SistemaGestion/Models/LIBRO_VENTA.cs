//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SistemaGestion.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class LIBRO_VENTA
    {
        public string Sociedad { get; set; }
        public string Centro { get; set; }
        public string ClaseDocumental { get; set; }
        public string NumeroDocto { get; set; }
        public string FechaContabilizacion { get; set; }
        public string Referencia { get; set; }
        public string FechaDocumento { get; set; }
        public string IndicadorImpuesto { get; set; }
        public string NombreCliente { get; set; }
        public string NumeroIdentFiscal { get; set; }
        public Nullable<decimal> Exento { get; set; }
        public Nullable<decimal> Afecto { get; set; }
        public Nullable<decimal> Iva { get; set; }
        public decimal BaseImponibleMP { get; set; }
    }
}
