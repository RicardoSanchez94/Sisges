using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaGestion.Models.ViewModels
{
    public class AcumuladoSobranteFaltanteView
    {
        public AcumuladoSobrante_Faltante AcumuladoSobrante_Faltante { get; set; }
        public List<TipoAceptacion> lstTipoAceptacion { get; set; }
        public List<TipoRechazo> lstTipoRechazo { get; set; }

        public int Total { get; set; }

        public List<AcumuladoSobranteCheckView> lstSobranteFaltantecheck { get; set; }
        //public AceptacionSobranteFaltante AceptacionSobranteFaltante { get; set; }

        public List<Sobrante_Faltante> Lstsobrante_Faltantes { get; set; }
        public AcumuladoSobranteFaltanteView ()
        {
            this.lstSobranteFaltantecheck = new List<AcumuladoSobranteCheckView>();
            this.Lstsobrante_Faltantes = new List<Sobrante_Faltante>();
        }
    }
}