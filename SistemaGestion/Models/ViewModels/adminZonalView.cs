using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaGestion.Models.ViewModels
{
    public class adminZonalView
    {
        public Guid idUsuario { get; set; }
        public List<UserProfileView> listUserRolZonal { get; set; }
        public List<zonas> listZonas { get; set; }
        public List<zonas> listZonasSeleccionadas { get; set; }

        public adminZonalView()
        {
            this.listZonas = new List<zonas>();
            this.listZonasSeleccionadas = new List<zonas>();
            this.listUserRolZonal = new List<UserProfileView>();
        }
    }
    public class zonas
    {
        public int idZona { get; set; }
        public string desZona { get; set; }
        public bool estado { get; set; }
    }
}