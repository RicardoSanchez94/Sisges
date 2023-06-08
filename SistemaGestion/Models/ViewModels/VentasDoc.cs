using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaGestion.Models.ViewModels
{
    public class VentasDoc
    {
        public string centro { get; set; }

        public DateTime fecha { get; set; }

        public string tipo_doc { get; set; }

        public string clase_doc { get; set; }

        public decimal totven { get; set; }
        public decimal totliv { get; set; }
        public decimal diff { get; set; }

    }
    public class Ventas
    {
        public List<VentasDoc> Venta { get; set; }

        public Ventas()
        {
            this.Venta = new List<VentasDoc>();
        }
    }
}