using SistemaGestion.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaGestion.Models.DAO
{
    public class Dashboard
    {
        #region ObjetoContenedor
        //GestionDataContext dc = new GestionDataContext();
        SisGesEntities3 db = new SisGesEntities3();
        Detalles lb = new Detalles();
        #endregion

        public List<DetalleDahsboard> DetalleDiferencias(DateTime Fecha)
        {
            var formato = ("dd-MM-yyyy");
            var FechaString = Fecha.ToString(formato);
            lb.Detalle = (from e in db.Proc_Dashboard_Principal(FechaString)
                          select new DetalleDahsboard
                          {
                              Fecha = e.Fecha,
                              Centro = e.Centro,
                              Total_Tienda = e.Total_Tienda,
                              Total_SAP = e.Total_SAP,
                              Diferencia = e.Diferencia

                          }).ToList();
            
            
           return lb.Detalle.Where(x => x.Centro != "CD01").ToList();
         
        }

        public List<DetalleDahsboard> DetalleAdminDif(DateTime fecha)
        {
            var formato = ("dd-MM-yyyy");
            var FechaString = fecha.ToString(formato);

            lb.Detalle = (from e in db.Proc_Dashboard_Principal_Temp(FechaString)
                          select new DetalleDahsboard
                          {
                              Fecha = e.Fecha,
                              Centro = e.Centro,
                              Total_Tienda = e.Total_Tienda,
                              Total_SAP = e.Total_SAP,
                              Diferencia = e.Diferencia

                          }).ToList();
            return lb.Detalle;
        }
    }
}