using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaGestion.Models.ViewModels
{
    public class SencillosTiendasView
    {
        public List<DetalleSencillo> SencillosRecepcion { get; set; }

        public List<Sencillos_Tiendas> SencillosDevolucion { get; set; }

        public SencillosTiendasView()
        {
            SencillosRecepcion = new List<DetalleSencillo>();
            SencillosDevolucion = new List<Sencillos_Tiendas>();
        }
    }
}