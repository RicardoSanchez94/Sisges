using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaGestion.Models.ViewModels
{
    public class CargaSencilloView
    {
        public DateTime Inicio { get; set; }

        public DateTime Fin { get; set; }

        public HttpPostedFileBase oFile { get; set; }

    }
}