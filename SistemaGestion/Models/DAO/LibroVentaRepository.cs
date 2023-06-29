using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Xml;
using System.Data.SqlClient;
using System.Threading.Tasks;
using SistemaGestion.Helpers;
using SistemaGestion.Models.ViewModels;
using SpreadsheetLight;

namespace SistemaGestion.Models.DAO
{
    public class LibroVentaRepository
    {
        private SisGesEntities3 db = new SisGesEntities3();
        private string Ambiente = "SAPFSP";
        private string con = Helper.ConexionStringModel();

        #region Carga de libro venta Manual
        //Carga Libro Venta 
        public ResponseModel LibroVenta(string Archivo = "lv.csv")
        {
            var response = new ResponseModel();
            try
            {
                var context = new SisGesEntities3();
                //var Ruta = Archivo;
                //string Archivo = string.Empty;
                //Archivo = string.Format("c:/carga/{0}", Archivo);
                string Prueba = string.Format("exec dbo.Proc_IMPORTAR_LIBRO_VENTA_SAP @P_Archivo ='{0}'", Archivo);
                context.Database.ExecuteSqlCommand(Prueba);
                response.respuesta = "Se cargo Correctamente";
                response.error = true;
                return response;

            }
            catch (Exception ex)
            {
                response.error = false;
                response.respuesta = "Error al cargar el libro de venta" + ex.Message;
                return response;
            }

        }

        #endregion

        #region Generacion de Idoc Para Reproceso

        //public ResponseModel GeneratNota(string Fecha, int local, int Folio, string TipoDocumento)
        //{
        //    var response = new ResponseModel();
        //    try
        //    {
        //        string pa = string.Empty;
        //        var context = new SisGesEntities3();

        //        Fecha = string.Format("{0}", Fecha);
        //        DateTime txtFechaCalendar = DateTime.Parse(Fecha);
        //        Fecha = txtFechaCalendar.ToString("yyyyMMdd");

        //        using (SqlConnection cn = new SqlConnection(con))
        //        {
        //            cn.Open();
        //            SqlCommand cmd = new SqlCommand("Proc_leerNCFC_Ind", cn);
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.Parameters.AddWithValue("@Tienda_I", local);
        //            cmd.Parameters.AddWithValue("@Fecha_I", Fecha);
        //            cmd.Parameters.AddWithValue("@Ambiente", "SAPFSP");
        //            cmd.Parameters.AddWithValue("@NDoc", Folio);
        //            cmd.Parameters.AddWithValue("@TipoDoc", TipoDocumento);
        //            cmd.CommandTimeout = 1000;
        //            cmd.ExecuteReader();



        //        }

        //        context.Database.CommandTimeout = 1000;
        //        context.Proc_LeerNCFC(local, Fecha, Ambiente);
        //        //context.Database.ExecuteSqlCommand


        //        response.respuesta = "Se cargo Correctamente";
        //        response.error = true;
        //        return response;

        //    }
        //    catch (Exception ex)
        //    {
        //        response.error = false;
        //        response.respuesta = "Procedimiento Leer NCFC no se ejecuto correctamente " + ex.Message;
        //        return response;
        //    }



        //}

        //public ResponseModel ImprimirNota(string Fecha, int local)
        //{
        //    var response = new ResponseModel();
        //    try
        //    {
        //        var context = new SisGesEntities3();
        //        //var Ruta = Archivo;
        //        //string Archivo = string.Empty;
        //        Fecha = string.Format("{0}", Fecha);
        //        DateTime txtFechaCalendar = DateTime.Parse(Fecha);
        //        Fecha = txtFechaCalendar.ToString("yyyyMMdd");

        //        context.Database.CommandTimeout = 1000;
        //        context.Proc_GenIdocNCFC(local, Fecha);
        //        response.respuesta = "Se cargo Correctamente";
        //        response.error = true;
        //        return response;

        //    }
        //    catch (Exception ex)
        //    {
        //        response.error = false;
        //        response.respuesta = "Proceso de Generar Nota no se genero  " + ex.Message;
        //        return response;
        //    }



        //}




        //public ResponseModel IdocClientes(string Fecha, int local)
        //{
        //    var response = new ResponseModel();
        //    try
        //    {
        //        var context = new SisGesEntities3();
        //        Fecha = string.Format("{0}", Fecha);
        //        DateTime txtFechaCalendar = DateTime.Parse(Fecha);
        //        Fecha = txtFechaCalendar.ToString("yyyyMMdd");
        //        //string Prueba = string.Format("exec Proc_GENERAR_IDOC_X_BCPL @I_Tienda={0},@C_Fecha ='{1}'", local, Fecha);
        //        //context.Database.ExecuteSqlCommand(Prueba);
        //        context.Database.CommandTimeout = 1000;
        //        context.Proc_IdocClientes2(Fecha, local, Ambiente);
        //        response.respuesta = "Se cargo Correctamente";
        //        response.error = true;
        //        return response;

        //    }
        //    catch (Exception ex)
        //    {
        //        response.error = false;
        //        response.respuesta = "Proceso Idoc Clientes No se genero!" + ex.Message;
        //        return response;
        //    }



        //}

