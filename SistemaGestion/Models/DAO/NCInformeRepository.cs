using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SistemaGestion.Models.DAO;
using SistemaGestion.Models.Repositorios;
using SistemaGestion.Models.Security;
using SistemaGestion.Models.ViewModels;

namespace SistemaGestion.Models.DAO
{
    public class NCInformeRepository
    {
        private AdminUsuarios ADU = new AdminUsuarios();
        private SisGesEntities3 db;
        //private interfacesIntellecEntities db;

        public NCInformeRepository()
        {
            db = new SisGesEntities3();
        }


        // RETORNA LA LISTA DE ZONAS Y SU NUMERO DE NC TOTALES POR SUS TIENDAS ENTRE FECHAS
        // PARA ROL ADMIN-CUADRATURA-CONTRALORIA
        public List<IndicadorNC> GetTotalNCZona(DateTime inicio, DateTime fin, string user)
        {
            //PRIMERO SE DEBE OBTENER SI ES ZONAL O ES UN ROL QUE VE TODAS LAS ZONAS
            var data = new List<IndicadorNC>();
            //var list = db.notaCreditoEspecial.Where(x => x.Fecha_Emision_Nota_Credito >= inicio && x.Fecha_Emision_Nota_Credito <= fin).ToList();


            var cencos = db.CENTROS_LOCAL.Where(z => z.IdZona != null).ToList();
            var zonas = db.ZONA.ToList();

            var fi = new DateTime(2022, 10, 04);
            var ff = new DateTime(2022, 11, 23);

            (from p1 in db.notaCreditoEspecial.ToList()
             join p2 in cencos on p1.IdTienda_notacredito equals p2.CEN_Codigo
             join p3 in zonas on p2.IdZona equals p3.IdZona
             where p1.Fecha_Emision_Nota_Credito >= inicio.Date && p1.Fecha_Emision_Nota_Credito <= fin.Date
             select new
             {
                 idZona = p3.IdZona,
                 NZona = p3.nombreZona,
                 TipoNC = p1.TIPONOTACREDITO,
             }).ToList().GroupBy(z => new { z.idZona, z.NZona, z.TipoNC })
                .Select(g =>
                new
                {
                    idZona = g.Key.idZona,
                    NumNC = g.Count(),
                    TipoNC = g.Key.TipoNC,
                    Zona = g.Key.NZona
                }).OrderBy(x => x.Zona).ToList().ForEach(item => data.Add(new IndicadorNC()
                {

                    idZona = item.idZona,
                    NombreZona = item.Zona,
                    TipoNC = item.TipoNC,
                    cantNC = item.NumNC

                }));
            return data;
        }
        // RETORNA LA CANTIDAD DE NC DE LAS TIENDAS PERTENECIENTES A LA ZONA DEL USUARIO
        // PARA ROL ZONAL
        public List<NcDetailsModel> GetTotalNCTiendaZona(DateTime inicio, DateTime fin, string user, int? idZona)
        {
            //PRIMERO SE DEBE OBTENER SI ES ZONAL O ES UN ROL QUE VE TODAS LAS ZONAS
            var data = new List<NcDetailsModel>();

            // SI LA ZONA ES NULL ES POR QUE ESTA CONSULTANDO UN USUARIO TIPO ZONA O TIENDA
            // SI LA ZONA TUVIERA VALOR SERIA SELECCIONADO DESDE LA VISTA
            if (idZona == null)
            {
                var isZonal = ADU.GetRolesByLogiName(user).Where(z => z.RolNombre == "Zonal").Any();
                var listazonas = new List<int>();
                if (isZonal)
                {
                    db.Persona.Where(x => x.Run.Replace(".", "") == user).FirstOrDefault()
                                                                         .Usuario.ZONA
                                                                         .ToList()
                                                                         .ForEach(item =>
                                                                         {
                                                                             listazonas.Add(item.IdZona);
                                                                         });
                }


                var lista1 = (from p1 in db.notaCreditoEspecial.ToList()
                              join p2 in db.CENTROS_LOCAL.ToList() on p1.IdTienda_notacredito equals p2.CEN_Codigo
                              join p3 in db.ZONA.ToList() on p2.IdZona equals p3.IdZona
                              where p1.Fecha_Emision_Nota_Credito >= inicio.Date && p1.Fecha_Emision_Nota_Credito <= fin.Date && listazonas.Contains(p3.IdZona)
                              group new { p1, p2, p3 }
                              by new { p1.Fecha_Emision_Nota_Credito, p1.IdTienda_notacredito, p3.IdZona, p2.CEN_Codigo, p2.CEN_Nombre, p3.nombreZona, p1.TIPONOTACREDITO }
                              into
                              m
                              select new
                              {
                                  idZona = m.Key.IdZona,
                                  NZona = m.Key.nombreZona,
                                  TipoNC = m.Key.TIPONOTACREDITO,
                                  cantNC = m.Count(),
                                  idCencos = m.Key.IdTienda_notacredito,
                                  nomCencos = m.Key.CEN_Nombre,
                                  Fecha = m.Key.Fecha_Emision_Nota_Credito
                              }).ToList();

                foreach (var a in lista1.GroupBy(z => new { z.Fecha, z.idCencos }).Select(z => new { idCencos = z.Key.idCencos, Fecha = z.Key.Fecha }).ToList())
                {
                    var subdata = lista1.Where(x => x.idCencos == a.idCencos && x.Fecha == a.Fecha).ToList();
                    int NCEspecialN = 0;
                    var NCNormalN = 0;

                    foreach (var m in subdata)
                    {
                        if (m.TipoNC.Contains("ESPECIAL"))
                        {
                            NCEspecialN = m.cantNC;

                        }
                        if (m.TipoNC.Contains("NORMAL"))
                        {
                            NCNormalN = m.cantNC;
                        }
                    }
                    data.Add(
                        new NcDetailsModel()
                        {
                            idCencos = (int)subdata.FirstOrDefault().idCencos,
                            fechaNC = (DateTime)subdata.FirstOrDefault().Fecha,
                            cantidadNCEspecial = NCEspecialN,
                            idZona = subdata.FirstOrDefault().idZona,
                            nomZona = subdata.FirstOrDefault().NZona,
                            nomCencos = subdata.FirstOrDefault().nomCencos,
                            cantidadNCNormal = NCNormalN

                        });
                }

            }
            else
            {
                var lista1 = (from p1 in db.notaCreditoEspecial.ToList()
                              join p2 in db.CENTROS_LOCAL.ToList() on p1.IdTienda_notacredito equals p2.CEN_Codigo
                              join p3 in db.ZONA.ToList() on p2.IdZona equals p3.IdZona
                              where p1.Fecha_Emision_Nota_Credito >= inicio.Date && p1.Fecha_Emision_Nota_Credito <= fin.Date && p3.IdZona == idZona
                              group new { p1, p2, p3 }
                              by new { p1.Fecha_Emision_Nota_Credito, p1.IdTienda_notacredito, p3.IdZona, p2.CEN_Codigo, p2.CEN_Nombre, p3.nombreZona, p1.TIPONOTACREDITO }
                                             into
                                             m
                              select new
                              {
                                  idZona = m.Key.IdZona,
                                  NZona = m.Key.nombreZona,
                                  TipoNC = m.Key.TIPONOTACREDITO,
                                  cantNC = m.Count(),
                                  idCencos = m.Key.IdTienda_notacredito,
                                  nomCencos = m.Key.CEN_Nombre,
                                  Fecha = m.Key.Fecha_Emision_Nota_Credito
                              }).ToList();

                foreach (var a in lista1.GroupBy(z => new { z.Fecha, z.idCencos }).Select(z => new { idCencos = z.Key.idCencos, Fecha = z.Key.Fecha }).ToList())
                {
                    var subdata = lista1.Where(x => x.idCencos == a.idCencos && x.Fecha == a.Fecha).ToList();
                    int NCEspecialN = 0;
                    var NCNormalN = 0;

                    foreach (var m in subdata)
                    {
                        if (m.TipoNC.Contains("ESPECIAL"))
                        {
                            NCEspecialN = m.cantNC;

                        }
                        if (m.TipoNC.Contains("NORMAL"))
                        {
                            NCNormalN = m.cantNC;
                        }
                    }
                    data.Add(
                        new NcDetailsModel()
                        {
                            idCencos = (int)subdata.FirstOrDefault().idCencos,
                            fechaNC = (DateTime)subdata.FirstOrDefault().Fecha,
                            cantidadNCEspecial = NCEspecialN,
                            idZona = subdata.FirstOrDefault().idZona,
                            nomZona = subdata.FirstOrDefault().NZona,
                            nomCencos = subdata.FirstOrDefault().nomCencos,
                            cantidadNCNormal = NCNormalN

                        });
                }

            }
            return data;
        }

