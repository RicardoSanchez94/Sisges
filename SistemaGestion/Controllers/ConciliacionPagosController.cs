using OfficeOpenXml;
using SistemaGestion.Models;
using SistemaGestion.Models.DAO;
using SistemaGestion.Models.ViewModels;
using SistemaGestion.Models.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using Newtonsoft.Json;

namespace SistemaGestion.Controllers
{
    [Authorize]
    public class ConciliacionPagosController : Controller
    {
        // GET: ConciliacionPagos
        #region Objetos
        private ConciliacionPagos carga = new ConciliacionPagos();
        private Negocio Ng = new Negocio();
        private CuentaContables Cuenta = new CuentaContables();
        private string Login = Thread.CurrentPrincipal.Identity.Name;
        private UserLoginView user = new UserLoginView();

        public ConciliacionPagosController()
        {
            user = JsonConvert.DeserializeObject<UserLoginView>(Login);
        }
        #endregion
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles("Administrador", "Cuadratura")]
        #region ConciliacionAutomatica
        public ActionResult ConciliacionAuto()
        {
            List<ConciliacionAutoPagos> lst = new List<ConciliacionAutoPagos>();
            return View(lst);
        }

        [HttpPost]
        public ActionResult ObtenerConciliacionAuto(DateTime Fecha, int local)
        {
            var ConciliadasAuto = carga.Conciliacion(Fecha);
            ConciliadasAuto.DetalleConciliacionPagos.Where(x => x.IdTienda == local).ToList();
            return PartialView("_Auto", ConciliadasAuto);
        }

        //Verifica si la fecha ingresada existe o no
        [HttpPost]
        public JsonResult ValidadorConciliacionAuto(DateTime Fecha)
        {
            List<ConciliaciondePagos> lst = new List<ConciliaciondePagos>();
            lst = carga.GetConciliacion(Fecha);
            var resultado = new ResponseModel();
            if (lst.Any())
            {
                resultado.error = false;
                resultado.respuesta = "Esta Fecha ya fue conciliada.. Desea volver a Generar?";
                return Json(resultado);
            }
            else
            {
                resultado = carga.Conciliacion_Automatica(Fecha);
                if (!resultado.error)
                {
                    Auditoria auditoria = new Auditoria();
                    var UserJson = Thread.CurrentPrincipal.Identity.Name;
                    var Deserialize = JsonConvert.DeserializeObject<UserLoginView>(UserJson);
                    auditoria.IdUsuario = Deserialize.idUsuario;
                    auditoria.Descripcion = "Se Genero Reproceso de la Conciliacion Automatica Pagos por el usurio :" + Deserialize.LoginName;
                    auditoria.Fecha = DateTime.Now;
                    Ng.Auditoria(auditoria);
                }
                return Json(resultado);
            }

        }

        [HttpPost]
        public JsonResult ConciliacionAuto(DateTime Fecha)
        {
           
            var resultado = new ResponseModel();
            resultado = carga.Conciliacion_Automatica(Fecha);
            if (!resultado.error)
            {
                Auditoria auditoria = new Auditoria();
                var UserJson = Thread.CurrentPrincipal.Identity.Name;
                var Deserialize = JsonConvert.DeserializeObject<UserLoginView>(UserJson);
                auditoria.IdUsuario = Deserialize.idUsuario;
                auditoria.Descripcion = "Se Genero Conciliacion Automatica por el usurio :" + Deserialize.LoginName;
                auditoria.Fecha = DateTime.Now;
                Ng.Auditoria(auditoria);
            }
            return Json(resultado);

        }

        [HttpPost]
        public ActionResult ExcelConciliadasA(DateTime Fecha)
        {
            using (ExcelPackage excelPackage = new LibrosExcel().ReportConciliadasAPagos(carga.Conciliacion(Fecha)))
            {
                MemoryStream memoryStream = new MemoryStream();
                excelPackage.SaveAs((Stream)memoryStream);
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                DateTime dateTime = DateTime.Now;
                dateTime = dateTime.Date;
                string fileDownloadName = "ConciliadasAutomaticaPagos_" + dateTime.ToString("dd/MM/yyyy") + ".xlsx";
                memoryStream.Position = 0L;
                return File((Stream)memoryStream, contentType, fileDownloadName);
            }
        }

