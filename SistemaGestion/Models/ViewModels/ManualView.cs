using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaGestion.Models.ViewModels
{
    public class ManualView
    {
        public string AutorizadorTbk { get; set; }

        public decimal MontoTbk { get; set; }

        public List<Mp> LstMp { get; set; }

        public int TrxMP { get; set; }

        public DateTime Fecha { get; set; }

        public int Centro { get; set; } 

        public int Local { get; set; }

        public int Tipo_Pago { get; set; }

        public class Mp
        {

            public int Trx { get; set; }

            public string AutorizadorMP { get; set; }

            public decimal Monto { get; set; }

            public string Nombre { get; set; }


        }
    }
}