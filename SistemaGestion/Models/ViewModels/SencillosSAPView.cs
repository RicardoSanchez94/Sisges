using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaGestion.Models.ViewModels
{
    public class SencillosSAPView
    {
        public int? idTienda { get; set; }
        public int ?MontoTesoreria { get; set; }

        public int  ?MontoSAP { get; set; }

        public string Banco { get; set; }

        public string DiaLiberacion { get; set; }
        public string DiaEntrega { get; set; }

        public bool ?Estado { get; set; }

        public string Asignacion { get; set; }

        public DateTime ?FechaDoc { get; set; }

    }
}