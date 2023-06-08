using SistemaGestion.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SistemaGestion.Models.DAO
{
    public class mpContable
    {
        #region objeto
        //private GestionDataContext dc = new GestionDataContext();
        private SisGesEntities3 db = new SisGesEntities3();
        private Negocio Ng = new Negocio();
        #endregion


        //Lee Idoc Mp
        #region Generar MP

        public List<fnIdocVentas_MP_Result> GenerarIdocMP(DateTime Fecha)
        {
            //List<IDOC_PAGOS_FI> FI = new List<IDOC_PAGOS_FI>();
            var FechaString = Fecha.ToString("yyyyMMdd");
            ResponseModel response = new ResponseModel();
            List<fnIdocVentas_MP_Result> lst = new List<fnIdocVentas_MP_Result>();
            var lt = new List<int>();
            try
            {
                //var tiempo = Stopwatch.StartNew();
                //Parallel.ForEach(db.CENTROS_LOCAL.Where(x => x.CEN_estado == "y" && x.CEN_Codigo != 26).ToList(),async item =>
                //{

                //    using (var context = db.Database.Connection)
                //    {
                //        context.OpenAsync();
                //        Debug.WriteLine(item.CEN_Codigo);
                //        lst.AddRange(await db.fnIdocVentas_MP(FechaString, item.CEN_Codigo).ToListAsync());
                //    }



                //});
                //Debug.WriteLine(tiempo.Elapsed.TotalSeconds);

                foreach (var item in db.CENTROS_LOCAL.Where(x => x.CEN_estado == "y" && x.CEN_Codigo != 26).ToList())
                {
                    //lt = item.CEN_Codigo;
                    Debug.WriteLine(item.CEN_Codigo);
                    lst.AddRange(db.fnIdocVentas_MP(FechaString, item.CEN_Codigo).ToList());
                }
                return lst;

            }
            catch (Exception ex)
            {

                response.error = false;
                response.respuesta = "Procedimiento Insert Tarea No se Genero  " + ex.InnerException;
                return lst;
            }



        }
      
        #endregion

        //Busca la data del Idoc FII
        public List<IDOC_FI> GenerarIdocFI( DateTime Fecha)
        {

            var FechaString = Fecha.ToString("ddMMyyyy");
      
            ResponseModel response = new ResponseModel();
            List<IDOC_FI> lst = new List<IDOC_FI>();
            var lt = new List<int>();
            try
            {

                //foreach (var item in db.CENTROS_LOCAL.Where(x => x.CEN_estado == "y" && x.CEN_Codigo != 26).ToList())
                //{
                //    //lt = item.CEN_Codigo;
                ////    lt.Add(item.CEN_Codigo);
                //    lst.AddRange(db.fnIdocVentas_FI(FechaString, item.CEN_Codigo).ToList());
                //}

                lst = db.IDOC_FI.Where(x => x.FECHAINT == FechaString).ToList();
                //var prueba = lst.Where(x => x.FECHAINT == "02022023" && x.NEWBS == "40" && x.WERKS == "T013").ToList();
                return lst;

            }
            catch (Exception ex)
            {

                response.error = false;
                response.respuesta = "Procedimiento Generar Fi No se Genero  " + ex.InnerException;
                return lst;
            }


         
        }


        //Trae la lista de la conciliacion que no existen en la tabla de conciliadas Manaul
        public List<fn_Conciliacion_Manual_aPI_Result> ListaConciliacion(DateTime Fecha, int Tienda)
        {
            List<fn_Conciliacion_Manual_aPI_Result> lst = new List<fn_Conciliacion_Manual_aPI_Result>();
       
            lst = db.fn_Conciliacion_Manual_aPI(Fecha, Tienda).Where(x => x.Estado == false).ToList();
         
            return lst;
        }
        //Obtener la Transacciones por Monto para compara con TBX
        public List<fn_Conciliacion_Manual_aPI_Result> GetConciliacionManualMP(DateTime Fecha, int Tienda, decimal monto)
        {
            List<fn_Conciliacion_Manual_aPI_Result> lst = new List<fn_Conciliacion_Manual_aPI_Result>();

            //var Fechastring = Fecha.ToString("yyyyMMdd");
            lst = db.fn_Conciliacion_Manual_aPI(Fecha, Tienda).ToList();

            lst = lst.Where(x => x.Tipo == false && Math.Abs(x.MontoAfecto.Value) == Math.Abs(monto)).ToList();

            return lst;
        }
        //Obtiene la diferencias que no se encuentran en la tabla de diferencias 
        public List<fn_Conciliacion_Manual_aPI_Result> ListaDiferencias(DateTime Fecha, int Tienda)
        {
            List<fn_Conciliacion_Manual_aPI_Result> lst = new List<fn_Conciliacion_Manual_aPI_Result>();
            //var Fechastring = Fecha.ToString("yyyyMMdd");

            lst = db.fn_Conciliacion_Manual_aPI(Fecha, Tienda).Where(x => x.Estado == false).ToList();
        
            return lst;
        }


        //Genera la Conciliacion Automatica
        public ResponseModel Conciliacion_Automatica(DateTime Fecha)
        {
            var response = new ResponseModel();
            try
            {
                Conciliacion Oconciliacion = new Conciliacion();

                Oconciliacion = db.Conciliacion.Where(x => x.FechaConciliar == Fecha).SingleOrDefault();

                db.spConciliacionAutomatica(Fecha,Oconciliacion == null ? Guid.Empty : Oconciliacion.id);
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

        //Trae todas la conciliadas Automaticas 
        public Conciliacion Obtenerconciliadas(DateTime Fecha, int local)
        {
            Conciliacion oConciliacion = new Conciliacion();
           
            oConciliacion = db.Conciliacion.Where(x => x.FechaConciliar == Fecha).SingleOrDefault();
         
         

            return oConciliacion;

        }


        //Verfica si la fecha Existe 
        public List<Conciliacion> GetConciliacion(DateTime Fecha)
        {
            List<Conciliacion> lst = new List<Conciliacion>();

            lst = db.Conciliacion.Where(x => x.FechaConciliar == Fecha).ToList();

            return lst;
        }

        public ResponseModel InserConciliacionManual(ManualView manual)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                using (var context = db.Database.BeginTransaction())
                {
                    var local = Ng.getTienda(manual.Centro);
                    //DateTime FechaSistema = DateTime.Now;
                    Conciliacion Oconciliacion = new Conciliacion();
                    Oconciliacion = db.Conciliacion.Where(x => x.FechaConciliar == manual.Fecha).SingleOrDefault();
                    Ventas_Apitbk api = db.Ventas_Apitbk.Where(x => x.codigoAutorizacion == manual.AutorizadorTbk && x.fechaTransaccion == manual.Fecha && x.codigoLocal == manual.Centro).SingleOrDefault();
                    MEDIOSPAGOS mp = db.MEDIOSPAGOS.Where(x => x.TRX == manual.TrxMP && x.IdTienda == local.Local && x.Fecha == manual.Fecha).SingleOrDefault();

                    DetalleConciliacion conciliacion = (new DetalleConciliacion
                    {
                        Id = Guid.NewGuid(),
                        idConciliacion = Oconciliacion.id,
                        TRX = manual.TrxMP,
                        AutorizadorTbk = manual.AutorizadorTbk,
                        AutorizadorMp = mp.Autorizador,
                        FechaMp = mp.Fecha,
                        FechaTbk = manual.Fecha,
                        CodigoLocal = manual.Centro,
                        MontoTbk = manual.MontoTbk,
                        MontoMp = mp.Monto_Aprobado,
                        IdTienda = mp.IdTienda,
                        Tipo_Pago = mp.Tipo_Pago,
                        TarjetaTBk = api.numeroTarjeta,
                        TarjetaMp = mp.Tarjeta,
                        TipoConciliacion = "M",
                        montoVentaTbk = api.montoVenta,
                        ordenPedido = api.ordenPedido
                       

                    });





                
                    mp.Estado = true;
                    api.Estado = true;

                    db.DetalleConciliacion.Add(conciliacion);
                    db.SaveChanges();
                    context.Commit();

                }
                response.respuesta = "Se Inserto Correctamente";
                response.error = false;
                return response;
            }
            catch (Exception ex)
            {

                response.error = true;
                response.respuesta = "Procedimiento de Insercion No se genero " + ex.InnerException;
                return response;
            }

        }

        public ResponseModel InsertarDiferencias(DiferenciaView Dif)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                using (var context = db.Database.BeginTransaction())
                {
                    Conciliacion Oconciliacion = new Conciliacion();
                    Oconciliacion = db.Conciliacion.Where(x => x.FechaConciliar == Dif.Fecha).SingleOrDefault();
                    //Cuenta CC = db.Cuenta.Where(x => x.Codigo == Dif.CuentaContable).SingleOrDefault();
                    Ventas_Apitbk api = db.Ventas_Apitbk.Where(x => x.codigoAutorizacion == Dif.Autorizador && x.fechaTransaccion == Dif.Fecha && x.codigoLocal == Dif.Local).SingleOrDefault();
                    MEDIOSPAGOS mp = db.MEDIOSPAGOS.Where(x => x.Autorizador == Dif.Autorizador && x.Fecha == Dif.Fecha && x.IdTienda == Dif.Local).SingleOrDefault();
                    DetalleConciliacion DifConciliacion = null;
                    DetalleConciliacion DifConciliacion2 = null;


                    if (api == null)
                    {
                        ACREEDOR_REL Acrededor = db.ACREEDOR_REL.Where(x => x.Local == Dif.Local && x.Tipo_Pago == 1).SingleOrDefault();
                      DifConciliacion = (new DetalleConciliacion
                        {

                            Id = Guid.NewGuid(),
                            FechaMp = Dif.Fecha,
                            CodigoCuenta = Dif.CuentaContable,
                            TipoAsiento = Dif.TipoAsiento,
                            TRX = mp.TRX,
                            AutorizadorMp = Dif.Autorizador,
                            TipoConciliacion = "D",
                            idConciliacion = Oconciliacion.id,
                            idAcreededor = Acrededor.Id,
                            MontoMp = mp.Monto_Aprobado,
                            TarjetaMp = mp.Tarjeta,
                            IdTienda = mp.IdTienda,
                           


                        });

                        DifConciliacion2 = (new DetalleConciliacion
                        {

                            Id = Guid.NewGuid(),
                            FechaMp = Dif.Fecha,
                            CodigoCuenta = Dif.CuentaContable2,
                            TipoAsiento = Dif.TipoAsiento2,
                            TRX = mp.TRX,
                            AutorizadorMp = Dif.Autorizador,
                            TipoConciliacion = "D",
                            idConciliacion = Oconciliacion.id,
                            idAcreededor = Acrededor.Id,
                            MontoMp = mp.Monto_Aprobado,
                            TarjetaMp = mp.Tarjeta,
                            IdTienda = mp.IdTienda


                        });
                        mp.Estado = true;
                       

                    }
                    else
                    {
                        ACREEDOR_REL Acrededor = db.ACREEDOR_REL.Where(x => x.Local == Dif.Centro && x.Tipo_Pago == 1).SingleOrDefault();
                     DifConciliacion = (new DetalleConciliacion
                        {

                            Id = Guid.NewGuid(),
                            FechaTbk = Dif.Fecha,
                            CodigoCuenta = Dif.CuentaContable,
                            TipoAsiento = Dif.TipoAsiento,
                            CodigoLocal = api.codigoLocal,
                            AutorizadorTbk = Dif.Autorizador,
                            TipoConciliacion = "D",
                            idConciliacion = Oconciliacion.id,
                            idAcreededor = Acrededor.Id,
                            MontoTbk = api.montoAfecto,
                            TarjetaTBk = api.numeroTarjeta,
                            IdTienda = api.CODIGO_COMERCIO_TBK.Local,
                            montoVentaTbk = api.montoVenta,
                            ordenPedido = api.ordenPedido



                     });

                        DifConciliacion2 = (new DetalleConciliacion
                        {

                            Id = Guid.NewGuid(),
                            FechaTbk = Dif.Fecha,
                            CodigoCuenta = Dif.CuentaContable2,
                            TipoAsiento = Dif.TipoAsiento2,
                            CodigoLocal = api.codigoLocal,
                            AutorizadorTbk = Dif.Autorizador,
                            TipoConciliacion = "D",
                            idConciliacion = Oconciliacion.id,
                            idAcreededor = Acrededor.Id,
                            MontoTbk = api.montoAfecto,
                            TarjetaTBk = api.numeroTarjeta,
                            IdTienda = api.CODIGO_COMERCIO_TBK.Local,
                            montoVentaTbk = api.montoVenta,
                            ordenPedido = api.ordenPedido


                        });
                        api.Estado = true;
                        //db.DetalleConciliacion.Add(DifConciliacion);
                        //db.DetalleConciliacion.Add(DifConciliacion2);
                        //db.SaveChanges();
                        //context.Commit();

                    }


                    //Diferencia_Conciliacion DifConciliacion = (new Diferencia_Conciliacion
                    //{
                    //    id = Guid.NewGuid(),
                    //    Fecha = Dif.Fecha,
                    //    Cod_Cuenta = Dif.CuentaContable,
                    //    TipoAsiento = Dif.TipoAsiento,
                    //    Tienda = api.codigoLocal,
                    //    Cod_TRX = Dif.Autorizador,
                    //    numeroTarjeta = api.numeroTarjeta,
                    //    FechaDiferencia = DateTime.Now,
                    //    idTiendaAcrededor = Acrededor.Id,
                    //    Monto = Dif.Monto



                    //    //TipoAsiento = 

                    //});

                    //Diferencia_Conciliacion DifConciliacion2 = (new Diferencia_Conciliacion
                    //{
                    //    id = Guid.NewGuid(),
                    //    Fecha = Dif.Fecha,
                    //    Cod_Cuenta = Dif.CuentaContable2,
                    //    TipoAsiento = Dif.TipoAsiento2,
                    //    Tienda = api.codigoLocal,
                    //    Cod_TRX = Dif.Autorizador,
                    //    numeroTarjeta = api.numeroTarjeta,
                    //    FechaDiferencia = DateTime.Now,
                    //    idTiendaAcrededor = Acrededor.Id,
                    //    Monto = Dif.Monto
                    //});

                    db.DetalleConciliacion.Add(DifConciliacion);
                    db.DetalleConciliacion.Add(DifConciliacion2);
                    db.SaveChanges();
                    context.Commit();

                }
                response.respuesta = "Se Inserto Correctamente";
                response.error = false;
                return response;
            }
            catch (Exception ex)
            {

                response.error = true;
                response.respuesta = "Procedimiento de Insercion No se genero " + ex.Message;
                return response;
            }
        }

      
     




    }
}