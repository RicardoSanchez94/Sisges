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
    
    public partial class Proc_TotaleTransbank_Result
    {
        public string Fecha { get; set; }
        public int IdTienda { get; set; }
        public Nullable<int> CodigoComercio { get; set; }
        public string TipoPago { get; set; }
        public Nullable<int> Trx { get; set; }
        public Nullable<decimal> Total { get; set; }
    }
}
