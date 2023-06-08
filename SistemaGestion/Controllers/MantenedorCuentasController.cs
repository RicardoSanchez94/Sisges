using SistemaGestion.Models;
using SistemaGestion.Models.DAO;
using SistemaGestion.Models.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SistemaGestion.Controllers
{
    [Authorize]
    public class MantenedorCuentasController : Controller
    {
        private CuentaContables carga = new CuentaContables();
        // GET: MantenedorCuentas

        [AuthorizeRoles("Administrador", "Cuadratura")]
        public ActionResult Index()
        {
          List<Cuenta> lst = new List<Cuenta>();
            lst = carga.ObtenerCuentasContables();
            return View(lst);
        }

        public ActionResult Edit(int Codigo)
        {
            Cuenta CuentaContables = new Cuenta();
            CuentaContables = carga.ObtenerCuentasContablesID(Codigo);
            return PartialView("_Edit",CuentaContables);
        }

        [HttpPost]
        public JsonResult ModificarCuentaContable(Cuenta CuentaContable)
        {
            ResponseModel response = new ResponseModel();
            Cuenta CuentaContables = new Cuenta();
            response = carga.ModificarCuenta(CuentaContable);
            return Json(response);
        }

        [HttpGet]
        public ActionResult NewCuentaContables ()
        {
            Cuenta CuentaContables = new Cuenta();
            return PartialView("_New",CuentaContables);
        }

        public ActionResult AggCuentaContable(Cuenta CuentaContable)
        {
            ResponseModel response = new ResponseModel();
            response = carga.InsertarCuentaContable(CuentaContable);

            return Json(response);
        }

    }
}