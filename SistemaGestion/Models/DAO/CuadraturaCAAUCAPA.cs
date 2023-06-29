using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace SistemaGestion.Models.DAO
{
    public class CuadraturaCAAUCAPA
    {
        private SisGesEntities3 db = new SisGesEntities3();

        public async Task<ResponseModel> ReprocesoConciliacionCAAU(DateTime Fecha)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                var url = string.Format("http://172.18.14.98:1234/Api/GeneracionAuto/ConciliacionAutomatica?Fecha={0}",Fecha.ToString("yyyy-MM-dd"));
                using (HttpClient client = new HttpClient())
                {
                    HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Get,url);
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    client.Timeout = Timeout.InfiniteTimeSpan;
                    var respuesta = await client.SendAsync(msg);
                    var contenido = await respuesta.Content.ReadAsStringAsync();
                    //var json = JsonConvert.SerializeObject(contenido); 
                    var json = JsonConvert.DeserializeObject<ResponseModel>(contenido);

                    response.respuesta = json.respuesta;
                    response.error = false;
                }
            }
            catch (Exception ex)
            {

                response.error = true;
                response.respuesta = "Procedimiento de Insercion automatica No se genero " + ex.Message;
            }
            return response;

        }
        //public ResponseModel ConciliacionAutomaticaCAAU(DateTime Fecha)
        //{
        //    ResponseModel response = new ResponseModel();
        //    try
        //    {
        //        db.Database.CommandTimeout = 1000000;
        //        db.spConciliacionAutomaticaCAAU(Fecha);
        //        response.respuesta = "Se cargo Correctamente";
        //        response.error = false;
        //        return response;
        //    }
        //    catch (Exception ex)
        //    {

        //        response.error = true;
        //        response.respuesta = "Procedimiento de Insercion automatica No se genero " + ex.Message;
        //        return response;
        //    }

        //}
        public ResponseModel ConciliacionAutomaticaCAPA(DateTime Fecha)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                db.Database.CommandTimeout = 1000000;
                db.spConciliacionAutomaticaCAPA(Fecha);
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

        public List<InterfazAutorizacion> CaauAutomatica(DateTime Fecha)
        {
            List<InterfazAutorizacion> lstCaau = new List<InterfazAutorizacion>();
            try
            {
                lstCaau = db.InterfazAutorizacion.Where(x => x.Estado.Equals("A") && x.FechaAutoriza==Fecha).ToList();

                return lstCaau;
            }
            catch (Exception ex)
            {
                return lstCaau;
            }
        }

        public List<InterfazAutorizacion> getCAAU(DateTime Fecha)
        {
            List<InterfazAutorizacion> lstCaau = new List<InterfazAutorizacion>();
            try
            {
                lstCaau = db.InterfazAutorizacion.Where(x => x.Estado.Equals("A") && x.FechaAutoriza == Fecha).ToList();

                return lstCaau;
            }
            catch (Exception ex)
            {
                return lstCaau;
            }
        }

        public List<fnReporte_CAAU_Result> getFuncionCAAU(DateTime Fecha)
        {
            List<fnReporte_CAAU_Result> lstCaau = new List<fnReporte_CAAU_Result>();
            try
            {
                db.Database.CommandTimeout = 10000000;
                lstCaau = db.fnReporte_CAAU(Fecha,"").ToList();

                return lstCaau;
            }
            catch (Exception ex)
            {
                var prueba = ex;

                return lstCaau;
            }
        }

        public List<fnReporte_CAAU_AVANCES_Result> getFuncionCAAUAvances(DateTime Fecha)
        {
            List<fnReporte_CAAU_AVANCES_Result> lstCaau = new List<fnReporte_CAAU_AVANCES_Result>();
            try
            {
                db.Database.CommandTimeout = 10000000;
                lstCaau = db.fnReporte_CAAU_AVANCES(Fecha, "").ToList();

                return lstCaau;
            }
            catch (Exception ex)
            {
                var prueba = ex;

                return lstCaau;
            }
        }

        public List<fnReporte_CAPA_PAGOS_Result> getFuncionCAPA(DateTime Fecha)
        {
            List<fnReporte_CAPA_PAGOS_Result> lstCaPa = new List<fnReporte_CAPA_PAGOS_Result>();
            try
            {
                db.Database.CommandTimeout = 10000000;
                lstCaPa = db.fnReporte_CAPA_PAGOS(Fecha, "").ToList();

                return lstCaPa;
            }
            catch (Exception ex)
            {
                var prueba = ex;

                return lstCaPa;
            }
        }

    }
}