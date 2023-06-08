using Newtonsoft.Json;
using OfficeOpenXml;
using SistemaGestion.Models;
using SistemaGestion.Models.DAO;
using SistemaGestion.Models.Security;
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
    [Authorize]
    public class CentralizacionPagosController : Controller
    {
        // GET: CentralizacionPagos
        private CentralizacionPagos Pago = new CentralizacionPagos();
        private string Login = Thread.CurrentPrincipal.Identity.Name;
        private UserLoginView user = new UserLoginView();
        private Negocio ng = new Negocio();
     

        public CentralizacionPagosController ()
        {
            user = JsonConvert.DeserializeObject<UserLoginView>(Login);
        }

        public ActionResult Index()
        {
            List<fnDashboard_Pagos_Result> lst = new List<fnDashboard_Pagos_Result>();

            return View(lst);
        }

        [HttpPost]
        [AuthorizeRoles("Administrador", "Cuadratura")]
        public ActionResult Index(DateTime Fecha, int local)
        {
            List<fnDashboard_Pagos_Result> lst = new List<fnDashboard_Pagos_Result>();
            lst = Pago.Totales(Fecha, local);

            return PartialView("_Totales",lst);
        }

        [HttpGet]
        [AuthorizeRoles("Administrador", "Cuadratura")]
        public ActionResult Pagos()
        {
            List<PAGOS> lst = new List<PAGOS>();
            return View(lst);
        }

        [HttpPost]
        public ActionResult Pagos(DateTime Fecha, int local)
        {
            List<PAGOS> lst = new List<PAGOS>();
            lst = Pago.ListadePagos(Fecha, local);
            return PartialView("_Pagos",lst);
        }

        [HttpPost]
        public ActionResult ExcelPagos(DateTime Fecha, int Tienda)
        {
            using (ExcelPackage excelPackage = new LibrosExcel().ExecelPagos(Pago.ListadePagos(Fecha,Tienda)))
            {
                MemoryStream memoryStream = new MemoryStream();
                excelPackage.SaveAs((Stream)memoryStream);
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                DateTime dateTime = DateTime.Now;
                dateTime = dateTime.Date;
                string fileDownloadName = "Pagos" + dateTime.ToString("dd/MM/yyyy") + ".xlsx";
                memoryStream.Position = 0L;
                return File((Stream)memoryStream, contentType, fileDownloadName);
            }
        }


        #region MP
        [HttpGet]
        public ActionResult IndexMP()
        {
           

            return View();
        }

      
        [HttpPost]
        public ActionResult IdocMP(DateTime Inicio)
        {
            using (ExcelPackage excelPackage = new LibrosExcel().IdocMpPago(Pago.ListaIdocMP(Inicio)))
            {
                MemoryStream memoryStream = new MemoryStream();
                excelPackage.SaveAs((Stream)memoryStream);
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                DateTime dateTime = DateTime.Now;
                dateTime = dateTime.Date;
                string fileDownloadName = "IdocMPpagos" + Inicio.ToString("dd/MM/yyyy") + ".xlsx";
                memoryStream.Position = 0L;
                ng.Auditoria(new Auditoria { IdUsuario = user.idUsuario , Fecha = DateTime.Now, Descripcion = "Se Genero IdocMPPagos por el usurio :" + user.LoginName  });

                return File((Stream)memoryStream, contentType, fileDownloadName);
            }
        }

        [HttpGet]
        public ActionResult IndexFI()
        {


            return View();
        }

        [HttpPost]
        public ActionResult IdocFI(DateTime Inicio)
        {
            using (ExcelPackage excelPackage = new LibrosExcel().IdocFI(Pago.PagosFI(Inicio)))
            {
                MemoryStream memoryStream = new MemoryStream();
                excelPackage.SaveAs((Stream)memoryStream);
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                DateTime dateTime = DateTime.Now;
                dateTime = dateTime.Date;
                string fileDownloadName = "IdocFIPagos" + Inicio.ToString("dd-MM-yyyy") + ".xlsx";
                memoryStream.Position = 0L;
                ng.Auditoria(new Auditoria { IdUsuario = user.idUsuario, Fecha = DateTime.Now, Descripcion = "Se Genero IdocFIPagos por el usurio :" + user.LoginName });
                return File((Stream)memoryStream, contentType, fileDownloadName);
            }
        }


        #endregion

    }
}