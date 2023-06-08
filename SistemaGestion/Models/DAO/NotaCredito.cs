using SistemaGestion.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaGestion.Models.DAO
{
    public class NotaCredito
    {
        #region objeto
        //private GestionDataContext dc = new GestionDataContext();
        private SisGesEntities3 db = new SisGesEntities3();
        private Negocio Ng = new Negocio();
        #endregion

        public NotaCreditoView getPremisasData(DateTime fecha)
        {
            var data = new NotaCreditoView();
            var listNC = db.fnReporte_PremisasNC(fecha).ToList();
            data.ListPremisa1 = GetListNCDto(listNC.Where(x => x.tipo_premisa == 1).ToList());
            data.ListPremisa2 = GetListNCDto(listNC.Where(x => x.tipo_premisa == 2).ToList());
            data.ListPremisa3 = GetListNCDto(listNC.Where(x => x.tipo_premisa == 3).ToList());
            data.ListPremisa4 = GetListNCDto(listNC.Where(x => x.tipo_premisa == 4).ToList());
            return data;
        }

        public List<notaCreditoDTO> GetListNCDto(List<fnReporte_PremisasNC_Result> lista)
        {
            var ListaAux = new List<notaCreditoDTO>();

            lista.ForEach(i => ListaAux.Add(new notaCreditoDTO()
            {
                TRX = i.TRX,
                Fecha_Emision_Nota_Credito = i.Fecha_Emision_Nota_Credito,
                IdTienda_notacredito = i.IdTienda_notacredito,
                Caja_notacredito = i.Caja_notacredito,
                TIPONOTACREDITO = i.TIPONOTACREDITO,
                Folio_notacredito = i.Folio_notacredito,
                Cliente_notacredito = i.Cliente_notacredito,
                Total_notacredito = i.Total_notacredito,
                Documento_Origen = i.Documento_Origen,
                Trx_Origen = i.Trx_Origen,
                Nro_Boleta = i.Nro_Boleta,
                IdTienda_Origen_Boleta = i.IdTienda_Origen_Boleta,
                Caja_Origen_Boleta = i.Caja_Origen_Boleta,
                Fecha_Boleta = i.Fecha_Boleta,
                Monto_Boleta = i.Monto_Boleta,
                Tipo_Pago_Origen = i.Tipo_Pago_Origen,
                Rut_Cajero = i.Rut_Cajero,
                Rut_Supervisor = i.Rut_Supervisor,
                Nombre_trabajador = i.Nombre_trabajador,
                Descripcion_pago = i.Descripcion_pago,
                tipo_premisa = i.tipo_premisa,
                isCheck = false
            }));
            return ListaAux;
        }


        public List<notaCreditoDTO> getNCListCheck(List<notaCreditoDTO> lista)
        {
            var response = new List<notaCreditoDTO>();
            if (lista != null && lista.Count > 0)
            {
                lista.ForEach(i =>
                            {
                                if (i.isCheck)
                                {
                                    response.Add(new notaCreditoDTO()
                                    {
                                        TRX = i.TRX,
                                        Fecha_Emision_Nota_Credito = i.Fecha_Emision_Nota_Credito,
                                        IdTienda_notacredito = i.IdTienda_notacredito,
                                        Caja_notacredito = i.Caja_notacredito,
                                        TIPONOTACREDITO = i.TIPONOTACREDITO,
                                        Folio_notacredito = i.Folio_notacredito,
                                        Cliente_notacredito = i.Cliente_notacredito,
                                        Total_notacredito = i.Total_notacredito,
                                        Documento_Origen = i.Documento_Origen,
                                        Trx_Origen = i.Trx_Origen,
                                        Nro_Boleta = i.Nro_Boleta,
                                        IdTienda_Origen_Boleta = i.IdTienda_Origen_Boleta,
                                        Caja_Origen_Boleta = i.Caja_Origen_Boleta,
                                        Fecha_Boleta = i.Fecha_Boleta,
                                        Monto_Boleta = i.Monto_Boleta,
                                        Tipo_Pago_Origen = i.Tipo_Pago_Origen,
                                        Rut_Cajero = i.Rut_Cajero,
                                        Rut_Supervisor = i.Rut_Supervisor,
                                        Nombre_trabajador = i.Nombre_trabajador,
                                        Descripcion_pago = i.Descripcion_pago,
                                        tipo_premisa = i.tipo_premisa
                                    });
                                }
                            }
                          );
            }
            return response;
        }


    }
}