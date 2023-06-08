using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaGestion.Models.ViewModels
{
    public class ReporteVentasView
    {
        public Nullable<int> idTienda { get; set; }
        public string TipodePago { get; set; }
        public Nullable<decimal> TotalVenta { get; set; }
        public Nullable<decimal> Porcentaje { get; set; }
        public Nullable<int> Trx { get; set; }
    }
}