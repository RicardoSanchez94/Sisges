using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaGestion.Models.ViewModels
{
    public class AcumuladoFaltanteView
    {

        public class AcumuladoFaltante
        {

            public int idTienda { get; set; }

            public string idEmpleado { get; set; }

            public int Total { get; set; }
        }

        public class AceptadosRechazados
        {
            List<AcumuladoSobrante_Faltante> lstAceptadosRechazados { get; set; }

            public AceptadosRechazados()
            {
                this.lstAceptadosRechazados = new List<AcumuladoSobrante_Faltante>();
            }
        }

        public class Data
        {
            public List<AcumuladoFaltante> AcumuladoFaltante { get; set; }

            public List<AcumuladoSobrante_Faltante> aceptadosRechazados { get; set; }

            public Data()
            {
                this.aceptadosRechazados = new List<AcumuladoSobrante_Faltante>();
            }

        }
    }
}