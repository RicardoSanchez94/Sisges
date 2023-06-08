using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaGestion.Models.ViewModels
{
    public class DashboardPagoView
    {
        public DateTime Fecha { get; set; }

        public decimal Monto { get; set; }

        public int Local { get; set; }

        public int Trx { get; set; }

        public string Tipotrx { get; set; }

        public List<MP> MPPagos { get; set; }

        public List<TBK> TbkPagos { get; set; }


        public class MP
        {
            public DateTime Fecha { get; set; }

            public decimal Monto { get; set; }

            public int idTienda { get; set; }

            public string Tipo { get; set; }
        }

        public class TBK
        {
            public DateTime Fecha { get; set; }

            public decimal Monto { get; set; }

            public int idTienda { get; set; }

            public string Tipo { get; set; }
        }


    }
}