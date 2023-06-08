using SistemaGestion.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace SistemaGestion.Models.DAO
{
    public class TesoreriaConciliacion
    {
        private SisGesEntities3 db = new SisGesEntities3();
        public ResponseModel ConciliarSencillos(DateTime Inicio, DateTime Fin)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                db.spConAutomaticaSencilloSAP(Inicio, Fin);
                
                response.respuesta = "Se cargo Correctamente";
                response.error = false;
              
                return response;
            }
            catch (Exception ex)
            {

                response.error = true;
                response.respuesta = "Procedimiento de Insercion automatica No se genero " + ex.Message;
                return response;
            }


        }

        public List<SencillosSAPView> ListaCuadraturaTesoreria(DateTime Inicio, DateTime Fin)
        {
            Sencillos lst = new Sencillos();

            lst = db.Sencillos.Where(x => x.FechaInicio == Inicio || x.FechaFin == Fin).SingleOrDefault();
            ////DetalleSencillo dt = db.DetalleSencillo.Where(x => x.IdSencillo == lst.Id).FirstOrDefault();
            List<SencillosSAPView> SAP = new List<SencillosSAPView>();




            lst.DetalleSencillo.Where(x => x.IdSencillosSAP != null).ToList().ForEach(detalle => SAP.Add(new SencillosSAPView()
            {
                idTienda = detalle.IdTienda,
                Banco = detalle.Banco,
                DiaEntrega = detalle.DiaEntrega,
                DiaLiberacion = detalle.DiaLiberacion,
                Estado = detalle.ConciliacionTesoreria,
                MontoSAP = detalle.SencillosSAP.ImporML,
                MontoTesoreria = detalle.NuevoTotal,
                FechaDoc = detalle.SencillosSAP.Fecha_Doc,
                Asignacion = detalle.SencillosSAP.Asignacion
            }));
            var prueba = SAP.Where(x => x.Estado == false).ToList();

            return SAP;
        }

        public Sencillos ValidarConciliacion(DateTime Inicio, DateTime Fin)
        {
            Sencillos lst = new Sencillos();
            lst = db.Sencillos.Where(x => x.FechaInicio == Inicio || x.FechaFin == Fin).FirstOrDefault();
            lst.DetalleSencillo.Where(x => x.IdSencillosSAP != null).ToList();
            return lst;
        }

        public List<fnSencillosTiendaNoConciliados_Result> Noconciliadas(DateTime Inicio, DateTime Fin)
        {
            try
            {
                List<fnSencillosTiendaNoConciliados_Result> lst = new List<fnSencillosTiendaNoConciliados_Result>();

                lst = db.fnSencillosTiendaNoConciliados().Where(x => x.FechaInicio >= Inicio &&  x.FechaFin <= Fin).ToList();
                return lst;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<ResponseModel> Alertas()
        {
            ResponseModel response = new ResponseModel();
            try
            {
                var url = "http://172.18.14.247/Pruebas/AlertaSencillo";
                using (HttpClient cliente = new HttpClient())
                {
                    HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    var respuesta = await cliente.SendAsync(requestMessage);
                    var contenido = await respuesta.Content.ReadAsStringAsync();
                    //total = JsonConvert.DeserializeObject<Transacciones.Root>(contenido);
                    response.respuesta = "Se enviaron las Alertas Correctamente";
                    response.error = false;

                    return response;
                }
            }
            catch (Exception)
            {

                response.error = true;
                response.respuesta = "Error al ejecutar la Api de Alertas";

                return response;
            }

        }

        public ReporteTesoreriaView ReporteTesoreria(DateTime Inicio, DateTime Fin)
        {
            ReporteTesoreriaView rp = new ReporteTesoreriaView();
            rp.NoConciliadas = db.fnSencillosTiendaNoConciliados().Where(x => x.FechaInicio == Inicio || x.FechaFin == Fin).ToList();

            Sencillos lst = new Sencillos();

            lst = db.Sencillos.Where(x => x.FechaInicio == Inicio || x.FechaFin == Fin).SingleOrDefault();

            lst.DetalleSencillo.Where(x => x.IdSencillosSAP != null).ToList().ForEach(detalle => rp.SAP.Add(new SencillosSAPView()
            {
                idTienda = detalle.IdTienda,
                Banco = detalle.Banco,
                DiaEntrega = detalle.DiaEntrega,
                DiaLiberacion = detalle.DiaLiberacion,
                Estado = detalle.ConciliacionTesoreria,
                MontoSAP = detalle.SencillosSAP.ImporML,
                MontoTesoreria = detalle.NuevoTotal,
                FechaDoc = detalle.SencillosSAP.Fecha_Doc,
                Asignacion = detalle.SencillosSAP.Asignacion
            }));



            return rp;
        }

        public ResponseModel EliminarCarta(Guid id)
        {
            ResponseModel response = new ResponseModel();
            try
            {
               


                using (var context = new SisGesEntities3())
                {
                    // 1. Obtener la instancia del objeto que deseas eliminar
                    Sencillos sn = context.Sencillos.Where(x => x.Id == id).SingleOrDefault();
                    List<SencillosSAP> sp = context.SencillosSAP
                        .Where(x => x.Fecha_Doc >= sn.FechaInicio && x.Fecha_Doc <= sn.FechaFin && x.ConciliacionTesoreria == true)
                        .ToList();


                    if (sp.Any(x => x.ConciliacionTesoreria.HasValue))
                    {
                        response.error = true;
                        response.respuesta = "Error al Eliminar ya se encuentra Conciliado";
                        return response;
                    }
                    //foreach (SencillosSAP sencillo in sp.Any())
                    //{
                    //    // Verificar si el campo ConciliacionTesoreria es 1
                    //    if (sencillo.ConciliacionTesoreria == true)
                    //    {
                    //        // Actualizar el campo ConciliacionTesoreria a 0
                    //        //sencillo.ConciliacionTesoreria = false;

                    //        // Marcar la entidad como modificada
                    //        //context.SencillosSAP.Add(sencillo);
                    //        context.Entry(sencillo).State = EntityState.Modified;
                    //        //context.SaveChanges();
                    //    }
                    //}



                    var montos = context.MontoSencillo
                     .Join(context.DetalleSencillo,
                      m => m.IdDatalleSencillo,
                     d => d.Id,
                    (m, d) => new { Monto = m, Detalle = d })
                    .Where(x => x.Detalle.IdSencillo == sn.Id)
                    .Select(x => x.Monto).ToList();



                    //var td = context.Sencillos_Tiendas
                    //    .Join(context.DetalleSencillo,
                    //    x => x.idDetalleSencillo,
                    //    d => d.Id,
                    //    (x, d) => new { Tienda = x, DetalleSencillo = d })
                    //    .Where(x=>x.DetalleSencillo.IdSencillo ==sn.Id)
                    //    .Select(x => x.Tienda).ToList();

                    //List<Sencillos_Tiendas> td = (from d in db.DetalleSencillo
                    //                              join t in sn.DetalleSencillo.ToList() on d.Id equals                                      t.Id
                    //                              where d.ConciliacionTesoreria == false
                    //                              select t).ToList();

                    var recordsToDelete = (from st in context.Sencillos_Tiendas 
                                          join ds in context.DetalleSencillo on
                                          st.idDetalleSencillo equals ds.Id where ds.IdSencillo == sn.Id select st).ToList();

                 


                    // Eliminar las entidades relacionadas
                    context.MontoSencillo.RemoveRange(montos);
                    context.Sencillos_Tiendas.RemoveRange(recordsToDelete);
                    context.DetalleSencillo.RemoveRange(sn.DetalleSencillo);

                   

                    if (sn != null)
                    {
                        // 2. Agregar la instancia a la colección de objetos que serán eliminados

                        context.Sencillos.Remove(sn);


                        // 3. Llamar al método SaveChanges() para aplicar los cambios
                        context.SaveChanges();


                    }
                }

                response.respuesta = "Se Elimino Correctamente";
                response.error = false;
                return response;

            }
            catch (Exception ex)
            {

                response.error = true;
                response.respuesta =  "Error al Eliminar" + ex.Message;
                return response;
            }
          
        }
    }
}