using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaGestion.Models.ViewModels
{
    public class AceptacionSobranteFaltanteView
    {
        public Sobrante_Faltante Sobrante_Faltante { get; set; }
        public List<TipoAceptacion> lstTipoAceptacion { get; set; }
        //public AceptacionSobranteFaltante AceptacionSobranteFaltante { get; set; }
    }
}