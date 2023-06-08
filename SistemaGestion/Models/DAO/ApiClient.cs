using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RestSharp;
using Newtonsoft.Json;
using SistemaGestion.Models.Repositorios;
using SistemaGestion.Models.ViewModels;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;

namespace SistemaGestion.Models.DAO
{
    public class ApiClient
    {
        #region objetos
        private SisGesEntities3 db = new SisGesEntities3();
        Lista objeto = new Lista();

        public static string TokenId = "x-client-id";

        public static string TokenHeaderPass = "8eefa93c78f05a8ea717b0b23151bd7e";

        #endregion

        #region obtenerTotales api
        public async Task<TransacionesTotales.Root> TotalesAPI(DateTime FechaIni, DateTime FechaFin, int Local)
        {
            
            try
            {
                //Fecha de Inico Convertida YYYY-MM-DD
                var fechaI = FechaIni.ToString("yyyy-MM-dd");
                ////Fecha de Fin Convertida YYYY-MM-DD
                var fechafin = FechaFin.ToString("yyyy-MM-dd");
                var url = string.Format("https://api.transbank.cl/transbank/publico/transacciones/totales?fecha-desde={0}&fecha-hasta={1}&codigos-comercio={2}", fechaI, fechafin, Local);
                HttpClient cliente = new HttpClient();
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
                requestMessage.Headers.Add(TokenId, TokenHeaderPass);
                var response = await cliente.SendAsync(requestMessage);
                var contenido = await response.Content.ReadAsStringAsync();
                var total = JsonConvert.DeserializeObject<TransacionesTotales.Root>(contenido);
                return total;
            }
            catch (Exception ex)
            {
                var response = new ResponseModel();
                response.error = false;
                response.respuesta = "Procedimiento Transaciones Totales no se genero  " + ex.Message;
                throw ex;
            }
          
        }

        public TransacionesTotales.Root ObtenerTotales(DateTime FechaIni, DateTime FechaFin, int Local)
        {
            var response = new ResponseModel();

            try
            {
                //Fecha de Inico Convertida YYYY-MM-DD
                var fechaI = FechaIni.ToString("yyyy-MM-dd");
                ////Fecha de Fin Convertida YYYY-MM-DD
                var fechafin = FechaFin.ToString("yyyy-MM-dd");
                //Formato de envio a la API
                var url = string.Format("https://api.transbank.cl/transbank/publico/transacciones/totales?fecha-desde={0}&fecha-hasta={1}&codigos-comercio={2}", fechaI, fechafin, Local);
                var client = new RestClient(url);
                var request = new RestRequest(Method.GET);
                request.AddHeader("Accept", "application/json");
                //request.AddHeader("x-mock", "200");
                request.AddHeader("x-client-id", "8eefa93c78f05a8ea717b0b23151bd7e");
                request.RequestFormat = DataFormat.Json;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                IRestResponse response1 = client.Execute(request);
                var JsonString = response1.Content;
                var totales = JsonConvert.DeserializeObject<TransacionesTotales.Root>(JsonString);
                response.respuesta = "Se cargo Correctamente";
                response.error = true;
                return totales;
            }
            catch (Exception ex)
            {
                response.error = false;
                response.respuesta = "Procedimiento Transaciones Totales no se genero  " + ex.Message;
                throw ex;
            }
        }
        #endregion

        #region TransanccionesApi

        public List<Ventas_Apitbk> ObtenerTransacciones(int local, DateTime Fecha)
        {



            //var formato = ("yyyy-MM-dd");
            //var localstring = local.ToString();
            //var FechaString = Fecha.ToString(formato);
            var prueba = db.Ventas_Apitbk.Where(d => d.codigoLocal == local && d.fechaTransaccion == Fecha).ToList();

            //objeto.Api = (from d in db.Transbankapi
            //              where d.codigoLocal == localstring && d.fechaTransaccion == FechaString
            //              orderby d.tipoProducto
            //              select new DetalleTransbank.Data
            //              {
            //                  fechaTransaccion = d.fechaTransaccion,
            //                  numeroTarjeta = d.numeroTarjeta,
            //                  totalCuotas = d.totalCuotas,
            //                  montoCuotas = d.montoCuotas,
            //                  codigoAutorizacion = d.codigoAutorizacion,
            //                  codigoLocal = d.codigoLocal,
            //                  tipoProducto = d.tipoProducto,
            //                  montoAfecto = d.montoAfecto,
            //                  tipoTransaccion = d.tipoTransaccion,
            //                  fechaAbono = d.fechaAbono,


            //              }).ToList();
            return prueba;
        }

        //Obtiene los tipo de pago
        ///List<SelectListItem> lst = new List<SelectListItem>();


        #endregion
    }
}