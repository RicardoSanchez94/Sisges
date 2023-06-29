using OfficeOpenXml;
using SistemaGestion.Models;
using SistemaGestion.Models.DAO;
using SistemaGestion.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft;
using Newtonsoft.Json;
using SistemaGestion.Models.Security;
using System.Threading;

namespace SistemaGestion.Controllers
{
    [Authorize]
    public class MediosPagoController : Controller
    {
        // GET: MediosPago
        #region Variables
        private mpContable carga = new mpContable();
        private NotaCredito NCDAC = new NotaCredito();
        private Negocio Ng = new Negocio();
        private CuentaContables Cuenta = new CuentaContables();
        private string Login = Thread.CurrentPrincipal.Identity.Name;
        private UserLoginView user = new UserLoginView();
        //Prueba


        public MediosPagoController()
        {
            user = JsonConvert.DeserializeObject<UserLoginView>(Login);
        }

        #endregion

        #region ConciliacionAutomatica


        [HttpGet]
        [AuthorizeRoles("Administrador", "Cuadratura")]
        public ActionResult ConCiliacionAutomatica()
        {
            List<Conciliacion> lst = new List<Conciliacion>();

            return View(lst);
        }

        [HttpPost]
        public JsonResult ValidadorConciliacionAuto(DateTime Fecha)
        {
            List<Conciliacion> lst = new List<Conciliacion>();
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
                    auditoria.Descripcion = "Se Genero Reproceso de la Conciliacion Automatica Ventas por el usuario :" + Deserialize.LoginName;
                    auditoria.Fecha = DateTime.Now;
                    Ng.Auditoria(auditoria);
                }

