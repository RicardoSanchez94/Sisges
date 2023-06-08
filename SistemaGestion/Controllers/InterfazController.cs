using Newtonsoft.Json;
using OfficeOpenXml;
using SistemaGestion.Models;
using SistemaGestion.Models.DAO;
using SistemaGestion.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SistemaGestion.Controllers
{
    public class InterfazController : Controller
    {
        private Negocio ng = new Negocio();
        private CuadraturaCAAUCAPA caaucapa = new CuadraturaCAAUCAPA();

        // GET: Interfaz

        #region CauuVentas
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ConciliacionAutomatica(DateTime Fecha)
        {
            ResponseModel response = new ResponseModel();
            response = caaucapa.ConciliacionAutomaticaCAAU(Fecha);
            response.error = false;
            if (!response.error)
            {
                Auditoria auditoria = new Auditoria();
                var UserJson = Thread.CurrentPrincipal.Identity.Name;
                var Deserialize = JsonConvert.DeserializeObject<UserLoginView>(UserJson);
                auditoria.IdUsuario = Deserialize.idUsuario;
                auditoria.Descripcion = $" El usuario :{Deserialize.LoginName} genero ConciliacionAutomatica CAAU";
                auditoria.Fecha = DateTime.Now;
                ng.Auditoria(auditoria);
            }
            return Json(response);
        }

        public ActionResult getConciliadasCAAU(DateTime fecha)
        {
            List<fnReporte_CAAU_Result> data = new List<fnReporte_CAAU_Result>();

            data = caaucapa.getFuncionCAAU(fecha);
            
            return PartialView("_DataCAAU", data);
        }

        [HttpPost]
        public ActionResult ExcelConciliadasA(DateTime Fecha)
        {
            using (ExcelPackage excelPackage = new LibrosExcel().ReportConciliadasAutoCaau(caaucapa.getFuncionCAAU(Fecha)))
            {
                MemoryStream memoryStream = new MemoryStream();
                excelPackage.SaveAs((Stream)memoryStream);
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                DateTime dateTime = DateTime.Now;
                dateTime = dateTime.Date;
                string fileDownloadName = "ConciliadasCAAUVentas_" + Fecha.ToString("dd/MM/yyyy") + ".xlsx";
                memoryStream.Position = 0L;
                return File((Stream)memoryStream, contentType, fileDownloadName);
            }
        }

        #endregion

        #region CAAUAVANCES

        public ActionResult Avances()
        {
            return View();
        }

        public ActionResult getAvancesConcilicion(DateTime fecha)
        {
            List<fnReporte_CAAU_AVANCES_Result> data = new List<fnReporte_CAAU_AVANCES_Result>();

            data = caaucapa.getFuncionCAAUAvances(fecha);

            return PartialView("_DataCAAUAvances", data);
        }

        [HttpPost]
        public ActionResult ExcelConciliadasAvances(DateTime Fecha)
        {
            using (ExcelPackage excelPackage = new LibrosExcel().ReportConciliadasAutoCaauAvances(caaucapa.getFuncionCAAUAvances(Fecha)))
            {
                MemoryStream memoryStream = new MemoryStream();
                excelPackage.SaveAs((Stream)memoryStream);
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                DateTime dateTime = DateTime.Now;
                dateTime = dateTime.Date;
                string fileDownloadName = "ConciliadasCAAUAvances_" + Fecha.ToString("dd/MM/yyyy") + ".xlsx";
                memoryStream.Position = 0L;
                return File((Stream)memoryStream, contentType, fileDownloadName);
            }
        }
        #endregion

        #region CAPA
        public ActionResult CAPA()
        {
            return View();
        }

        public ActionResult getConciliadasCAPA(DateTime fecha)
        {
            List<fnReporte_CAPA_PAGOS_Result> data = new List<fnReporte_CAPA_PAGOS_Result>();

            data = caaucapa.getFuncionCAPA(fecha);

            return PartialView("_DataCAPA", data);
        }
        [HttpPost]
        public ActionResult ConciliacionAutomaticaCAPA(DateTime Fecha)
        {
            ResponseModel response = new ResponseModel();
            response = caaucapa.ConciliacionAutomaticaCAPA(Fecha);
            response.error = false;
            if (!response.error)
            {
                Auditoria auditoria = new Auditoria();
                var UserJson = Thread.CurrentPrincipal.Identity.Name;
                var Deserialize = JsonConvert.DeserializeObject<UserLoginView>(UserJson);
                auditoria.IdUsuario = Deserialize.idUsuario;
                auditoria.Descripcion = $" El usuario :{Deserialize.LoginName} genero ConciliacionAutomatica CAPA";
                auditoria.Fecha = DateTime.Now;
                ng.Auditoria(auditoria);
            }
            return Json(response);
        }

        [HttpPost]
        public ActionResult ExcelConciliadasACAPA(DateTime Fecha)
        {
            using (ExcelPackage excelPackage = new LibrosExcel().ReportConciliadasCAPA(caaucapa.getFuncionCAPA(Fecha)))
            {
                MemoryStream memoryStream = new MemoryStream();
                excelPackage.SaveAs((Stream)memoryStream);
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                DateTime dateTime = DateTime.Now;
                dateTime = dateTime.Date;
                string fileDownloadName = "ConciliadasCAPA_" + Fecha.ToString("dd/MM/yyyy") + ".xlsx";
                memoryStream.Position = 0L;
                return File((Stream)memoryStream, contentType, fileDownloadName);
            }
        }


        #endregion
    }
}
