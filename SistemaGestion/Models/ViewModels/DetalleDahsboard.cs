using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaGestion.Models.ViewModels
{
    public class DetalleDahsboard
    {
        public string Fecha { get; set; }

        public string Centro { get; set; }

        public decimal Total_Tienda { get; set; }

        public decimal Total_SAP { get; set; }

        public System.Nullable<decimal> Diferencia { get; set; }
    }
    public class Detalles
    {

        public List<DetalleDahsboard> Detalle { get; set; }

        public Detalles()
        {
            this.Detalle = new List<DetalleDahsboard>();

        }

    }
}