using Newtonsoft.Json;
using SistemaGestion.Models;
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
    [Authorize]
    public class AdministracionController : Controller
    {
        // GET: Administracion
        private AdministracionGS administracionGS = new AdministracionGS();
        private Negocio ng = new Negocio();
        private SisGesEntities3 db = new SisGesEntities3();


        public ActionResult ReprocesoTbk ()
        {
            return View();
        }

        public async Task<ActionResult> GetReprocesoTbk(DateTime Fecha)
        {
            ResponseModel response = new ResponseModel();
            response = await ng.ReprocesoTbk(Fecha);
            return Json(response);
        }

        public ActionResult ReprocesoInterfaces()
        {
            return View();
        }

        public async Task<ActionResult> GetReprocesoInterfaces(DateTime Fecha)
        {
            AdminUsuarios ADU = new AdminUsuarios();
            var UserJson = Thread.CurrentPrincipal.Identity.Name;
            var Deserialize = JsonConvert.DeserializeObject<UserLoginView>(UserJson);
           
           

            ResponseModel response = new ResponseModel();
            
            response = await ng.GetToken(Deserialize);
            if (!response.error)
            {
                response = await ng.ReprocesoInterfaces(Fecha,response.token);
            }
          
            return Json(response);
        }


        public ActionResult Index()
        {

            return View();
        }

        public ActionResult BarridoGS()
        {

            return View();
        }
        [HttpPost]
        public ActionResult BarridoGS(DateTime Inicio, DateTime Fin)
        {
            ResponseModel response = new ResponseModel();
            if (!db.Periodo.Where(x => x.Cerrado == false && x.AnoNumero == Inicio.Year && x.MesNumero == Inicio.Month).Any())
            {
                response.error = true;
                response.respuesta = "El periodo que intenta reprocesar se encuentra cerrado";
                return Json(response);
            }
            var res = administracionGS.BarridoGS(Inicio, Fin);
            return Json(res);
        }

        public ActionResult ReprocesoMediosPagos()
        {

            return View();
        }
        [HttpPost]
        public ActionResult ReprocesoMediosPagos(DateTime Fecha)
        {
            ResponseModel response = new ResponseModel();

            if (!db.Periodo.Where(x => x.Cerrado == false && x.AnoNumero == Fecha.Year && x.MesNumero == Fecha.Month).Any())
            {
                response.error = true;
                response.respuesta = "El periodo que intenta reprocesar se encuentra cerrado";
                return Json(response);
            }

            response = administracionGS.GenerarMediosPagosPorFecha(Fecha);

            Auditoria auditoria = new Auditoria();

            var UserJson = Thread.CurrentPrincipal.Identity.Name;
            var Deserialize = JsonConvert.DeserializeObject<UserLoginView>(UserJson);

            auditoria.Descripcion = $"Se realiza reproceso de Medios de pago por el Usuario {Deserialize.LoginName}";
            auditoria.IdUsuario = Deserialize.idUsuario;
            auditoria.Fecha = DateTime.Now;

            ng.Auditoria(auditoria);

            return Json(response);
        }

        [HttpGet]
        public ActionResult CerrarPeriodo()
        {
            return View();
        }


   
        [HttpPost]
        public async Task<ActionResult> ENDPeriodo()
        {
            ResponseModel response = new ResponseModel();

            response = ng.CierrePeriodo();
            if (!response.error)
            {
                Auditoria auditoria = new Auditoria();

                var UserJson = Thread.CurrentPrincipal.Identity.Name;
                var Deserialize = JsonConvert.DeserializeObject<UserLoginView>(UserJson);
                auditoria.Descripcion = $"{response.respuesta} por el Usuario {Deserialize.LoginName}";
                auditoria.IdUsuario = Deserialize.idUsuario;
                auditoria.Fecha = DateTime.Now;

                 await ng.GetAlertaCierre(auditoria.Descripcion);
                ng.Auditoria(auditoria);
            }

           

            return Json(response);
        }

    }
}
