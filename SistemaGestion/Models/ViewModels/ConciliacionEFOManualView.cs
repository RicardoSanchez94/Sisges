using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaGestion.Models.ViewModels
{
    public class ConciliacionEFOManualView
    {
        public List<LibroMayor> ListaFI {get;set;}

        public List<LibroMayor> ListaZQ { get; set; }

        public ConciliacionEFOManualView ()
        {
            ListaFI = new List<LibroMayor>();
            ListaZQ = new List<LibroMayor>();
        }

    }
}