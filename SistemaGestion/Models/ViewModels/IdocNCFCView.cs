using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaGestion.Models.ViewModels
{
    public class IdocNCFCView
    {
        public DateTime FechaVenta { get; set; }

        public int Centro { get; set; }

        public string TipoDoc { get; set; }
        public string Folio { get; set; }

        public decimal Monto { get; set; }

        public string Rut { get; set; }

        public bool isCheck { get; set; }

    }
}