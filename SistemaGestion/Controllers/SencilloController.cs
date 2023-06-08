using Newtonsoft.Json;
using Owin;
using RestSharp;
using SistemaGestion.Models.DAO;
using SistemaGestion.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SistemaGestion.Controllers
{
    public class SencilloController : Controller
    {
        private SencilloNegocio sng;
        //private IAppBuilder _usuario;
        public SencilloController() {

            //_usuario = app;
            sng = new SencilloNegocio();
        }
        // GET: Sencillo
        public ActionResult Index()
        {

            return View();
        }

        public async Task<ActionResult> SaveDeposito(DepositoView deposito)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                var prueba = Thread.CurrentPrincipal.Identity.Name;
                var prueba2 = JsonConvert.DeserializeObject<UserLoginView>(prueba);

                
                var url = string.Format("http://172.18.14.247/Pruebas/Token");
                var peticcion = new HttpRequestMessage(HttpMethod.Post, url);
                var prueba232 = (new { rut = prueba2.LoginName, password = prueba2.Password });
                peticcion.Content = new StringContent(JsonConvert.SerializeObject(prueba232),Encoding.UTF8, "application/json");

                string Credencials = string.Format("{0}:{1}",prueba2.LoginName,prueba2.Password);
                string RespeustaCont = "";
                using (HttpClient cliente = new HttpClient())
                {
                    //cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic",Convert.ToBase64String(Encoding.UTF8.GetBytes(Credencials)));
                    var request = await cliente.PostAsync(url,peticcion.Content);
                    RespeustaCont = await request.Content.ReadAsStringAsync();

                }

                var hola = RespeustaCont;

              
            }

            catch (Exception)
            {

                throw;
            }
           
            


            return View(response);
        }
    }
}