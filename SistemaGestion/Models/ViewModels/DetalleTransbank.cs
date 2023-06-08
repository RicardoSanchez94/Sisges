using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaGestion.Models.ViewModels
{
    public class DetalleTransbank
    {
        public class Data
        {
            public string fechaTransaccion { get; set; }
            public string numeroTarjeta { get; set; }
            public string tipoProducto { get; set; }
            public string tipoCuota { get; set; }
            public string totalCuotas { get; set; }
            public string montoCuotas { get; set; }
            public string codigoAutorizacion { get; set; }
            public string montoAfecto { get; set; }
            public string montoExentoTotal { get; set; }
            public string codigoLocal { get; set; }
            public string nombreLocal { get; set; }
            public string tipoTransaccion { get; set; }
            public string fechaAbono { get; set; }
            public string ordenPedido { get; set; }
            public string montoVenta { get; set; }
        }
    }
    public class Lista
    {

        public List<DetalleTransbank.Data> Api { get; set; }

        public Lista()
        {
            this.Api = new List<DetalleTransbank.Data>();

        }

    }
}