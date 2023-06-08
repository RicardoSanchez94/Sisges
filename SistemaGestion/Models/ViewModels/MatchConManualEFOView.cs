using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SistemaGestion.Models.ViewModels
{
    public class MatchConManualEFOView
    {
        public LibroMayor FI { get; set; }

        public List<LibroMayor> ListaZQ { get; set; }
        public List<SelectListItem> ZQLista { get; set; }
        
        public Guid idZQ { get; set; }

        public MatchConManualEFOView()
        {
            FI = new LibroMayor();
            ListaZQ = new List<LibroMayor>();
            ZQLista = new List<SelectListItem>();
        }

      
    }
}