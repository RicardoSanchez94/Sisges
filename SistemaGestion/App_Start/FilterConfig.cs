using SistemaGestion.Models.DAO;
using System.Web;
using System.Web.Mvc;

namespace SistemaGestion
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            //filters.Add(new VerficacionPerfiles());
        }
    }
}