        ////Imprime Idoc Clientes Global
        //public ResponseModel GeneraBcpClienteIdoc()
        //{
        //    var response = new ResponseModel();
        //    try
        //    {
        //        var context = new SisGesEntities3();
        //        context.Proc_GenIdocCLI();
        //        response.respuesta = "Se cargo Correctamente";
        //        response.error = true;
        //        return response;
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }

        //}

        //Procesa idoc Clientes x Rut
        public ResponseModel LeeIdocClientesXRut(string rut)
        {
            var response = new ResponseModel();
            try
            {
                var context = new SisGesEntities3();
                context.Proc_LeerClientes_Indiv(rut, Ambiente);
                context.Database.CommandTimeout = 180;
                response.respuesta = "Se cargo Correctamente";
                response.error = true;
                return response;
            }
            catch (Exception ex)
            {
                response.error = false;
                response.respuesta = "El rut Ingresado no existe.. " + ex.Message;
                return response;
            }
        }

        //Imprime idoc Clientes x Rut
        public ResponseModel GeneraIdocClientes()
        {
            var response = new ResponseModel();
            try
            {
                var context = new SisGesEntities3();
                context.Proc_GenIdocClientes_Indiv();
                response.respuesta = "Se Genero Correctamente";
                response.error = true;
                return response;
            }
            catch (Exception ex)
            {
                response.error = false;
                response.respuesta = "El rut Ingresado no existe.. " + ex.Message;
                return response;
            }
        }




        #endregion

        #region Reproceso En Produccion
        //Genera notas de credito por lista de folio
        public ResponseModel ListaFoliosNCFC(List<IdocNCFCView> lst)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                var lineafolio = "";
                var lineaRut = "";
                var contador = 1;
                int centro = 0;
                string TipoDoc = string.Empty;
                DateTime FechaVenta = DateTime.Today;
                foreach (var item in lst.Where(x => x.isCheck == true))
                {
                    if (lst.Where(x => x.isCheck == true).Count() != contador)
                    {
                        lineafolio += item.Folio.ToString() + ",";
                        lineaRut += item.Rut.ToString() + ",";
                    }
                    else
                    {
                        lineafolio += item.Folio.ToString() + "";
                        lineaRut += item.Rut.ToString() + "";
                        centro = item.Centro;
                        FechaVenta = item.FechaVenta;
                        TipoDoc = item.TipoDoc;
                    }


                    contador++;
                }