        public List<NcDetailsByCenco> getDetalleByCenco(DateTime fecha, int idcenco, int idzona)
        {

            var data = new List<NcDetailsByCenco>();

            db.notaCreditoEspecial.Where(x => x.Fecha_Emision_Nota_Credito == fecha && x.IdTienda_notacredito == idcenco).ToList()
            .ForEach(i => data.Add(new NcDetailsByCenco()
            {
                Descripcion_pago = i.Descripcion_pago,
                Documento_Origen = i.Documento_Origen,
                Fecha_Emision_Nota_Credito = (DateTime)i.Fecha_Emision_Nota_Credito,
                Folio_notacredito = i.Folio_notacredito,
                IdTienda_notacredito = (int)i.IdTienda_notacredito,
                Monto_Boleta = (decimal)i.Monto_Boleta,
                Nro_Boleta = i.Nro_Boleta,
                TIPONOTACREDITO = i.TIPONOTACREDITO,
                Total_notacredito = (decimal)i.Total_notacredito,
                hora_emision_nc = i.Hora_Emision_Nota_Credito,
                Rut_Supervisor = i.Rut_Supervisor,
                nombre_Jefatura_Autoriza = i.Nombre_Jefe,
                TRX = i.TRX

            }));

            return data;
        }

        public List<NcDetailsByCenco> getDataNCINforme(DateTime inicio, DateTime fin, string user, int idZona)
        {
            var data = new List<NcDetailsByCenco>();

            if (idZona != 0)
            {
                data.AddRange((from p1 in db.notaCreditoEspecial.ToList()
                               join p2 in db.CENTROS_LOCAL.ToList() on p1.IdTienda_notacredito equals p2.CEN_Codigo
                               join p3 in db.ZONA.ToList() on p2.IdZona equals p3.IdZona
                               where p1.Fecha_Emision_Nota_Credito >= inicio.Date && p1.Fecha_Emision_Nota_Credito <= fin.Date && p3.IdZona == idZona
                               select new NcDetailsByCenco()
                               {
                                   TRX = p1.TRX,
                                   Fecha_Emision_Nota_Credito = p1.Fecha_Emision_Nota_Credito,
                                   IdTienda_notacredito = p1.IdTienda_notacredito,
                                   Caja_notacredito = p1.Caja_notacredito,
                                   TIPONOTACREDITO = p1.TIPONOTACREDITO,
                                   Folio_notacredito = p1.Folio_notacredito,
                                   Cliente_notacredito = p1.Cliente_notacredito,
                                   Total_notacredito = p1.Total_notacredito,
                                   Documento_Origen = p1.Documento_Origen,
                                   Trx_Origen = p1.Trx_Origen,
                                   Nro_Boleta = p1.Nro_Boleta,
                                   IdTienda_Origen_Boleta = p1.IdTienda_Origen_Boleta,
                                   Caja_Origen_Boleta = p1.Caja_Origen_Boleta,
                                   Fecha_Boleta = p1.Fecha_Boleta,
                                   Monto_Boleta = p1.Monto_Boleta,
                                   Tipo_Pago_Origen = p1.Tipo_Pago_Origen,
                                   Rut_Cajero = p1.Rut_Cajero,
                                   Rut_Supervisor = p1.Rut_Supervisor,
                                   Nombre_trabajador = p1.Nombre_trabajador,
                                   Descripcion_pago = p1.Descripcion_pago,
                                   descTienda = p2.CEN_Nombre,
                                   hora_emision_nc = p1.Hora_Emision_Nota_Credito,
                                   Monto_Medio_de_Pago = p1.Monto_Boleta,
                                   nombre_Jefatura_Autoriza = p1.Nombre_Jefe,
                                   hora_autorizacion_nota_credito = p1.Hora_Aprov_Nota_Credito


                                   // FALTA AGREGAR COLUMNAS QUE AGREGARIA PATRICIO A LA TABLA DE BD
                                   //hora_emision_nc 
                                   //Monto_Medio_de_Pago 
                                   //id_Cajero 
                                   //nombre_Jefatura_Autoriza 
                                   //hora_autorizacion_nota_credito 


                               }).ToList());

            }
            else
            {
                var isZonal = ADU.GetRolesByLogiName(user).Where(z => z.RolNombre == "Zonal").Any();
                var listazonas = new List<int>();
                if (isZonal)
                {
                    db.Persona.Where(x => x.Run.Replace(".", "") == user).FirstOrDefault()
                                                                         .Usuario.ZONA
                                                                         .ToList()
                                                                         .ForEach(item =>
                                                                         {
                                                                             listazonas.Add(item.IdZona);
                                                                         });
                }


                data.AddRange((from p1 in db.notaCreditoEspecial.ToList()
                               join p2 in db.CENTROS_LOCAL.ToList() on p1.IdTienda_notacredito equals p2.CEN_Codigo
                               join p3 in db.ZONA.ToList() on p2.IdZona equals p3.IdZona
                               where p1.Fecha_Emision_Nota_Credito >= inicio.Date && p1.Fecha_Emision_Nota_Credito <= fin.Date && listazonas.Contains(p3.IdZona)
                               select new NcDetailsByCenco()
                               {
                                   TRX = p1.TRX,
                                   Fecha_Emision_Nota_Credito = p1.Fecha_Emision_Nota_Credito,
                                   IdTienda_notacredito = p1.IdTienda_notacredito,
                                   Caja_notacredito = p1.Caja_notacredito,
                                   TIPONOTACREDITO = p1.TIPONOTACREDITO,
                                   Folio_notacredito = p1.Folio_notacredito,
                                   Cliente_notacredito = p1.Cliente_notacredito,
                                   Total_notacredito = p1.Total_notacredito,
                                   Documento_Origen = p1.Documento_Origen,
                                   Trx_Origen = p1.Trx_Origen,
                                   Nro_Boleta = p1.Nro_Boleta,
                                   IdTienda_Origen_Boleta = p1.IdTienda_Origen_Boleta,
                                   Caja_Origen_Boleta = p1.Caja_Origen_Boleta,
                                   Fecha_Boleta = p1.Fecha_Boleta,
                                   Monto_Boleta = p1.Monto_Boleta,
                                   Tipo_Pago_Origen = p1.Tipo_Pago_Origen,
                                   Rut_Cajero = p1.Rut_Cajero,
                                   Rut_Supervisor = p1.Rut_Supervisor,
                                   Nombre_trabajador = p1.Nombre_trabajador,
                                   Descripcion_pago = p1.Descripcion_pago,
                                   descTienda = p2.CEN_Nombre,
                               }).ToList());
            }

            return data;
        }

