using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Web;
using System.Data.Entity;
using SistemaGestion.Models.ViewModels;
using SistemaGestion.Models.Repositorios;
using System.Data.SqlClient;
using SistemaGestion.Helpers;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;
using System.Text;

namespace SistemaGestion.Models.DAO
{
    public class Negocio
    {
        private SisGesEntities3 db = new SisGesEntities3();

        public async Task<ResponseModel> GetCentralizacion()
        {
            ResponseModel response = new ResponseModel();
            try
            {
                var Url = "http://172.18.14.98:1234/Pruebas/Token";
                response.error = false;
                response.respuesta = "Se genero el Reproceso de Centralizacion";
            }
            catch (Exception ex)
            {

                response.error = true;
                response.respuesta = "Error al consumir EndPoint de Centralizacion " + ex.Message;
            }
            return response;
        }

        public async Task<ResponseModel> ReprocesoTbk (DateTime Fecha)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                var url = string.Format("http://172.18.14.98:1234/ApiTransbank/ConsultarTBK?fechaInicio={0}",Fecha);
                using (HttpClient cliente = new HttpClient())
                {

                    HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    cliente.Timeout = Timeout.InfiniteTimeSpan;
                    var respuesta = await cliente.SendAsync(requestMessage);
                    var contenido = await respuesta.Content.ReadAsStringAsync();
                    if (respuesta.StatusCode == HttpStatusCode.OK || respuesta.StatusCode == HttpStatusCode.Created)
                    {
                        response.error = false;
                        response.respuesta = "Se genero el Reproceso de Tbk" + contenido;
                    }
                    else
                    {
                        response.error = true;
                        response.respuesta = "Mensaje de Api :" + respuesta.StatusCode.ToString();
                    }

                   

                }
               
            }
            catch (Exception ex)
            {

                response.error = true;
                response.respuesta = "Error al Reprocesar las trx de Tbk" + ex.Message;
            }
            return response;
        }

        public async Task<ResponseModel> ReprocesoInterfaces(DateTime Fecha, string Token)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                var url = "http://172.18.14.98:1234/Pruebas/LecturaArchivosSFT";
                var parametros = new
                {
                    Fecha = Fecha
                };
                var jsonP = Newtonsoft.Json.JsonConvert.SerializeObject(parametros);
                var content = new StringContent(jsonP, Encoding.UTF8, "application/json");
                var Bearer = string.Format("Bearer " + Token);
                using (HttpClient httpClient = new HttpClient())
                {
                   

                    httpClient.DefaultRequestHeaders.Add("Authorization", Bearer);

                    HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
                    requestMessage.Content = content;

                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    httpClient.Timeout = Timeout.InfiniteTimeSpan;

                    var respuesta = await httpClient.SendAsync(requestMessage);
                    var contenido = await respuesta.Content.ReadAsStringAsync();
                }

                response.error = false;
                response.respuesta = "Se genero el Reproceso de Interfaces";
            }
            catch (Exception ex)
            {

                response.error = true;
                response.respuesta = "Error al Reprocesar las trx de Interfaces" + ex.Message;
            }
            return response;
        }

        public async Task<ResponseModel> GetToken(UserLoginView login)
        {
            ResponseModel response = new ResponseModel();
            try
            {

                var url = "http://172.18.14.98:1234/Pruebas/Token";

                var parametros = new
                {
                    rut = login.LoginName,
                    pass = login.Password,

                };
                var jsonP = Newtonsoft.Json.JsonConvert.SerializeObject(parametros);
                var content = new StringContent(jsonP, Encoding.UTF8, "application/json");
                using (HttpClient cliente = new HttpClient())
                {


                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    var request = await cliente.PostAsync(url, content);
                  
                    var contenido = await request.Content.ReadAsStringAsync();
                    var Json = JsonConvert.DeserializeObject<ApiSiges>(contenido);
                    response.token = Json.result;
                    response.respuesta = "Genero el Token Correctamente";
                    response.error = false;

               
                }

              
            }
            catch (Exception ex)
            {

                response.error = true;
                response.respuesta = "Error al ejecutar la Api de Alertas" + ex.Message;
            }
            return response;
        }

        public List<Fn_MP_Cuenta_Rut_Result> MPCuentaRut(DateTime Fecha)
        {
            List<Fn_MP_Cuenta_Rut_Result> lst = new List<Fn_MP_Cuenta_Rut_Result>();
            lst = db.Fn_MP_Cuenta_Rut(Fecha).ToList();
            return lst;
        }
        public List<Fn_PAGOS_Cuenta_Rut_Result> PagosCuentaRut(DateTime Fecha)
        {
            List<Fn_PAGOS_Cuenta_Rut_Result> lst = new List<Fn_PAGOS_Cuenta_Rut_Result>();
            lst = db.Fn_PAGOS_Cuenta_Rut(Fecha).ToList();
            return lst;
        }
        public async Task<List<fnReporte_Nota_Credito_Result>> ListaNotaCredito(DateTime Inicio, DateTime Fin, int local)
        {
            //List<fnReporte_Nota_Credito_Result> lst = new List<fnReporte_Nota_Credito_Result>();
            Fin = Fin.AddHours(23).AddMinutes(59).AddSeconds(59);
            var lst = await db.fnReporte_Nota_Credito(Inicio, Fin, local).ToListAsync();
            return lst;

        }
        public async Task<List<fnReporte_PagosFSP_Result>> ListaPagoFSP(int ano, int mes)
        {
            List<fnReporte_PagosFSP_Result> lst = new List<fnReporte_PagosFSP_Result>();
            //Fin = Fin.AddHours(23).AddMinutes(59).AddSeconds(59);
            try
            {
                db.Database.CommandTimeout = 0;
                lst = await db.fnReporte_PagosFSP(ano, mes).ToListAsync();
                return lst;
            }
            catch (Exception ex)
            {
                var error = ex.Message;
                return lst;
            }
           
        

        }
        public CODIGO_COMERCIO_TBK getTienda(int CodigoComercio)
        {
            CODIGO_COMERCIO_TBK tienda = new CODIGO_COMERCIO_TBK();
            tienda = db.CODIGO_COMERCIO_TBK.Where(x => x.CodigoComercio == CodigoComercio).SingleOrDefault();
            return tienda;
        }

        //Tiendas tbk Ventas
        public List<LocalesTransbank> TiendasTbk()
        {
            List<CODIGO_COMERCIO_TBK> lst = new List<CODIGO_COMERCIO_TBK>();
            var lst2 = new List<LocalesTransbank>();
            lst2 = (from d in db.CODIGO_COMERCIO_TBK.Where(x => x.Tipo == 1)
                    select new LocalesTransbank
                    {
                        Centro = d.Local,
                        Nombre_Centro = d.NombreLocal,
                        Codigo = d.CodigoComercio,

                    }).OrderBy(x => x.Centro).ToList();
            return lst2;
        }

        //Pagos
        public List<LocalesTransbank> TiendasTbkPagos()
        {
            List<CODIGO_COMERCIO_TBK> lst = new List<CODIGO_COMERCIO_TBK>();
            var lst2 = new List<LocalesTransbank>();
            lst2 = (from d in db.CODIGO_COMERCIO_TBK.Where(x => x.Tipo == 2)
                    select new LocalesTransbank
                    {
                        Centro = d.Local,
                        Nombre_Centro = d.NombreLocal,
                        Codigo = d.CodigoComercio,

                    }).OrderBy(x => x.Centro).ToList();
            return lst2;
        }

        public List<ElementoJsonKeyTBK> TiendaConciliacion (DateTime Fecha)
        {
            List<ElementoJsonKeyTBK> lst = new List<ElementoJsonKeyTBK>();

            List<Ventas_Apitbk> api = db.Ventas_Apitbk.Where(x => x.fechaTransaccion == Fecha && x.Estado == false).ToList();
            List<MEDIOSPAGOS> mp = db.MEDIOSPAGOS.Where(x => x.Fecha == Fecha && x.Estado == false && ( x.Tipo_Pago == 5 || x.Tipo_Pago == 9) && !x.Tipo.Equals("NOTACREDITO")).ToList();
            List<CODIGO_COMERCIO_TBK> cc = db.CODIGO_COMERCIO_TBK.ToList();
            List<DetalleConciliacion> lstdetalleConciliacions = db.DetalleConciliacion.Where(x => x.TipoConciliacion.Equals("M") && x.Conciliacion.FechaConciliar == Fecha).ToList();

            var lstTiendasConciliadas = cc.Where(x => lstdetalleConciliacions.Select(y => y.IdTienda).Contains(x.Local)).Select(x => new ElementoJsonKeyTBK { Value = x.Local, Text = x.NombreLocal }).ToList();

            var pruebaapi = api.Where(x => x.CODIGO_COMERCIO_TBK.Local == 19).ToList();
            var pruebamp = mp.Where(x => x.IdTienda == 19).ToList();

            //var Tiendaapi = api.GroupBy(x => new { x.CODIGO_COMERCIO_TBK.Local, x.CODIGO_COMERCIO_TBK.NombreLocal }).Select(x => new ElementoJsonKeyTBK { Value = x.Key.Local, Text = x.Key.NombreLocal }).ToList();
            //var TiendaMp = mp.GroupBy(x => new { x.IdTienda, x.CENTROS_LOCAL.CEN_Nombre }).Select(x => new ElementoJsonKeyTBK { Value = x.Key.IdTienda, Text = cc.Where(y=>y.Local == x.Key.IdTienda).Select(h=>h.NombreLocal).FirstOrDefault() }).ToList();

            //lst = Tiendaapi.Union(TiendaMp).ToList();

            lst = (api.GroupBy(x => new { x.CODIGO_COMERCIO_TBK.Local, x.CODIGO_COMERCIO_TBK.NombreLocal }).Select(x => new ElementoJsonKeyTBK { Value = x.Key.Local, Text = x.Key.NombreLocal }).ToList()).Union(mp.GroupBy(x => new { x.IdTienda, x.CENTROS_LOCAL.CEN_Nombre }).Select(x => new ElementoJsonKeyTBK { Value = x.Key.IdTienda, Text = cc.Where(y => y.Local == x.Key.IdTienda).Select(h => h.NombreLocal).FirstOrDefault() }).ToList()).ToList();
            lst.AddRange(lstTiendasConciliadas);
             return lst.GroupBy(x => new { x.Value,x.Text }).Select(x=> new ElementoJsonKeyTBK {Value = x.Key.Value, Text=x.Key.Text }).ToList();
        }

        public List<ElementoJsonKeyTBK> TiendaConciliacionDiferencia(DateTime Fecha)
        {
            List<ElementoJsonKeyTBK> lst = new List<ElementoJsonKeyTBK>();

            List<Ventas_Apitbk> api = db.Ventas_Apitbk.Where(x => x.fechaTransaccion == Fecha && x.Estado == false).ToList();
            List<MEDIOSPAGOS> mp = db.MEDIOSPAGOS.Where(x => x.Fecha == Fecha && x.Estado == false && (x.Tipo_Pago == 5 || x.Tipo_Pago == 9) && !x.Tipo.Equals("NOTACREDITO")).ToList();
            List<CODIGO_COMERCIO_TBK> cc = db.CODIGO_COMERCIO_TBK.ToList();
            List<DetalleConciliacion> lstdetalleConciliacions = db.DetalleConciliacion.Where(x => x.TipoConciliacion.Equals("D") && x.Conciliacion.FechaConciliar == Fecha).ToList();
            //Conciliacion conciliacion = db.Conciliacion.Where(x => x.FechaConciliar == Fecha).SingleOrDefault();
            //List<DetalleConciliacion> lstdetalleConciliacions = conciliacion.DetalleConciliacion.Where(x => x.TipoConciliacion.Equals("D")).ToList();
            var lstTiendasConciliadas = cc.Where(x => lstdetalleConciliacions.Select(y => y.IdTienda).Contains(x.Local)).Select(x => new ElementoJsonKeyTBK { Value = x.Local, Text = x.NombreLocal }).ToList();


            //var Tiendaapi = api.GroupBy(x => new { x.CODIGO_COMERCIO_TBK.Local, x.CODIGO_COMERCIO_TBK.NombreLocal }).Select(x => new ElementoJsonKeyTBK { Value = x.Key.Local, Text = x.Key.NombreLocal }).ToList();
            //var TiendaMp = mp.GroupBy(x => new { x.IdTienda, x.CENTROS_LOCAL.CEN_Nombre }).Select(x => new ElementoJsonKeyTBK { Value = x.Key.IdTienda, Text = cc.Where(y=>y.Local == x.Key.IdTienda).Select(h=>h.NombreLocal).FirstOrDefault() }).ToList();

            //lst = Tiendaapi.Union(TiendaMp).ToList();

            lst = (api.GroupBy(x => new { x.CODIGO_COMERCIO_TBK.Local, x.CODIGO_COMERCIO_TBK.NombreLocal }).Select(x => new ElementoJsonKeyTBK { Value = x.Key.Local, Text = x.Key.NombreLocal }).ToList()).Union(mp.GroupBy(x => new { x.IdTienda, x.CENTROS_LOCAL.CEN_Nombre }).Select(x => new ElementoJsonKeyTBK { Value = x.Key.IdTienda, Text = cc.Where(y => y.Local == x.Key.IdTienda).Select(h => h.NombreLocal).FirstOrDefault() }).ToList()).ToList();
            lst.AddRange(lstTiendasConciliadas);
            return lst.GroupBy(x => new { x.Value, x.Text }).Select(x => new ElementoJsonKeyTBK { Value = x.Key.Value, Text = x.Key.Text }).ToList();
        }

        //Trae la lista de Participacion por tienda
        public List<fnReporte_Participacion_Result> Participacion(int ConYear, int Mes, int Tienda)
        {
            List<fnReporte_Participacion_Result> lst = new List<fnReporte_Participacion_Result>();

            lst = db.fnReporte_Participacion(ConYear, Mes, Tienda).ToList();

            return lst;
        }

        public List<fnReporte_Avance_Result> Avance(int ConYear, int Mes, int Tienda)
        {
            List<fnReporte_Avance_Result> lst = new List<fnReporte_Avance_Result>();
            lst = db.fnReporte_Avance(ConYear, Mes, Tienda).OrderBy(x => x.Local).ToList();
            return lst;
        }
        public List<fnReporte_TicketPromedio_Result> TicketPromedio(int ConYear, int Mes, int Tienda)
        {
            List<fnReporte_TicketPromedio_Result> lst = new List<fnReporte_TicketPromedio_Result>();
            lst = db.fnReporte_TicketPromedio(ConYear, Mes, Tienda).ToList();
            return lst;
        }

        public List<ReporteVentasView> DetalleReporteVentas(int ConYear, int Mes, int Tienda)
        {


            List<ReporteVentasView> ventasViews = new List<ReporteVentasView>();
            
            List<fnReporte_Ventas_Result> lst = new List<fnReporte_Ventas_Result>();

            lst = db.fnReporte_Ventas(ConYear, Mes, Tienda).ToList();

            decimal Total = 0;


            
            Total = lst.Where(x => x.TipodePago != "NOTACREDITO").Sum(x => x.Monto).Value - (lst.Where(x =>x.TipodePago == "NOTACREDITO").Sum(x => x.Monto).Value);
            foreach (var a in lst.Select(x => x.Pago).Distinct())
            {
                ReporteVentasView ReporteVentas = new ReporteVentasView();
                ReporteVentas.idTienda = Tienda;
                ReporteVentas.TipodePago = a;
                ReporteVentas.Trx = lst.Where(x => x.idTienda == Tienda && x.Pago == a).Count();
                ReporteVentas.TotalVenta = lst.Where(x => x.idTienda == Tienda && x.Pago == a && x.TipodePago != "NOTACREDITO").Sum(x => x.Monto) - (lst.Where(x => x.idTienda == Tienda && x.Pago == a && x.TipodePago == "NOTACREDITO").Sum(x => x.Monto));
                ReporteVentas.Porcentaje = (ReporteVentas.TotalVenta / Total) * 100;
                ventasViews.Add(ReporteVentas);

            }

           
            return ventasViews;
        }

      

        public List<fnReporte_Ventas_Result> ReporteVentas2(int ConYear, int Mes, int Tienda)
        {
            List<fnReporte_Ventas_Result> lst = new List<fnReporte_Ventas_Result>();
            lst = db.fnReporte_Ventas(ConYear, Mes, 0).ToList();

             
            return lst;
        }

        public List<MEDIOSPAGOS> MP ( DateTime Fecha)
        {
            List<MEDIOSPAGOS> lst = new List<MEDIOSPAGOS>();
            try
            {
                //if (local == 0)
                //{
                    lst = db.MEDIOSPAGOS.Where(x => x.Fecha == Fecha).ToList();
                //}
                //else
                //{
                //    lst = db.MEDIOSPAGOS.Where(x => x.Fecha == Fecha && x.IdTienda == local).ToList();
                //}
            }
            catch (Exception)
            {

                throw;
            }

           
           
            return lst;

        }

        public void Auditoria(Auditoria auditoria)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                db.Database.Connection.Open();
                db.Auditoria.Add(auditoria);
                db.SaveChanges();
                db.Database.Connection.Close();
                response.error = false;
                response.respuesta = "Se a guardado la auditoria correctamente";
               
            }
            catch (Exception ex)
            {
                response.error = true;
                response.respuesta = "Ocurrio un error" + ex.Message;
      
            }
        }

        public ResponseModel CierrePeriodo()
        {
            ResponseModel response = new ResponseModel();
            try
            {
                //otenemos el mes aperturado en curso
                Periodo Aperturado = db.Periodo.Where(x => x.Cerrado == false).SingleOrDefault();
                //validamos el sigiente mes que se apertura
           
                int ano = Aperturado.MesNumero == 12 ? Aperturado.AnoNumero + 1 : Aperturado.AnoNumero;
                int mes = Aperturado.MesNumero == 12 ? 1 : Aperturado.MesNumero + 1;
                DateTime Fecha = new DateTime(ano, mes, 1);
                db.AperturaPeriodo(ano, mes);
                db.Database.Connection.Close();


                response.respuesta = $"Se ha aperturado el año {ano} y el Mes {Fecha.ToString("MMMM")}";
                response.error = false;
                response.Datos = "{ano:" + ano + ", mes:" + mes + "}";
                return response;
            }
            catch (Exception ex)
            {
                response.error = true;
                response.respuesta = "Ocurrio un error" + ex.Message;
                return response;
            }
        }

        public List<fnReporteBoletasContraloria_Result> BoletasContraloria(int mes, int yr)
        {
            List<fnReporteBoletasContraloria_Result> lst = new List<fnReporteBoletasContraloria_Result>();
            try
            {
                db.Database.CommandTimeout = 1000;
                lst = db.fnReporteBoletasContraloria(yr, mes).ToList();
            }
            catch (Exception)
            {

                throw;
            }
            
            return lst;
        }

        public Usuario GetUsuario (Guid id)
        {
            Usuario usuario = db.Usuario.Where(x => x.Id == id).FirstOrDefault();
            
            return usuario;
        }

        public List<fnReporteNCContraloria_Result> GetNcControlaria (DateTime Inicio, DateTime Fin)
        {
            List<fnReporteNCContraloria_Result> lst = new List<fnReporteNCContraloria_Result>();
            try
            {
                lst = db.fnReporteNCContraloria(Inicio, Fin).ToList();
                return lst;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<notaCredito> GetNcControlariaNew(DateTime Inicio, DateTime Fin)
        {
            List<notaCredito> lst = new List<notaCredito>();
            try
            {
                db.Configuration.LazyLoadingEnabled = true;
                lst = db.notaCredito.Where(x => x.Fecha_Emision >= Inicio && x.Fecha_Emision <= Fin).Include(n => n.EMPLEADO).Include(n => n.EMPLEADO1).ToList();

                // Busca los registros que tengan la propiedad de navegación Empleado como nula
                //var notasSinEmpleado = lst.Where(n => n.EMPLEADO == null);

                //// Itera a través de los registros y establece la propiedad de navegación Empleado para cada uno
                //foreach (var nota in notasSinEmpleado)
                //{
                //    // Busca el Empleado correspondiente por el Rut_Cajero
                //    lst.Where(x=>x == nota).SingleOrDefault().EMPLEADO = db.EMPLEADO.FirstOrDefault(e => e.ID_EMPLEADO == nota.Rut_Cajero);

                    
                //}

                return lst;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<fn_ReporteNcFULLCuadratura_Result> GetNcCuadratura(DateTime Inicio, DateTime Fin)
        {
            List<fn_ReporteNcFULLCuadratura_Result> lst = new List<fn_ReporteNcFULLCuadratura_Result>();
            try
            {
                db.Configuration.LazyLoadingEnabled = true;
                db.Database.CommandTimeout = 10000;
                lst = db.fn_ReporteNcFULLCuadratura(Inicio, Fin).ToList();

                return lst;
            }
            catch (Exception)
            {

                throw;
            }
        }



        public async Task<ResponseModel> GetAlertaCierre(string r)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                //var Credeenciales = Helper.Endpoints();
                var url = string.Format("https://172.18.14.98:1234//GeneracionAuto/EnvioCorreo?RespuestaCierre={0}", r);
                using (HttpClient cliente = new HttpClient())
                {
                 
                    HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    var respuesta = await cliente.SendAsync(requestMessage);
                    var contenido = await respuesta.Content.ReadAsStringAsync();
                    response.respuesta = "Genero el Token Correctamente";
                  
                    response.error = false;

                    return response;
                }
            }
            catch (Exception ex)
            {

                response.error = true;
                response.respuesta = "Error al ejecutar la Api de Alertas de Cierre" + ex.Message;
                

                return response;
            }
        }


        public async Task<ResponseModel> GetAlertaCartaInstruccion(string r,DateTime inicio, DateTime fin)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                //var Credeenciales = Helper.Endpoints();

                //var builder = new UriBuilder("https://localhost:7153/api/Sencillo/AlertaCartaInstruccion");
                //builder.Query = $"respuesta={Uri.EscapeDataString(r)}&inicio={Uri.EscapeDataString(inicio.ToString())}&fin={Uri.EscapeDataString(fin.ToString())}";
                //r = r.Replace(" ", "%");

                //var url = string.Format("https://localhost:7153/Sencillo/AlertaCartaInstruccion");
                var url = string.Format("http://172.18.14.98:1234/Sencillo/AlertaCartaInstruccion");

                var parametros = new
                {
                    respuesta = r,
                    inicio = inicio,
                    fin = fin
                }; 
                
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(parametros);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                //var response = await httpClient.PostAsync(url, content);
                
                using (HttpClient cliente = new HttpClient())
                {

                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    var request = await cliente.PostAsync(url, content);
                    var contenido = await request.Content.ReadAsStringAsync();
                    response.respuesta = "Genero el Token Correctamente";

                    response.error = false;

                    return response;
                }
            }
            catch (Exception ex)
            {

                response.error = true;
                response.respuesta = "Error al ejecutar la Api de Alertas de Cierre" + ex.Message;


                return response;
            }
        }

        #region no se usa 
        //public ResponseModel AperturaLibroMayor()
        //{
        //    ResponseModel response = new ResponseModel();
        //    //try
        //    //{
        //    //    var Mesaperturado = db.LibroMayor.Where(x => x.Estado.HasValue).SingleOrDefault();
        //    //    Mesaperturado.Estado = false;
        //    //    DateTime date = new DateTime(Mesaperturado.Ano.Value,Mesaperturado.Mes.Value,1);
        //    //    date = date.AddMonths(1);
        //    //    var MesxAperturar = new LibroMayor {
        //    //        id = Guid.NewGuid(),
        //    //        Estado = true,
        //    //        FechaCreacion = DateTime.Now,
        //    //        Ano = date.Year,
        //    //        Mes = date.Month,


        //    //    };
        //    //    db.LibroMayor.Add(MesxAperturar);
        //    //    db.SaveChanges();
        //    //    response.error = false;
        //    //    response.respuesta = "Se inserto correctamente";
        //    //}
        //    //catch (Exception ex)
        //    //{

        //    //    response.error = true;
        //    //    response.respuesta = "Error al aperturar el mes " + ex.Message;
        //    //}
        //    return response;
        //}
        #endregion

    }
}