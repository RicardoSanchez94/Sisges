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
    
    public partial class fnReporte_CAPA_PAGOS_Result
    {
        public string CodigoEmpresa { get; set; }
        public Nullable<System.DateTime> FechaAutorizacion { get; set; }
        public string CodigoAgencia { get; set; }
        public Nullable<int> NumeroTerminal { get; set; }
        public string CodigoUsuario { get; set; }
        public string NumeroAutorizacion { get; set; }
        public string NumeroBoleta { get; set; }
        public string TipoArchivo { get; set; }
        public string CodigoCliente { get; set; }
        public Nullable<int> Numerocuenta { get; set; }
        public Nullable<int> Monto { get; set; }
        public Nullable<int> CodigoMedioPago { get; set; }
        public string DetalleMovimiento { get; set; }
        public Nullable<int> NroTrxPos { get; set; }
        public string NroRef { get; set; }
        public Nullable<int> NroCaja { get; set; }
        public string Rut_GL { get; set; }
        public string Estado { get; set; }
        public Nullable<int> Cuenta_GL { get; set; }
        public Nullable<decimal> Monto_GL { get; set; }
        public Nullable<decimal> MontoPago_GL { get; set; }
    }
}
