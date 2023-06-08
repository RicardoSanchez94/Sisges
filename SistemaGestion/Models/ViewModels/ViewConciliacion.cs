using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaGestion.Models.ViewModels
{
    public class ViewConciliacion
    {
        public string idTransbank { get; set; }

        public string idMediosPagos { get; set; }

        public string Fecha { get; set; }

        public string MontosTransbank { get; set; }

        public decimal MontosMediosPagos { get; set; }

        public decimal MontosMediosAfecto { get; set; }

        public string MontoTransbankAfecto { get; set; }


    }
}