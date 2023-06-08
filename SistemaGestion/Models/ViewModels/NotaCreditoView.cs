using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaGestion.Models.ViewModels
{

    public class NotaCreditoViewSample
    {
        public List<notaCreditoDTO> ListP1 { get; set; }
        public List<notaCreditoDTO> ListP3 { get; set; }

    }

    public class NotaCreditoView
    {
        public List<notaCreditoDTO> ListPremisa1 { get; set; }
        public List<notaCreditoDTO> ListPremisa2 { get; set; }
        public List<notaCreditoDTO> ListPremisa3 { get; set; }
        public List<notaCreditoDTO> ListPremisa4 { get; set; }

        public NotaCreditoView()
        {
            this.ListPremisa1 = new List<notaCreditoDTO>();
            this.ListPremisa2 = new List<notaCreditoDTO>();
            this.ListPremisa3 = new List<notaCreditoDTO>();
            this.ListPremisa4 = new List<notaCreditoDTO>();
        }
    }

    public class notaCreditoDTO
    {
        public int TRX { get; set; }
        public Nullable<System.DateTime> Fecha_Emision_Nota_Credito { get; set; }
        public Nullable<int> IdTienda_notacredito { get; set; }
        public Nullable<int> Caja_notacredito { get; set; }
        public string TIPONOTACREDITO { get; set; }
        public string Folio_notacredito { get; set; }
        public string Cliente_notacredito { get; set; }
        public Nullable<decimal> Total_notacredito { get; set; }
        public string Documento_Origen { get; set; }
        public Nullable<int> Trx_Origen { get; set; }
        public Nullable<long> Nro_Boleta { get; set; }
        public Nullable<int> IdTienda_Origen_Boleta { get; set; }
        public Nullable<int> Caja_Origen_Boleta { get; set; }
        public Nullable<System.DateTime> Fecha_Boleta { get; set; }
        public Nullable<decimal> Monto_Boleta { get; set; }
        public Nullable<int> Tipo_Pago_Origen { get; set; }
        public string Rut_Cajero { get; set; }
        public string Rut_Supervisor { get; set; }
        public string Nombre_trabajador { get; set; }
        public string Descripcion_pago { get; set; }
        public Nullable<int> tipo_premisa { get; set; }
        public bool isCheck { get; set; }





    }
}