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
    
    public partial class notaCreditoEspecial1
    {
        public int TRX { get; set; }
        public Nullable<System.DateTime> Fecha_Emision_Nota_Credito { get; set; }
        public string Hora_Emision_Nota_Credito { get; set; }
        public Nullable<int> IdTienda_notacredito { get; set; }
        public Nullable<int> Caja_notacredito { get; set; }
        public string TIPONOTACREDITO { get; set; }
        public string Folio_notacredito { get; set; }
        public string Cliente_notacredito { get; set; }
        public Nullable<decimal> Total_notacredito { get; set; }
        public string Documento_Origen { get; set; }
        public Nullable<int> Trx_Origen { get; set; }
        public Nullable<long> Nro_Boleta { get; set; }
        public Nullable<int> IdTienda_Origen_Boleta { get; set; }
        public Nullable<int> Caja_Origen_Boleta { get; set; }
        public Nullable<System.DateTime> Fecha_Boleta { get; set; }
        public Nullable<decimal> Monto_Boleta { get; set; }
        public Nullable<int> Tipo_Pago_Origen { get; set; }
        public string Rut_Cajero { get; set; }
        public string Rut_Supervisor { get; set; }
        public string Nombre_trabajador { get; set; }
        public string Descripcion_pago { get; set; }
        public string Nombre_Jefe { get; set; }
        public string Hora_Aprov_Nota_Credito { get; set; }
    }
}
