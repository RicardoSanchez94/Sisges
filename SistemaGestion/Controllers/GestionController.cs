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
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SistemaGestion.Controllers
{
    [Authorize]
    public class GestionController : Controller
    {
        // GET: Gestion
        #region Objetos
        private SisGesEntities3 db = new SisGesEntities3();
        private LibroVentaRepository carga = new LibroVentaRepository();
        private Negocio ng = new Negocio();
        private string Login = Thread.CurrentPrincipal.Identity.Name;
        private UserLoginView user = new UserLoginView();



        public GestionController()
        {
            user = JsonConvert.DeserializeObject<UserLoginView>(Login);
        }

        #endregion

        #region DetalleVenta
        [AuthorizeRoles("Administrador", "Cuadratura")]
        public ActionResult DetalleVenta()
        {
            return View();
        }

        public JsonResult Listar(string local, string documento, string fechaCon)
        {
          

            if (local == "0")
            {
                return Json("Debe Seleccionar Al menos una tienda");

            }

            if (documento == "" && fechaCon == "")
            {
                return Json(new { data = 0 }, JsonRequestBehavior.AllowGet);
            }

            if (documento != "")
            {
                documento = documento.Replace("TODOS", "");
            }

            var ListaDetalle = (from p in db.Proc_ListaDetalleVenta(fechaCon, local, documento)
                                select p).ToList();
            return Json(new { data = ListaDetalle }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ExcelVentas(string local, string documento, DateTime Fecha)
        {
            using (ExcelPackage excelPackage = new LibrosExcel().ReporteVentas(carga.DetalleVenta(local, documento, Fecha)))
            {
                MemoryStream memoryStream = new MemoryStream();
                excelPackage.SaveAs((Stream)memoryStream);
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                DateTime dateTime = DateTime.Now;
                dateTime = dateTime.Date;
                string fileDownloadName = "DetalleVenta" + dateTime.ToString("dd/MM/yyyy") + ".xlsx";
                memoryStream.Position = 0L;
                return File((Stream)memoryStream, contentType, fileDownloadName);
            }
        }
        #endregion

        #region LibroVenta
        [HttpGet]
        [AuthorizeRoles("Administrador", "Cuadratura")]
        public ActionResult LibroVenta()
        {
            List<Proc_ListaLibroVenta_Result> lst = new List<Proc_ListaLibroVenta_Result>();
            return View(lst);
        }

        public ActionResult AdminLibro(string local, DateTime Fecha)
        {

            List<Proc_ListaLibroVenta_Result> lst = carga.LibroVentaSap(local, Fecha);
           
            return PartialView("_DetalleLibro", lst);
        }
        #endregion

        #region LocalesACtivos
        // LocalesActivosPor Envio T000
        public JsonResult Tienda()
        {
            List<Locales> lst = null;
            lst = (from d in db.Proc_LOCALES_ACTIVOS()
                   select new Locales
                   {
                       Centro = d.Centro,
                       Nombre_Centro = d.Nombre_Centro,
                       Codigo = d.Codigo,

                   }).ToList();
            return Json(new { data = lst }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region TipoDocumento

        public JsonResult ClaseDocumento()
        {
            List<Documento> lst = null;
            lst = (from d in db.Proc_CLASE_DOCUMENTO()
                   select new Documento
                   {
                       NombreCorto = d.NombreCorto,
                       Descripcion = d.Descripcion,

                   }).ToList();
            return Json(new { data = lst }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region IdocGeneradosPorlasVentas

        [HttpGet]
        [AuthorizeRoles("Administrador", "Cuadratura")]
        public ActionResult Ventas()
        {
            List<Proc_ListaIdocVenta_Result> lst = new List<Proc_ListaIdocVenta_Result>();
            return View(lst);
        }

        public ActionResult OVentas(DateTime Fecha, string local)
        {
            List<Proc_ListaIdocVenta_Result> lst = carga.IdocdeVentas(Fecha, local);
            return PartialView("_Ventas",lst);
        }
        #endregion

        #region CargaDeLibroVenta       

        [HttpPost]
        public JsonResult CargaLibro(string Archivo = "lv.csv")
        {
            var resultado = carga.LibroVenta(Archivo);
            return Json(resultado);
        }

        public ActionResult saveLV(HttpPostedFileBase oFile)
        {
            ResponseModel response = new ResponseModel();

            string ruta = Server.MapPath("~/File/");
            if (oFile == null)
            {
                response.error = true;
                response.respuesta = "Debe cargar un archivo";
                return Json(response);
            }
            string extension = Path.GetExtension(oFile.FileName);

            if (extension.Equals(".XML"))
            {
                response = carga.CargaLibroVenta(oFile, ruta);
            }
            else if(extension.Equals(".xlsx"))
            {
                response = carga.CargaLibroVentaXLS(oFile, ruta);
            }
            else
            {
                response.error = true;
                response.respuesta = "La extencion no es correcta";
            }
            if (!response.error)
            {
                Auditoria auditoria = new Auditoria();
                var UserJson = Thread.CurrentPrincipal.Identity.Name;
                var Deserialize = JsonConvert.DeserializeObject<UserLoginView>(UserJson);
                auditoria.IdUsuario = Deserialize.idUsuario;
                auditoria.Descripcion = "Se cargar libro de Venta por el usurio :" + Deserialize.LoginName;
                auditoria.Fecha = DateTime.Now;
                ng.Auditoria(auditoria);
            }

            return Json(response);
        }

        #endregion

        #region GenerarIdocReversaXTienda

        public ActionResult GenerarIdoc()
        {
            return View();
        }

        public ActionResult Reproceso(string Fecha, string Local)
        {
            DetalleDahsboard detalleDahsboard = new DetalleDahsboard();
            detalleDahsboard.Fecha = Fecha;
            detalleDahsboard.Centro = Local;
            return PartialView("_Reproceso", detalleDahsboard);
        }

        [HttpPost]
        public ActionResult ReprocesoNCFC(DateTime Fecha, string Local, string Documento)
        {
            if (Local != "")
            {
                Local = Local.Replace("T0", "");
            }

            int numLocal = int.Parse(Local);

            

            List<VENTAS> ventas = new List<VENTAS>();
            List<IdocNCFCView> NCFC = new List<IdocNCFCView>();
            ventas = db.VENTAS.Where(x => x.Fecha_Venta.Year == Fecha.Year && x.Fecha_Venta.Month == Fecha.Month && x.Fecha_Venta.Day == Fecha.Day && x.Local == numLocal && x.Tipo_Documento == Documento).ToList();
            foreach (var item in ventas)
            {
                IdocNCFCView NcFC = new IdocNCFCView();
                NcFC.Centro = item.Local;
                NcFC.FechaVenta = item.Fecha_Venta;
                NcFC.Folio = item.Folio_Documento;
                NcFC.TipoDoc = item.Tipo_Documento;
                NcFC.Monto = item.Total_Venta.Value;
                NcFC.Rut = item.Cliente;
                NcFC.isCheck = false;

                NCFC.Add(NcFC);

            }
            return PartialView("_ReprocesoNCFC", NCFC);
        }

        public JsonResult FoliosNCFC(List<IdocNCFCView> ListNCFC) {
            var resultado = new ResponseModel();
            //DateTime fecha = 
            DateTime FechaDataTime = ListNCFC.GroupBy(x => x.FechaVenta).Select(x => x.Key).FirstOrDefault(); 

            if (!db.Periodo.Where(x => x.Cerrado == false && x.AnoNumero == FechaDataTime.Year && x.MesNumero == FechaDataTime.Month).Any())
            {
                resultado.error = true;
                resultado.respuesta = "El periodo que intenta reprocesar se encuentra cerrado";
                return Json(resultado);
            }

            resultado = carga.ListaFoliosNCFC(ListNCFC);
            var ls = ListNCFC.GroupBy(x =>  new { x.Centro, x.FechaVenta }).Select(x=> new IdocNCFCView() { Centro = x.Key.Centro, FechaVenta = x.Key.FechaVenta }).FirstOrDefault();
            if (resultado.error)
            {
                Auditoria auditoria = new Auditoria();
                var UserJson = Thread.CurrentPrincipal.Identity.Name;
                var Deserialize = JsonConvert.DeserializeObject<UserLoginView>(UserJson);
                auditoria.IdUsuario = Deserialize.idUsuario;
                auditoria.Descripcion = $" Se genero Reproceso de NCFC para la tienda: {ls.Centro} con Fecha : {ls.FechaVenta} por el Usuario : {Deserialize.LoginName}" ;
                auditoria.Fecha = DateTime.Now;
                ng.Auditoria(auditoria);
            }

            return Json(resultado);
        }

        #region Proceso antiguo
        //Idoc Clientes
        //[HttpPost]
        //public async Task<JsonResult> GeneraIdoc(string Fecha, string local, string Documento,int Folio)
        //{

        //    if (local != "")
        //    {
        //        local = local.Replace("T0", "");
        //    }

        //    int numLocal = int.Parse(local);

        //    var resultado = new ResponseModel();
        //    if (numLocal == 0)
        //    {
        //        resultado.error = false;
        //        resultado.respuesta = "Debe al menos enviar Una Tienda";
        //        return Json(resultado);

        //    }

        //    if (Documento == "1")
        //    {
        //        //Paso 4
        //        //Descarga datos Idoc de Venta

        //            //Paso 5
        //            //Genera Idoc de Venta
        //            resultado =  carga.GENERAR_VENTAS_IDOC(Fecha, numLocal);
        //            if (resultado.error)
        //            {
        //                //Paso 6
        //                //Genera Archivos Idoc de Venta
        //                resultado = carga.GENERAR_IDOC_X_BCP();
        //            }

        //    }


        //    if (Documento == "2")
        //    {
        //        resultado = carga.GeneratNota(Fecha, numLocal,Folio,Documento);
        //        if (resultado.error)
        //        {
        //            //Paso 2
        //            //Genera Archivos de Facturas y Notas de Crédito
        //            resultado = carga.ImprimirNota(Fecha, numLocal);
        //            if (resultado.error)
        //            {
        //                //Paso 3
        //                // Genera Idoc de Cliente
        //                resultado = carga.IdocClientes(Fecha, numLocal);
        //                if (resultado.error)
        //                {
        //                    resultado = carga.GeneraBcpClienteIdoc();
        //                }


        //            }
        //        }
        //    }
        //    //Paso 1
        //    // Genera Idoc Facturas y Notas de Crédito
        //    if (Documento == "3")
        //    {
        //        resultado = carga.GeneratNota(Fecha, numLocal,Folio,Documento);
        //        if (resultado.error)
        //        {
        //            //Paso 2
        //            //Genera Archivos de Facturas y Notas de Crédito
        //            resultado = carga.ImprimirNota(Fecha, numLocal);
        //            if (resultado.error)
        //            {
        //                //Paso 3
        //                // Genera Idoc de Cliente
        //                resultado = carga.IdocClientes(Fecha, numLocal);
        //                if (resultado.error)
        //                {

        //                   resultado = carga.GeneraBcpClienteIdoc();
        //                    if(resultado.error){
        //                    //Paso 4
        //                    //Descarga datos Idoc de Venta

        //                            //Paso 5
        //                            //Genera Idoc de Venta
        //                            resultado =  carga.GENERAR_VENTAS_IDOC(Fecha, numLocal);
        //                            if (resultado.error)
        //                            {
        //                                //Paso 6
        //                                //Genera Archivos Idoc de Venta
        //                                resultado = carga.GENERAR_IDOC_X_BCP();
        //                            }

        //                    }
        //                }
        //            }
        //        }
        //    }



        //    return Json(resultado);
        //}
        #endregion
        [HttpPost]
        public JsonResult GeneraIdocXDoC(string Fecha, string local, string Documento)
        {
            var resultado = new ResponseModel();
            if (local != "")
            {
                local = local.Replace("T0", "");
            }

            DateTime FechaDataTime = DateTime.Parse(Fecha); 

            if (!db.Periodo.Where(x=>x.Cerrado == false && x.AnoNumero == FechaDataTime.Year && x.MesNumero == FechaDataTime.Month).Any())
            {
                resultado.error = true;
                resultado.respuesta = "El periodo que intenta reprocesar se encuentra cerrado";
                return Json(resultado);
            }


            int numLocal = int.Parse(local);

        
            if (Documento == "BOLETA")
            {
                //Paso 4
                //Descarga datos Idoc de Venta
                
                    resultado =carga.GENERAR_VENTAS_IDOC(Fecha, numLocal);
                    if (resultado.error)
                    {
                        //Paso 6
                        //Genera Archivos Idoc de Venta
                        resultado =  carga .GENERAR_IDOC_X_BCP();
                    }
                
            }
            if (resultado.error)
            {
                Auditoria auditoria = new Auditoria();
                var UserJson = Thread.CurrentPrincipal.Identity.Name;
                var Deserialize = JsonConvert.DeserializeObject<UserLoginView>(UserJson);
                auditoria.IdUsuario = Deserialize.idUsuario;
                auditoria.Descripcion = " Se genero Reproceso de Boletas para la tienda : " + numLocal + "con Fecha : " + Fecha + "por el Usuario :" + Deserialize.LoginName;
                auditoria.Fecha = DateTime.Now;
                ng.Auditoria(auditoria);
            }

            return Json(resultado);
        }

        #endregion

        #region IdocClientesXRut

        [AuthorizeRoles("Administrador", "Cuadratura")]
        public ActionResult IdocClientes()
        {
            return View();
        }

        [HttpPost]
        public ActionResult IdocClientes(string rut)
        {
            var resultado = new ResponseModel();


            if (rut == "")
            {
                resultado.error = false;
                resultado.respuesta = "Debe al menos enviar un rut";
                return Json(resultado);
            }

            if (rut != "")
            {
                rut = rut.Replace("-", "");
            }


            resultado = carga.LeeIdocClientesXRut(rut);
            if (resultado.error)
            {
                resultado = carga.GeneraIdocClientes();
            }


                return Json(resultado);
        }

        #endregion

        #region GeneraNotaCreditoXFolio

        [AuthorizeRoles("Administrador", "Cuadratura")]
        public ActionResult NotaFolio()
        {
            return View();
        }

        //[HttpPost]
        //public ActionResult NotaFolio(string Local, string Fecha, int Documento,string TipoDocumento)
        //{
        //    var resultado = new ResponseModel();
        //    if (Local == "0")
        //    {
        //        Local = Local.Replace("T0", "");
        //        resultado.error = false;
        //        resultado.respuesta = "Debe al menos enviar Una Tienda";
        //        return Json(resultado);

        //    }
        //    if (Documento == 0)
        //    {
        //        resultado.error = false;
        //        resultado.respuesta = "Debe ingresar un numero de Documento";
        //        return Json(resultado);
        //    }



        //    int numLocal = int.Parse(Local);
        //    resultado = carga.LeerNCFCreditoXFolio(numLocal, Fecha, Documento, TipoDocumento);
        //    if (resultado.error)
        //    {
        //        resultado = carga.NotaCreditoXFolio(numLocal, Fecha, Documento);
        //    }
        //    return Json(resultado);
        //}

        #endregion

        
    }
}