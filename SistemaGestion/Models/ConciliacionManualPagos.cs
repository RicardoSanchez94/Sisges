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
    
    public partial class ConciliacionManualPagos
    {
        public System.Guid id { get; set; }
        public Nullable<int> TrxMP { get; set; }
        public string Autorizadortbk { get; set; }
        public Nullable<System.DateTime> FechaMp { get; set; }
        public Nullable<System.DateTime> Fechatbk { get; set; }
        public Nullable<int> codigoLocal { get; set; }
        public Nullable<int> idtienda { get; set; }
        public Nullable<decimal> MontoMp { get; set; }
        public Nullable<decimal> Montotbk { get; set; }
        public Nullable<short> Tipo_Pago { get; set; }
        public Nullable<System.DateTime> FechaConciliacion { get; set; }
        public string TarjetaMp { get; set; }
        public string TarjetaTbk { get; set; }
    }
}