                using (SqlConnection cn = new SqlConnection(con))
                {
                    cn.Open();



                    //Generamos la data de NCFC
                    SqlCommand cmd = new SqlCommand("Proc_leerNCFC_IND", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Tienda_I", centro);
                    cmd.Parameters.AddWithValue("@Fecha_I", FechaVenta.ToString("yyyyMMdd"));
                    cmd.Parameters.AddWithValue("@Ambiente", Ambiente);
                    cmd.Parameters.AddWithValue("@NDoc", lineafolio);
                    cmd.Parameters.AddWithValue("@TipoDoc", TipoDoc);
                    cmd.CommandTimeout = 1000;
                    cmd.ExecuteReader();
                    cn.Close();



                    cn.Open();

                    //Creamos el TXT
                    cmd = new SqlCommand("Proc_GenIdocNCFC_Indiv", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Tienda_I", centro);
                    cmd.Parameters.AddWithValue("@Fecha_I", FechaVenta.ToString("yyyyMMdd"));
                    //cmd.Parameters.AddWithValue("@NDoc", int.Parse(item.Folio));
                    cmd.CommandTimeout = 1000;
                    cmd.ExecuteReader();
                    cn.Close();


                    //Generamos la data de los clientes
                    cn.Open();
                    cmd = new SqlCommand("Proc_LeerClientes_Indiv", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Rut", lineaRut);
                    cmd.Parameters.AddWithValue("@Ambiente", Ambiente);
                    cmd.CommandTimeout = 1000;
                    cmd.ExecuteReader();
                    cn.Close();


                    //Creamos los txt de los clientes
                    cn.Open();
                    cmd = new SqlCommand("Proc_GenIdocClientes_Indiv", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 1000;
                    cmd.ExecuteReader();

                    cn.Close();

                }




                response.respuesta = "Se Genero Correctamente";
                response.error = true;
                return response;
            }
            catch (Exception ex)
            {
                response.error = false;
                response.respuesta = "Idoc no se genero " + ex.Message;
                return response;
            }
        }

        public ResponseModel GENERAR_VENTAS_IDOC(string Fecha, int local)
        {
            var response = new ResponseModel();
            //var con = "data source=172.18.14.96;initial catalog=sisges;user id=sa;password=ZxJgMuaD9b;MultipleActiveResultSets=True;";

            Fecha = string.Format("{0}", Fecha);
            DateTime txtFechaCalendar = DateTime.Parse(Fecha);
            Fecha = txtFechaCalendar.ToString("yyyyMMdd");
            try
            {
                using (SqlConnection cn = new SqlConnection(con))
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("Proc_IDOC_VENTA", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@I_Tienda", local);
                    cmd.Parameters.AddWithValue("@C_Fecha", Fecha);
                    cmd.Parameters.AddWithValue("@Ambiente", "SAPFSP");
                    cmd.CommandTimeout = 1000;
                    cmd.ExecuteReader();



                }


                response.respuesta = "Se cargo Correctamente";
                response.error = true;
                return response;


            }
            catch (Exception ex)
            {
                response.error = false;
                response.respuesta = "Proceso Generar Ventas Idoc No se Genero " + ex.Message;
                return response;
            }



        }

        public ResponseModel GENERAR_IDOC_X_BCP()
        {
            var response = new ResponseModel();
            try
            {

                using (SqlConnection cn = new SqlConnection(con))
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("Proc_IDOC_X_BCP_IND", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.CommandTimeout = 1000;
                    cmd.ExecuteReader();



                }


                response.respuesta = "Se cargo Correctamente";
                response.error = true;
                return response;

            }
            catch (Exception ex)
            {
                response.error = false;
                response.respuesta = "Generar Idoc X BCP No se Genero" + ex.InnerException.Message;
                return response;
            }



        }

        #endregion

        #region Clases para la vistas
        public List<Proc_ListaDetalleVenta_Result> DetalleVenta(string local, string documento, DateTime Fecha)
        {
            var f = string.Format("dd-MM-yyyy");
            var fechaCon = Fecha.ToString(f);

            if (documento != "")
            {
                documento = documento.Replace("TODOS", "");
            }

            List<Proc_ListaDetalleVenta_Result> lst = new List<Proc_ListaDetalleVenta_Result>();

            lst = (from p in db.Proc_ListaDetalleVenta(fechaCon, local, documento)
                   select new Proc_ListaDetalleVenta_Result
                   {
                       Centro = p.Centro,
                       Caja = p.Caja,
                       Fecha = p.Fecha,
                       Folio_Documento = p.Folio_Documento,
                       Tipo_Documento = p.Tipo_Documento,
                       Total = p.Total,


                   }).ToList();


            return lst;
        }

        public List<Proc_ListaLibroVenta_Result> LibroVentaSap(string local, DateTime Fecha, string documento = "")
        {
            var formato = ("dd-MM-yyyy");
            var FechaString = Fecha.ToString(formato);
            if (local == "T000")
            {
                local = local.Replace("T000", "");
            }
            List<Proc_ListaLibroVenta_Result> lst = new List<Proc_ListaLibroVenta_Result>();
            lst = (from d in db.Proc_ListaLibroVenta(FechaString, local, documento)
                   select d).ToList();

            return lst;
        }

        public List<Proc_ListaIdocVenta_Result> IdocdeVentas(DateTime Fecha, string local)
        {
            List<Proc_ListaIdocVenta_Result> lst = new List<Proc_ListaIdocVenta_Result>();
            var formato = ("dd-MM-yyyy");
            var FechaString = Fecha.ToString(formato);
            if (local == "T000")
            {
                local = local.Replace("T000", "");
            }

            lst = (from e in db.Proc_ListaIdocVenta(FechaString, local)
                   select e).ToList();
            return lst;

        }


        public ResponseModel CargaLibroVenta(HttpPostedFileBase oFile, string ruta)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                //string path = @"C:\File\lv.xml";
                //string pathFisico = path.Replace(@"\\", @"/");
                string rutaVirtual = ruta + "lv.xml";
                if (!System.IO.Directory.Exists(ruta))
                {
                    System.IO.Directory.CreateDirectory(ruta);
                }

                if (System.IO.File.Exists(rutaVirtual))
                {
                    var fechaHoy = DateTime.Now;
                    string nombreArchivoAntiguo = fechaHoy.Day.ToString() + "-" + fechaHoy.Month.ToString() + "-" + fechaHoy.Year.ToString() + "-" + fechaHoy.Hour.ToString() + fechaHoy.Minute.ToString() + fechaHoy.Second.ToString() + ".XML";
                    var rutaAntiguo = ruta + "Antiguo\\" + nombreArchivoAntiguo;
                    if (!System.IO.Directory.Exists(ruta + "Antiguo"))
                    {
                        System.IO.Directory.CreateDirectory(ruta + "Antiguo");
                    }
                    System.IO.File.Move(rutaVirtual, rutaAntiguo);
                    //System.IO.File.Delete(rutaVirtual);
                }

                string rutaVirtualXML = ruta + oFile.FileName;
                oFile.SaveAs(rutaVirtualXML);

                System.IO.File.Move(rutaVirtualXML, rutaVirtual);

                XmlDocument sl = new XmlDocument();
                //sl.Load(path);
                sl.Load(rutaVirtual);

                var tabla = new DataTable();
                tabla.Columns.Add("Sociedad");
                tabla.Columns.Add("Centro");
                List<LIBRO_VENTA> lstLibro = new List<LIBRO_VENTA>();
                DataTable dt = new DataTable();
                dt.Columns.Add("Sociedad", typeof(string));
                dt.Columns.Add("Centro", typeof(string));
                dt.Columns.Add("ClaseDocumental", typeof(string));
                dt.Columns.Add("NumeroDocto", typeof(string));
                dt.Columns.Add("FechaContabilizacion", typeof(string));
                dt.Columns.Add("Referencia", typeof(string));
                dt.Columns.Add("FechaDocumento", typeof(string));
                dt.Columns.Add("IndicadorImpuesto", typeof(string));
                dt.Columns.Add("NombreCliente", typeof(string));
                dt.Columns.Add("NumeroIdentFiscal", typeof(string));
                dt.Columns.Add("Exento", typeof(decimal));
                dt.Columns.Add("Afecto", typeof(decimal));
                dt.Columns.Add("Iva", typeof(decimal));
                dt.Columns.Add("BaseImponibleMP", typeof(decimal));

                int countValidador = 1;

                foreach (XmlNode item in sl.DocumentElement.ChildNodes[4].ChildNodes[0].ChildNodes)
                {
                    if (item.LocalName == "Row")
                    {
                        if (countValidador == 1)
                        {
                            if (item.ChildNodes[0].InnerText != "Sociedad")
                            {
                                response.error = true;
                                response.respuesta = "EL ARCHIVO CARGADO NO ES EL CORRECTO FAVOR REVISAR";
                                return response;
                            }

                        }

                        if (item.ChildNodes[0].InnerText != "Sociedad")
                        {
                            if (item.ChildNodes[0].InnerText != "")
                            {
                                LIBRO_VENTA libro = new LIBRO_VENTA();
                                var fechaFontabilizacion = Convert.ToDateTime(item.ChildNodes[4].InnerText).ToShortDateString();
                                var fechaDocumento = Convert.ToDateTime(item.ChildNodes[6].InnerText).ToShortDateString();
                                //var prueba = item.Attributes["ss:Name"].Value;
                                libro.Sociedad = item.ChildNodes[0].InnerText;
                                libro.Centro = item.ChildNodes[1].InnerText;
                                libro.ClaseDocumental = item.ChildNodes[2].InnerText;
                                libro.NumeroDocto = item.ChildNodes[3].InnerText;
                                libro.FechaContabilizacion = fechaFontabilizacion.Replace("-", ".");
                                //libro.FechaContabilizacion = item.ChildNodes[4].InnerText.Substring(8, 9) + "." + item.ChildNodes[4].InnerText.Substring(5, 6) + "." + item.ChildNodes[4].InnerText.Substring(0, 3);
                                libro.Referencia = item.ChildNodes[5].InnerText;
                                //libro.FechaDocumento = item.ChildNodes[6].InnerText.Replace("-","."); 
                                libro.FechaDocumento = fechaDocumento.Replace("-", ".");
                                libro.IndicadorImpuesto = item.ChildNodes[7].InnerText;
                                libro.NombreCliente = item.ChildNodes[8].InnerText;
                                libro.NumeroIdentFiscal = item.ChildNodes[9].InnerText;
                                libro.Exento = Convert.ToDecimal(item.ChildNodes[10].InnerText.Replace(".", ","));
                                libro.Afecto = Convert.ToDecimal(item.ChildNodes[11].InnerText.Replace(".", ","));
                                libro.Iva = Convert.ToDecimal(item.ChildNodes[12].InnerText.Replace(".", ","));
                                libro.BaseImponibleMP = Convert.ToDecimal(item.ChildNodes[13].InnerText.Replace(".", ","));
                                lstLibro.Add(libro);
                                dt.Rows.Add(
                                    item.ChildNodes[0].InnerText,
                                    item.ChildNodes[1].InnerText,
                                    item.ChildNodes[2].InnerText,
                                    item.ChildNodes[3].InnerText,
                                    libro.FechaContabilizacion,
                                    item.ChildNodes[5].InnerText,
                                    libro.FechaDocumento,
                                    item.ChildNodes[7].InnerText,
                                    item.ChildNodes[8].InnerText,
                                    item.ChildNodes[9].InnerText,
                                    Convert.ToDecimal(item.ChildNodes[10].InnerText.Replace(".", ",")),
                                    Convert.ToDecimal(item.ChildNodes[11].InnerText.Replace(".", ",")),
                                    Convert.ToDecimal(item.ChildNodes[12].InnerText.Replace(".", ",")),
                                    Convert.ToDecimal(item.ChildNodes[13].InnerText.Replace(".", ","))
                                    );
                            }
                        }
                    }

                    countValidador++;
                }

                var eliminar = lstLibro.Select(x => x.Centro).Distinct().ToList();
                var fechaelimianr = lstLibro.Select(x => x.FechaContabilizacion).Distinct().ToList();

                var datosaeliminar = db.LIBRO_VENTA.Where(x => eliminar.Contains(x.Centro) && fechaelimianr.Contains(x.FechaContabilizacion)).ToList();

                db.LIBRO_VENTA.RemoveRange(datosaeliminar);

                //foreach (var item in datosaeliminar)
                //{
                //    db.LIBRO_VENTA.Remove(item);                    
                //}
                db.SaveChanges();

                //while (!string.IsNullOrEmpty(sl.GetCellValueAsString(row, 3)))
                //{
                //    tabla.Rows.Add(new object[]
                //        {
                //            sl.GetCellValueAsString(row, 3),
                //            sl.GetCellValueAsString(row, 4)
                //        });
                //    string prueba1 = sl.GetCellValueAsString(row, 3);
                //    string prueba2 = sl.GetCellValueAsString(row, 4);
                //    string prueba3 = sl.GetCellValueAsString(row, 6);
                //    string prueba4 = sl.GetCellValueAsString(row, 7);
                //    string prueba5 = sl.GetCellValueAsString(row, 8);
                //    string prueba6 = sl.GetCellValueAsString(row, 9);
                //    string prueba7 = sl.GetCellValueAsString(row, 10);
                //    string prueba8 = sl.GetCellValueAsString(row, 11);
                //    string prueba9 = sl.GetCellValueAsString(row, 12);
                //    row++;
                //}
                var parametros = new SqlParameter("@TAU", SqlDbType.Structured);
                parametros.Value = dt;
                parametros.TypeName = "dbo.TypeLibroVenta";
                db.Database.ExecuteSqlCommand("EXEC spInsertLibroVentas @TAU", parametros);

                response.respuesta = "Se cargo Correctamente";
                response.error = false;
                return response;


            }
            catch (Exception ex)
            {
                response.error = true;
                response.respuesta = "Problemas al cargar el libro de Venta" + ex.Message;
                return response;
            }
            //return response;
        }

