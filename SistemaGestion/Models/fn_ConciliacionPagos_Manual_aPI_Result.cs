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
    
    public partial class fn_ConciliacionPagos_Manual_aPI_Result
    {
        public Nullable<System.DateTime> Fecha { get; set; }
        public Nullable<int> Centro { get; set; }
        public string CodAutorizador { get; set; }
        public Nullable<decimal> MontoV { get; set; }
        public Nullable<decimal> MontoAfecto { get; set; }
        public Nullable<bool> Tipo { get; set; }
        public string NombreLocal { get; set; }
        public Nullable<int> Trx { get; set; }
    }
}
