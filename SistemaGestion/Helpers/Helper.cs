using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaGestion.Helpers
{
    public static class Helper
    {
        public static string ConexionStringModel()
        {

            //var builder = new ConfigurationBuilder()
            //  .SetBasePath(AppContext.BaseDirectory)
            //  .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            //IConfiguration configuration = builder.Build();
            var conexion = "Data Source = 172.18.14.96; Initial catalog = sisges; User ID = sa; Password = ZxJgMuaD9b;";
            return conexion;
        }
    }

}