using Newtonsoft.Json;
using SistemaGestion.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SistemaGestion.Models.DAO
{
    public class CentralizacionPagos
    {
        private SisGesEntities3 db = new SisGesEntities3();
        private Negocio Ng = new Negocio();
                
        public List<PAGOS> ListadePagos(DateTime Fecha, int local)
        {
            List<PAGOS> lst = new List<PAGOS>();
            if (local == 0)
            {
                lst = db.PAGOS.Where(x => x.Fecha == Fecha && x.IdTienda == local).ToList();
            }
            else
            {
                lst = db.PAGOS.Where(x => x.Fecha == Fecha).ToList();
            }
           
            return lst;
        }

        public List<fnDashboard_Pagos_Result> Totales (DateTime Fecha, int local)
        {
            List<fnDashboard_Pagos_Result> lst = new List<fnDashboard_Pagos_Result>();

            lst = db.fnDashboard_Pagos(Fecha, local).ToList();
            
            return lst;
        }
    

        public List<PAGOS_MP> ListaIdocMP(DateTime fecha)
        {
            List<PAGOS_MP> PagosMP = new List<PAGOS_MP>();
            //var FechaString = fecha.ToString("yyyy-MM-dd");
            var FechaString = fecha.ToString("ddMMyyyy");
            ResponseModel response = new ResponseModel();
            try
            {
                //db.Proc_LeerPAGOSMP(fecha);
                response.respuesta = "Se cargo Correctamente";
                response.error = true;
                if (response.error)
                {
                    PagosMP = db.PAGOS_MP.Where(x => x.FECHAINT == FechaString).ToList();
                }


            }
            catch (Exception ex)
            {

                response.error = false;
                response.respuesta = "Procedimiento Insert Tarea No se Genero  " + ex.Message;
            }
            
           
            return PagosMP;
        }


        public List<IDOC_PAGOS_FI> PagosFI (DateTime Fecha)
        {
            //List<IDOC_PAGOS_FI> FI = new List<IDOC_PAGOS_FI>();
             var fechastring = Fecha.ToString("ddMMyyyy");
            ResponseModel response = new ResponseModel();
            List<fnIdocPagos_FI_Result> lst = new List<fnIdocPagos_FI_Result>();
            List<IDOC_PAGOS_FI> list2 = new List<IDOC_PAGOS_FI>();
            var con = "data source=172.18.14.96;initial catalog=sisges;user id=sa;password=ZxJgMuaD9b;MultipleActiveResultSets=True;";
            var lt = new List<int>();
            try
            {
                 
                string CommandText = "Select * from IDOC_PAGOS_FI where FECHAINT = @Fecha ";

                using (SqlConnection conn = new SqlConnection(con))
                {
                    conn.Open();
                    //_logger.LogInformation("Conexion con Servidores: " + item.ServidorIP);
                    using (SqlCommand cmd = new SqlCommand(CommandText, conn))
                    {
                        cmd.Parameters.Add("@Fecha", SqlDbType.VarChar);
                        cmd.Parameters["@Fecha"].Value = fechastring;
                        var reader = cmd.ExecuteReader();

                        //var r = Serializer(reader.Read());
                        var dt = new DataTable();
                        dt.Load(reader);
                        string Json = string.Empty;
                        Json = JsonConvert.SerializeObject(dt);

                        list2.AddRange(JsonConvert.DeserializeObject<List<IDOC_PAGOS_FI>>(Json));


                        conn.Close();
                    }
                }

                return list2;

            }
            catch (Exception ex)
            {

                response.error = false;
                response.respuesta = "Procedimiento Insert Tarea No se Genero  " + ex.InnerException;
                return list2;
            }


          
        }

    }



    
}