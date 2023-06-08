using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaGestion.Models.Repositorios
{
    public class Transacciones
    {
        public class Datum
        {
            public string fechaTransaccion { get; set; }
            public string numeroTarjeta { get; set; }
            public string tipoProducto { get; set; }
            //public string tipoCuota { get; set; }
            public string totalCuotas { get; set; }
            public string montoCuotas { get; set; }
            public string codigoAutorizacion { get; set; }
            //public string montoAfecto { get; set; }
            //public string montoExentoTotal { get; set; }
            public string codigoLocal { get; set; }
            public string nombreLocal { get; set; }
            public string tipoTransaccion { get; set; }
            public string fechaAbono { get; set; }
            //public string ordenPedido { get; set; }
            public string montoVenta { get; set; }
        }

        public class Links
        {
            public string self { get; set; }
            public string first { get; set; }
            public string prev { get; set; }
            public string next { get; set; }
            public string last { get; set; }
        }

        public class Meta
        {
            public int _rsLength { get; set; }
            public string _msgId { get; set; }
            public string _version { get; set; }
            public DateTime _rqDateTime { get; set; }
            public string _clientId { get; set; }
            public string _transactionId { get; set; }
        }

        public class Root
        {
            public List<Datum> data { get; set; }
            public Meta meta { get; set; }
            public Links links { get; set; }
        }
    }

    public class TransaccionesDetalle
    {
        public Transacciones.Root Root { get; set; }

        //public Transacciones response { get; set; }



        public TransaccionesDetalle()
        {
            this.Root = new Transacciones.Root();
            //this.response = new ResponseModel();
        }

    }
}