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
    
    public partial class MEDIOSPAGOS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MEDIOSPAGOS()
        {
            this.DetalleConciliacion = new HashSet<DetalleConciliacion>();
        }
    
        public int TRX { get; set; }
        public int IdTienda { get; set; }
        public int Caja { get; set; }
        public System.DateTime Fecha { get; set; }
        public string Tipo { get; set; }
        public string Folio { get; set; }
        public Nullable<short> Trans { get; set; }
        public Nullable<decimal> Monto { get; set; }
        public short Tipo_Pago { get; set; }
        public string Pago { get; set; }
        public string Tarjeta { get; set; }
        public string Autorizador { get; set; }
        public string AUTH_RESP_CD { get; set; }
        public decimal Monto_Aprobado { get; set; }
        public string NCheque { get; set; }
        public string RutCheque { get; set; }
        public string Banco { get; set; }
        public string FechaCheque { get; set; }
        public Nullable<bool> Estado { get; set; }
    
        public virtual CENTROS_LOCAL CENTROS_LOCAL { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DetalleConciliacion> DetalleConciliacion { get; set; }
    }
}