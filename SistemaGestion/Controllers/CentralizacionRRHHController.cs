using Newtonsoft.Json;
using SistemaGestion.Models.DAO;
using SistemaGestion.Models.Repositorios;
using SistemaGestion.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SistemaGestion.Controllers
{
    public class CentralizacionRRHHController : Controller
    {
        // GET: CentralizacionRRHH
        private Negocio ne = new Negocio();

        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> GetCentralizacion()
        {
            ResponseModel response = new ResponseModel();

            AdminUsuarios ADU = new AdminUsuarios();
            var UserJson = Thread.CurrentPrincipal.Identity.Name;
            var Deserialize = JsonConvert.DeserializeObject<UserLoginView>(UserJson);

            response = await ne.GetToken(Deserialize);

            return Json(response);
        }

    }
}