        #endregion

        #region Conciliadas Manual

        public ActionResult ConciliacionManual()
        {
            List<fn_ConciliacionPagos_Manual_aPI_Result> lst = new List<fn_ConciliacionPagos_Manual_aPI_Result>();
            return View(lst);
        }
        [HttpPost]
        public ActionResult ConciliacionManual(DateTime Fecha, int local)
        {
            List<fn_ConciliacionPagos_Manual_aPI_Result> lst = new List<fn_ConciliacionPagos_Manual_aPI_Result>();
            lst = carga.ConciliacionManaul(Fecha, local);
            return PartialView("_NoConciliacion", lst);
        }

        [HttpPost]
        public ActionResult AutotbkMontotbk(string Autorizador, decimal Monto, DateTime Fecha, int Tienda)
        {
            ManualView Manual = new ManualView();
            List<fn_ConciliacionPagos_Manual_aPI_Result> lst = new List<fn_ConciliacionPagos_Manual_aPI_Result>();
            var local = Ng.getTienda(Tienda);
            Manual.AutorizadorTbk = Autorizador;
            Manual.MontoTbk = Monto;
            Manual.Fecha = Fecha;
            Manual.Centro = Tienda;
            Manual.LstMp = new List<ManualView.Mp>();
            Manual.Local = local.Local;
            lst = carga.GetConciliacionManualMP(Fecha, local.Local, Monto);
            foreach (var item in lst)
            {
                ManualView.Mp mp = new ManualView.Mp();
                mp.AutorizadorMP = item.CodAutorizador;
                mp.Monto = item.MontoAfecto.Value;
                mp.Trx = item.Trx.Value;
                mp.Nombre = ("Autorizador: " + item.CodAutorizador + " Monto: " + item.MontoAfecto);

                Manual.LstMp.Add(mp);
            }

            return PartialView("_ConciliarXMontoMP", Manual);

        }

        public JsonResult AgregarConciliacionManual(ManualView manual)
        {
            ResponseModel response = new ResponseModel();
            ManualView Manual = new ManualView();
            response = carga.InserConciliacionManual(manual);
            if (!response.error)
            {
                Auditoria auditoria = new Auditoria();
                var UserJson = Thread.CurrentPrincipal.Identity.Name;
                var Deserialize = JsonConvert.DeserializeObject<UserLoginView>(UserJson);
                auditoria.IdUsuario = Deserialize.idUsuario;
                auditoria.Descripcion = " Se inserto la conciliacion Manual por el usuario :" + Deserialize.LoginName;
                auditoria.Fecha = DateTime.Now;
                Ng.Auditoria(auditoria);
            }
            return Json(response);
        }
        
        public ActionResult ConciliadasManual(DateTime Fecha, int local)
        {
          
           var lst = carga.Conciliacion(Fecha);
            lst.DetalleConciliacionPagos.Where(x => x.IdTienda == local).ToList();
            return PartialView("_ConciliadasManual", lst);
        }

        [HttpPost]
        public ActionResult ExcelConciliadasM(DateTime Fecha)
        {
            using (ExcelPackage excelPackage = new LibrosExcel().ReportConciliadasMPagos(carga.Conciliacion(Fecha)))
            {
                MemoryStream memoryStream = new MemoryStream();
                excelPackage.SaveAs((Stream)memoryStream);
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                DateTime dateTime = DateTime.Now;
                dateTime = dateTime.Date;
                string fileDownloadName = "ConciliadasAutomaticaPagos" + Fecha.ToString("dd/MM/yyyy") + ".xlsx";
                memoryStream.Position = 0L;
                return File((Stream)memoryStream, contentType, fileDownloadName);
            }
        }

        #endregion

        #region ConciliacionDiferencias
        public ActionResult DifConciliacion()
        {
            List<fn_ConciliacionPagos_Manual_aPI_Result> lst = new List<fn_ConciliacionPagos_Manual_aPI_Result>();
            return View(lst);
        }
        [HttpPost]
        public ActionResult DifConciliacion(DateTime Fecha, int local)
        {
            List<fn_ConciliacionPagos_Manual_aPI_Result> lst = new List<fn_ConciliacionPagos_Manual_aPI_Result>();
            lst = carga.ListaDiferencias(Fecha, local);
            return PartialView("_Reversas", lst);
        }

