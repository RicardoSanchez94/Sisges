using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaGestion.Models.DAO
{
    public class ResponseModel
    {
        public bool error { get; set; }
        public string respuesta { get; set; }
        public string urlNext { get; set; }
        public string redirect { get; set; }

        public string Datos { get; set; }

        public ResponseModel()
        {
            error = true;
            respuesta = "";
            urlNext = "";
            redirect = "";
        }
    }
}