        public List<NcDetailsByCenco> getDataNCINforme(DateTime inicio, DateTime fin)
        {
            var data = new List<NcDetailsByCenco>();


            //data.AddRange((from p1 in db.notaCreditoEspecial.ToList()
            //               join p2 in db.CENTROS_LOCAL.ToList() on p1.IdTienda_notacredito equals p2.CEN_Codigo
            //               where p1.Fecha_Emision_Nota_Credito >= inicio.Date && p1.Fecha_Emision_Nota_Credito <= fin.Date
            //               && p1.Tipo_Pago_Origen != 113
            //               select new NcDetailsByCenco()
            //               {
            //                   TRX = p1.TRX,
            //                   Fecha_Emision_Nota_Credito = p1.Fecha_Emision_Nota_Credito,
            //                   IdTienda_notacredito = p1.IdTienda_notacredito,
            //                   Caja_notacredito = p1.Caja_notacredito,
            //                   TIPONOTACREDITO = p1.TIPONOTACREDITO,
            //                   Folio_notacredito = p1.Folio_notacredito,
            //                   Cliente_notacredito = p1.Cliente_notacredito,
            //                   Total_notacredito = p1.Total_notacredito,
            //                   Documento_Origen = p1.Documento_Origen,
            //                   Trx_Origen = p1.Trx_Origen,
            //                   Nro_Boleta = p1.Nro_Boleta,
            //                   IdTienda_Origen_Boleta = p1.IdTienda_Origen_Boleta,
            //                   Caja_Origen_Boleta = p1.Caja_Origen_Boleta,
            //                   Fecha_Boleta = p1.Fecha_Boleta,
            //                   Monto_Boleta = p1.Monto_Boleta,
            //                   Tipo_Pago_Origen = p1.Tipo_Pago_Origen,
            //                   Rut_Cajero = p1.Rut_Cajero,
            //                   Rut_Supervisor = p1.Rut_Supervisor,
            //                   Nombre_trabajador = p1.Nombre_trabajador,
            //                   Descripcion_pago = p1.Descripcion_pago,
            //                   descTienda = p2.CEN_Nombre,
            //                   hora_emision_nc = p1.Hora_Emision_Nota_Credito,
            //                   Monto_Medio_de_Pago = p1.Monto_Boleta,
            //                   nombre_Jefatura_Autoriza = p1.Nombre_Jefe,
            //                   hora_autorizacion_nota_credito = p1.Hora_Aprov_Nota_Credito


            //                   // FALTA AGREGAR COLUMNAS QUE AGREGARIA PATRICIO A LA TABLA DE BD
            //                   //hora_emision_nc 
            //                   //Monto_Medio_de_Pago 
            //                   //id_Cajero 
            //                   //nombre_Jefatura_Autoriza 
            //                   //hora_autorizacion_nota_credito 


            //               }).ToList());



            data.AddRange((from p1 in db.Fn_NC_Especial(inicio,fin).ToList()
                         select new NcDetailsByCenco()
                           {
                               TRX = p1.TRX.Value,
                               Fecha_Emision_Nota_Credito = p1.Fecha_Emision_Nota_Credito,
                               IdTienda_notacredito = p1.IdTienda_notacredito,
                               Caja_notacredito = p1.Caja_notacredito,
                               TIPONOTACREDITO = p1.TIPONOTACREDITO,
                               Folio_notacredito = p1.Folio_notacredito,
                               Cliente_notacredito = p1.Cliente_notacredito,
                               Total_notacredito = p1.Total_notacredito,
                               Documento_Origen = p1.Documento_Origen,
                               Trx_Origen = p1.Trx_Origen,
                               Nro_Boleta = p1.Nro_Boleta,
                               IdTienda_Origen_Boleta = p1.IdTienda_Origen_Boleta,
                               Caja_Origen_Boleta = p1.Caja_Origen_Boleta,
                               Fecha_Boleta = p1.Fecha_Boleta,
                               Monto_Boleta = p1.Monto_Boleta,
                               Tipo_Pago_Origen = p1.Tipo_Pago_Origen,
                               Rut_Cajero = p1.Rut_Cajero,
                               Rut_Supervisor = p1.Rut_Supervisor,
                               Nombre_trabajador = p1.Nombre_Trabajador,
                               Descripcion_pago = p1.Descripcion_pago,
                               descTienda = p1.CEN_Nombre,
                               hora_emision_nc = p1.Hora_Emision_Nota_Credito,
                               Monto_Medio_de_Pago = p1.Monto_Boleta,
                               nombre_Jefatura_Autoriza = p1.Nombre_Jefe,
                               hora_autorizacion_nota_credito = p1.Hora_Aprov_Nota_Credito

                           }).ToList());

            
            return data;
        }



    }
}