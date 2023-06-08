using SistemaGestion.Helpers;
using SistemaGestion.Models.DAO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SistemaGestion.Models.Repositorios
{
    public class AdministracionGS
    {
        private SisGesEntities3 db;
        private string con = Helper.ConexionStringModel();

        //private interfacesIntellecEntities db;

        public AdministracionGS()
        {
            db = new SisGesEntities3();
        }

        public ResponseModel BarridoGS(DateTime FechaI, DateTime fechaF)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                using (SqlConnection cn = new SqlConnection(con))
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("Proc_TRASPASA_GS_SISGES_BARRIDO", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@P_fecha1", FechaI.ToString("yyyyMMdd"));
                    cmd.Parameters.AddWithValue("@P_fecha2", fechaF.ToString("yyyyMMdd"));                   
                    cmd.CommandTimeout = 1000;
                    cmd.ExecuteReader();

                    cn.Close();
                    response.error = false;
                    response.respuesta = "Se han procesado cerrectamente";
                }
                return response;
            }
            catch (Exception ex)
            {
                response.error = true;
                response.respuesta = "Ha ocurrido un error " + ex.Message;
                return response;
            }

            
        }

        public ResponseModel GenerarMediosPagosPorFecha(DateTime Fecha)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                //string conexion = Helper.Helper.ConexionGlobalModel();
                //string conexion = "data source=172.18.14.29;initial catalog=GlobalSTORE;user id=sa;password=SoporteTI5951;MultipleActiveResultSets=True;App=EntityFramework;";

                var strFecha = Fecha.ToString("yyyyMMdd");

                using (SqlConnection conn = new SqlConnection(con))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand("Proc_GENERAR_MEDIOS_PAGOS_X_FECHA", conn))
                    {
                        cmd.CommandTimeout = 500;
                        cmd.CommandText = $@"exec dbo.Proc_GENERAR_MEDIOS_PAGOS_X_FECHA @P_Fecha='{strFecha}'";
                        cmd.ExecuteNonQuery();

                    }
                    conn.Close();
                    response.error = false;
                    response.respuesta = "Se han procesado cerrectamente";
                }
                return response;
            }
            catch (Exception ex)
            {
                response.error = true;
                response.respuesta = "Ha ocurrido un error " + ex.Message;
                return response;
            }
            
        }

    }
}