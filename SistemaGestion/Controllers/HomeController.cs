using SistemaGestion.Models.DAO;
using SistemaGestion.Models.Security;
using SistemaGestion.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

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
        [HttpGet]

        public ActionResult CerrarSesion()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.Abandon();

            // Invalidar la cookie de autenticación
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            cookie.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie);

            // Deshabilitar la caché del navegador
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            return RedirectToAction("LogIn", "Account");
        }

    }
}