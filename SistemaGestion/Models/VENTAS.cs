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
    
    public partial class VENTAS
    {
        public int TRX { get; set; }
        public Nullable<short> TRX_Impreso { get; set; }
        public Nullable<short> Tipo_Trx { get; set; }
        public int Local { get; set; }
        public string Cajero { get; set; }
        public int Caja { get; set; }
        public System.DateTime Fecha_Venta { get; set; }
        public Nullable<System.DateTime> Fecha_Vta_Sistema { get; set; }
        public Nullable<System.DateTime> Fecha_Venta_Linea { get; set; }
        public short Caja_Fuera_Linea { get; set; }
        public short Modo_Entrenamiento { get; set; }
        public short Codigo_Anula { get; set; }
        public string Cliente { get; set; }
        public string Vendedor { get; set; }
        public short Venta_Iniciada { get; set; }
        public Nullable<decimal> Total_Venta { get; set; }
        public string Caja2 { get; set; }
        public string Folio_Documento { get; set; }
        public string Tipo_Documento { get; set; }
        public Nullable<int> Tipo_Impto { get; set; }
        public Nullable<decimal> Impto { get; set; }
        public Nullable<decimal> Neto { get; set; }
        public decimal Bruto { get; set; }
    }
}
