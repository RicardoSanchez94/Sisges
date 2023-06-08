using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaGestion.Models.ViewModels
{
    public class IndicadorNC
    {
        public int idZona { get; set; }
        public string NombreZona { get; set; }

        public string TipoNC { get; set; }
        public int cantNC { get; set; }

    }

    public class NCdetailsMain
    {
        public int idZona { get; set; }
        public List<NcDetailsModel> listNCDetails { get; set; }
        public NCdetailsMain()
        {
            this.listNCDetails = new List<NcDetailsModel>();
        }
    }
    public class NcDetailsModel
    {
        public int idZona { get; set; }
        public string nomZona { get; set; }

        public DateTime fechaNC { get; set; }
        public int idCencos { get; set; }
        public string nomCencos { get; set; }
        public int cantidadNCNormal { get; set; }
        public int cantidadNCEspecial { get; set; }
    }
    public class NcDetailsByCenco
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

        public string descTienda { get; set; }
        public string hora_emision_nc { get; set; }
        public Nullable<decimal> Monto_Medio_de_Pago { get; set; }
        public int id_Cajero { get; set; }
        public string nombre_Jefatura_Autoriza { get; set; }
        public string hora_autorizacion_nota_credito { get; set; }
    }
}