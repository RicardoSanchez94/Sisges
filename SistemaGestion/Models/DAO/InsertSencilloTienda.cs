using SistemaGestion.Models.Repositorios;
using SistemaGestion.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SistemaGestion.Models.DAO
{
    public class InsertSencilloTienda
    {
        private SisGesEntities3 db = new SisGesEntities3();
        AdminUsuarios ADU = new AdminUsuarios();

        public ResponseModel InsertaRecepcion(string CodigoRemito, int Tienda, Guid idSencillosTienda)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                using (var context = db.Database.BeginTransaction())
                {

                    Remito rm = new Remito();

                    var tienda = db.Sencillos_Tiendas.Where(x => x.id == idSencillosTienda).FirstOrDefault();


                    rm.Codigo = CodigoRemito;
                    rm.Fecha = DateTime.Now;
                    rm.idTienda = Tienda;
                    rm.CodigoTipoRemito = tienda.CodigoEstadoSencillo.Value == 2 ? 1 : 2;
                    rm.idSencillosTienda = idSencillosTienda;

                    db.Remito.Add(rm);

                    db.SaveChanges();

                    context.Commit();
                }

                response.respuesta = "Se insertó correctamente";
                response.error = false;

                return response;
            }
            catch (Exception ex)
            {
                response.respuesta = "No se insertó. " + ex.Message;
                response.error = true;

                return response;

            }

        }

        public ResponseModel InsertaDevolucion(string CodigoRemito, int Tienda, Guid idSencillosTienda, string Deposito)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                using (var context = db.Database.BeginTransaction())
                {

                    Remito rm = new Remito();

                    var tienda = db.Sencillos_Tiendas.Where(x => x.id == idSencillosTienda).Include(x=>x.Remito).FirstOrDefault();

                    if (tienda.Remito is null)
                    {
                        tienda.Remito = db.Remito.Where(x => x.idSencillosTienda == idSencillosTienda).ToList();
                    }

                    if (tienda.Remito.Where(x => x.Codigo == CodigoRemito).Any())
                    {
                        response.respuesta = "El Codigo de Remito Devolucion No puede Ser igual al Aceptado";
                        response.error = true;

                        return response;
                    }




                    rm.Codigo = CodigoRemito;
                    rm.Fecha = DateTime.Now;
                    rm.idTienda = Tienda;
                    rm.CodigoTipoRemito = tienda.CodigoEstadoSencillo.Value == 2 ? 1 : 2;
                    rm.idSencillosTienda = idSencillosTienda;
                    rm.NumeroDepostio = Deposito;

                    db.Remito.Add(rm);

                    db.SaveChanges();

                    context.Commit();
                }

                response.respuesta = "Se insertó correctamente";
                response.error = false;

                return response;
            }
            catch (Exception ex)
            {
                response.respuesta = "No se insertó. " + ex.Message;
                response.error = true;

                return response;

            }

        }

        public Sencillos_Tiendas GetInsertRemito(Guid idSencillotienda)
        {
            Sencillos_Tiendas lst = new Sencillos_Tiendas();
            lst = db.Sencillos_Tiendas.Where(x => x.id == idSencillotienda).SingleOrDefault();
            return lst;
        }

        public List<Sencillos_Tiendas> GetSencilloTienda(UserLoginView Deserialize)
        {
            List<Sencillos_Tiendas> lst = new List<Sencillos_Tiendas>();
            //List<DetalleSencillo> lst1 = new List<DetalleSencillo>();
            var userRol = ADU.GetRolesByLogiName(Deserialize.LoginName);

            if (userRol.Where(x => x.RolNombre == "Tienda").Any())
            {
                Persona ps = db.Persona.Where(x => x.Id == Deserialize.idUsuario).SingleOrDefault();

                lst = db.Sencillos_Tiendas.Where(x => x.idTienda == ps.RunCuerpo).ToList();
             
            }
            else
            {
                db.Configuration.LazyLoadingEnabled = true;
                db.Sencillos_Tiendas.Include(n => n.DetalleSencillo.SencillosSAP).Include(f=> f.Remito).Load();

                lst = db.Sencillos_Tiendas.Where
                    (x => x.DetalleSencillo.ConciliacionTesoreria == true
                  //&& 
                  //x.DetalleSencillo.Sencillos.Fecha.Value.Month == DateTime.Now.Month
                  ).Include(n => n.DetalleSencillo.SencillosSAP).Include(f => f.Remito).OrderBy(x => x.idTienda).ToList();

                //var tienda33 = lst.Where(x =>x.idTienda == 33).ToList();
                //var tipo4 = tienda33.Where(x => x.CodigoEstadoSencillo == 4).ToList();
                foreach (var item in lst)
                {
                    var sencillosSAP = item?.DetalleSencillo?.SencillosSAP;
                    var Remito = item?.Remito;
                    if (sencillosSAP == null)
                    {
                        item.DetalleSencillo.SencillosSAP = db.SencillosSAP.Where(x => x.id == item.DetalleSencillo.IdSencillosSAP).SingleOrDefault();
                        lst.Where(x => x == item).SingleOrDefault().DetalleSencillo.SencillosSAP = item.DetalleSencillo.SencillosSAP;
                    }
                    //if (Remito == null)
                    //{
                    //    item.Remito = db.SencillosSAP.Where(x => x.id == item.).SingleOrDefault();
                    //    lst.Where(x => x == item).SingleOrDefault().DetalleSencillo.SencillosSAP = item.DetalleSencillo.SencillosSAP;
                    //}
                }




            }





            return lst;
        }

        public List<Sobrante_Faltante> GetSobranteFaltante(UserLoginView Deserialize, DateTime Fecha)
        {
            List<Sobrante_Faltante> lst = new List<Sobrante_Faltante>();
            var userRol = ADU.GetRolesByLogiName(Deserialize.LoginName);

            if (userRol.Where(x => x.RolNombre == "Tienda").Any())
            {
                Persona ps = db.Persona.Where(x => x.Id == Deserialize.idUsuario).SingleOrDefault();

                lst = db.Sobrante_Faltante.Where(x => x.IDTIENDA == ps.RunCuerpo && x.FECHA == Fecha).Include(x => x.EMPLEADO).OrderBy(x => x.IDTIENDA).ToList();
                var faltantes = lst.Where(x => x.EMPLEADO is null).ToList();
                foreach (var item in faltantes)
                {
                    lst.Where(x => x == item).SingleOrDefault().EMPLEADO = db.EMPLEADO.Where(x => x.ID_EMPLEADO == item.ID_Empleado).SingleOrDefault();
                }

            }
            else
            {
                db.Configuration.LazyLoadingEnabled = true;
                //db.Sencillos_Tiendas.Include(n => n.DetalleSencillo.SencillosSAP).Load();
                lst = db.Sobrante_Faltante.Where(x => x.FECHA == Fecha).Include(x => x.EMPLEADO).OrderBy(x => x.IDTIENDA).ToList();

                var faltantes = lst.Where(x => x.EMPLEADO is null).ToList();
                foreach (var item in faltantes)
                {
                    lst.Where(x => x == item).SingleOrDefault().EMPLEADO = db.EMPLEADO.Where(x => x.ID_EMPLEADO == item.ID_Empleado).SingleOrDefault();
                }


            }





            return lst;
        }

        public AcumuladoFaltanteView.Data GetAcumuladoSobranteFaltante(UserLoginView Deserialize, DateTime Fecha)
        {
            //List<Sobrante_Faltante> lst = new List<Sobrante_Faltante>();
            AcumuladoFaltanteView.Data data = new AcumuladoFaltanteView.Data();
            List<AcumuladoSobrante_Faltante> lst = new List<AcumuladoSobrante_Faltante>();
            //data.AcumuladoFaltante lstAcumulados = new data.AcumuladoFaltante();
        
            data.AcumuladoFaltante = new List<AcumuladoFaltanteView.AcumuladoFaltante>();
            //data.aceptadosRechazados = new List<AcumuladoFaltanteView.AceptadosRechazados>();
            //AcumuladoFaltanteView.Data lstAcumulados = new List<AcumuladoFaltanteView>();
            var userRol = ADU.GetRolesByLogiName(Deserialize.LoginName);

            if (userRol.Where(x => x.RolNombre == "Tienda").Any())
            {
                Persona ps = db.Persona.Where(x => x.Id == Deserialize.idUsuario).SingleOrDefault();


                data.AcumuladoFaltante = db.Sobrante_Faltante.Where(x => x.IDTIENDA == ps.RunCuerpo && x.FECHA.Year == Fecha.Year && x.FECHA.Month == Fecha.Month && x.TIPO == "FALTANTE" && x.TOTAL >= 1000 && !x.IdAcumulado.HasValue ).GroupBy(x => new { x.IDTIENDA, x.ID_Empleado }).Select(x => new AcumuladoFaltanteView.AcumuladoFaltante { idTienda = x.Key.IDTIENDA, idEmpleado = x.Key.ID_Empleado, Total = x.Sum(y => y.TOTAL) }).ToList();


               lst = db.AcumuladoSobrante_Faltante.Where(
                    x => x.Sobrante_Faltante.Where(f => f.IDTIENDA == ps.RunCuerpo && f.FECHA.Year == Fecha.Year && f.FECHA.Month == Fecha.Month).Any()).ToList();

                data.aceptadosRechazados = lst;
                //lst = db.AcumuladoSobrante_Faltante.Where(x => x.IDTIENDA == ps.RunCuerpo && x.Ano == Fecha.Year && x.Mes == Fecha.Month && x.TotalAcumulado > 0 && x.TIPO == "FALTANTE").Include(x => x.EMPLEADO).Include(x => x.TipoAceptacion).OrderBy(x => x.IDTIENDA).ToList();
                //var faltantes = lst.Where(x => x.EMPLEADO is null).ToList();
                //foreach (var item in faltantes)
                //{
                //    lst.Where(x => x == item).SingleOrDefault().EMPLEADO = db.EMPLEADO.Where(x => x.ID_EMPLEADO == item.ID_Empleado).SingleOrDefault();
                //}

            }
            else
            {
                data.AcumuladoFaltante = db.Sobrante_Faltante.Where(x =>x.FECHA.Year == Fecha.Year && x.FECHA.Month == Fecha.Month && x.TIPO == "FALTANTE" && x.TOTAL >= 1000 && !x.IdAcumulado.HasValue).GroupBy(x => new { x.IDTIENDA, x.ID_Empleado }).Select(x => new AcumuladoFaltanteView.AcumuladoFaltante { idTienda = x.Key.IDTIENDA, idEmpleado = x.Key.ID_Empleado, Total = x.Sum(y => y.TOTAL) }).ToList();
                
                lst = db.AcumuladoSobrante_Faltante.Where(
                x => x.Sobrante_Faltante.Where(f => f.FECHA.Year == Fecha.Year && f.FECHA.Month == Fecha.Month).Any()).ToList();

                data.aceptadosRechazados = lst;
            }
            //else
            //{
            //    //db.Configuration.LazyLoadingEnabled = true;
            //    ////db.Sencillos_Tiendas.Include(n => n.DetalleSencillo.SencillosSAP).Load();
            //    //lst = db.AcumuladoSobrante_Faltante.Where(x => x.Ano == Fecha.Year && x.Mes == Fecha.Month && x.TotalAcumulado > 0 && x.TIPO == "FALTANTE").Include(x => x.EMPLEADO).Include(x => x.TipoAceptacion).OrderBy(x => x.IDTIENDA).ToList();

            //    //var faltantes = lst.Where(x => x.EMPLEADO is null).ToList();
            //    //foreach (var item in faltantes)
            //    //{
            //    //    lst.Where(x => x == item).SingleOrDefault().EMPLEADO = db.EMPLEADO.Where(x => x.ID_EMPLEADO == item.ID_Empleado).SingleOrDefault();
            //    //}

            //    db.Configuration.LazyLoadingEnabled = true;
            //    //db.Sencillos_Tiendas.Include(n => n.DetalleSencillo.SencillosSAP).Load();

            //    lstAcumulados = db.Sobrante_Faltante.Where(x => x.FECHA.Year == Fecha.Year && x.FECHA.Month == Fecha.Month && x.TIPO == "FALTANTE" && x.TOTAL >= 1000).GroupBy(x => new { x.IDTIENDA, x.ID_Empleado }).Select(x => new AcumuladoFaltanteView { idTienda = x.Key.IDTIENDA, idEmpleado = x.Key.ID_Empleado, Total = x.Sum(y => y.TOTAL) }).ToList();
            //    //lst = db.AcumuladoSobrante_Faltante.Where(x => x.Ano == Fecha.Year && x.Mes == Fecha.Month && x.TotalAcumulado > 0 && x.TIPO == "FALTANTE").Include(x => x.EMPLEADO).Include(x => x.TipoAceptacion).OrderBy(x => x.IDTIENDA).ToList();

            //    //var faltantes = lst.Where(x => x.EMPLEADO is null).ToList();
            //    //foreach (var item in faltantes)
            //    //{
            //    //    lst.Where(x => x == item).SingleOrDefault().EMPLEADO = db.EMPLEADO.Where(x => x.ID_EMPLEADO == item.ID_Empleado).SingleOrDefault();
            //    //}


            //}


            //return lst.OrderBy(x => x.IDTIENDA).ToList();



            return data;
        }

        public AceptacionSobranteFaltanteView GetAceptacionSobranteFaltante(Sobrante_Faltante sobrante_Faltante)
        {
            AceptacionSobranteFaltanteView oAceptacionSobranteFaltante = (new AceptacionSobranteFaltanteView
            {
                //AceptacionSobranteFaltante = new AceptacionSobranteFaltante(),
                lstTipoAceptacion = db.TipoAceptacion.ToList(),
                Sobrante_Faltante = sobrante_Faltante
                //IdTienda = sobrante_Faltante.IDTIENDA,
                //Fecha = sobrante_Faltante.FECHA,
                //Medio_Pago = sobrante_Faltante.MEDIO_PAGO,
                //Caja = sobrante_Faltante.CAJA,
                //Tipo = sobrante_Faltante.TIPO,
                //FechHora = sobrante_Faltante.FECHHORA,
                //TipoAceptacion = db.TipoAceptacion.ToList()
            });






            return oAceptacionSobranteFaltante;

        }
        public AcumuladoSobranteFaltanteView GetAceptacionAcumuladoFaltante(List<AcumuladoSobranteCheckView> oAcumuladoSobranteFaltante)
        {
            List<Sobrante_Faltante> lst = new List<Sobrante_Faltante>();
            AcumuladoSobranteFaltanteView oAcumuladoView = new AcumuladoSobranteFaltanteView();

            oAcumuladoView.AcumuladoSobrante_Faltante = new AcumuladoSobrante_Faltante();
            //oAcumuladoView.AcumuladoSobrante_Faltante = db.AcumuladoSobrante_Faltante.Where(x => x.ID_Empleado == oAcumuladoSobranteFaltante.ID_Empleado && x.Ano == oAcumuladoSobranteFaltante.Ano && x.Mes == oAcumuladoSobranteFaltante.Mes && x.TIPO == "FALTANTE" && x.IDTIENDA == oAcumuladoSobranteFaltante.IDTIENDA).SingleOrDefault();
            oAcumuladoView.lstTipoAceptacion = db.TipoAceptacion.ToList();
            oAcumuladoView.lstTipoRechazo = db.TipoRechazo.ToList();
            foreach (var item in oAcumuladoSobranteFaltante.Where(x => x.isCheck))
            {
                Sobrante_Faltante sf = db.Sobrante_Faltante.Where(x => x.IDTIENDA == item.IDTIENDA && x.ID_Empleado == item.ID_Empleado && x.FECHA == item.Fecha && x.CAJA == item.CAJA).SingleOrDefault();

                //sf.IdAcumulado = oAcumuladoView.AcumuladoSobrante_Faltante.Id;
                lst.Add(sf);
            }
            oAcumuladoView.Total = lst.Sum(x => x.TOTAL);
            oAcumuladoView.Lstsobrante_Faltantes = lst.ToList();


            return oAcumuladoView;

        }
        public ResponseModel InsertarAcumuladoSobranteFaltante(AcumuladoSobranteFaltanteView oAceptacion)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                using (var context = db.Database.BeginTransaction())
                {
                    //AcumuladoSobrante_Faltante acsf = new AcumuladoSobrante_Faltante();
                    //acsf.Id = Guid.NewGuid();
                    oAceptacion.AcumuladoSobrante_Faltante.Id = Guid.NewGuid();
                    oAceptacion.AcumuladoSobrante_Faltante.FechaAprobacion = DateTime.Now;
                    var agrupado = oAceptacion.Lstsobrante_Faltantes
                        .GroupBy(f => new { f.FECHA.Year, f.FECHA.Month, f.IDTIENDA })
                       .Select(f => new { Fechayear = f.Key.Year, FechaMes = f.Key.Month, idtienda = f.Key.IDTIENDA })
                       .FirstOrDefault();

                    oAceptacion.AcumuladoSobrante_Faltante.Ano = agrupado.Fechayear;
                    oAceptacion.AcumuladoSobrante_Faltante.Mes = agrupado.FechaMes;
                    oAceptacion.AcumuladoSobrante_Faltante.idTienda = agrupado.idtienda;




                    List<Sobrante_Faltante> lst = new List<Sobrante_Faltante>();
                    foreach (var item in oAceptacion.Lstsobrante_Faltantes)
                    {
                        Sobrante_Faltante sf = db.Sobrante_Faltante.Where(x => x.IDTIENDA == item.IDTIENDA && x.ID_Empleado == item.ID_Empleado && x.FECHA == item.FECHA && x.CAJA == item.CAJA).SingleOrDefault();

                        sf.IdAcumulado = oAceptacion.AcumuladoSobrante_Faltante.Id;

                        //oAceptacion.AcumuladoSobrante_Faltante.Ano = item.FECHA.Year;
                        //oAceptacion.AcumuladoSobrante_Faltante.Mes = item.FECHA.Month;
                    lst.Add(sf);
                    }
                    oAceptacion.AcumuladoSobrante_Faltante.TotalAcumulado = oAceptacion.Total;
                    //if (oAceptacion.AcumuladoSobrante_Faltante.CodigoTipoAceptacion == 1)
                    //{
                    //    //oAceptacion.AcumuladoSobrante_Faltante.Cuotas = oAcumulado.CodigoTipoAceptacion = oAceptacion.AcumuladoSobrante_Faltante.Cuotas

                    //    oAceptacion.AcumuladoSobrante_Faltante.CodigoTipoRechazo = null;
                    //    oAceptacion.AcumuladoSobrante_Faltante.

                    //}
                    //else
                    //{
                    //    oAcumulado.CodigoTipoRechazo = oAcumulado.CodigoTipoAceptacion = oAceptacion.AcumuladoSobrante_Faltante.CodigoTipoRechazo;
                    //}
                    //oAcumulado.CodigoTipoAceptacion = oAceptacion.AcumuladoSobrante_Faltante.CodigoTipoAceptacion;
                    ////oAcumulado.CodigoTipoRechazo = oAceptacion.AcumuladoSobrante_Faltante.CodigoTipoRechazo;
                    //oAcumulado.Observacion = oAceptacion.AcumuladoSobrante_Faltante.Observacion;
                    //oAcumulado.FechaAprobacion = DateTime.Now;

                    db.AcumuladoSobrante_Faltante.Add(oAceptacion.AcumuladoSobrante_Faltante);
                    
                    db.SaveChanges();

                    context.Commit();
                }

         

                response.respuesta = "Se insertó correctamente";
                response.error = false;

                return response;
            }
            catch (Exception ex)
            {
                response.respuesta = "ha ocurrido un error " + ex.Message;
                response.error = true;
                return response;
            }


        }

        public ResponseModel InsertarAceptacionSobranteFaltante(AceptacionSobranteFaltanteView oAceptacion)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                //using (var context = db.Database.BeginTransaction())
                //{

                //    //Remito rm = new Remito();
                //    oAceptacion.AceptacionSobranteFaltante.Id = Guid.NewGuid();
                //    oAceptacion.AceptacionSobranteFaltante.IdTienda = oAceptacion.Sobrante_Faltante.IDTIENDA;
                //    oAceptacion.AceptacionSobranteFaltante.Fecha = oAceptacion.Sobrante_Faltante.FECHA.Date;
                //    oAceptacion.AceptacionSobranteFaltante.Medio_Pago = oAceptacion.Sobrante_Faltante.MEDIO_PAGO;
                //    oAceptacion.AceptacionSobranteFaltante.Caja = oAceptacion.Sobrante_Faltante.CAJA;
                //    oAceptacion.AceptacionSobranteFaltante.Tipo = oAceptacion.Sobrante_Faltante.TIPO;
                //    oAceptacion.AceptacionSobranteFaltante.FechHora = oAceptacion.Sobrante_Faltante.FECHHORA;
                //    oAceptacion.AceptacionSobranteFaltante.FecheCreacion = DateTime.Now;


                //    db.AceptacionSobranteFaltante.Add(oAceptacion.AceptacionSobranteFaltante);

                //    db.SaveChanges();

                //    context.Commit();
                //}

                response.respuesta = "Se insertó correctamente";
                response.error = false;

                return response;
            }
            catch (Exception ex)
            {
                response.respuesta = "ha ocurrido un error " + ex.Message;
                response.error = true;
                return response;
            }


        }

        public List<Sobrante_Faltante> GetListSobranteFaltantePorEmpleado(int ano, int mes, int IdTienda, string IdEmpleado)
        {
            List<Sobrante_Faltante> lst = new List<Sobrante_Faltante>();



            //Persona ps = db.Persona.Where(x => x.Id == Deserialize.idUsuario).SingleOrDefault();
            db.Configuration.LazyLoadingEnabled = true;

            lst = db.Sobrante_Faltante.Where(x => x.IDTIENDA == IdTienda && x.ID_Empleado == IdEmpleado && x.FECHA.Year == ano && x.FECHA.Month == mes && Math.Abs(x.TOTAL) >= 1000 && !x.IdAcumulado.HasValue && x.TIPO.Equals("FALTANTE")).Include(x => x.EMPLEADO).OrderBy(x => x.IDTIENDA).ToList();
            var faltantes = lst.Where(x => x.EMPLEADO is null).ToList();
            foreach (var item in faltantes)
            {
                lst.Where(x => x == item).SingleOrDefault().EMPLEADO = db.EMPLEADO.Where(x => x.ID_EMPLEADO == item.ID_Empleado).SingleOrDefault();
            }



            return lst;
        }

        public List<Sobrante_Faltante> GetSobranteFaltante(Guid IdAcumulado)
        {
            List<Sobrante_Faltante> lst = new List<Sobrante_Faltante>();



            //Persona ps = db.Persona.Where(x => x.Id == Deserialize.idUsuario).SingleOrDefault();
            db.Configuration.LazyLoadingEnabled = true;

            lst = db.Sobrante_Faltante.Where(x=>x.IdAcumulado == IdAcumulado).ToList();
            var faltantes = lst.Where(x => x.EMPLEADO is null).ToList();
            foreach (var item in faltantes)
            {
                lst.Where(x => x == item).SingleOrDefault().EMPLEADO = db.EMPLEADO.Where(x => x.ID_EMPLEADO == item.ID_Empleado).SingleOrDefault();
            }



            return lst;
        }




    }
}