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
    public class CuadraturaEFOController : Controller
    {
        // GET: CuadraturaEFO
        private CuadraturaEFO efo = new CuadraturaEFO();
        private Negocio ng = new Negocio();
        public ActionResult Index()
        {
            return View();
        }

        //[HttpPost]
        //public ActionResult AperturaMes()
        //{
        //    ResponseModel response = new ResponseModel();
        //    response = ng.AperturaLibroMayor();

           
        //    return Json(response);
        //}

        public ActionResult CargaLibroMayor()
        {
            return PartialView("_CargaLibroMayor");
        }


        [HttpPost]
        public ActionResult saveLibroMayor(HttpPostedFileBase oFile)
        {
            ResponseModel response = new ResponseModel();


            if (oFile == null)
            {
                response.error = true;
                response.respuesta = "Debe cargar un archivo";
                return Json(response);
            }
            string extension = Path.GetExtension(oFile.FileName);

            if (extension.Equals(".xlsx"))
            {
                string ruta = Server.MapPath("~/File/");
                if (!System.IO.Directory.Exists(ruta + "CartaInstruccion"))
                {
                    System.IO.Directory.CreateDirectory(ruta + "CartaInstruccion");
                }
                response = efo.CargalibroMayor(oFile, ruta);
              
            
                if (!response.error)
                {
                    Auditoria auditoria = new Auditoria();
                    var UserJson = Thread.CurrentPrincipal.Identity.Name;
                    var Deserialize = JsonConvert.DeserializeObject<UserLoginView>(UserJson);
                    auditoria.IdUsuario = Deserialize.idUsuario;
                    auditoria.Descripcion = $" Se cargo el libro mayor por el Usuario : {Deserialize.LoginName}";
                    auditoria.Fecha = DateTime.Now;
                    ng.Auditoria(auditoria);
                }
            }
            else
            {
                response.error = true;
                response.respuesta = "La extencion no es correcta";
            }


            return Json(response);
        }

        public ActionResult ConciliacionAutomatica ()
        {
            ResponseModel response = new ResponseModel();
            response = efo.ConciliacionAutomatica();
            response.error = false;
            if (!response.error)
            {
                Auditoria auditoria = new Auditoria();
                var UserJson = Thread.CurrentPrincipal.Identity.Name;
                var Deserialize = JsonConvert.DeserializeObject<UserLoginView>(UserJson);
                auditoria.IdUsuario = Deserialize.idUsuario;
                auditoria.Descripcion = $" El usuario :{Deserialize.LoginName} genero ConciliacionAutomatica EFO";
                 auditoria.Fecha = DateTime.Now;
                ng.Auditoria(auditoria);
            }
            return Json(response);
        }

        [HttpPost]
        public ActionResult ExcelConciliadasA(int num)
        {
            using (ExcelPackage excelPackage = new LibrosExcel().ReportConciliadasAutoEfo(efo.Conciliadas(num)))
            {
                MemoryStream memoryStream = new MemoryStream();
                excelPackage.SaveAs((Stream)memoryStream);
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                DateTime dateTime = DateTime.Now;
                dateTime = dateTime.Date;
                string fileDownloadName = "ConciliadasEFO_" + dateTime.ToString("dd/MM/yyyy") + ".xlsx";
                memoryStream.Position = 0L;
                return File((Stream)memoryStream, contentType, fileDownloadName);
            }
        }

        [HttpPost]
        public ActionResult ExcelNoConciliadas()
        {
            using (ExcelPackage excelPackage = new LibrosExcel().ReporteNoConciliadas(efo.NoConciliadas()))
            {
                MemoryStream memoryStream = new MemoryStream();
                excelPackage.SaveAs((Stream)memoryStream);
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                DateTime dateTime = DateTime.Now;
                dateTime = dateTime.Date;
                string fileDownloadName = "NoConciliadasEFO_" + dateTime.ToString("dd/MM/yyyy") + ".xlsx";
                memoryStream.Position = 0L;
                return File((Stream)memoryStream, contentType, fileDownloadName);
            }
        }

        [HttpGet]
        public ActionResult ConciliacionManual()
        {
            ConciliacionEFOManualView manual = new ConciliacionEFOManualView();
            manual = efo.ConciliacionManual();
            return View(manual);
        }

        public ActionResult MontosZQ (Guid id)
        {
            MatchConManualEFOView oEFO = new MatchConManualEFOView();
            oEFO = efo.MontosZQ(id);

            foreach (var a in oEFO.ListaZQ)
            {
                var Texto = $"{ a.Importe_ML.ToString("C")} - {a.Fecha_Documento.ToString("dd-MM-yyyy")} - {a.Descripcion }";
                oEFO.ZQLista.Add(new SelectListItem() { Text = Texto, Value = a.Id.ToString() });
               
            }
            return PartialView("_ConciliarXMontoZQ", oEFO);
        }

        public ActionResult InsertarConciliacionM(Guid Id,Guid Id1)
        {
            ResponseModel response = new ResponseModel();
            response = efo.InsertarConciliacionM(Id, Id1);
            return Json(response);
        }

       




    }
}