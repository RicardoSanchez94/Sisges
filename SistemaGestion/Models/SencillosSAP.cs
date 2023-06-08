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
    
    public partial class SencillosSAP
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SencillosSAP()
        {
            this.DetalleSencillo = new HashSet<DetalleSencillo>();
        }
    
        public System.Guid id { get; set; }
        public string Referencia { get; set; }
        public string Asignacion { get; set; }
        public string N_doc { get; set; }
        public string Cla { get; set; }
        public Nullable<int> Periodo { get; set; }
        public Nullable<System.DateTime> Fecha_Doc { get; set; }
        public string IO { get; set; }
        public string Lib_Mayor { get; set; }
        public Nullable<int> ImporML { get; set; }
        public string Texto { get; set; }
        public string RutProvedor { get; set; }
        public Nullable<bool> ConciliacionTesoreria { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DetalleSencillo> DetalleSencillo { get; set; }
    }
}
