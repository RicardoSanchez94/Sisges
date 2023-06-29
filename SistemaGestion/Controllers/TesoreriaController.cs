using Newtonsoft.Json;
using OfficeOpenXml;
using Rotativa.MVC;
using SistemaGestion.Models;
using SistemaGestion.Models.DAO;
using SistemaGestion.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SistemaGestion.Controllers
{
    public class TesoreriaController : Controller
    {
        #region Objetos
        private SisGesEntities3 db = new SisGesEntities3();
        private LibroVentaRepository carga = new LibroVentaRepository();
        private TesoreriaConciliacion Teo = new TesoreriaConciliacion();
        private Negocio Ng = new Negocio();
        #endregion
        // GET: Tesoreria
        public ActionResult Index()
        {
           
            return View();
        }
        [HttpPost]
        public ActionResult Index(DateTime Inicio, DateTime Fin)
        {
            var model = db.Sencillos.Where(x=>x.FechaInicio >= Inicio && x.FechaFin <= Fin).ToList();
            return PartialView("_SencillosCargados",model);
        }



        public ActionResult CargaCartaInstruccion()
        {
            return PartialView("_CargaCartaInstruccion");
        }

        public ActionResult CargaSencillo()
        {
            return PartialView("_CargaSencilloSAP");
        }

        [HttpPost]
        public ActionResult DetailsSencillo(Guid Id)
        {
            //var prueba = Guid.Parse(id);
            var model = db.DetalleSencillo.Where(x => x.IdSencillo == Id).OrderBy(x=>x.IdTienda).ToList();
            return PartialView("_DetailsSencillo", model);
        }

        [HttpPost]
        public ActionResult PDFTesoreria(Guid Id)
        {
            //var prueba = Guid.Parse(id);
            var model = db.DetalleSencillo.Where(x => x.IdSencillo == Id).OrderBy(x => x.IdTienda).ToList();
            return new ViewAsPdf("PDFTesoreria", model)
            {
                FileName = $"Tesoreria.pdf"
            };
        }

        [HttpPost]
        public async Task<ActionResult> saveCartaInstruccion(CargaSencilloView SencilloT)
        {
            ResponseModel response = new ResponseModel();
            Sencillos sn = db.Sencillos.Where(x => x.FechaInicio == SencilloT.Inicio || x.FechaFin == SencilloT.Fin).FirstOrDefault();

            if (sn !=null)
            {
                response.error = true;
                response.respuesta = "El periodo Ingresado ya se realizo, Elimina el periodo si deseas a volver a cargar";
                return Json(response);
            }

            if (SencilloT.Inicio == SencilloT.Fin)
            {
                response.error = true;
                response.respuesta = "Las Fechas de Carga No pueden ser Iguales";
                return Json(response);
            }

            if (SencilloT.Fin < SencilloT.Inicio)
            {
                response.error = true;
                response.respuesta = "La Fecha de Fin no puede ser menor a la Fecha de Inicio";
                return Json(response);
            }


            if (SencilloT.oFile == null)
            {
                response.error = true;
                response.respuesta = "Debe cargar un archivo";
                return Json(response);
            }
            string extension = Path.GetExtension(SencilloT.oFile.FileName);

            if (extension.Equals(".xlsx"))
            {
                string ruta = Server.MapPath("~/File/");
                if (!System.IO.Directory.Exists(ruta + "CartaInstruccion"))
                {
                    System.IO.Directory.CreateDirectory(ruta + "CartaInstruccion");
                }
                response = carga.CargaCartaInstruccion(SencilloT, ruta);
                response.error = false;
          
                if (!response.error)
                {
                    Auditoria auditoria = new Auditoria();
                    var UserJson = Thread.CurrentPrincipal.Identity.Name;
                    var Deserialize = JsonConvert.DeserializeObject<UserLoginView>(UserJson);
                    auditoria.IdUsuario = Deserialize.idUsuario;
                    Persona username = db.Persona.Where(x => x.Id == Deserialize.idUsuario).FirstOrDefault();
                    auditoria.Descripcion = "Se Cargo la carta de Instruccion por el usuario : " + Deserialize.LoginName + " - "  + username.Nombre;
                    auditoria.Fecha = DateTime.Now;
                    //Ng.Auditoria(auditoria);

                    await Ng.GetAlertaCartaInstruccion(auditoria.Descripcion, SencilloT.Inicio,SencilloT.Fin);
                }
            }
            else
            {
                response.error = true;
                response.respuesta = "La extencion no es correcta";
            }


            return Json(response);
        }


        [HttpPost]
        public ActionResult saveSencillosSAP(HttpPostedFileBase oFile)
        {
            ResponseModel response = new ResponseModel();
            //List<SencillosSAP> sp = db.SencillosSAP.Where(x => x.Fecha_Doc >= SencilloT.Inicio && x.Fecha_Doc <= SencilloT.Fin).ToList();
            //if (sp != null)
            //{
            //    response.error = true;
            //    response.respuesta = "El periodo Ingresado ya se realizo, Elimina el periodo si deseas a volver a cargar";
            //    return Json(response);
            //}
            //if (SencilloT.Inicio == SencilloT.Fin)
            //{
            //    response.error = true;
            //    response.respuesta = "Las Fechas de Carga No pueden ser Iguales";
            //    return Json(response);
            //}

            //if (SencilloT.Fin < SencilloT.Inicio)
            //{
            //    response.error = true;
            //    response.respuesta = "La Fecha de Fin no puede ser menor a la Fecha de Inicio";
            //    return Json(response);
            //}

            //if (SencilloT.oFile == null)
            //{
            //    response.error = true;
            //    response.respuesta = "Debe cargar un archivo";
            //    return Json(response);
            //}
            string extension = Path.GetExtension(oFile.FileName);

            if (extension.Equals(".xlsx"))
            {
                string ruta = Server.MapPath("~/File/");
                if (!System.IO.Directory.Exists(ruta + "CartaInstruccion"))
                {
                    System.IO.Directory.CreateDirectory(ruta + "CartaInstruccion");
                }
                response = carga.CargaSencillo(oFile, ruta);
            }
            else
            {
                response.error = true;
                response.respuesta = "La extencion no es correcta";
            }


            return Json(response);
        }

        public ActionResult ConciliacionSencillo()
        {
            return View();
        }

       

        public ActionResult ConciliacionAutomatica(DateTime Inicio, DateTime Fin)
        {
            List<SencillosSAPView> lst = new List<SencillosSAPView>();
            lst = Teo.ListaCuadraturaTesoreria(Inicio,Fin).ToList();
            return PartialView("_ConciliadasAutomatica",lst);
        }
        [HttpPost]
        public ActionResult ConciliacionSencillosSAP(DateTime Inicio, DateTime Fin)
        {
            ResponseModel response = new ResponseModel();
            response = Teo.ConciliarSencillos(Inicio, Fin);
            return Json(response);
        }

        [HttpPost]
        public JsonResult ValidadorConciliacionAuto(DateTime Inicio, DateTime Fin)
        {

            Sencillos lst = new Sencillos();
            lst = Teo.ValidarConciliacion(Inicio, Fin);
            var resultado = new ResponseModel();
            var xsw = lst.DetalleSencillo.Any(x => x.ConciliacionTesoreria == true);
            resultado = Teo.ConciliarSencillos(Inicio, Fin);
            if (!resultado.error)
            {
                Auditoria auditoria = new Auditoria();
                var UserJson = Thread.CurrentPrincipal.Identity.Name;
                var Deserialize = JsonConvert.DeserializeObject<UserLoginView>(UserJson);
                auditoria.IdUsuario = Deserialize.idUsuario;
                auditoria.Descripcion = "Se Genero Reproceso de la Conciliacion Sencillos SAP Pagos por el usurio :" + Deserialize.LoginName;
                auditoria.Fecha = DateTime.Now;
                Ng.Auditoria(auditoria);
            }
            return Json(resultado);
            //if (lst.DetalleSencillo.Any(x => x.ConciliacionTesoreria == true))
            //{
            //    resultado.error = false;
            //    resultado.respuesta += "Estas Fecha : " + Inicio.ToString("dd-MM-yyyy") + Fin.ToString("dd-MM-yyyy") + " ya fueron conciliada, El dia :"+ lst.Fecha.Value.ToString("dd-MM-yyyy");
            //    return Json(resultado);
            //}
            //else
            //{
            //    resultado = Teo.ConciliarSencillos(Inicio,Fin);
            //    if (!resultado.error)
            //    {
            //        Auditoria auditoria = new Auditoria();
            //        var UserJson = Thread.CurrentPrincipal.Identity.Name;
            //        var Deserialize = JsonConvert.DeserializeObject<UserLoginView>(UserJson);
            //        auditoria.IdUsuario = Deserialize.idUsuario;
            //        auditoria.Descripcion = "Se Genero Reproceso de la Conciliacion Sencillos SAP Pagos por el usurio :" + Deserialize.LoginName;
            //        auditoria.Fecha = DateTime.Now;
            //        Ng.Auditoria(auditoria);
            //    }
            //    return Json(resultado);
            //}

        }

        public ActionResult NoConciliacion(DateTime Inicio, DateTime Fin)
        {
            List<fnSencillosTiendaNoConciliados_Result> lst = new List<fnSencillosTiendaNoConciliados_Result>();
            lst = Teo.Noconciliadas(Inicio,Fin).ToList();
            return PartialView("_NoConciliadas",lst);
        }

        public async Task<JsonResult> Alertas ()
        {
            ResponseModel response = new ResponseModel();
            response = await Teo.Alertas();
            return Json(response);
        }

        [HttpPost]
        public ActionResult ReporTesoreria(DateTime Inicio, DateTime Fin)
        {
            string users = User.Identity.Name;
            var Deserialize = JsonConvert.DeserializeObject<UserLoginView>(users);
            var usuariorol = Ng.GetUsuario(Deserialize.idUsuario);

            if (usuariorol.Rol.Any(x => x.Codigo == 6))
            {

                using (ExcelPackage excelPackage = new LibrosExcel().ReporteTesoreria(Teo.ReporteTesoreria(Inicio, Fin)))
                {
                    return GenerateExcelFileSencillo(excelPackage, Inicio, Fin, "TesoreriaSencillo_", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                }
            }
            else if (usuariorol.Rol.Any(x => x.Codigo == 1 || x.Codigo == 2))
            {
                var Tesoreria = Teo.ReporteTesoreria(Inicio, Fin);
                var Cuadratura = Teo.ReporteCuadratura(Inicio, Fin);
                using (ExcelPackage excelPackage = new LibrosExcel().ReporteSencilloCuadratura(Tesoreria,Cuadratura))
                {
                    return GenerateExcelFileSencillo(excelPackage, Inicio, Fin, "CuadraturaSencillo_", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                }
            }


            return RedirectToAction("AccessDenied"); // Acceso denegado si no cumple ninguna condición


         
        }
        private FileResult GenerateExcelFileSencillo(ExcelPackage excelPackage, DateTime Inicio,DateTime Fin,string fileNamePrefix, string contentType)
        {
            MemoryStream memoryStream = new MemoryStream();
            excelPackage.SaveAs(memoryStream);
            memoryStream.Position = 0L;
            string fileDownloadName = $"{fileNamePrefix}{Inicio.ToString("dd-MM-yyyy")}{Fin.ToString("dd-MM-yyyy")}.xlsx";

            return File(memoryStream, contentType, fileDownloadName);
        }
        public ActionResult EliminarCarta(Guid id)
        {
            ResponseModel response = new ResponseModel();
            response = Teo.EliminarCarta(id);
            return Json(response);
        }

    }
}
