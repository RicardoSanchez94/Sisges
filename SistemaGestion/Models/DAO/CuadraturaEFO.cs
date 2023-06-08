using SistemaGestion.Helpers;
using SistemaGestion.Models.ViewModels;
using SpreadsheetLight;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SistemaGestion.Models.DAO
{
    public class CuadraturaEFO
    {
        private SisGesEntities3 db = new SisGesEntities3();
        public ResponseModel CargalibroMayor(HttpPostedFileBase oFile, string ruta)
        {
            ResponseModel response = new ResponseModel();
            var columna = 0;
            try
            {
                //string path = @"C:\File\lv.xml";
                //string pathFisico = path.Replace(@"\\", @"/");
                string rutaVirtual = ruta + "LirboMayor\\LibroMayor.xlsx";
                ruta = ruta + "LirboMayor\\";
                if (!System.IO.Directory.Exists(ruta))
                {
                    System.IO.Directory.CreateDirectory(ruta);
                }

                if (System.IO.File.Exists(rutaVirtual))
                {

                    System.IO.File.Delete(rutaVirtual);
                }

                string rutaVirtualXML = ruta + oFile.FileName;
                oFile.SaveAs(rutaVirtualXML);

                System.IO.File.Move(rutaVirtualXML, rutaVirtual);



                SLDocument sl = new SLDocument(rutaVirtual);

                //LibroMayor lbm = db.LibroMayor.Where(x => x.Estado == true).SingleOrDefault();
                List<LibroMayor> lst = new List<LibroMayor>();


                //var tabla = new DataTable();
                //tabla.Columns.Add("Sociedad");
                //tabla.Columns.Add("Centro");

                DataTable dt = new DataTable();
                dt.Columns.Add("Texto_cab_documento", typeof(string));
                dt.Columns.Add("Lib_mayor", typeof(string));
                dt.Columns.Add("Soc", typeof(string));
                dt.Columns.Add("Fecha_Compensacion", typeof(string));
                dt.Columns.Add("Centro", typeof(string));
                dt.Columns.Add("Asignacion", typeof(string));
                dt.Columns.Add("Referencia", typeof(string));
                dt.Columns.Add("N_Documento", typeof(string));
                dt.Columns.Add("Div", typeof(string));
                dt.Columns.Add("Clase", typeof(string));
                dt.Columns.Add("Fecha_Documento", typeof(DateTime));
                dt.Columns.Add("CT", typeof(int));
                dt.Columns.Add("Importe_MD", typeof(decimal));
                dt.Columns.Add("Importe_ML", typeof(decimal));
                dt.Columns.Add("ML", typeof(string));
                dt.Columns.Add("IndicadorIMP", typeof(string));
                dt.Columns.Add("Doc_Compensacion", typeof(string));
                dt.Columns.Add("Usuario", typeof(string));
                dt.Columns.Add("Descripcion", typeof(string));
                dt.Columns.Add("Id", typeof(Guid));
                dt.Columns.Add("Estado", typeof(bool));


                using (var context = db.Database.BeginTransaction())
                {
                    //var sencillo = new Sencillos();
                    //sencillo.Id = Guid.NewGuid();
                    //sencillo.Fecha = DateTime.Now;

                    int row = 8;
                    while (!string.IsNullOrEmpty(sl.GetCellValueAsString(row, 6)))
                    {
                        columna = row;


                        LibroMayor detalle = new LibroMayor();

                        //MontoSencillo montoSencillo = new MontoSencillo();

                     

                        detalle.Id = Guid.NewGuid();
                        detalle.Texto_cab_documento = sl.GetCellValueAsString(row, 4);
                        detalle.Lib_mayor = sl.GetCellValueAsString(row, 6);
                        detalle.Soc = sl.GetCellValueAsString(row, 7);
                        detalle.Fecha_Compensacion = !string.IsNullOrEmpty(sl.GetCellValueAsString(row, 8)) ? sl.GetCellValueAsDateTime(row, 8) : detalle.Fecha_Compensacion;
                        detalle.Centro = sl.GetCellValueAsString(row, 9);
                        detalle.Asignacion = sl.GetCellValueAsString(row, 10);
                        detalle.Referencia = sl.GetCellValueAsString(row, 11);
                        detalle.N_Documento = sl.GetCellValueAsString(row, 12);
                        detalle.Div = sl.GetCellValueAsString(row, 13);
                        detalle.Clase = sl.GetCellValueAsString(row, 14);
                        detalle.Fecha_Documento = !string.IsNullOrEmpty(sl.GetCellValueAsString(row, 15)) ? DateTime.Parse(sl.GetCellValueAsString(row, 15).Replace(".", "-")) : detalle.Fecha_Documento;
                        detalle.CT = int.Parse(sl.GetCellValueAsString(row, 16));
                        detalle.Importe_MD = !string.IsNullOrEmpty(sl.GetCellValueAsString(row, 17)) ? decimal.Parse(sl.GetCellValueAsString(row, 17)) : 0;
                        detalle.Importe_ML = !string.IsNullOrEmpty(sl.GetCellValueAsString(row, 18)) ? decimal.Parse(sl.GetCellValueAsString(row, 18)) : 0;
                        detalle.ML = sl.GetCellValueAsString(row, 19);
                        detalle.IndicadorIMP = sl.GetCellValueAsString(row, 20);
                        detalle.Doc_Compensacion = sl.GetCellValueAsString(row, 21);
                        detalle.Usuario = sl.GetCellValueAsString(row, 22);
                        detalle.Descripcion = sl.GetCellValueAsString(row, 23);
                        detalle.Estado = false;

                        //db.DetalleLibroMayor.Add(detalle);

                        lst.Add(detalle);


                        row++;
                        if (string.IsNullOrEmpty(sl.GetCellValueAsString(row, 6)))
                        {
                            if (!string.IsNullOrEmpty(sl.GetCellValueAsString(row + 4, 6)))
                            {
                                row = row + 6;
                            }
                        }
                    }
                    List<string> allowedClasses = new List<string> { "FI", "ZQ" };
                    foreach (var detalle in lst.Where(x => allowedClasses.Contains(x.Clase)))
                    {
                        dt.Rows.Add(
                         detalle.Texto_cab_documento,
                         detalle.Lib_mayor,
                         detalle.Soc,
                         detalle.Fecha_Compensacion,
                         detalle.Centro,
                         detalle.Asignacion,
                         detalle.Referencia,
                         detalle.N_Documento,
                         detalle.Div,
                         detalle.Clase,
                         detalle.Fecha_Documento,
                         detalle.CT,
                         detalle.Importe_MD,
                         detalle.Importe_ML,
                         detalle.ML,
                         detalle.IndicadorIMP,
                         detalle.Doc_Compensacion,
                         detalle.Usuario,
                         detalle.Descripcion,
                         detalle.Id,
                         detalle.Estado

                         );

                    }

                   

                    var con = Helper.ConexionStringModel();
                    using (SqlConnection sql = new SqlConnection(con))
                    {
                        sql.Open();
                        //_logger.LogInformation("Apertura de Conexion BD SisGes");

                        using (SqlCommand cmd = new SqlCommand("spInser_detallelibromayor", sql))
                        {
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@TAU", dt);
                            //cmd.Parameters.AddWithValue("@TAU", null);


                            //parametros.SqlDbType = SqlDbType.Structured;
                            var reader1 = cmd.ExecuteNonQuery();
                        }
                        sql.Close();
                    }


                }



                response.respuesta = "Se cargo Correctamente";
                response.error = false;
                return response;


            }
            catch (Exception ex)
            {
                columna = columna;
                response.error = true;
                response.respuesta = "Problemas al cargar el libro de Venta" + ex.Message;
                return response;
            }
            //return response;
        }

        public ResponseModel ConciliacionAutomatica()
        {
            ResponseModel response = new ResponseModel();
            try
            {
                db.spInsertConciliacionAuto_LibMayor();
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

        public List<LibroMayorDEP> Conciliadas(int com)
        {
            List<LibroMayorDEP> lst = new List<LibroMayorDEP>();
            if (com == 1)
            {
                lst = db.LibroMayorDEP.Where(x=>x.Estado.Equals("CA")).ToList();
            }
            else
            {
                lst = db.LibroMayorDEP.Where(x => x.Estado.Equals("CM")).ToList();
            }
            
            return lst;
        }

        public List<LibroMayor> NoConciliadas()
        {
            List<LibroMayor> lst = new List<LibroMayor>();
            lst = db.LibroMayor.Where(x => x.Estado == false).ToList();
            return lst;
        }

        public ConciliacionEFOManualView ConciliacionManual()
        {
            ConciliacionEFOManualView manual = new ConciliacionEFOManualView();

            manual.ListaFI = db.LibroMayor.Where(x => x.Clase.Equals("FI") &&  x.Estado == false).ToList();
            manual.ListaZQ = db.LibroMayor.Where(x => x.Clase.Equals("ZQ") && x.Estado == false).ToList();

            return manual;
        }

        public MatchConManualEFOView MontosZQ(Guid id )
        {
            MatchConManualEFOView oEFO = new MatchConManualEFOView();
            oEFO.FI = db.LibroMayor.Where(x => x.Id == id).SingleOrDefault();
            oEFO.ListaZQ = db.LibroMayor.Where(x => x.Clase.Equals("ZQ") && x.Estado == false && Math.Abs(x.Importe_ML) == Math.Abs(oEFO.FI.Importe_ML) && x.Fecha_Documento > oEFO.FI.Fecha_Documento).ToList();

            return oEFO;
        }

        public ResponseModel InsertarConciliacionM (Guid Id, Guid Id1)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                using (var context = db.Database.BeginTransaction())
                {
                    LibroMayor Con1 = db.LibroMayor.Where(x => x.Id == Id).SingleOrDefault();
                    LibroMayor Con2 = db.LibroMayor.Where(x => x.Id == Id1).SingleOrDefault();
                    LibroMayorDEP MayorDEP = (new LibroMayorDEP
                    {
                        Texto_cab_documento = Con1.Texto_cab_documento,
                        Soc = Con1.Soc,
                        Asignacion = Con1.Asignacion,
                        Referencia = Con1.Referencia,
                        N_Documento = Con1.N_Documento,
                        Clase = Con1.Clase,
                        Fecha_Documento = Con1.Fecha_Documento,
                        CT = Con1.CT,
                        Importe_MD = Con1.Importe_MD,
                        Importe_ML = Con1.Importe_ML,
                        Doc_Compensacion = Con1.Doc_Compensacion,
                        Usuario = Con1.Usuario,
                        Descripcion = Con1.Descripcion,
                        Lib_mayor = Con1.Lib_mayor,
                        abs = Math.Abs(Con1.Importe_ML),
                        IdLibroMayor = Con1.Id,
                        Texto_cab_documento1 = Con2.Texto_cab_documento,
                        Soc1 = Con2.Soc,
                        Asignacion1 = Con2.Asignacion,
                        Referencia1 = Con2.Referencia,
                        N_Documento1 = Con2.N_Documento,
                        Clase1 = Con2.Clase,
                        Fecha_Documento1 = Con2.Fecha_Documento,
                        CT1 = Con2.CT,
                        Importe_MD1 = Con2.Importe_MD,
                        Importe_ML1 = Con2.Importe_ML,
                        Doc_Compensacion1 = Con2.Doc_Compensacion,
                        Usuario1 = Con2.Usuario,
                        Descripcion1 = Con2.Descripcion,
                        Lib_mayor1 = Con2.Lib_mayor,
                        abs1 = Math.Abs(Con2.Importe_ML),
                        IdLibroMayor1 = Con2.Id,
                        Estado = "CM"


                    });
                    Con1.Estado = true;
                    Con2.Estado = true;
                    db.LibroMayorDEP.Add(MayorDEP);
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
    }
}