                return Json(resultado);
            }

        }

        [HttpPost]
        public JsonResult ConciliacionAuto(DateTime Fecha)
        {
            //List<Conciliacion> lst = new List<Conciliacion>();
            //lst = carga.GetConciliacion(Fecha);
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


        public ActionResult ObtenerConciliacionAuto(DateTime Fecha, int local)
        {
            //List<Conciliacion> lst = new List<Conciliacion>();
            //lst = carga.Obtenerconciliadas(Fecha, local);
            var prueba = carga.Obtenerconciliadas(Fecha, local);


            prueba.DetalleConciliacion = prueba.DetalleConciliacion.Where(x => x.IdTienda == local).ToList();


            //prueba.DetalleConciliacion.ToList().AddRange(pruebaDetalle);
            return PartialView("_Auto", prueba);
        }

        public ActionResult ExcelConciliadasA(DateTime Fecha, int Tienda)
        {
            using (ExcelPackage excelPackage = new LibrosExcel().ReportConciliadasA(carga.Obtenerconciliadas(Fecha, Tienda)))
            {
                MemoryStream memoryStream = new MemoryStream();
                excelPackage.SaveAs((Stream)memoryStream);
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                DateTime dateTime = DateTime.Now;
                dateTime = dateTime.Date;
                string fileDownloadName = "Conciliadas Automatica" + dateTime.ToString("dd/MM/yyyy") + ".xlsx";
                memoryStream.Position = 0L;
                return File((Stream)memoryStream, contentType, fileDownloadName);


            }
        }

        #endregion

        #region ConciliacionManual
        public ActionResult ConciliacionManual()
        {
            List<fn_Conciliacion_Manual_aPI_Result> lst = new List<fn_Conciliacion_Manual_aPI_Result>();
            return View(lst);
        }

        [HttpPost]
        public ActionResult ConciliacionManual(DateTime Fecha, int local)
        {
            List<fn_Conciliacion_Manual_aPI_Result> lst = new List<fn_Conciliacion_Manual_aPI_Result>();
            lst = carga.ListaConciliacion(Fecha, local);
            return PartialView("_Conciliacion", lst);
        }
        [HttpPost]
        public ActionResult AutotbkMontotbk(string Autorizador, decimal Monto, DateTime Fecha, int Tienda)
        {
            ManualView Manual = new ManualView();
            List<fn_Conciliacion_Manual_aPI_Result> lst = new List<fn_Conciliacion_Manual_aPI_Result>();
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
        public ActionResult ExcelConciliadasM(DateTime Fecha, int Tienda)
        {

       


            using (ExcelPackage excelPackage = new LibrosExcel().ReportConciliadasM(carga.Obtenerconciliadas(Fecha, Tienda)))
            {
                MemoryStream memoryStream = new MemoryStream();
                excelPackage.SaveAs((Stream)memoryStream);
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                DateTime dateTime = DateTime.Now;
                dateTime = dateTime.Date;
                string fileDownloadName = "Conciliadas Manual" + dateTime.ToString("dd/MM/yyyy") + ".xlsx";
                memoryStream.Position = 0L;
                return File((Stream)memoryStream, contentType, fileDownloadName);
            }

        }

        [AuthorizeRoles("Administrador", "Cuadratura")]
        public ActionResult ConciliadasManual(DateTime Fecha, int local)
        {
            //List<Conciliacion_Manual> lst = new List<Conciliacion_Manual>();
            Conciliacion Oconciliacion = new Conciliacion();
            var Conciliadas = carga.Obtenerconciliadas(Fecha, local);


            Conciliadas.DetalleConciliacion = Conciliadas.DetalleConciliacion.Where(x => x.IdTienda == local).ToList();
            //Oconciliacion = carga.ObtenerConciliadasManual(Fecha, local);
            return PartialView("_ConciliadasManual", Conciliadas);
        }

        #endregion

        #region ConciliacionDiferencias

        [AuthorizeRoles("Administrador", "Cuadratura")]
        public ActionResult DifConciliacion()
        {
            List<fn_Conciliacion_Manual_aPI_Result> lst = new List<fn_Conciliacion_Manual_aPI_Result>();
            return View(lst);
        }

        [HttpPost]
        public ActionResult DifConciliacion(DateTime Fecha, int local)
        {
            List<fn_Conciliacion_Manual_aPI_Result> lst = new List<fn_Conciliacion_Manual_aPI_Result>();
            lst = carga.ListaDiferencias(Fecha, local);
            return PartialView("_Reversas", lst);
        }





        public ActionResult Diferencias(string Autorizador, decimal Monto, DateTime Fecha, int Tienda)
        {
            DiferenciaView Dif = new DiferenciaView();
            List<fn_Conciliacion_Manual_aPI_Result> lst = new List<fn_Conciliacion_Manual_aPI_Result>();
            List<Cuenta> lstCuenta = new List<Cuenta>();
            var local = Ng.getTienda(Tienda);
            Dif.Autorizador = Autorizador;
            Dif.Monto = Monto;
            Dif.Fecha = Fecha;
            if (local == null)
            {
                Dif.Centro = 0;
            }
            else
            {
                Dif.Centro = local.Local;
            }
          
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
            //List<Diferencia_Conciliacion> lst = new List<Diferencia_Conciliacion>();
            var Diferencia = carga.Obtenerconciliadas(Fecha, local);


            Diferencia.DetalleConciliacion = Diferencia.DetalleConciliacion.Where(x => x.IdTienda == local).ToList();
            //lst = carga.ObtenerLasReversas(Fecha, local);
            return PartialView("_ReversasManuales", Diferencia);
        }




        public ActionResult ExcelDiferencias(DateTime Fecha, int Tienda)
        {
            using (ExcelPackage excelPackage = new LibrosExcel().IdocMPDif(carga.Obtenerconciliadas(Fecha, Tienda)))
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

        #region NC
        [AuthorizeRoles("Administrador", "Cuadratura", "Contraloria")]
        public ActionResult nc()
        {
            return View();
        }

        // OBTIENE LA DATA COMPLETA PARA CARGAR LAS DIFERENTES TABLAS
        public ActionResult getDataNCPremisas(DateTime fecha)
        {
            var data = NCDAC.getPremisasData(fecha);

            return PartialView("_DataNCPremisas", data);
        }


        // RECIVE DATA PARA GENERAR LOS DOCUMENOS
        [HttpPost]
        public ActionResult GenerateDoc(List<notaCreditoDTO> ListPremisa1, List<notaCreditoDTO> ListPremisa2, List<notaCreditoDTO> ListPremisa3, List<notaCreditoDTO> ListPremisa4)
        {
            var NCFULL = new NotaCreditoView();
            NCFULL.ListPremisa1 = ListPremisa1;
            NCFULL.ListPremisa2 = ListPremisa2;
            NCFULL.ListPremisa3 = ListPremisa3;
            NCFULL.ListPremisa4 = ListPremisa4;

            using (ExcelPackage excelPackage = new LibrosExcel().IdocNCPremisas(NCFULL))
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



        #endregion


        [HttpGet]
        public JsonResult TiendasConciliacion(DateTime Fecha)
        {

            var Tienda = Ng.TiendaConciliacion(Fecha).ToList();

            

            return Json(new { data = Tienda.OrderBy(x=>x.Value).ToList() }, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public JsonResult TiendasConciliacionDiferencia(DateTime Fecha)
        {

            var Tienda = Ng.TiendaConciliacionDiferencia(Fecha).ToList();



            return Json(new { data = Tienda.OrderBy(x => x.Value).ToList() }, JsonRequestBehavior.AllowGet);

        }



        public ActionResult Conciliacion()
        {
            //List<fnDiferencias_Conciliacion_Result> lst = new List<fnDiferencias_Conciliacion_Result>();
            List<fn_Conciliacion_Manual_aPI_Result> lst = new List<fn_Conciliacion_Manual_aPI_Result>();
            return View(lst);
        }

        [HttpPost]
        public ActionResult Conciliacion(DateTime Fecha,int local)
        {

            List<fn_Conciliacion_Manual_aPI_Result> lst = new List<fn_Conciliacion_Manual_aPI_Result>();
            lst = carga.ListaConciliacion(Fecha, local);
            return PartialView("_Conciliacion", lst);
        }


        #region MediosPagos
        [AuthorizeRoles("Administrador", "Cuadratura")]
        public ActionResult mp()
        {

            List<MEDIOSPAGOS> lst = new List<MEDIOSPAGOS>();
            return View(lst);
        }

        //public ActionResult ObtenerMP(int local, DateTime Fecha)
        //{
        //    List<MEDIOSPAGOS> lst = Ng.MP(local, Fecha);
        //    return PartialView("_ListaMP", lst);
        //}

    


        #endregion

        #region MP
        //Trae la lista de los Medios de pago por fecha
        public ActionResult GeneraMediosPagos(DateTime Fecha)
        {
            using (ExcelPackage excelPackage = new LibrosExcel().ReporteMP(Ng.MP(Fecha)))
            {
                MemoryStream memoryStream = new MemoryStream();
                excelPackage.SaveAs((Stream)memoryStream);
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                DateTime dateTime = DateTime.Now;
                dateTime = dateTime.Date;
                string fileDownloadName = "MediosdePago" + Fecha.ToString("dd/MM/yyyy") + ".xlsx";
                memoryStream.Position = 0L;
                Ng.Auditoria(new Auditoria { IdUsuario = user.idUsuario, Fecha = DateTime.Now, Descripcion = "Se Genero IdocMPVentas por el usurio :" + user.LoginName });
                return File((Stream)memoryStream, contentType, fileDownloadName);
            }
        }

        public ActionResult IdocMP ()
        {
            return View();
        }

        [HttpPost]
        public ActionResult IdocMP(DateTime Inicio)
        {
            using (ExcelPackage excelPackage = new LibrosExcel().IdocMpVentas(carga.GenerarIdocMP(Inicio).ToList()))
            {
                MemoryStream memoryStream = new MemoryStream();
                excelPackage.SaveAs((Stream)memoryStream);
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                DateTime dateTime = DateTime.Now;
                dateTime = dateTime.Date;
                string fileDownloadName = "IdocMPVentas" + Inicio.ToString("dd/MM/yyyy") + ".xlsx";
                memoryStream.Position = 0L;
                Ng.Auditoria(new Auditoria { IdUsuario = user.idUsuario, Fecha = DateTime.Now, Descripcion = "Se Genero IdocMPVentas por el usurio :" + user.LoginName });

                return File((Stream)memoryStream, contentType, fileDownloadName);
            }
        }

        //[HttpPost]
        //public JsonResult mp(DateTime Fecha, int local)
        //{
        //    var resultado = new ResponseModel();
        //    if (local == 0)
        //    {
        //        resultado.error = false;
        //        resultado.respuesta = "Debe al menos enviar Una Tienda";
        //        return Json(resultado);

        //    }

        //    else
        //    {
        //        resultado = carga.LeerMP(local, Fecha);
        //        if (resultado.error == false)
        //        {
        //            resultado = carga.GenIdocMp(local, Fecha);
        //        }
        //    }
        //    return Json(resultado);
        //}
        #endregion

        #region FI

        //Vista Para Generar FI
        [AuthorizeRoles("Administrador", "Cuadratura")]
        public ActionResult fi()
        {

            //List<Proc_GenIdocMP_Result> lst = new List<Proc_GenIdocMP_Result>();
            return View();
        }

        [HttpPost]
        public ActionResult fi(DateTime Fecha)
        {
            using (ExcelPackage excelPackage = new LibrosExcel().IdocFI(carga.GenerarIdocFI(Fecha)))
            {
                MemoryStream memoryStream = new MemoryStream();
                excelPackage.SaveAs((Stream)memoryStream);
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                DateTime dateTime = DateTime.Now;
                dateTime = dateTime.Date;
                string fileDownloadName = "IdocFI" + Fecha.ToString("yyyyMMdd") + ".xlsx";
                memoryStream.Position = 0L;
                Ng.Auditoria(new Auditoria { IdUsuario = user.idUsuario, Fecha = DateTime.Now, Descripcion = "Se Genero IdocFIVentas por el usurio :" + user.LoginName });
                return File((Stream)memoryStream, contentType, fileDownloadName);
            }
        }



        #endregion




    }
}