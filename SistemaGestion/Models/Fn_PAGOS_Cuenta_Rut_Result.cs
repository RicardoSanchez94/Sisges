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
    
    public partial class Fn_PAGOS_Cuenta_Rut_Result
    {
        public string Tipo_Trx { get; set; }
        public int IdTienda { get; set; }
        public short Trx_Global { get; set; }
        public int Caja { get; set; }
        public Nullable<short> Tipo { get; set; }
        public int TRX { get; set; }
        public System.DateTime Fecha { get; set; }
        public string Cajero { get; set; }
        public Nullable<decimal> Monto { get; set; }
        public string Autorizador { get; set; }
        public string Autorizador_TBK { get; set; }
        public string NumeroReferencia { get; set; }
        public string RutCliente { get; set; }
        public short Tipo_Pago { get; set; }
        public string Pago { get; set; }
        public decimal Monto_Pago { get; set; }
        public string Tarjeta { get; set; }
        public string NCheque { get; set; }
        public string RutCheque { get; set; }
        public string Banco { get; set; }
        public string FechaCheque { get; set; }
        public string Estado { get; set; }
        public Nullable<bool> EstadoConciliado { get; set; }
        public Nullable<int> cuenta { get; set; }
        public Nullable<int> rut_cliente { get; set; }
    }
}
