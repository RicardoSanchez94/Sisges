using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaGestion.Models.DAO
{
    public class ApiPagos
    {
        private SisGesEntities3 bd = new SisGesEntities3();
        public List<Pagos_Apitbk> ObtenerTrxPagos(int local, DateTime Fecha)
        {
            List<Pagos_Apitbk> lst = new List<Pagos_Apitbk>();
            lst = bd.Pagos_Apitbk.Where(x => x.codigoLocal == local && x.fechaTransaccion == Fecha).ToList();
            return lst;
        }
    }
}