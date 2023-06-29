using Newtonsoft.Json;
using OfficeOpenXml;
using SistemaGestion.Models;
using SistemaGestion.Models.DAO;
using SistemaGestion.Models.Repositorios;
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
    public class TiendaController : Controller
    {
        private InsertSencilloTienda tienda = new InsertSencilloTienda();
        private Negocio NG = new Negocio();

        // GET: Tienda
        #region Sencillo
        public ActionResult Sencillo()
        {
            AdminUsuarios ADU = new AdminUsuarios();
            List<Sencillos_Tiendas> lst = new List<Sencillos_Tiendas>();
            var UserJson = Thread.CurrentPrincipal.Identity.Name;
            var Deserialize = JsonConvert.DeserializeObject<UserLoginView>(UserJson);
            var userRol = ADU.GetRolesByLogiName(Deserialize.LoginName);

            lst = tienda.GetSencilloTienda(Deserialize);

            return View(lst);
        }

        //[HttpPost]
        //[AuthorizeRoles("Administrador", "Cuadratura", "Tienda")]
        //public ActionResult ExcelSencillo(DateTime Fecha)
        //{
        //    AdminUsuarios ADU = new AdminUsuarios();
        //    AcumuladoFaltanteView.Data lst = new AcumuladoFaltanteView.Data();
        //    string users = User.Identity.Name;
        //    var Deserialize = JsonConvert.DeserializeObject<UserLoginView>(users);
        //    var usuariorol = NG.GetUsuario(Deserialize.idUsuario);

        //    if (usuariorol.Rol.Any(x => x.Codigo == 2 || x.Codigo == 1))
        //    {
        //        using (ExcelPackage excelPackage = new LibrosExcel().ReporteSobranteFaltante(Fecha))
        //        {
        //            return GenerateExcelFileSencillo(excelPackage, Fecha, "AcumuladoFaltante", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        //        }
        //    }
        //    else if (usuariorol.Rol.Any(x => x.Codigo == 7))
        //    {
        //        using (ExcelPackage excelPackage = new LibrosExcel().ReporteSobranteFaltanteRRHH(Fecha))
        //        {
        //            return GenerateExcelFileSencillo(excelPackage, Fecha, "SobranteFaltanteRRHH", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        //        }
        //    }


        //    return RedirectToAction("AccessDenied"); // Acceso denegado si no cumple ninguna condición


        //}

        //private FileResult GenerateExcelFileSencillo(ExcelPackage excelPackage, DateTime FechaMesYear, string fileNamePrefix, string contentType)
        //{
        //    MemoryStream memoryStream = new MemoryStream();
        //    excelPackage.SaveAs(memoryStream);
        //    memoryStream.Position = 0L;
        //    string fileDownloadName = $"{fileNamePrefix}{FechaMesYear.Year}{FechaMesYear.ToString("MMMM")}.xlsx";

        //    return File(memoryStream, contentType, fileDownloadName);
        //}

        public ActionResult DetalleSencillo(Guid id)
        {
            Sencillos_Tiendas lst = tienda.GetInsertRemito(id);
            return PartialView("_InsertarRemito", lst);
        }

        public ActionResult Devolucion(Guid id)
        {
            Sencillos_Tiendas lst = tienda.GetInsertRemito(id);
            return PartialView("_InsertarDevolucion", lst);
        }
        // POST:
        [HttpPost]
        public ActionResult Remito(string CodigoRemito, int Tienda, Guid idSencillosTienda)
        {
            ResponseModel response = new ResponseModel();

            response = tienda.InsertaRecepcion(CodigoRemito, Tienda, idSencillosTienda);

            return Json(response);
        }

        [HttpPost]
        public ActionResult RemitoDevolucion(string CodigoRemito, int Tienda, Guid idSencillosTienda, string Deposito)
        {
            ResponseModel response = new ResponseModel();

            response = tienda.InsertaDevolucion(CodigoRemito, Tienda, idSencillosTienda, Deposito);

            return Json(response);
        }

        public ActionResult SencilloCuadratura()
        {
            AdminUsuarios ADU = new AdminUsuarios();
            List<Sencillos_Tiendas> lst = new List<Sencillos_Tiendas>();
            var UserJson = Thread.CurrentPrincipal.Identity.Name;
            var Deserialize = JsonConvert.DeserializeObject<UserLoginView>(UserJson);
            var userRol = ADU.GetRolesByLogiName(Deserialize.LoginName);

            lst = tienda.GetSencilloTienda(Deserialize);

            return View(lst);

        }

        #endregion

        #region Sobrante 
        [HttpGet]
        [AuthorizeRoles("Administrador", "Cuadratura","Tienda", "RRHH")]
        public ActionResult SobranteFaltante()
        {
            //AdminUsuarios ADU = new AdminUsuarios();
            //List<Sobrante_Faltante> lst = new List<Sobrante_Faltante>();
            //var UserJson = Thread.CurrentPrincipal.Identity.Name;
            //var Deserialize = JsonConvert.DeserializeObject<UserLoginView>(UserJson);
            //var userRol = ADU.GetRolesByLogiName(Deserialize.LoginName);

            //lst = tienda.GetSobranteFaltante(Deserialize, Fecha);
            return View();
        }
        [HttpPost]
        [AuthorizeRoles("Administrador", "Cuadratura", "Tienda", "RRHH")]
        public ActionResult SobranteFaltante(DateTime Fecha)
        {
            AdminUsuarios ADU = new AdminUsuarios();
            AcumuladoFaltanteView.Data lst = new AcumuladoFaltanteView.Data();
            var UserJson = Thread.CurrentPrincipal.Identity.Name;
            var Deserialize = JsonConvert.DeserializeObject<UserLoginView>(UserJson);
            var userRol = ADU.GetRolesByLogiName(Deserialize.LoginName);

            lst = tienda.GetAcumuladoSobranteFaltante(Deserialize, Fecha);
            return PartialView("_SobranteFaltante", lst);
        }
        [AuthorizeRoles("Administrador", "Cuadratura", "Tienda", "RRHH")]
        public ActionResult ListSobranteFaltante(DateTime Fecha, int IdTienda, string IdEmpleado)
        {

            var lst = tienda.GetListSobranteFaltantePorEmpleado(Fecha.Year,Fecha.Month ,IdTienda, IdEmpleado);

            return PartialView("_ListSobranteFaltante", lst);
        }
        [HttpGet]
        [AuthorizeRoles("Administrador", "Cuadratura", "Tienda", "RRHH")]
        public ActionResult AceptarRechzar(Sobrante_Faltante item)
        {


            //AdminUsuarios ADU = new AdminUsuarios();
            //List<Sobrante_Faltante> lst = new List<Sobrante_Faltante>();
            //var UserJson = Thread.CurrentPrincipal.Identity.Name;
            //var Deserialize = JsonConvert.DeserializeObject<UserLoginView>(UserJson);
            //var userRol = ADU.GetRolesByLogiName(Deserialize.LoginName);

            AceptacionSobranteFaltanteView oAceptacionSobranteFaltante = tienda.GetAceptacionSobranteFaltante(item);
            return PartialView("_AceptarRechzar", oAceptacionSobranteFaltante);
        }

        //No funciona
        [HttpPost]
        public ActionResult AceptarRechzar(AceptacionSobranteFaltanteView item)
        {

            var response = tienda.InsertarAceptacionSobranteFaltante(item);

            return Json(response);
        }
       
        [HttpPost]
        public ActionResult AceptarRechazarAcumulado(List<AcumuladoSobranteCheckView> lstSobranteFaltantecheck)
        {


            //AdminUsuarios ADU = new AdminUsuarios();
            //List<Sobrante_Faltante> lst = new List<Sobrante_Faltante>();
            //var UserJson = Thread.CurrentPrincipal.Identity.Name;
            //var Deserialize = JsonConvert.DeserializeObject<UserLoginView>(UserJson);
            //var userRol = ADU.GetRolesByLogiName(Deserialize.LoginName);

            var oAceptacionSobranteFaltante = tienda.GetAceptacionAcumuladoFaltante(lstSobranteFaltantecheck);
            return PartialView("_AceptarRechzarAcumulado", oAceptacionSobranteFaltante);
        }
        [HttpPost]
        public ActionResult AceptarRechzarAcumulado(AcumuladoSobranteFaltanteView item)
        {

           
            var response = tienda.InsertarAcumuladoSobranteFaltante(item);

            return Json(response);
        }

        [HttpPost]
        [AuthorizeRoles("Administrador", "Cuadratura", "Tienda", "RRHH")]
        public ActionResult ExcelSobranteFaltante(DateTime Fecha)
        {
            AdminUsuarios ADU = new AdminUsuarios();
            AcumuladoFaltanteView.Data lst = new AcumuladoFaltanteView.Data();
            string users = User.Identity.Name;
            var Deserialize = JsonConvert.DeserializeObject<UserLoginView>(users);
            var usuariorol = NG.GetUsuario(Deserialize.idUsuario);

            if (usuariorol.Rol.Any(x =>x.Codigo == 2 || x.Codigo == 1))
            {
                using (ExcelPackage excelPackage = new LibrosExcel().ReporteSobranteFaltante(Fecha))
                {
                    return GenerateExcelFile(excelPackage, Fecha, "AcumuladoFaltante", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                }
            }
            else if (usuariorol.Rol.Any(x => x.Codigo == 5 ))
            {
                using (ExcelPackage excelPackage = new LibrosExcel().ReporteSobranteFaltanteRRHH(Fecha))
                {
                    return GenerateExcelFile(excelPackage, Fecha, "SobranteFaltanteRRHH", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                }
            }
       

            return RedirectToAction("AccessDenied"); // Acceso denegado si no cumple ninguna condición

         
        }

        private FileResult GenerateExcelFile(ExcelPackage excelPackage, DateTime FechaMesYear, string fileNamePrefix, string contentType)
        {
            MemoryStream memoryStream = new MemoryStream();
            excelPackage.SaveAs(memoryStream);
            memoryStream.Position = 0L;
            string fileDownloadName = $"{fileNamePrefix}{FechaMesYear.Year}{FechaMesYear.ToString("MMMM")}.xlsx";

            return File(memoryStream, contentType, fileDownloadName);
        }

        public  ActionResult DetalleAceptadosRechazados (Guid ID)
        {

            var lst = tienda.GetSobranteFaltante(ID);
            return PartialView("_DetalleAceptados",lst);
        }

        #endregion
    }
}