using SistemaGestion.Models;
using SistemaGestion.Models.DAO;
using SistemaGestion.Models.Repositorios;
using SistemaGestion.Models.Security;
using SistemaGestion.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SistemaGestion.Controllers
{
    [Authorize]
    public class ApiTransbankController : Controller
    {
        // GET: ApiTransbank
        #region Aperturas de Objetos
        private ApiClient carga = new ApiClient();
        private Negocio ng = new Negocio();
        private prueba objeto = new prueba();
        private TransaccionesDetalle lista = new TransaccionesDetalle();
        //GestionDataContext dc = new GestionDataContext();
        private SisGesEntities3 db = new SisGesEntities3();

        #endregion
        
        #region LocalesTransbank
        public JsonResult TiendaTransbank()
        {
        
               List<LocalesTransbank> lst2 = new List<LocalesTransbank>();
            lst2 = ng.TiendasTbk();
            
            return Json(new { data = lst2 }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Controladores de Vistas
        [HttpGet]
        public ActionResult TransaccionesTotales()
        {
            TransacionesTotales.Root transacionestotales = new TransacionesTotales.Root();
            return View(transacionestotales);
        }

        [AuthorizeRoles("Administrador", "Cuadratura")]
        public ActionResult Index()
        {
            List<SelectListItem> lst = new List<SelectListItem>();
            using (Models.SisGesEntities3 db = new Models.SisGesEntities3())
            {
                lst = (from d in db.TIPO_PAGO.ToList()
                       select new SelectListItem
                       {
                           Value = d.id.ToString(),
                           Text = d.tipo

                       }).ToList();
            }


            return View(lst);
        }
        [HttpGet]
        public JsonResult CodigosTBk(int Tipo)
        {
            List<ElementoJsonKeyTBK> lst = new List<ElementoJsonKeyTBK>();

            lst = (from d in db.CODIGO_COMERCIO_TBK.ToList()
                   where d.Tipo == Tipo
                   select new ElementoJsonKeyTBK
                   {
                       Value = d.CodigoComercio,
                       Text = d.NombreLocal + -d.CodigoComercio

                   }).ToList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TransaccionesApi()
        {
            List<Ventas_Apitbk> lst = new List<Ventas_Apitbk>();

            return View(lst);
        }

        #endregion

        #region DashboarMediosPagos

        [HttpPost]
        public ActionResult TransaccionesApi(int local, DateTime Fecha)
        {

            //List<Transbankapi> lst = new List<Transbankapi>();
            List<Ventas_Apitbk> lst = carga.ObtenerTransacciones(local, Fecha);
            return PartialView("_Detalle", lst);
        }

        [HttpPost]
        public ActionResult TransaccionesTotales(DateTime FechaIni, DateTime FechaFin, int local)
        {


            var response = new ResponseModel();
            if (local == 0)
            {
                response.error = false;
                response.respuesta = "Debe Seleccionar al menos una tienda";
                return Json(response);
            }
            var totaldias = (FechaFin - FechaIni).Days;
            if (totaldias > 30 || totaldias < 0)
            {
                response.error = false;
                response.respuesta = "El rango de las fechas debe ser inferior a 30 dias";
                return Json(response);
            }
            else if (FechaIni >= DateTime.Today || FechaFin >= DateTime.Today)
            {
                response.error = false;
                response.respuesta = "El rango de las fechas debe ser inferior al Dia de Hoy";
                return Json(response);
            }


            objeto.response = response;


            var FechaI = FechaIni.ToString("yyyMMdd");
            var fechaf = FechaFin.ToString("yyyMMdd");

            SisGesEntities3 db = new SisGesEntities3();

            objeto.TotalesTranbank = (from d in db.Proc_TotalesTransbankApi(FechaI, fechaf, local)
                                      select new Proc_TotalesTransbankApi_Result
                                      {
                                          Fecha = d.Fecha,
                                          IdTienda = d.IdTienda,
                                          TipoPago = d.TipoPago,
                                          Trx = d.Trx,
                                          Total = d.Total != null ? Math.Round(d.Total.Value) : 0
                                      }).ToList();


            objeto.TotalesMedios = (from d in db.Proc_TotaleMediosPagos(FechaI, fechaf, local)
                                    select new Proc_TotaleMediosPagos_Result
                                    {
                                        Fecha = d.Fecha,
                                        IdTienda = d.IdTienda,
                                        TipoPago = d.TipoPago,
                                        Total = d.Total != null ? Math.Round(d.Total.Value) * (-1) : 0
                                    }).ToList();

            objeto.Root = carga.ObtenerTotales(FechaIni, FechaFin, local);

            return PartialView("_Transbank", objeto);
        }


        #endregion
    }
}