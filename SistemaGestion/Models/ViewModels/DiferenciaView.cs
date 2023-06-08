using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SistemaGestion.Models.ViewModels
{
    public class DiferenciaView
    {
        public DateTime Fecha { get; set; }

        public int Centro { get; set; }

        public string Autorizador { get; set; }

        public string Tarjeta { get; set; }

        public int Local { get; set; }

        public int TipoAsiento { get; set; }

        public int TipoAsiento2 { get; set; }

        public int CuentaContable { get; set; }
        
        public int CuentaContable2 { get; set; }

        public List<CuentaContables> ListaCuentas1 {get;set;}

        public List<CuentaContables> ListaCuentas2 { get; set; }
        public decimal Monto { get; set; }

       


        public class CuentaContables
        {
            public int Codigo { get; set; }

            public string NumeroCuenta { get; set; }

            public string Descripcion { get; set; }
            public string Nombre { get; set; }
        }

        public static IEnumerable<SelectListItem> Asiento()
        {
            IList<SelectListItem> lista = new List<SelectListItem>();
            lista.Add(new SelectListItem() { Text = "40", Value = "40" });
            lista.Add(new SelectListItem() { Text = "50", Value = "50" });
            return lista;
        }

        

    }

}