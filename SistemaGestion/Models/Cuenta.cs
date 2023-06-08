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
    
    public partial class Cuenta
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Cuenta()
        {
            this.Diferencias_Pagos = new HashSet<Diferencias_Pagos>();
            this.DetalleConciliacionPagos = new HashSet<DetalleConciliacionPagos>();
            this.DetalleConciliacion = new HashSet<DetalleConciliacion>();
        }
    
        public int Codigo { get; set; }
        public Nullable<int> TipoPago { get; set; }
        public string NumeroCuenta { get; set; }
        public string Descripcion { get; set; }
        public bool Estado { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Diferencias_Pagos> Diferencias_Pagos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DetalleConciliacionPagos> DetalleConciliacionPagos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DetalleConciliacion> DetalleConciliacion { get; set; }
    }
}
