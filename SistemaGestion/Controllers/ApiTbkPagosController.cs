using SistemaGestion.Models;
using SistemaGestion.Models.DAO;
using SistemaGestion.Models.Security;
using SistemaGestion.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SistemaGestion.Controllers
{
    [Authorize]

    public class ApiTbkPagosController : Controller
    {
        private ApiPagos carga = new ApiPagos();
        private Negocio ng = new Negocio();
        // GET: ApiTbkPagos

        #region LocalesTransbankPagos
        public JsonResult TiendaTransbank()
        {

            List<LocalesTransbank> lst2 = new List<LocalesTransbank>();
            lst2 = ng.TiendasTbkPagos();

            return Json(new { data = lst2 }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles("Administrador", "Cuadratura")]
        public ActionResult TrkPagoApi()
        {
            List<Pagos_Apitbk> lst = new List<Pagos_Apitbk>();
            return View(lst);
        }
        [HttpPost]
       public ActionResult TrkPagoApi(int Local, DateTime Fecha)
        {
            List<Pagos_Apitbk> lst = new List<Pagos_Apitbk>();
            lst = carga.ObtenerTrxPagos(Local, Fecha);
            return PartialView("_TrxPagos",lst);
        }

    }
}