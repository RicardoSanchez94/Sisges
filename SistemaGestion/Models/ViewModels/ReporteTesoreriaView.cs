using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaGestion.Models.ViewModels
{
    public class ReporteTesoreriaView
    {
        public List<SencillosSAPView> SAP { get; set; }
        public List<fnSencillosTiendaNoConciliados_Result> NoConciliadas { get; set; }

        public ReporteTesoreriaView()
        {
            this.SAP = new List<SencillosSAPView>();
            this.NoConciliadas = new List<fnSencillosTiendaNoConciliados_Result>();
        }
    }
}