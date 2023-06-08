using SistemaGestion.Models.Repositorios;
using SistemaGestion.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaGestion.Models.DAO
{
    public class ConciliacionPagos
    {
        #region objeto
        //private GestionDataContext dc = new GestionDataContext();
        private SisGesEntities3 db = new SisGesEntities3();
        private Negocio Ng = new Negocio();
        #endregion

        public List<ElementoJsonKeyTBK> TiendaConciliacion(DateTime Fecha)
        {
            List<ElementoJsonKeyTBK> lst = new List<ElementoJsonKeyTBK>();

            List<Pagos_Apitbk> api = db.Pagos_Apitbk.Where(x => x.fechaTransaccion == Fecha && x.Estado == false).ToList();
            List<PAGOS> mp = db.PAGOS.Where(x => x.Fecha == Fecha && x.EstadoConciliado == false && (x.Tipo_Pago == 5 || x.Tipo_Pago == 9) && !x.Tipo_Trx.Equals("NOTACREDITO")).ToList();
            List<CODIGO_COMERCIO_TBK> cc = db.CODIGO_COMERCIO_TBK.ToList();
            List<DetalleConciliacionPagos> lstdetalleConciliacions = db.DetalleConciliacionPagos.Where(x => x.TipoConciliacion.Equals("M") && x.ConciliaciondePagos.FechaConciliar == Fecha).ToList();

            var lstTiendasConciliadas = cc.Where(x => lstdetalleConciliacions.Select(y => y.IdTienda).Contains(x.Local)).Select(x => new ElementoJsonKeyTBK { Value = x.Local, Text = x.NombreLocal }).ToList();

          

            lst = (api.GroupBy(x => new { x.CODIGO_COMERCIO_TBK.Local, x.CODIGO_COMERCIO_TBK.NombreLocal }).Select(x => new ElementoJsonKeyTBK { Value = x.Key.Local, Text = x.Key.NombreLocal }).ToList()).Union(mp.GroupBy(x => new { x.IdTienda, x.CENTROS_LOCAL.CEN_Nombre }).Select(x => new ElementoJsonKeyTBK { Value = x.Key.IdTienda, Text = cc.Where(y => y.Local == x.Key.IdTienda).Select(h => h.NombreLocal).FirstOrDefault() }).ToList()).ToList();
            lst.AddRange(lstTiendasConciliadas);
            return lst.GroupBy(x => new { x.Value, x.Text }).Select(x => new ElementoJsonKeyTBK { Value = x.Key.Value, Text = x.Key.Text }).ToList();
        }

        public List<ElementoJsonKeyTBK> TiendaConciliacionDiferencia(DateTime Fecha)
        {
            List<ElementoJsonKeyTBK> lst = new List<ElementoJsonKeyTBK>();

            List<Pagos_Apitbk> api = db.Pagos_Apitbk.Where(x => x.fechaTransaccion == Fecha && x.Estado == false).ToList();
            List<PAGOS> mp = db.PAGOS.Where(x => x.Fecha == Fecha && x.EstadoConciliado == false && (x.Tipo_Pago == 5 || x.Tipo_Pago == 9) && !x.Tipo_Trx.Equals("NOTACREDITO")).ToList();
            List<CODIGO_COMERCIO_TBK> cc = db.CODIGO_COMERCIO_TBK.ToList();
            List<DetalleConciliacionPagos> lstdetalleConciliacions = db.DetalleConciliacionPagos.Where(x => x.TipoConciliacion.Equals("D") && x.ConciliaciondePagos.FechaConciliar == Fecha).ToList();
          
            var lstTiendasConciliadas = cc.Where(x => lstdetalleConciliacions.Select(y => y.IdTienda).Contains(x.Local)).Select(x => new ElementoJsonKeyTBK { Value = x.Local, Text = x.NombreLocal }).ToList();


            lst = (api.GroupBy(x => new { x.CODIGO_COMERCIO_TBK.Local, x.CODIGO_COMERCIO_TBK.NombreLocal }).Select(x => new ElementoJsonKeyTBK { Value = x.Key.Local, Text = x.Key.NombreLocal }).ToList()).Union(mp.GroupBy(x => new { x.IdTienda, x.CENTROS_LOCAL.CEN_Nombre }).Select(x => new ElementoJsonKeyTBK { Value = x.Key.IdTienda, Text = cc.Where(y => y.Local == x.Key.IdTienda).Select(h => h.NombreLocal).FirstOrDefault() }).ToList()).ToList();
            lst.AddRange(lstTiendasConciliadas);
            return lst.GroupBy(x => new { x.Value, x.Text }).Select(x => new ElementoJsonKeyTBK { Value = x.Key.Value, Text = x.Key.Text }).ToList();
        }

        //Genera La Conciliacion Automatica
        #region ConciliacionAutomatica
        public ResponseModel Conciliacion_Automatica(DateTime Fecha)
        {
            var response = new ResponseModel();
            try
            {
                ConciliaciondePagos Oconciliacion = new ConciliaciondePagos();

                Oconciliacion = db.ConciliaciondePagos.Where(x => x.FechaConciliar == Fecha).SingleOrDefault();

               
                db.spInsertConciliacionAutoPagos(Fecha, Oconciliacion == null ? Guid.Empty : Oconciliacion.id);
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

        //Verifica Si la fecha ingresada Existe
        public List<ConciliaciondePagos> GetConciliacion(DateTime Fecha)
        {
            List<ConciliaciondePagos> lst = new List<ConciliaciondePagos>();

            lst = db.ConciliaciondePagos.Where(x => x.FechaConciliar == Fecha).ToList();

            return lst;
        }
        #endregion


        public ConciliaciondePagos Conciliacion(DateTime Fecha)
        {
            ConciliaciondePagos oConciliacionPagos = new ConciliaciondePagos();

            oConciliacionPagos = db.ConciliaciondePagos.Where(x => x.FechaConciliar == Fecha).SingleOrDefault();
            
            return oConciliacionPagos;

        }

        #region ConciliacionManual

        // Obtiene las no conciliadas
        public List<fn_ConciliacionPagos_Manual_aPI_Result> ConciliacionManaul(DateTime Fecha, int Tienda)
        {
            List<fn_ConciliacionPagos_Manual_aPI_Result> lst = new List<fn_ConciliacionPagos_Manual_aPI_Result>();

            lst = db.fn_ConciliacionPagos_Manual_aPI(Fecha, Tienda).ToList();

            //var lstCon = db.ConciliacionManualPagos.Where(x => x.Fechatbk == Fecha).Select(x => x.Autorizadortbk).ToList();
            //var lstCon1 = db.Conciliacion_Manual.Where(x => x.FechaTbk == Fecha).Select(x => x.TrxMP).ToList();
            return lst;

        }
        //Obtener la Transacciones por Monto para comparar con TBX
        public List<fn_ConciliacionPagos_Manual_aPI_Result> GetConciliacionManualMP(DateTime Fecha, int Tienda, decimal monto)
        {
            List<fn_ConciliacionPagos_Manual_aPI_Result> lst = new List<fn_ConciliacionPagos_Manual_aPI_Result>();

            //var Fechastring = Fecha.ToString("yyyyMMdd");
            lst = db.fn_ConciliacionPagos_Manual_aPI(Fecha, Tienda).ToList();

            lst = lst.Where(x => x.Tipo == false && Math.Abs(x.MontoAfecto.Value) == Math.Abs(monto)).ToList();

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
                    Pagos_Apitbk api = db.Pagos_Apitbk.Where(x => x.codigoAutorizacion == manual.AutorizadorTbk && x.fechaTransaccion == manual.Fecha && x.codigoLocal == manual.Centro).SingleOrDefault();
                    PAGOS mp = db.PAGOS.Where(x => x.TRX == manual.TrxMP && x.IdTienda == local.Local && x.Fecha == manual.Fecha).SingleOrDefault();
                    DetalleConciliacionPagos conciliacion = (new DetalleConciliacionPagos
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
                        MontoMp = mp.Monto,
                        IdTienda = mp.IdTienda,
                        Tipo_Pago = mp.Tipo_Pago,
                        TarjetaTBk = api.numeroTarjeta,
                        TarjetaMp = mp.Tarjeta,
                        TipoConciliacion = "M",
                        MontoVentatbk = api.montoVenta
                    });

                    mp.EstadoConciliado = true;
                    api.Estado = true;


                    db.DetalleConciliacionPagos.Add(conciliacion);
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
        #endregion




        //public List<ConciliacionManualPagos> ListaConciliadasManual(DateTime Fecha, int local)
        //{
        //    List<ConciliacionManualPagos> lst = new List<ConciliacionManualPagos>();
        //    lst = db.ConciliacionManualPagos.Where(x => x.Fechatbk == Fecha && x.idtienda == local).ToList();

        //    return lst;
        //}

        public List<fn_ConciliacionPagos_Manual_aPI_Result> ListaDiferencias(DateTime Fecha, int Tienda)
        {
            List<fn_ConciliacionPagos_Manual_aPI_Result> lst = new List<fn_ConciliacionPagos_Manual_aPI_Result>();
            //var Fechastring = Fecha.ToString("yyyyMMdd");

            lst = db.fn_ConciliacionPagos_Manual_aPI(Fecha, Tienda).ToList();
            

            return lst;
        }

       

        public ResponseModel InsertarDiferencias(DiferenciaView Dif)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                using (var context = db.Database.BeginTransaction())
                {
                    ConciliaciondePagos Oconciliacion = new ConciliaciondePagos();
                    Oconciliacion = db.ConciliaciondePagos.Where(x => x.FechaConciliar == Dif.Fecha).SingleOrDefault();
                    //Cuenta CC = db.Cuenta.Where(x => x.Codigo == Dif.CuentaContable).SingleOrDefault();
                    Pagos_Apitbk api = db.Pagos_Apitbk.Where(x => x.codigoAutorizacion == Dif.Autorizador && x.fechaTransaccion == Dif.Fecha && x.codigoLocal == Dif.Local).SingleOrDefault();
                    PAGOS mp = db.PAGOS.Where(x => x.Autorizador == Dif.Autorizador && x.Fecha == Dif.Fecha && x.IdTienda == Dif.Local).SingleOrDefault();
                    DetalleConciliacionPagos DifConciliacion = null;
                    DetalleConciliacionPagos DifConciliacion2 = null;


                    if (api == null)
                    {
                        ACREEDOR_REL Acrededor = db.ACREEDOR_REL.Where(x => x.Local == Dif.Local && x.Tipo_Pago == 2).SingleOrDefault();
                        DifConciliacion = (new DetalleConciliacionPagos
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
                            MontoMp = mp.Monto,
                            TarjetaMp = mp.Tarjeta,
                            IdTienda = mp.IdTienda


                        });

                        DifConciliacion2 = (new DetalleConciliacionPagos
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
                            MontoMp = mp.Monto,
                            TarjetaMp = mp.Tarjeta,
                            IdTienda = mp.IdTienda


                        });
                        mp.EstadoConciliado = true;


                    }
                    else
                    {
                        ACREEDOR_REL Acrededor = db.ACREEDOR_REL.Where(x => x.Local == Dif.Centro && x.Tipo_Pago == 2).SingleOrDefault();
                        DifConciliacion = (new DetalleConciliacionPagos
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
                            IdTienda = api.CODIGO_COMERCIO_TBK.Local


                        });

                        DifConciliacion2 = (new DetalleConciliacionPagos
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
                            IdTienda = api.CODIGO_COMERCIO_TBK.Local


                        });
                        api.Estado = true;
                     
                    }


                    db.DetalleConciliacionPagos.Add(DifConciliacion);
                    db.DetalleConciliacionPagos.Add(DifConciliacion2);
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


        //Trae las Reversas Manuales que el usurio Ingreso con sus Respectiva Cuentas Contables
        //public List<Diferencias_Pagos> ObtenerLasReversas(DateTime Fecha, int Tienda)
        //{
        //    List<Diferencias_Pagos> lst = new List<Diferencias_Pagos>();

        //    var CC = db.Cuenta.ToList();

        //    var TiendaACC = db.ACREEDOR_REL.Where(x => x.Local == Tienda && x.Tipo_Pago == 2).SingleOrDefault();

        //    lst = db.Diferencias_Pagos.Where(x => x.Fecha == Fecha && x.idTiendaAcredededor == TiendaACC.Id).OrderBy(x => x.TipoAsiento).ToList();

        //    var lstPrueba = lst.Where(x => CC.Contains(x.Cuenta)).OrderBy(x => x.TipoAsiento).ToList();
        //    return lst;
        //}


    }
}