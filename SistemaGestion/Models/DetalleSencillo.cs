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
    
    public partial class DetalleSencillo
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DetalleSencillo()
        {
            this.MontoSencillo = new HashSet<MontoSencillo>();
            this.Sencillos_Tiendas = new HashSet<Sencillos_Tiendas>();
        }
    
        public System.Guid IdSencillo { get; set; }
        public Nullable<int> IdTienda { get; set; }
        public string Banco { get; set; }
        public Nullable<int> Total { get; set; }
        public string DiaLiberacion { get; set; }
        public string DiaEntrega { get; set; }
        public System.Guid Id { get; set; }
        public Nullable<int> NuevoTotal { get; set; }
        public Nullable<bool> ConciliacionTesoreria { get; set; }
        public Nullable<System.Guid> IdSencillosSAP { get; set; }
        public Nullable<System.DateTime> FechaLiberacion { get; set; }
        public Nullable<System.DateTime> FechaEntrega { get; set; }
    
        public virtual CENTROS_LOCAL CENTROS_LOCAL { get; set; }
        public virtual Sencillos Sencillos { get; set; }
        public virtual SencillosSAP SencillosSAP { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MontoSencillo> MontoSencillo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Sencillos_Tiendas> Sencillos_Tiendas { get; set; }
    }
}