        public ResponseModel CargaLibroVentaXLS(HttpPostedFileBase oFile, string ruta)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                string rutaVirtual = ruta + "lv.xlsx";
                if (!System.IO.Directory.Exists(ruta))
                {
                    System.IO.Directory.CreateDirectory(ruta);
                }

                if (System.IO.File.Exists(rutaVirtual))
                {
                    var fechaHoy = DateTime.Now;
                    string nombreArchivoAntiguo = fechaHoy.Day.ToString() + "-" + fechaHoy.Month.ToString() + "-" + fechaHoy.Year.ToString() + "-" + fechaHoy.Hour.ToString() + fechaHoy.Minute.ToString() + fechaHoy.Second.ToString() + ".xlsx";
                    var rutaAntiguo = ruta + "Antiguo\\" + nombreArchivoAntiguo;
                    if (!System.IO.Directory.Exists(ruta + "Antiguo"))
                    {
                        System.IO.Directory.CreateDirectory(ruta + "Antiguo");
                    }
                    System.IO.File.Move(rutaVirtual, rutaAntiguo);
                    //System.IO.File.Delete(rutaVirtual);
                }
                string rutaVirtualXML = ruta + oFile.FileName;

                //string rutaVirtualXML = ruta + oFile.FileName;
                oFile.SaveAs(rutaVirtualXML);
                SLDocument sl = new SLDocument(rutaVirtualXML);
                if (!sl.GetCellValueAsString(9, 3).Equals("Soc."))
                {
                    System.IO.File.Delete(rutaVirtualXML);
                    response.error = true;
                    response.respuesta = "El formato subido no es correcto ";
                    return response;
                }


