using SistemaGestion.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaGestion.Models.DAO
{
    public class Diferentes_Documentos
    {
        #region

        SisGesEntities3 db = new SisGesEntities3();
        Ventas vt = new Ventas();

        #endregion

        //public List<VentasDoc> DiferenciasDoc(string local, DateTime date, string documento)
        //{

        //    if ((local == "0" || local == "T000") && (documento == "0" || documento == "TODOS"))
        //    {
        //        vt.Venta = (from e in db.DASHBOARDVEN
        //                    where e.fecha == date
        //                    orderby e.centro
        //                    select new VentasDoc
        //                    {
        //                        centro = e.centro,
        //                        fecha = e.fecha,
        //                        tipo_doc = e.tipo_doc,
        //                        clase_doc = e.clase_doc,
        //                        totven = e.totven,
        //                        totliv = e.totliv,
        //                        diff = e.diff

        //                    }).ToList();
        //    }
        //    else if ((local == "0" || local == "T000") && (documento != "0" || documento != "TODOS"))
        //    {
        //        vt.Venta = (from e in db.DASHBOARDVEN
        //                    where e.fecha == date
        //                    && e.tipo_doc == documento
        //                    orderby e.centro
        //                    select new VentasDoc
        //                    {
        //                        centro = e.centro,
        //                        fecha = e.fecha,
        //                        tipo_doc = e.tipo_doc,
        //                        clase_doc = e.clase_doc,
        //                        totven = e.totven,
        //                        totliv = e.totliv,
        //                        diff = e.diff

        //                    }).ToList();
        //    }
        //    else if ((local != "0" || local != "T000") && (documento == "0" || documento == "TODOS"))
        //    {
        //        vt.Venta = (from e in db.DASHBOARDVEN
        //                    where e.fecha == date
        //                    && e.centro == local
        //                    orderby e.centro
        //                    select new VentasDoc
        //                    {
        //                        centro = e.centro,
        //                        fecha = e.fecha,
        //                        tipo_doc = e.tipo_doc,
        //                        clase_doc = e.clase_doc,
        //                        totven = e.totven,
        //                        totliv = e.totliv,
        //                        diff = e.diff

        //                    }).ToList();
        //    }
        //    else
        //    {
        //        vt.Venta = (from e in db.DASHBOARDVEN
        //                    where e.centro == local && e.fecha == date && e.tipo_doc == documento
        //                    orderby e.centro
        //                    select new VentasDoc
        //                    {
        //                        centro = e.centro,
        //                        fecha = e.fecha,
        //                        tipo_doc = e.tipo_doc,
        //                        clase_doc = e.clase_doc,
        //                        totven = e.totven,
        //                        totliv = e.totliv,
        //                        diff = e.diff

        //                    }).ToList();
        //    }

        //    return vt.Venta;
        //}

        public List<fnDashboard_Detalle_Temp_Result> DiferenciasDoc(DateTime date)
        {
            List<fnDashboard_Detalle_Temp_Result> lst = new List<fnDashboard_Detalle_Temp_Result>();

            lst = db.fnDashboard_Detalle_Temp(date.ToString()).ToList();
         


            return lst;
        }
    }
}