using SistemaGestion.Models.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaGestion.Models.Repositorios
{
    public class TransacionesTotales
    {


        public class Data
        {
            public string totalVentasAbonadas { get; set; }
            public string totalVentasAbonoPendiente { get; set; }
            public string totalVentasNoProcesadas { get; set; }
            public string totalVentasAnuladas { get; set; }
            public string totalRegistros { get; set; }
        }

        public class Links
        {
            public string self { get; set; }
        }

        public class Meta
        {
            public string _msgId { get; set; }
            public string _version { get; set; }
            public DateTime _rqDateTime { get; set; }
            public int _rsLength { get; set; }
            public string _clientId { get; set; }
            public string _transactionId { get; set; }
        }

        public class Root
        {
            public Data data { get; set; }
            public Meta meta { get; set; }
            public Links links { get; set; }

        }
    }

    public class prueba
    {
        public TransacionesTotales.Root Root { get; set; }

        public ResponseModel response { get; set; }

        public List<Proc_TotaleMediosPagos_Result> TotalesMedios { get; set; }

        public List<Proc_TotalesTransbankApi_Result> TotalesTranbank { get; set; }



        public prueba()
        {
            this.Root = new TransacionesTotales.Root();
            this.response = new ResponseModel();
            this.TotalesMedios = new List<Proc_TotaleMediosPagos_Result>();
            this.TotalesTranbank = new List<Proc_TotalesTransbankApi_Result>();
        }

    }
}