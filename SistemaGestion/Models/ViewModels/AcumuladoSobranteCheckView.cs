using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaGestion.Models.ViewModels
{
    public class AcumuladoSobranteCheckView
    {
        public string ID_Empleado { get; set; }
        public int IDTIENDA { get; set; }

        public DateTime Fecha { get; set; }

        public int CAJA { get; set; }

        public int TOTAL { get; set; }

        public bool isCheck { get; set; }
    }
}