                System.IO.File.Move(rutaVirtualXML, rutaVirtual);


                sl = new SLDocument(rutaVirtual);


                var tabla = new DataTable();
                tabla.Columns.Add("Sociedad");
                tabla.Columns.Add("Centro");
                List<LIBRO_VENTA> lstLibro = new List<LIBRO_VENTA>();
                DataTable dt = new DataTable();
                dt.Columns.Add("Sociedad", typeof(string));
                dt.Columns.Add("Centro", typeof(string));
                dt.Columns.Add("ClaseDocumental", typeof(string));
                dt.Columns.Add("NumeroDocto", typeof(string));
                dt.Columns.Add("FechaContabilizacion", typeof(string));
                dt.Columns.Add("Referencia", typeof(string));
                dt.Columns.Add("FechaDocumento", typeof(string));
                dt.Columns.Add("IndicadorImpuesto", typeof(string));
                dt.Columns.Add("NombreCliente", typeof(string));
                dt.Columns.Add("NumeroIdentFiscal", typeof(string));
                dt.Columns.Add("Exento", typeof(decimal));
                dt.Columns.Add("Afecto", typeof(decimal));
                dt.Columns.Add("Iva", typeof(decimal));
                dt.Columns.Add("BaseImponibleMP", typeof(decimal));

                int countValidador = 1;
                int row = 11;

                while (!string.IsNullOrEmpty(sl.GetCellValueAsString(row, 3)))
                {

                    LIBRO_VENTA libro = new LIBRO_VENTA();
                    var fechaFontabilizacion = Convert.ToDateTime(sl.GetCellValueAsString(row, 8)).ToShortDateString();
                    var fechaDocumento = Convert.ToDateTime(sl.GetCellValueAsString(row, 10)).ToShortDateString();
                    //var prueba = item.Attributes["ss:Name"].Value;
                    libro.Sociedad = sl.GetCellValueAsString(row, 3);
                    libro.Centro = sl.GetCellValueAsString(row, 4);
                    libro.ClaseDocumental = sl.GetCellValueAsString(row, 6);
                    libro.NumeroDocto = sl.GetCellValueAsString(row, 7);
                    libro.FechaContabilizacion = fechaFontabilizacion.Replace("-", ".");
                    //libro.FechaContabilizacion = item.ChildNodes[4].InnerText.Substring(8, 9) + "." + item.ChildNodes[4].InnerText.Substring(5, 6) + "." + item.ChildNodes[4].InnerText.Substring(0, 3);
                    libro.Referencia = sl.GetCellValueAsString(row, 9);
                    //libro.FechaDocumento = item.ChildNodes[6].InnerText.Replace("-","."); 
                    libro.FechaDocumento = fechaDocumento.Replace("-", ".");
                    libro.IndicadorImpuesto = sl.GetCellValueAsString(row, 11);
                    libro.NombreCliente = sl.GetCellValueAsString(row, 12);
                    libro.NumeroIdentFiscal = sl.GetCellValueAsString(row, 13);
                    libro.Exento = Convert.ToDecimal(sl.GetCellValueAsString(row, 14).Replace(".", ","));
                    libro.Afecto = Convert.ToDecimal(sl.GetCellValueAsString(row, 15).Replace(".", ","));
                    libro.Iva = Convert.ToDecimal(sl.GetCellValueAsString(row, 16).Replace(".", ","));
                    libro.BaseImponibleMP = Convert.ToDecimal(sl.GetCellValueAsString(row, 17).Replace(".", ","));
                    lstLibro.Add(libro);
                    dt.Rows.Add(
                         libro.Sociedad,
                         libro.Centro,
                         libro.ClaseDocumental,
                         libro.NumeroDocto,
                         libro.FechaContabilizacion,
                         libro.Referencia,
                         libro.FechaDocumento,
                         libro.IndicadorImpuesto,
                         libro.NombreCliente,
                         libro.NumeroIdentFiscal,
                         libro.Exento,
                         libro.Afecto,
                         libro.Iva,
                         libro.BaseImponibleMP
                         );
                    row++;
                    countValidador++;
                }

