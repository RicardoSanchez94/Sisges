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
    
    public partial class notaCredito
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public notaCredito()
        {
            this.boletaOrigen = new HashSet<boletaOrigen>();
        }
    
        public int TRX { get; set; }
        public System.DateTime Fecha_Emision { get; set; }
        public string Hora_Emision { get; set; }
        public int IdTienda { get; set; }
        public int Caja { get; set; }
        public string Tipo { get; set; }
        public string Folio { get; set; }
        public string Cliente { get; set; }
        public decimal Total { get; set; }
        public string Rut_Cajero { get; set; }
        public string Rut_Supervisor { get; set; }
        public string Hora_Aprobacion { get; set; }
    
        public virtual EMPLEADO EMPLEADO { get; set; }
        public virtual EMPLEADO EMPLEADO1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<boletaOrigen> boletaOrigen { get; set; }
    }
}
