using Newtonsoft.Json;
using SistemaGestion.Models.DAO;
using SistemaGestion.Models.Repositorios;
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
    public class AdminUserController : Controller
    {
        private AdminUsuarios ADU = new AdminUsuarios();

        [AuthorizeRoles("Administrador")]
        public ActionResult PanelControl()
        {
            return View();
        }

        public ActionResult ListaUsuarios()
        {
            //**  SE INSTALO System.Linq.Dynamic POR NuGet **

            //*******************************************************************************
            //********* LISTA DE DATOS QUE VIENEN DESDE EL CLIENTE, DESDE DATATABLE *********
            //*******************************************************************************
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            var search = Request.Form.GetValues("search[value]").FirstOrDefault();
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            //*******************************************************************************
            //******************* VARIABLES PARA DIFERENTES USOS *****************************
            //*******************************************************************************
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;
            int sInfoFiltered = 0;
            //*******************************************************************************
            //**********         OBTENGO LOS DATOS A MOSTRAR EN LA TABLA     ****************
            //*******************************************************************************
            IEnumerable<UserProfileView> data = new List<UserProfileView>();
            if (User.Identity.IsAuthenticated)
            {
                var Json = JsonConvert.DeserializeObject<UserLoginView>(User.Identity.Name);
                //string loginName = User.Identity.Name;

                data = ADU.GetUsersDataView(Json.LoginName);//.ToList().OrderBy(sortColumn + " " + sortColumnDir);
            }

            Func<UserProfileView, string> orderingFunction = (c => sortColumn == "loginName" ? c.LoginName : sortColumn == "nombres" ? c.nombres : sortColumn == "ApellidoPaterno" ? c.ApellidoPaterno : sortColumn == "ApellidoMaterno" ? c.ApellidoMaterno : c.LoginName);
            if (sortColumnDir == "desc")
            {
                data = data.OrderByDescending(orderingFunction).AsQueryable();
            }
            else
            {
                data = data.OrderBy(orderingFunction).AsQueryable();
            }

            // CANTIDAD DE REGISTROS OBTENIDOS
            recordsTotal = data.Count();

            // SI LA VARIABLE SEARCH VIENE CON DATOS SE HACE EL FILTRO A LAS COLUMNAS DE ORIGEN DE DATOS
            if (!(string.IsNullOrEmpty(search)))
            {
                data = data.Where(
                    p => (p.LoginName.ToLower().Contains(search.ToLower().Trim())) ||
                         (p.nombres.ToLower().Contains(search.ToLower().Trim())) ||
                         (p.ApellidoPaterno.ToLower().Contains(search.ToLower().Trim())) ||
                         (p.ApellidoMaterno.ToLower().Contains(search.ToLower().Trim()))
                        ).ToList();
                // SETEA EL NUMERO TOTAL DE REGISTROS CUANDO SE FILTRA
                sInfoFiltered = data.Count();
            }
            else
            {
                // AL CARGAR LA PAGINA NO EXISTE FILTRO POR ENDE EL TOTAL SERA EL INICIAL
                sInfoFiltered = recordsTotal;
            }
            //SE FILTRA NUEVAMENTE EL ORIGEN DE DATOS MOSTRANDO LOS REGISTROS DE 10 EN 10, DEPENDIENDO DEL LARGO QUE SE DEFINE
            var dataFinal = data.Skip(skip).Take(pageSize).ToList();

            // SE ENVIAN LOS DATOS AL CLIENTE, SOLO SE ENVIA EL RANGO DE REGISTROS, NO LA DATA COMPLETA.
            return Json(new { draw = draw, recordsFiltered = sInfoFiltered, recordsTotal = recordsTotal, data = dataFinal }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult adminZonas()
        {
            var model = ADU.GetZonasUSer();
            return PartialView("_adminZonas", model);
        }
        public ActionResult zonasByUser(Guid idUser)
        {
            var model = ADU.getZonasByUser(idUser);
            return PartialView("_zonasByUser", model);
        }

        [HttpPost]
        public ActionResult SaveAsigZonas(adminZonalView user, int[] zonas_seleccionadas = null)
        {
            var res = new ResponseModel();
            try
            {

                if (zonas_seleccionadas != null)
                {
                    res = ADU.saveAsigZona(user, zonas_seleccionadas);
                    //return Json(res);
                }
                else
                {
                    res.error = false;
                    res.respuesta = "DEBE SELECCIONAR A LO MENOS 1 ZONA";
                    //return Json(res);
                }

            }
            catch (Exception EX)
            {
                res.error = false;
                res.respuesta = EX.ToString();
                //return Json(res);
            }
            return Json(res);
        }

    }
}