                var eliminar = lstLibro.Select(x => x.Centro).Distinct().ToList();
                var fechaelimianr = lstLibro.Select(x => x.FechaContabilizacion).Distinct().ToList();

                var datosaeliminar = db.LIBRO_VENTA.Where(x => eliminar.Contains(x.Centro) && fechaelimianr.Contains(x.FechaContabilizacion)).ToList();

                db.LIBRO_VENTA.RemoveRange(datosaeliminar);


                db.SaveChanges();


                var parametros = new SqlParameter("@TAU", SqlDbType.Structured);
                parametros.Value = dt;
                parametros.TypeName = "dbo.TypeLibroVenta";
                db.Database.ExecuteSqlCommand("EXEC spInsertLibroVentas @TAU", parametros);

                response.respuesta = "Se cargo Correctamente";
                response.error = false;
                return response;

            }
            catch (Exception ex)
            {
                response.error = true;
                response.respuesta = "Ocurrio un error " + ex.Message;
                return response;

            }
        }

        public ResponseModel CargaCartaInstruccion(CargaSencilloView SencilloT, string ruta)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                //string path = @"C:\File\lv.xml";
                //string pathFisico = path.Replace(@"\\", @"/");
                string rutaVirtual = ruta + "CartaInstruccion\\CartaInstruccion.xlsx";
                ruta = ruta + "CartaInstruccion\\";
                if (!System.IO.Directory.Exists(ruta))
                {
                    System.IO.Directory.CreateDirectory(ruta);
                }

                if (System.IO.File.Exists(rutaVirtual))
                {
                    //var fechaHoy = DateTime.Now;
                    //string nombreArchivoAntiguo = fechaHoy.Day.ToString() + "-" + fechaHoy.Month.ToString() + "-" + fechaHoy.Year.ToString() + "-" + fechaHoy.Hour.ToString() + fechaHoy.Minute.ToString() + fechaHoy.Second.ToString() + ".XML";
                    //var rutaAntiguo = ruta + "Antiguo\\" + nombreArchivoAntiguo;
                    //if (!System.IO.Directory.Exists(ruta + "Antiguo"))
                    //{
                    //    System.IO.Directory.CreateDirectory(ruta + "Antiguo");
                    //}
                    //System.IO.File.Move(rutaVirtual, rutaAntiguo);
                    //System.IO.File.Delete(rutaVirtual);
                    System.IO.File.Delete(rutaVirtual);
                }

                string rutaVirtualXML = ruta + SencilloT.oFile.FileName;
                SencilloT.oFile.SaveAs(rutaVirtualXML);

                System.IO.File.Move(rutaVirtualXML, rutaVirtual);

                SLDocument sl = new SLDocument(rutaVirtual);


                using (var context = db.Database.BeginTransaction())
                {
                    //int? ultimocorrelativo = db.Sencillos.Where(x=>x.Fecha.Value.Year == DateTime.Now.Year).OrderByDescending(x => x.Correlativo).Select(x => x.Correlativo).FirstOrDefault();
                    var ultimosencillo = db.Sencillos.Where(x => x.Fecha.Value.Year == DateTime.Now.Year).OrderByDescending(x => x.Correlativo).FirstOrDefault();
                    // Sencillo s = db.
                    var sencillo = new Sencillos();
                    sencillo.Id = Guid.NewGuid();
                    sencillo.Fecha = DateTime.Now;
                    sencillo.FechaInicio = SencilloT.Inicio;
                    sencillo.FechaFin = SencilloT.Fin;

                    if (ultimosencillo == null)
                    {
                        sencillo.Correlativo = 1;
                    }
                    else
                    {
                        if (ultimosencillo.Fecha.Value.Year < sencillo.Fecha.Value.Year)
                        {
                            sencillo.Correlativo = 1;
                        }
                        else
                        {
                            sencillo.Correlativo = ultimosencillo.Correlativo + 1;
                        }
                    }
                    
                  
                  
                    int row = 2;
                    while (!string.IsNullOrEmpty(sl.GetCellValueAsString(row, 1)))
                    {

                        DetalleSencillo detalleSencillo = new DetalleSencillo();
                        MontoSencillo montoSencillo = new MontoSencillo();

                        detalleSencillo.Id = Guid.NewGuid();
                        detalleSencillo.IdSencillo = sencillo.Id;
                        detalleSencillo.IdTienda = int.Parse(sl.GetCellValueAsString(row, 1));
                        detalleSencillo.Banco = sl.GetCellValueAsString(row, 2);


                        montoSencillo.Id = Guid.NewGuid();
                        montoSencillo.Monto1 = !string.IsNullOrEmpty(sl.GetCellValueAsString(row, 4)) ? int.Parse(sl.GetCellValueAsString(row, 4)) : 0;
                        montoSencillo.Monto2 = !string.IsNullOrEmpty(sl.GetCellValueAsString(row, 5)) ? int.Parse(sl.GetCellValueAsString(row, 5)) : 0;
                        montoSencillo.Monto3 = !string.IsNullOrEmpty(sl.GetCellValueAsString(row, 6)) ? int.Parse(sl.GetCellValueAsString(row, 6)) : 0;
                        montoSencillo.Monto4 = !string.IsNullOrEmpty(sl.GetCellValueAsString(row, 7)) ? int.Parse(sl.GetCellValueAsString(row, 7)) : 0;
                        montoSencillo.Monto5 = !string.IsNullOrEmpty(sl.GetCellValueAsString(row, 8)) ? int.Parse(sl.GetCellValueAsString(row, 8)) : 0;
                        montoSencillo.Monto6 = !string.IsNullOrEmpty(sl.GetCellValueAsString(row, 9)) ? int.Parse(sl.GetCellValueAsString(row, 9)) : 0;
                        montoSencillo.Monto7 = !string.IsNullOrEmpty(sl.GetCellValueAsString(row, 10)) ? int.Parse(sl.GetCellValueAsString(row, 10)) : 0;
                        detalleSencillo.Total = 0;/*int.Parse(sl.GetCellValueAsString(row, 11));*/
                        detalleSencillo.DiaLiberacion = sl.GetCellValueAsString(row, 12);
                        detalleSencillo.DiaEntrega = sl.GetCellValueAsString(row, 13);
                        detalleSencillo.FechaLiberacion = ObtenerFechaPorDiaDeLaSemana(sencillo.FechaInicio.Value, sencillo.FechaFin.Value, detalleSencillo.DiaLiberacion);
                        detalleSencillo.FechaEntrega = ObtenerFechaPorDiaDeLaSemana(sencillo.FechaInicio.Value, sencillo.FechaFin.Value, detalleSencillo.DiaEntrega);
                        detalleSencillo.NuevoTotal = int.Parse(sl.GetCellValueAsString(row, 11));
                        detalleSencillo.ConciliacionTesoreria = false;

                        detalleSencillo.MontoSencillo.Add(montoSencillo);
                        sencillo.DetalleSencillo.Add(detalleSencillo);
                        row++;
                    }

                    db.Sencillos.Add(sencillo);
                    db.SaveChanges();
                    context.Commit();


                }



                response.respuesta = "Se cargo Correctamente";
                response.error = false;
                return response;


            }
            catch (Exception ex)
            {
                response.error = true;
                response.respuesta = "Problemas al cargar el libro de Venta" + ex.Message;
                return response;
            }
            //return response;
        }

        public static DateTime? ObtenerFechaPorDiaDeLaSemana(DateTime fechaInicio, DateTime fechaFin, string diaDeLaSemana)
        {
            if (fechaInicio > fechaFin)
            {
                Console.WriteLine("La fecha de inicio no puede ser mayor que la fecha final.");
                return null;
            }
            diaDeLaSemana = diaDeLaSemana.ToLower();
            Dictionary<string, string> diasDeLaSemana = new Dictionary<string, string>()
            {
                {"lunes", "monday"},
                {"martes", "tuesday"},
                {"miercoles", "wednesday"},
                {"miércoles", "wednesday"},
                {"jueves", "thursday"},
                {"viernes", "friday"},
                {"sabado", "saturday"},
                {"domingo", "sunday"}
            };
            DayOfWeek dia;
            if (diasDeLaSemana.TryGetValue(diaDeLaSemana, out string diaEnIngles))
            {

                try
                {
                    dia = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), diaEnIngles, true);
                    for (DateTime fechaActual = fechaInicio; fechaActual <= fechaFin; fechaActual = fechaActual.AddDays(1))
                    {
                        if (fechaActual.DayOfWeek == dia)
                        {
                            return fechaActual;
                        }
                    }
                    /*Console.WriteLine("No se encontró el día de la semana especificado en el rango de fechas dado.");*/
                    return null;
                }
                catch (ArgumentException)
                {
                    //Console.WriteLine("El día de la semana proporcionado no es válido.");
                    return null;
                }


            }
            else
            {
                return null;
            }

        }


        public ResponseModel CargaSencillo(HttpPostedFileBase oFile, string ruta)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                //string path = @"C:\File\lv.xml";
                //string pathFisico = path.Replace(@"\\", @"/");
                string rutaVirtual = ruta + "CargaSencillo\\SencilloSAP.xlsx";
                ruta = ruta + "CargaSencillo\\";
                if (!System.IO.Directory.Exists(ruta))
                {
                    System.IO.Directory.CreateDirectory(ruta);
                }

                if (System.IO.File.Exists(rutaVirtual))
                {
                    //var fechaHoy = DateTime.Now;
                    //string nombreArchivoAntiguo = fechaHoy.Day.ToString() + "-" + fechaHoy.Month.ToString() + "-" + fechaHoy.Year.ToString() + "-" + fechaHoy.Hour.ToString() + fechaHoy.Minute.ToString() + fechaHoy.Second.ToString() + ".XML";
                    //var rutaAntiguo = ruta + "Antiguo\\" + nombreArchivoAntiguo;
                    //if (!System.IO.Directory.Exists(ruta + "Antiguo"))
                    //{
                    //    System.IO.Directory.CreateDirectory(ruta + "Antiguo");
                    //}
                    //System.IO.File.Move(rutaVirtual, rutaAntiguo);
                    //System.IO.File.Delete(rutaVirtual);
                    System.IO.File.Delete(rutaVirtual);
                }

                string rutaVirtualXML = ruta + oFile.FileName;
                oFile.SaveAs(rutaVirtualXML);

                System.IO.File.Move(rutaVirtualXML, rutaVirtual);

                SLDocument sl = new SLDocument(rutaVirtual);

                List<SencillosSAP> SAP = new List<SencillosSAP>();
                using (var context = db.Database.BeginTransaction())
                {
                    //var sencillo = new Sencillos();
                    //sencillo.Id = Guid.NewGuid();
                    //sencillo.Fecha = DateTime.Now;
                    int row = 11;

                    while (!string.IsNullOrEmpty(sl.GetCellValueAsString(row, 6)))
                    {

                        SencillosSAP detalleSencillo = new SencillosSAP();
                        detalleSencillo.id = Guid.NewGuid();
                        //detalleSencillo.IdSencillo = sencillo.Id;
                        //detalleSencillo.IdTienda = int.Parse(sl.GetCellValueAsString(row, 1));
                        detalleSencillo.Referencia = sl.GetCellValueAsString(row, 4);
                        detalleSencillo.Asignacion = sl.GetCellValueAsString(row, 5);
                        detalleSencillo.N_doc = sl.GetCellValueAsString(row, 6);
                        detalleSencillo.Cla = sl.GetCellValueAsString(row, 7);
                        detalleSencillo.Periodo = !string.IsNullOrEmpty(sl.GetCellValueAsString(row, 8)) ? int.Parse(sl.GetCellValueAsString(row, 8)) : 0;
                        detalleSencillo.Fecha_Doc = !string.IsNullOrEmpty(sl.GetCellValueAsString(row, 9)) ? DateTime.Parse(sl.GetCellValueAsString(row, 9).Replace(".", "-")) : detalleSencillo.Fecha_Doc;
                        detalleSencillo.IO = sl.GetCellValueAsString(row, 10);
                        detalleSencillo.Lib_Mayor = sl.GetCellValueAsString(row, 11);
                        detalleSencillo.ImporML = !string.IsNullOrEmpty(sl.GetCellValueAsString(row, 13)) ? int.Parse(sl.GetCellValueAsString(row, 13)) : 0;
                        detalleSencillo.Texto = sl.GetCellValueAsString(row, 16);
                        detalleSencillo.RutProvedor = sl.GetCellValueAsString(row, 17);
                        detalleSencillo.ConciliacionTesoreria = false;


                        SAP.Add(detalleSencillo);
                        //db.SencillosSAP.Add(detalleSencillo);
                        //db.SaveChanges();
                        // detalleSencillo.Add(detalleSencillo);
                        //sencillo.DetalleSencillo.Add(detalleSencillo);
                        row++;
                        if (string.IsNullOrEmpty(sl.GetCellValueAsString(row, 6)))
                        {
                            if (!string.IsNullOrEmpty(sl.GetCellValueAsString(row + 1, 6)))
                            {
                                row = row + 1;
                            }
                        }

                    }

                    var sencillosNoEncontrados = SAP
                       .GroupJoin( db.SencillosSAP,  // Tabla de la base de datos
                        s => new { s.Referencia, s.Fecha_Doc, s.ImporML },  // Claves de la lista SAP
                       se => new { se.Referencia, se.Fecha_Doc, se.ImporML },  // Claves de la tabla Sencillos
                       (s, seGroup) => new { Sencillo = s, SeGroup = seGroup })  // Objeto anónimo con el objeto Sy el grupo de objetos Sencillos correspondientes
                        .Where(x => !x.SeGroup.Any())  // Solo los objetos SAP que no tienen coincidencias en la tabla Sencillos
                        .Select(x => x.Sencillo)  // Seleccionar solo el objeto SAP
                        .ToList();

                    db.SencillosSAP.AddRange(sencillosNoEncontrados);
                    db.SaveChanges();
                    context.Commit();


                }



                response.respuesta = "Se cargo Correctamente";
                response.error = false;
                return response;


            }
            catch (Exception ex)
            {
                response.error = true;
                response.respuesta = "Problemas al cargar el libro de Venta" + ex.Message;
                return response;
            }
            //return response;
        }

        #endregion
    }
}