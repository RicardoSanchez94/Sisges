using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaGestion.Models.ViewModels
{
    public class ApiSiges
    {
        public string BaseUrl { get; set; }
        public Endpoints Endpoints { get; set; }
        public string success { get; set; }

        public string message { get; set; }
        public string result { get; set; }
    }

    public class Endpoints
    {
        public string Token { get; set; }
        public string Rut { get; set; }
        public string Pass { get; set; }
        public string LecturaArchivosSFT { get; set; }
        public string AlertasSencillo { get; set; }

    }

    public class Root
    {
        public ApiSiges ApiSisges { get; set; }
    }
}