using SistemaGestion.Models;
using SistemaGestion.Models.DAO;
using SistemaGestion.Models.Security;
using SistemaGestion.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SistemaGestion.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        // GET: Admin
        #region Obj

        SisGesEntities3 db = new SisGesEntities3();
        Dashboard carga = new Dashboard();
        Diferentes_Documentos dif = new Diferentes_Documentos();
        //GestionDataContext dc = new GestionDataContext();
        #endregion

        #region Dashboard-CargaManual

        [AuthorizeRoles("Administrador","Cuadratura")]
        public ActionResult Dash()
        {
            List<DetalleDahsboard> lst = new List<DetalleDahsboard>();
            return View(lst);
        }

        [HttpPost]
        public ActionResult Listar(DateTime Fecha)
        {

            //List<DetalleDahsboard> lst = new List<DetalleDahsboard>();
            List<DetalleDahsboard> lst = carga.DetalleAdminDif(Fecha);
            return PartialView("_Dif", lst);
        }
        #endregion

        #region DiferenciasDocumentos

        [AuthorizeRoles("Administrador", "Cuadratura")]
        public ActionResult Documento()
        {
            List<fnDashboard_Detalle_Temp_Result> lst = new List<fnDashboard_Detalle_Temp_Result>();
            return View(lst);
        }

        public ActionResult Ven(string local, DateTime date)
        {
            List<fnDashboard_Detalle_Temp_Result> lst = dif.DiferenciasDoc(date);
            var lst1 = new List<fnDashboard_Detalle_Temp_Result>();
            var lst2 = new List<fnDashboard_Detalle_Temp_Result>();
            if (local.Equals("0"))
            {
            
                lst1 = lst.Where(x => x.Total_SAP != x.Total_Tienda).ToList();
            }
            else
            {
                lst1 = lst.Where(x => x.Centro.Equals(local) && (x.Total_SAP != x.Total_Tienda)).ToList();
                //Se crea una solucion desde la funcion de tabla 
                //var lt = (lst1.Where(x => x.Tipo_Documento.Equals("REVERSADOCUMENTO")).Select(x => x.Total_SAP).FirstOrDefault());
                //lt = lt.HasValue ? lt : 0;
                //foreach (var item in lst1.Where(x=>x.Tipo_Documento.Equals("BOLETA")))
                //{
                   
                //    //item.Total_SAP = item.Total_SAP + lt;
                //    item.Diferencia = Decimal.ToInt32(item.Total_Tienda.Value - item.Total_SAP.Value);
                //}
                //foreach (var item in lst1.Where(x => x.Tipo_Documento.Equals("NOTACREDITO")))
                //{
                //    //item.Total_SAP = item.Total_SAP + lt;
                //    item.Diferencia = Decimal.ToInt32(item.Total_Tienda.Value - item.Total_SAP.Value);
                //}

            }

            

            return PartialView("_DashboardVen", lst1);
        }

        #endregion

        #region LibroVenta-Carga Manual
        [AuthorizeRoles("Administrador", "Cuadratura")]
        public ActionResult LibroVenta()
        {
            List<Proc_ListaLibroVenta_Temp_Result> lst = new List<Proc_ListaLibroVenta_Temp_Result>();
            return View(lst);
        }

        public ActionResult AdminLibro(string local, DateTime Fecha, string documento = "")
        {
            var formato = ("dd-MM-yyyy");
            var FechaString = Fecha.ToString(formato);
            List<Proc_ListaLibroVenta_Temp_Result> lst = new List<Proc_ListaLibroVenta_Temp_Result>();
               
            if (local == "T000")
            {
                local = local.Replace("T000", "");
            }

            lst = (from d in db.Proc_ListaLibroVenta_Temp(FechaString, local, documento)
                   select d).ToList();
            return PartialView("_LibroVenta", lst);
        }

        public ActionResult CargarLibroVenta()
        {
            string ruta = Server.MapPath("~/File/");
            //var postulante = NG.getPostulante(rut, ruta);
            //postulante.cv = true;
            return PartialView("_CargarLibroVenta");
        }


        #endregion

     
    }
}