using Newtonsoft.Json;
using OfficeOpenXml;
using SistemaGestion.Models;
using SistemaGestion.Models.DAO;
using SistemaGestion.Models.Repositorios;
using SistemaGestion.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SistemaGestion.Controllers
{
    [Authorize]
    public class InformesController : Controller
    {
        private Negocio NG = new Negocio();
        private NCInformeRepository NCR = new NCInformeRepository();


        // GET: Informes
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult NotaCredito()
        {
            return View();
        }
        public ActionResult globalNCDetails(DateTime inicio, DateTime fin)
        {
            AdminUsuarios ADU = new AdminUsuarios();
            var Json = JsonConvert.DeserializeObject<UserLoginView>(User.Identity.Name);
            //string LogName = User.Identity.Name;
            // SE DEFINE QUE TIPO DE USUARIO ESTA CONSULTANDO
            var userRol = ADU.GetRolesByLogiName(Json.LoginName);


            if (userRol.Where(x => x.RolNombre != "Zonal" && x.RolNombre != "Tienda").Any())
            {
                ViewBag.Back = true;
                var model = NCR.GetTotalNCZona(inicio, fin, Json.LoginName);
                return PartialView("_NCByZona", model);
            }
            else
            {
                if (userRol.Where(z => z.RolNombre == "Zonal").Any())
                {
                    ViewBag.Back = false;
                    ViewBag.isZonaReporte = false;
                    var model = NCR.GetTotalNCTiendaZona(inicio, fin, Json.LoginName, null);

                    return PartialView("_NCDetailsByZona", model);
                }
                else
                {
                    if (userRol.Where(z => z.RolNombre == "Tienda").Any())
                    {
                        return PartialView("");
                    }
                    else
                    {

                        return PartialView("");
                    }
                }
            }
        }

        public ActionResult detailsNCByZona(DateTime inicio, DateTime fin, int idZona)
        {
            ViewBag.Back = true;
            ViewBag.isZonaReporte = true;
            string users = User.Identity.Name;
            var model = NCR.GetTotalNCTiendaZona(inicio, fin, users, idZona);
            return PartialView("_NCDetailsByZona", model);
        }

        public ActionResult detailsNCByCenco(DateTime fecha, int idCencos, int idZona, string nomCenco)
        {
            var model = NCR.getDetalleByCenco(fecha, idCencos, idZona);
            ViewBag.nomCenco = nomCenco.ToUpper();
            ViewBag.fechaNC = fecha.ToString("dd-MM-yyyy");
            return PartialView("_detailsNCByCenco", model);
        }

        [HttpPost]
        public ActionResult GetReporteNCDetails(DateTime inicio, DateTime fin, int idZona)
        {
            string users = User.Identity.Name;
            var model = NCR.getDataNCINforme(inicio, fin, users, idZona);

            using (ExcelPackage excelPackage = new LibrosExcel().ReporteNCDetalle(model))
            {
                MemoryStream memoryStream = new MemoryStream();
                excelPackage.SaveAs((Stream)memoryStream);
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                DateTime dateTime = DateTime.Now;
                dateTime = dateTime.Date;
                string fileDownloadName = "NC" + dateTime.ToString("dd/MM/yyyy") + ".xlsx";
                memoryStream.Position = 0L;
                return File((Stream)memoryStream, contentType, fileDownloadName);
            }

        }

        //[HttpPost]
        //public ActionResult GetReporteNCFull(DateTime inicio, DateTime fin)
        //{
        //    string users = User.Identity.Name;
        //    var Deserialize = JsonConvert.DeserializeObject<UserLoginView>(users);

        //    var usuariorol = NG.GetUsuario(Deserialize.idUsuario);

        //    if (usuariorol.Rol.Any(x=>x.Codigo == 3 ))
        //    {
        //        using (ExcelPackage excelPackage = new LibrosExcel().ReportefullNCContraloria(NG.GetNcControlariaNew(inicio, fin)))
        //        {
        //            MemoryStream memoryStream = new MemoryStream();
        //            excelPackage.SaveAs((Stream)memoryStream);
        //            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //            DateTime dateTime = DateTime.Now;
        //            dateTime = dateTime.Date;
        //            string fileDownloadName = "NCControlaria" + inicio.ToString("dd/MM/yyyy") + fin.ToString("dd/MM/yyyy") + ".xlsx";
        //            memoryStream.Position = 0L;
        //            return File((Stream)memoryStream, contentType, fileDownloadName);
        //        }

        //    }
        //    if (usuariorol.Rol.Any(x => x.Codigo == 1 || x.Codigo == 2))
        //    {
        //        using (ExcelPackage excelPackage = new LibrosExcel().ReportefullNCCuadratura(NG.GetNcCuadratura(inicio, fin)))
        //        {
        //            MemoryStream memoryStream = new MemoryStream();
        //            excelPackage.SaveAs((Stream)memoryStream);
        //            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //            DateTime dateTime = DateTime.Now;
        //            dateTime = dateTime.Date;
        //            string fileDownloadName = "NCControlaria" + inicio.ToString("dd/MM/yyyy") + fin.ToString("dd/MM/yyyy") + ".xlsx";
        //            memoryStream.Position = 0L;
        //            return File((Stream)memoryStream, contentType, fileDownloadName);
        //        }

        //    }





        //}
        [HttpPost]
        public ActionResult GetReporteNCFull(DateTime inicio, DateTime fin)
        {
            string users = User.Identity.Name;
            var Deserialize = JsonConvert.DeserializeObject<UserLoginView>(users);
            var usuariorol = NG.GetUsuario(Deserialize.idUsuario);

            if (usuariorol.Rol.Any(x => x.Codigo == 3))
            {
                using (ExcelPackage excelPackage = new LibrosExcel().ReportefullNCContraloria(NG.GetNcControlariaNew(inicio, fin)))
                {
                    return GenerateExcelFile(excelPackage, inicio, fin, "NCControlaria", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                }
            }
            else if (usuariorol.Rol.Any(x => x.Codigo == 1 || x.Codigo == 2 || x.Codigo == 8))
            {
                using (ExcelPackage excelPackage = new LibrosExcel().ReportefullNCCuadratura(NG.GetNcCuadratura(inicio, fin)))
                {
                    return GenerateExcelFile(excelPackage, inicio, fin, "NCCuadratura", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                }
            }

            return RedirectToAction("AccessDenied"); // Acceso denegado si no cumple ninguna condición
        }

        private FileResult GenerateExcelFile(ExcelPackage excelPackage, DateTime inicio, DateTime fin, string fileNamePrefix, string contentType)
        {
            MemoryStream memoryStream = new MemoryStream();
            excelPackage.SaveAs(memoryStream);
            memoryStream.Position = 0L;
            string fileDownloadName = $"{fileNamePrefix}{inicio.ToString("dd/MM/yyyy")}{fin.ToString("dd/MM/yyyy")}.xlsx";
            return File(memoryStream, contentType, fileDownloadName);
        }

        #region antiguo
        //[HttpPost]
        //public ActionResult TablaNotaCredito(DateTime Inicio, DateTime Fin, int local)
        //{
        //    //DateTime fechaInicio = DateTime.Parse(Inicio);
        //    //DateTime fechaFin = DateTime.Parse(Fin);            
        //    var lst = NG.ListaNotaCredito(Inicio, Fin, local);
        //    return PartialView("_TablaNotaCredito", lst);
        //}

        //public async Task<ActionResult> ReporteNotaCredito(DateTime Inicio, DateTime Fin, int cboTIenda)
        //{
        //    using (ExcelPackage excelPackage = new LibrosExcel().ReporteNotaCredito(await NG.ListaNotaCredito(Inicio, Fin, cboTIenda)))
        //    {
        //        MemoryStream memoryStream = new MemoryStream();
        //        excelPackage.SaveAs((Stream)memoryStream);
        //        string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //        DateTime dateTime = DateTime.Now;
        //        dateTime = dateTime.Date;
        //        string fileDownloadName = "Informe Nota Credito " + dateTime.ToString("dd/MM/yyyy") + ".xlsx";
        //        memoryStream.Position = 0L;
        //        return File((Stream)memoryStream, contentType, fileDownloadName);
        //    }
        //}
        #endregion
        public ActionResult ReporteTarjetaFSP()
        {
            //List<fnReporte_Nota_Credito_Result> lst = new List<fnReporte_Nota_Credito_Result>();
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> ReporteTarjetaFSP(DateTime Inicio)
        {
            int ano = Inicio.Year;
            int mes = Inicio.Month;
            using (ExcelPackage excelPackage = new LibrosExcel().ReporteTarjetaFSP(await NG.ListaPagoFSP(ano, mes)))
            {
                MemoryStream memoryStream = new MemoryStream();
                excelPackage.SaveAs((Stream)memoryStream);
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                DateTime dateTime = DateTime.Now;
                dateTime = dateTime.Date;
                string fileDownloadName = "Informe Pagos FSP " + dateTime.ToString("dd/MM/yyyy") + ".xlsx";
                memoryStream.Position = 0L;
                return File((Stream)memoryStream, contentType, fileDownloadName);
            }
            //List<fnReporte_Nota_Credito_Result> lst = new List<fnReporte_Nota_Credito_Result>();

        }

        public ActionResult ReporteParticipacion()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ReporteParticipacion(DateTime Inicio, int Tienda)
        {
            int ano = Inicio.Year;
            int mes = Inicio.Month;
            using (ExcelPackage excelPackage = new LibrosExcel().ReporteParticipacion(NG.Participacion(ano, mes, Tienda)))
            {
                MemoryStream memoryStream = new MemoryStream();
                excelPackage.SaveAs((Stream)memoryStream);
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                DateTime dateTime = DateTime.Now;
                dateTime = dateTime.Date;
                string fileDownloadName = "Informe Participacion " + ano + mes + ".xlsx";
                memoryStream.Position = 0L;
                return File((Stream)memoryStream, contentType, fileDownloadName);
            }
            //List<fnReporte_Nota_Credito_Result> lst = new List<fnReporte_Nota_Credito_Result>();

        }

        public ActionResult Avance()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ReporteAvance(DateTime Inicio, int Tienda)
        {
            int ano = Inicio.Year;
            int mes = Inicio.Month;
            using (ExcelPackage excelPackage = new LibrosExcel().ReporteAvance(NG.Avance(ano, mes, Tienda)))
            {
                MemoryStream memoryStream = new MemoryStream();
                excelPackage.SaveAs((Stream)memoryStream);
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                DateTime dateTime = DateTime.Now;
                dateTime = dateTime.Date;
                string fileDownloadName = "Informe Avance " + ano + mes + ".xlsx";
                memoryStream.Position = 0L;
                return File((Stream)memoryStream, contentType, fileDownloadName);
            }
            //List<fnReporte_Nota_Credito_Result> lst = new List<fnReporte_Nota_Credito_Result>();

        }

        public ActionResult TicktPromedio()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ReporteTicktPromedio(DateTime Inicio, int Tienda)
        {
            int ano = Inicio.Year;
            int mes = Inicio.Month;
            using (ExcelPackage excelPackage = new LibrosExcel().ReporteTicketPromedio(NG.TicketPromedio(ano, mes, Tienda)))
            {
                MemoryStream memoryStream = new MemoryStream();
                excelPackage.SaveAs((Stream)memoryStream);
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                DateTime dateTime = DateTime.Now;
                dateTime = dateTime.Date;
                string fileDownloadName = "Ticket Promedio " + ano + mes + ".xlsx";
                memoryStream.Position = 0L;
                return File((Stream)memoryStream, contentType, fileDownloadName);
            }
            //List<fnReporte_Nota_Credito_Result> lst = new List<fnReporte_Nota_Credito_Result>();

        }

        public ActionResult ReporteVenta()
        {
            List<ReporteVentasView> lst = new List<ReporteVentasView>();
            return View();
        }

      

        [HttpPost]
        public ActionResult DetalleReporteVenta(DateTime Inicio, int Tienda)
        {
            List<ReporteVentasView> lst = new List<ReporteVentasView>();
            if (Tienda != 0)
            {
                int ano = Inicio.Year;
                int mes = Inicio.Month;
               
                lst = NG.DetalleReporteVentas(ano, mes, Tienda);
            }
           
            return PartialView("_Detalle",lst);
        }
        [HttpPost]
        public ActionResult ReporteVenta(DateTime Inicio, int Tienda)
        {
            int ano = Inicio.Year;
            int mes = Inicio.Month;
            using (ExcelPackage excelPackage = new LibrosExcel().ReporteVenta(NG.ReporteVentas2(ano, mes, Tienda),mes))
            {
                MemoryStream memoryStream = new MemoryStream();
                excelPackage.SaveAs((Stream)memoryStream);
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                DateTime dateTime = DateTime.Now;
                dateTime = dateTime.Date;
                string fileDownloadName = "ReporteVenta " + ano + mes + ".xlsx";
                memoryStream.Position = 0L;
                return File((Stream)memoryStream, contentType, fileDownloadName);
            }
            //List<fnReporte_Nota_Credito_Result> lst = new List<fnReporte_Nota_Credito_Result>();

        }

        public ActionResult InformeMPCuentaRut()
        {
            return View();
        }

        public ActionResult InformePagosCuentaRut()
        {
            return View();
        }

        public ActionResult ExcelMpCuentaRut(DateTime Fecha)
        {
            using (ExcelPackage excelPackage = new LibrosExcel().ReporteMPCuenta(NG.MPCuentaRut(Fecha)))
            {
                MemoryStream memoryStream = new MemoryStream();
                excelPackage.SaveAs((Stream)memoryStream);
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                DateTime dateTime = DateTime.Now;
                dateTime = dateTime.Date;
                string fileDownloadName = "MpCuentaRut "+ Fecha.ToString("yyyyMMdd")+ ".xlsx";
                memoryStream.Position = 0L;
                return File((Stream)memoryStream, contentType, fileDownloadName);
            }
        }

        public ActionResult ExcelPagosCuentaRut(DateTime Fecha)
        {
            using (ExcelPackage excelPackage = new LibrosExcel().ReportePagosCuenta(NG.PagosCuentaRut(Fecha)))
            {
                MemoryStream memoryStream = new MemoryStream();
                excelPackage.SaveAs((Stream)memoryStream);
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                DateTime dateTime = DateTime.Now;
                dateTime = dateTime.Date;
                string fileDownloadName = "PagoMPCuenta " + Fecha.ToString("yyyyMMdd") + ".xlsx";
                memoryStream.Position = 0L;
                return File((Stream)memoryStream, contentType, fileDownloadName);
            }
        }

        public ActionResult BoletasContraloria()
        {
            return View();

        }

        [HttpPost]

        public ActionResult BoletasContraloria(DateTime Fecha)
        {
            int ano = Fecha.Year;
            int mes = Fecha.Month;

            using (ExcelPackage excelPackage = new LibrosExcel().ReporteBoleasContraloria(NG.BoletasContraloria(mes, ano)))
            {
                MemoryStream memoryStream = new MemoryStream();
                Parallel.For(0, 1, x => { excelPackage.SaveAs((Stream)memoryStream); }
                  );
               
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                DateTime dateTime = DateTime.Now;
                dateTime = dateTime.Date;
                string fileDownloadName = "ReporteBoletasContraloria " + ano + mes + ".xlsx";
                memoryStream.Position = 0L;
                return File((Stream)memoryStream, contentType, fileDownloadName);
            }

            }

     
     

    }
}
