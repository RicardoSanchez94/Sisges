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
    
    public partial class Pagos_Apitbk
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Pagos_Apitbk()
        {
            this.DetalleConciliacionPagos = new HashSet<DetalleConciliacionPagos>();
        }
    
        public string codigoAutorizacion { get; set; }
        public int codigoLocal { get; set; }
        public Nullable<System.DateTime> fechaAbono { get; set; }
        public System.DateTime fechaTransaccion { get; set; }
        public Nullable<decimal> montoCuotas { get; set; }
        public decimal montoVenta { get; set; }
        public string nombreLocal { get; set; }
        public string numeroTarjeta { get; set; }
        public string tipoProducto { get; set; }
        public string tipoTransaccion { get; set; }
        public Nullable<int> totalCuotas { get; set; }
        public string tipoCuota { get; set; }
        public decimal montoAfecto { get; set; }
        public string montoExentoTotal { get; set; }
        public string ordenPedido { get; set; }
        public Nullable<bool> Estado { get; set; }
    
        public virtual CODIGO_COMERCIO_TBK CODIGO_COMERCIO_TBK { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DetalleConciliacionPagos> DetalleConciliacionPagos { get; set; }
    }
}