        public ActionResult Diferencias(string Autorizador, decimal Monto, DateTime Fecha, int Tienda)
        {
            DiferenciaView Dif = new DiferenciaView();
            List<fn_ConciliacionPagos_Manual_aPI_Result> lst = new List<fn_ConciliacionPagos_Manual_aPI_Result>();
            List<Cuenta> lstCuenta = new List<Cuenta>();
            var local = Ng.getTienda(Tienda);
            Dif.Autorizador = Autorizador;
            Dif.Monto = Monto;
            Dif.Fecha = Fecha;
            Dif.Centro = local.Local;
            Dif.Local = Tienda;
            Dif.ListaCuentas1 = new List<DiferenciaView.CuentaContables>();
            Dif.ListaCuentas2 = new List<DiferenciaView.CuentaContables>();
            lstCuenta = Cuenta.ObtenerCuentasContables();
            foreach (var item in lstCuenta)
            {
                DiferenciaView.CuentaContables CC = new DiferenciaView.CuentaContables();
                CC.Codigo = item.Codigo;
                CC.NumeroCuenta = item.NumeroCuenta;
                CC.Descripcion = item.Descripcion;
                CC.Nombre = (item.NumeroCuenta + " - " + item.Descripcion);
                Dif.ListaCuentas1.Add(CC);
                Dif.ListaCuentas2.Add(CC);

            }
            return PartialView("_Diferencias", Dif);
        }

        public JsonResult AgregarDiferencias(DiferenciaView Dif)
        {
            ResponseModel response = new ResponseModel();
            //ManualView Manual = new ManualView();
            response = carga.InsertarDiferencias(Dif);
            if (!response.error)
            {
                Auditoria auditoria = new Auditoria();
                var UserJson = Thread.CurrentPrincipal.Identity.Name;
                var Deserialize = JsonConvert.DeserializeObject<UserLoginView>(UserJson);
                auditoria.IdUsuario = Deserialize.idUsuario;
                auditoria.Descripcion = " Se inserto la Conciliacion Diferencia por el usuario :" + Deserialize.LoginName;
                auditoria.Fecha = DateTime.Now;
                Ng.Auditoria(auditoria);
            }
            return Json(response);
        }

        [HttpPost]
        public ActionResult ReversasManuales(DateTime Fecha, int local)
        {
            //List<Diferencias_Pagos> lst = new List<Diferencias_Pagos>();
            var lst = carga.Conciliacion(Fecha);
            lst.DetalleConciliacionPagos.Where(x => x.IdTienda == local).ToList();
            return PartialView("_ReversasManuales", lst);
        }

        public ActionResult ExcelDiferencias(DateTime Fecha, int Tienda)
        {
            using (ExcelPackage excelPackage = new LibrosExcel().IdocMPDifPagos(carga.Conciliacion(Fecha)))
            {
                MemoryStream memoryStream = new MemoryStream();
                excelPackage.SaveAs((Stream)memoryStream);
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                DateTime dateTime = DateTime.Now;
                dateTime = dateTime.Date;
                string fileDownloadName = "MP" + dateTime.ToString("dd/MM/yyyy") + ".xlsx";
                memoryStream.Position = 0L;
                return File((Stream)memoryStream, contentType, fileDownloadName);
            }

        }

        #endregion

        [HttpGet]
        public JsonResult TiendasConciliacion(DateTime Fecha)
        {

            var Tienda = carga.TiendaConciliacion(Fecha).ToList();



            return Json(new { data = Tienda.OrderBy(x => x.Value).ToList() }, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public JsonResult TiendasConciliacionDiferencia(DateTime Fecha)
        {

            var Tienda = carga.TiendaConciliacionDiferencia(Fecha).ToList();



            return Json(new { data = Tienda.OrderBy(x => x.Value).ToList() }, JsonRequestBehavior.AllowGet);

        }


    }
}