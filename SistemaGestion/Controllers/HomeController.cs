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
    public class HomeController : Controller
    {

        #region ObjetoContenedor
        private Dashboard carga = new Dashboard();

        #endregion
        [AuthorizeRoles("Administrador", "Cuadratura")]
        public ActionResult Index()
        {
            List<DetalleDahsboard> lst = new List<DetalleDahsboard>();
            return View(lst);
        }
        [HttpPost]
        public ActionResult Listar(DateTime Fecha)
        {


            //List<DetalleDahsboard> lst = new List<DetalleDahsboard>();
            List<DetalleDahsboard> lst = carga.DetalleDiferencias(Fecha);
            return PartialView("_Diferencias", lst);

        }
        public ActionResult CerrarSesion()
        {
            Session["User"] = null;
            return RedirectToAction("Login", "Account");
        }

    }
}