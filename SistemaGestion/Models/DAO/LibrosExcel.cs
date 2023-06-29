using OfficeOpenXml;
using OfficeOpenXml.Style;
using SistemaGestion.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SistemaGestion.Models.DAO
{
    public class LibrosExcel
    {

        SisGesEntities3 db = new SisGesEntities3();

        public ExcelPackage ReporteSobranteFaltante(DateTime Fecha)
        {
            //*************************************************************
            //FileInfo newFile = new FileInfo("C:\\excel\\Centros de costos.xlsx");
            //BORRAMOS EL ARCHIVO SI EXISTE, SI NO CREA UNO NUEVO.
            //newFile.Delete();
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            var excelPackage = new ExcelPackage();
            //Propiedades Hoja de excel
            //var zona = lst.Select(x => x.nombreZona).Distinct();


            var sheet = excelPackage.Workbook.Worksheets.Add("AcumuladoSobrante_Faltante");
            var sheet1 = excelPackage.Workbook.Worksheets.Add("SobranteFaltante");
            //sheet.Name = item;

            //********************************************************************************************************
            //encabezados tabla de datos
            #region AcumuladosSobrantefaltantes
            var rowindex = 1;

            sheet.Cells[rowindex, 1].Value = "Rut Cajero";
            sheet.Cells[rowindex, 2].Value = "Nombre Cajero";
            sheet.Cells[rowindex, 3].Value = "Tienda";
            sheet.Cells[rowindex, 4].Value = "Fecha";
            sheet.Cells[rowindex, 5].Value = "Tipo";
            sheet.Cells[rowindex, 6].Value = "Total";
            sheet.Cells[rowindex, 7].Value = "Estado";
            sheet.Cells[rowindex, 8].Value = "Cuotas";
            sheet.Cells[rowindex, 9].Value = "TipoRechazo";
            sheet.Cells[rowindex, 10].Value = "Observacion";



            sheet.Column(1).AutoFit(13);
            sheet.Column(2).AutoFit(40);
            sheet.Column(3).AutoFit(3);
            sheet.Column(4).AutoFit(13);
            sheet.Column(5).AutoFit(10);
            sheet.Column(6).AutoFit(15);
            sheet.Column(7).AutoFit(10);
            sheet.Column(8).AutoFit(10);
            sheet.Column(9).AutoFit(10);
            sheet.Column(10).AutoFit(10);



            sheet.Cells[rowindex, 1, rowindex, 10].Style.Font.Bold = true;
            sheet.Cells[rowindex, 1, rowindex, 10].AutoFilter = true;

            var bordesEncabezados = sheet.Cells[rowindex, 1, rowindex, 10].Style.Border;
            bordesEncabezados.Top.Style = bordesEncabezados.Right.Style = bordesEncabezados.Bottom.Style = bordesEncabezados.Left.Style = ExcelBorderStyle.Medium;


            // SE CARGAN LOS DATOS DE TODO EL DOCUMENTO
            //var datosPlan = lst;
            var col = 0;
            //Empezamos a escribir sobre ella
            rowindex = 2;

            col = 1;


           var AcumuladoFaltante = db.AcumuladoSobrante_Faltante.Where(x => x.Ano == Fecha.Year && x.Mes == Fecha.Month).ToList();


            foreach (var itemDatos in AcumuladoFaltante)
            {
                //if (itemDatos.Sobrante_Faltante.Select(x) is null)
                //{
                //    itemDatos.EMPLEADO = db.EMPLEADO.Where(x => x.ID_EMPLEADO == itemDatos.ID_Empleado).SingleOrDefault();
                //}

                var agrupadoempleado = itemDatos.Sobrante_Faltante.
                                       GroupBy(x=> new {x.IDTIENDA,x.ID_Empleado , x.EMPLEADO.NOMBRES, x.EMPLEADO.APELLIDOS }).
                                       Select(f=>new {f.Key.IDTIENDA,f.Key.ID_Empleado,f.Key.NOMBRES,f.Key.APELLIDOS }).SingleOrDefault();

                //var Empleado = db.EMPLEADO.Where(x => x.ID_EMPLEADO == agrupadoempleado.ID_Empleado).SingleOrDefault();

                col = 1;
                sheet.Cells[rowindex, col++].Value = agrupadoempleado.ID_Empleado;

                sheet.Cells[rowindex, col++].Value = agrupadoempleado.NOMBRES + " " + agrupadoempleado.APELLIDOS;
                sheet.Cells[rowindex, col++].Value = itemDatos.idTienda;
                sheet.Cells[rowindex, col++].Value = Fecha.ToString("MMMM");
                sheet.Cells[rowindex, col++].Value = "";
                sheet.Cells[rowindex, col++].Value = Math.Abs(itemDatos.TotalAcumulado.Value);
                sheet.Cells[rowindex, col++].Value = itemDatos.CodigoTipoAceptacion.HasValue ? itemDatos.TipoAceptacion.Nombre : "Pendiente";
                sheet.Cells[rowindex, col++].Value = itemDatos.Cuotas;
                sheet.Cells[rowindex, col++].Value = itemDatos.CodigoTipoRechazo.HasValue ? itemDatos.TipoRechazo.Nombre : "";
                sheet.Cells[rowindex, col++].Value = itemDatos.Observacion;


                rowindex++;


            }
            #endregion

            #region SobranteFaltante
            rowindex = 1;
            sheet1.Cells[rowindex, 1].Value = "Rut Cajero";
            sheet1.Cells[rowindex, 2].Value = "Nombre Cajero";
            sheet1.Cells[rowindex, 3].Value = "Tienda";
            sheet1.Cells[rowindex, 4].Value = "Fecha";
            sheet1.Cells[rowindex, 5].Value = "Caja";
            sheet1.Cells[rowindex, 6].Value = "Tipo";
            sheet1.Cells[rowindex, 7].Value = "Total";


            sheet1.Column(1).AutoFit(13);
            sheet1.Column(2).AutoFit(40);
            sheet1.Column(3).AutoFit(3);
            sheet1.Column(4).AutoFit(10);
            sheet1.Column(5).AutoFit(3);
            sheet1.Column(6).AutoFit(10);
            sheet1.Column(7).AutoFit(15);

            sheet1.Cells[rowindex, 1, rowindex, 7].Style.Font.Bold = true;
            sheet1.Cells[rowindex, 1, rowindex, 7].AutoFilter = true;

            var bordesEncabezados1 = sheet1.Cells[rowindex, 1, rowindex, 7].Style.Border;
            bordesEncabezados1.Top.Style = bordesEncabezados1.Right.Style = bordesEncabezados1.Bottom.Style = bordesEncabezados1.Left.Style = ExcelBorderStyle.Medium;

            // SE CARGAN LOS DATOS DE TODO EL DOCUMENTO
            col = 0;
            //Empezamos a escribir sobre ella
            rowindex = 2;
            col = 1;



            //sheet.Cells[rowindex++, col].Value = item;
            foreach (var item  in AcumuladoFaltante)
            {
                foreach (var itemDatos in item.Sobrante_Faltante)
                {
                    if (itemDatos.EMPLEADO is null)
                    {
                        itemDatos.EMPLEADO = db.EMPLEADO.Where(x => x.ID_EMPLEADO == itemDatos.ID_Empleado).SingleOrDefault();
                    }
                    col = 1;
                    sheet1.Cells[rowindex, col++].Value = itemDatos.ID_Empleado;
                    sheet1.Cells[rowindex, col++].Value = itemDatos.EMPLEADO.NOMBRES + " " + itemDatos.EMPLEADO.APELLIDOS;
                    sheet1.Cells[rowindex, col++].Value = itemDatos.IDTIENDA;
                    sheet1.Cells[rowindex, col++].Value = itemDatos.FECHA.ToString("dd-MM-yyyy");
                    sheet1.Cells[rowindex, col++].Value = itemDatos.CAJA;
                    sheet1.Cells[rowindex, col++].Value = itemDatos.TIPO;
                    sheet1.Cells[rowindex, col++].Value = Math.Abs(itemDatos.TOTAL);



                    rowindex++;
                }
       
            }

            #endregion
            //rowindex = 2;
            //sheet.Column(2).Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
            //var bordes = sheet.Cells[2, 1, rowindex + 1, 13].Style.Border;
            //bordes.Top.Style = bordes.Right.Style = bordes.Bottom.Style = bordes.Left.Style = ExcelBorderStyle.Thin;

            return excelPackage;
        }

        public ExcelPackage ReporteSobranteFaltanteRRHH(DateTime Fecha)
        {
            //*************************************************************
            //FileInfo newFile = new FileInfo("C:\\excel\\Centros de costos.xlsx");
            //BORRAMOS EL ARCHIVO SI EXISTE, SI NO CREA UNO NUEVO.
            //newFile.Delete();
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            var excelPackage = new ExcelPackage();
            //Propiedades Hoja de excel
            //var zona = lst.Select(x => x.nombreZona).Distinct();


            var sheet = excelPackage.Workbook.Worksheets.Add("SobranteFalante");
            //var sheet1 = excelPackage.Workbook.Worksheets.Add("SobranteFalante");
            //sheet.Name = item;

            //********************************************************************************************************
            //encabezados tabla de datos
            #region AcumuladosSobrantefaltantes
            var rowindex = 1;

            sheet.Cells[rowindex, 1].Value = "Rut Cajero";
            sheet.Cells[rowindex, 2].Value = "Nombre Cajero";
            sheet.Cells[rowindex, 3].Value = "Tienda";
            sheet.Cells[rowindex, 4].Value = "Fecha";
            sheet.Cells[rowindex, 5].Value = "Tipo";
            sheet.Cells[rowindex, 6].Value = "Total";
            sheet.Cells[rowindex, 7].Value = "Estado";
            sheet.Cells[rowindex, 8].Value = "Cuotas";
            sheet.Cells[rowindex, 9].Value = "TipoRechazo";
            sheet.Cells[rowindex, 10].Value = "Observacion";



            sheet.Column(1).AutoFit(13);
            sheet.Column(2).AutoFit(40);
            sheet.Column(3).AutoFit(3);
            sheet.Column(4).AutoFit(13);
            sheet.Column(5).AutoFit(10);
            sheet.Column(6).AutoFit(15);
            sheet.Column(7).AutoFit(10);
            sheet.Column(8).AutoFit(10);
            sheet.Column(9).AutoFit(10);
            sheet.Column(10).AutoFit(10);



            sheet.Cells[rowindex, 1, rowindex, 10].Style.Font.Bold = true;
            sheet.Cells[rowindex, 1, rowindex, 10].AutoFilter = true;

            var bordesEncabezados = sheet.Cells[rowindex, 1, rowindex, 10].Style.Border;
            bordesEncabezados.Top.Style = bordesEncabezados.Right.Style = bordesEncabezados.Bottom.Style = bordesEncabezados.Left.Style = ExcelBorderStyle.Medium;


            // SE CARGAN LOS DATOS DE TODO EL DOCUMENTO
            //var datosPlan = lst;
            var col = 0;
            //Empezamos a escribir sobre ella
            rowindex = 2;

            col = 1;


            var AcumuladoFaltante = db.AcumuladoSobrante_Faltante.Where(x => x.Ano == Fecha.Year && x.Mes == Fecha.Month && x.CodigoTipoAceptacion == 1).ToList();


            foreach (var itemDatos in AcumuladoFaltante)
            {
                //if (itemDatos.Sobrante_Faltante.Select(x) is null)
                //{
                //    itemDatos.EMPLEADO = db.EMPLEADO.Where(x => x.ID_EMPLEADO == itemDatos.ID_Empleado).SingleOrDefault();
                //}

                var agrupadoempleado = itemDatos.Sobrante_Faltante.
                                       GroupBy(x => new { x.IDTIENDA, x.ID_Empleado, x.EMPLEADO.NOMBRES, x.EMPLEADO.APELLIDOS }).
                                       Select(f => new { f.Key.IDTIENDA, f.Key.ID_Empleado, f.Key.NOMBRES, f.Key.APELLIDOS }).SingleOrDefault();

                //var Empleado = db.EMPLEADO.Where(x => x.ID_EMPLEADO == agrupadoempleado.ID_Empleado).SingleOrDefault();

                col = 1;
                sheet.Cells[rowindex, col++].Value = agrupadoempleado.ID_Empleado;

                sheet.Cells[rowindex, col++].Value = agrupadoempleado.NOMBRES + " " + agrupadoempleado.APELLIDOS;
                sheet.Cells[rowindex, col++].Value = itemDatos.idTienda;
                sheet.Cells[rowindex, col++].Value = Fecha.ToString("MMMM");
                sheet.Cells[rowindex, col++].Value = "";
                sheet.Cells[rowindex, col++].Value = Math.Abs(itemDatos.TotalAcumulado.Value);
                sheet.Cells[rowindex, col++].Value = itemDatos.CodigoTipoAceptacion.HasValue ? itemDatos.TipoAceptacion.Nombre : "Pendiente";
                sheet.Cells[rowindex, col++].Value = itemDatos.Cuotas;
                sheet.Cells[rowindex, col++].Value = itemDatos.CodigoTipoRechazo.HasValue ? itemDatos.TipoRechazo.Nombre : "";
                sheet.Cells[rowindex, col++].Value = itemDatos.Observacion;


                rowindex++;


            }
            #endregion

            //#region SobranteFaltante
            //rowindex = 1;
            //sheet1.Cells[rowindex, 1].Value = "Rut Cajero";
            //sheet1.Cells[rowindex, 2].Value = "Nombre Cajero";
            //sheet1.Cells[rowindex, 3].Value = "Tienda";
            //sheet1.Cells[rowindex, 4].Value = "Fecha";
            //sheet1.Cells[rowindex, 5].Value = "Caja";
            //sheet1.Cells[rowindex, 6].Value = "Tipo";
            //sheet1.Cells[rowindex, 7].Value = "Total";


            //sheet1.Column(1).AutoFit(13);
            //sheet1.Column(2).AutoFit(40);
            //sheet1.Column(3).AutoFit(3);
            //sheet1.Column(4).AutoFit(10);
            //sheet1.Column(5).AutoFit(3);
            //sheet1.Column(6).AutoFit(10);
            //sheet1.Column(7).AutoFit(15);

            //sheet1.Cells[rowindex, 1, rowindex, 7].Style.Font.Bold = true;
            //sheet1.Cells[rowindex, 1, rowindex, 7].AutoFilter = true;

            //var bordesEncabezados1 = sheet1.Cells[rowindex, 1, rowindex, 7].Style.Border;
            //bordesEncabezados1.Top.Style = bordesEncabezados1.Right.Style = bordesEncabezados1.Bottom.Style = bordesEncabezados1.Left.Style = ExcelBorderStyle.Medium;

            //// SE CARGAN LOS DATOS DE TODO EL DOCUMENTO
            //col = 0;
            ////Empezamos a escribir sobre ella
            //rowindex = 2;
            //col = 1;



            ////sheet.Cells[rowindex++, col].Value = item;
            //foreach (var item in AcumuladoFaltante)
            //{
            //    foreach (var itemDatos in item.Sobrante_Faltante)
            //    {
            //        if (itemDatos.EMPLEADO is null)
            //        {
            //            itemDatos.EMPLEADO = db.EMPLEADO.Where(x => x.ID_EMPLEADO == itemDatos.ID_Empleado).SingleOrDefault();
            //        }
            //        col = 1;
            //        sheet1.Cells[rowindex, col++].Value = itemDatos.ID_Empleado;
            //        sheet1.Cells[rowindex, col++].Value = itemDatos.EMPLEADO.NOMBRES + " " + itemDatos.EMPLEADO.APELLIDOS;
            //        sheet1.Cells[rowindex, col++].Value = itemDatos.IDTIENDA;
            //        sheet1.Cells[rowindex, col++].Value = itemDatos.FECHA.ToString("dd-MM-yyyy");
            //        sheet1.Cells[rowindex, col++].Value = itemDatos.CAJA;
            //        sheet1.Cells[rowindex, col++].Value = itemDatos.TIPO;
            //        sheet1.Cells[rowindex, col++].Value = Math.Abs(itemDatos.TOTAL);



            //        rowindex++;
            //    }

            //}

            //#endregion
            //rowindex = 2;
            //sheet.Column(2).Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
            //var bordes = sheet.Cells[2, 1, rowindex + 1, 13].Style.Border;
            //bordes.Top.Style = bordes.Right.Style = bordes.Bottom.Style = bordes.Left.Style = ExcelBorderStyle.Thin;

            return excelPackage;
        }


        public ExcelPackage ReportefullNCContraloria(List<notaCredito> listaDataNC)
        {
            //NotaCredito NCDAC = new NotaCredito();
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            var excelPackage = new ExcelPackage();

            var sheet = excelPackage.Workbook.Worksheets.Add("NC");

            var rowindex = 1;
            //sheet.Cells[rowindex, 1].Value = "Trx";
            sheet.Cells[rowindex, 1].Value = "Tipo";
            sheet.Cells[rowindex, 2].Value = "IdTienda";
            sheet.Cells[rowindex, 3].Value = "Descripción Tienda";
            sheet.Cells[rowindex, 4].Value = "N° Nota de Crédito";
            sheet.Cells[rowindex, 5].Value = "Fecha NC";
            sheet.Cells[rowindex, 6].Value = "Hora emision Nota de Credito";
            sheet.Cells[rowindex, 7].Value = "Caja emisión NC";
            sheet.Cells[rowindex, 8].Value = "Rut Cliente NC ";
            sheet.Cells[rowindex, 9].Value = "Monto Nota de crédito en $";
            sheet.Cells[rowindex, 10].Value = "N° boleta";
            sheet.Cells[rowindex, 11].Value = "Fecha_boleta";
            sheet.Cells[rowindex, 12].Value = "Tienda_Origen";
            sheet.Cells[rowindex, 13].Value = "Caja_Origen";
            sheet.Cells[rowindex, 14].Value = "Monto_boleta";
            sheet.Cells[rowindex, 15].Value = "Forma de Pago_OrigenD";
            sheet.Cells[rowindex, 16].Value = "Monto_Medio de Pago";
            sheet.Cells[rowindex, 17].Value = "ID_Cajero";
            sheet.Cells[rowindex, 18].Value = "Rut  cajero";
            sheet.Cells[rowindex, 19].Value = "Nombre_Cajero";
            sheet.Cells[rowindex, 20].Value = "Rut  supervisor";
            sheet.Cells[rowindex, 21].Value = "Nombre Jefatura que autoriza";
            sheet.Cells[rowindex, 22].Value = "Hora autorización Nota de Credito";
            sheet.Cells[rowindex, 23].Value = "Monto boleta sin rouding";
            sheet.Cells[rowindex, 24].Value = "Monto boleta rouding";
            sheet.Cells[rowindex, 25].Value = "Monto boleta total";

            sheet.Column(1).AutoFit(20);
            sheet.Column(2).AutoFit(12);
            sheet.Column(3).AutoFit(13);
            sheet.Column(4).AutoFit(15);
            sheet.Column(5).AutoFit(13);
            sheet.Column(6).AutoFit(13);
            sheet.Column(7).AutoFit(15);
            sheet.Column(8).AutoFit(20);
            sheet.Column(9).AutoFit(20);
            sheet.Column(10).AutoFit(20);
            sheet.Column(11).AutoFit(20);
            sheet.Column(12).AutoFit(20);
            sheet.Column(13).AutoFit(20);
            sheet.Column(14).AutoFit(20);
            sheet.Column(15).AutoFit(20);
            sheet.Column(16).AutoFit(20);
            sheet.Column(17).AutoFit(20);
            sheet.Column(18).AutoFit(20);
            sheet.Column(19).AutoFit(20);
            sheet.Column(20).AutoFit(20);
            sheet.Column(21).AutoFit(20);
            sheet.Column(22).AutoFit(20);

            sheet.Cells[rowindex, 1, rowindex, 25].Style.Font.Bold = true;
            sheet.Cells[rowindex, 1, rowindex, 25].AutoFilter = true;
            // SE CARGAN LOS DATOS DE TODO EL DOCUMENTO
            var datosPlan = listaDataNC;
            var col = 0;

            //Empezamos a escribir sobre ella
            rowindex = 2;

            var tiendas = db.CENTROS_LOCAL.ToList();

            foreach (var i in datosPlan)
            {
                
                col = 1;
                sheet.Cells[rowindex, col++].Value = i.Tipo;
                sheet.Cells[rowindex, col++].Value = i.IdTienda;
                sheet.Cells[rowindex, col++].Value = tiendas.Where(x => x.CEN_Codigo == i.IdTienda).Select(x => x.CEN_Nombre).SingleOrDefault().ToString();
                sheet.Cells[rowindex, col++].Value = i.Folio;
                sheet.Cells[rowindex, col++].Value = i.Fecha_Emision.ToString("dd-MM-yyyy");
                sheet.Cells[rowindex, col++].Value = i.Hora_Emision;
                sheet.Cells[rowindex, col++].Value = i.Caja;
                sheet.Cells[rowindex, col++].Value = i.Cliente;
                sheet.Cells[rowindex, col++].Value = Math.Abs(i.Total).ToString("N0");
                sheet.Cells[rowindex, col++].Value = i.boletaOrigen.GroupBy(x => x.Nro_Boleta).Select(x => x.Key).SingleOrDefault();
                sheet.Cells[rowindex, col++].Value = i.boletaOrigen.GroupBy(x => x.Fecha_Emision).Select(x => x.Key).SingleOrDefault().ToString("dd-MM-yyyy");
                sheet.Cells[rowindex, col++].Value = i.boletaOrigen.GroupBy(x => x.IdTienda).Select(x => x.Key).SingleOrDefault();
                sheet.Cells[rowindex, col++].Value = i.boletaOrigen.GroupBy(x => x.Caja).Select(x => x.Key).SingleOrDefault();
                sheet.Cells[rowindex, col++].Value = "";// i.boletaOrigen.GroupBy(x => x.Monto).Select(x => x.Key).SingleOrDefault().Value.ToString("N0");
                sheet.Cells[rowindex, col++].Value = i.boletaOrigen.GroupBy(x => x.TipoDocumento).Select(x => x.Key).SingleOrDefault();
                sheet.Cells[rowindex, col++].Value = ""; // i.Montomediodepago
                sheet.Cells[rowindex, col++].Value = ""; //i.id_Cajero
                sheet.Cells[rowindex, col++].Value = i.Rut_Cajero;
                sheet.Cells[rowindex, col++].Value = i.EMPLEADO is null ? "" : i.EMPLEADO.NOMBRES + " " + i.EMPLEADO.APELLIDOS;//i.Nombre_trabajador;
                sheet.Cells[rowindex, col++].Value = i.Rut_Supervisor;
                sheet.Cells[rowindex, col++].Value = i.EMPLEADO1 is null ? "" : i.EMPLEADO1.NOMBRES + " " + i.EMPLEADO1.APELLIDOS;// i.Nombre_Jefe;
                sheet.Cells[rowindex, col++].Value = i.Hora_Aprobacion;
                sheet.Cells[rowindex, col++].Value = i.boletaOrigen.Where(x=>x.Tipo_Pago != 113).GroupBy(x => x.Tipo_Pago).Select(g => g.Sum(x => x.Monto) );
                sheet.Cells[rowindex, col++].Value = i.boletaOrigen.Where(x => x.Tipo_Pago == 113).GroupBy(x => x.Tipo_Pago).Select(g => g.Sum(x => x.Monto) );
                sheet.Cells[rowindex, col++].Value = i.boletaOrigen.Sum(x=>x.Monto);
                // FALTA AGREGAR COLUMNAS QUE AGREGARIA PATRICIO A LA TABLA DE BD
                //hora_emision_nc 
                //Monto_Medio_de_Pago 
                //id_Cajero 
                //nombre_Jefatura_Autoriza 
                //hora_autorizacion_nota_credito 
                rowindex++;
            }

            return excelPackage;
        }

        public ExcelPackage ReportefullNCCuadratura(List<fn_ReporteNcFULLCuadratura_Result> listaDataNC)
        {
            //NotaCredito NCDAC = new NotaCredito();
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            var excelPackage = new ExcelPackage();

            var sheet = excelPackage.Workbook.Worksheets.Add("NC");

            var rowindex = 1;
            //sheet.Cells[rowindex, 1].Value = "Trx";
            sheet.Cells[rowindex, 1].Value = "Tipo";
            sheet.Cells[rowindex, 2].Value = "IdTienda";
            sheet.Cells[rowindex, 3].Value = "Descripción Tienda";
            sheet.Cells[rowindex, 4].Value = "N° Nota de Crédito";
            sheet.Cells[rowindex, 5].Value = "Fecha NC";
            sheet.Cells[rowindex, 6].Value = "Hora emision Nota de Credito";
            sheet.Cells[rowindex, 7].Value = "Caja emisión NC";
            sheet.Cells[rowindex, 8].Value = "Rut Cliente NC ";
            sheet.Cells[rowindex, 9].Value = "Monto Nota de crédito en $";
            sheet.Cells[rowindex, 10].Value = "N° boleta";
            sheet.Cells[rowindex, 11].Value = "Fecha_boleta";
            sheet.Cells[rowindex, 12].Value = "Tienda_Origen";
            sheet.Cells[rowindex, 13].Value = "Caja_Origen";
            sheet.Cells[rowindex, 14].Value = "Monto_boleta";
            sheet.Cells[rowindex, 15].Value = "Forma de Pago_OrigenD";
            sheet.Cells[rowindex, 16].Value = "Descripcion";
            sheet.Cells[rowindex, 17].Value = "Rut  cajero";
            sheet.Cells[rowindex, 18].Value = "Nombre_Cajero";
            sheet.Cells[rowindex, 19].Value = "Rut  supervisor";
            sheet.Cells[rowindex, 20].Value = "Nombre Jefatura que autoriza";
            sheet.Cells[rowindex, 21].Value = "Hora autorización Nota de Credito";
  

            sheet.Column(1).AutoFit(20);
            sheet.Column(2).AutoFit(3);
            sheet.Column(3).AutoFit(13);
            sheet.Column(4).AutoFit(10);
            sheet.Column(5).AutoFit(13);
            sheet.Column(6).AutoFit(7);
            sheet.Column(7).AutoFit(7);
            sheet.Column(8).AutoFit(20);
            sheet.Column(9).AutoFit(10);
            sheet.Column(10).AutoFit(10);
            sheet.Column(11).AutoFit(10);
            sheet.Column(12).AutoFit(3);
            sheet.Column(13).AutoFit(3);
            sheet.Column(14).AutoFit(10);
            sheet.Column(15).AutoFit(3);
            sheet.Column(16).AutoFit(20);
            sheet.Column(17).AutoFit(20);
            sheet.Column(18).AutoFit(20);
            sheet.Column(19).AutoFit(20);
            sheet.Column(20).AutoFit(20);
            sheet.Column(21).AutoFit(10);
    

            sheet.Cells[rowindex, 1, rowindex, 21].Style.Font.Bold = true;
            sheet.Cells[rowindex, 1, rowindex, 21].AutoFilter = true;
            // SE CARGAN LOS DATOS DE TODO EL DOCUMENTO
            var datosPlan = listaDataNC;
            var col = 0;

            //Empezamos a escribir sobre ella
            rowindex = 2;

            var tiendas = db.CENTROS_LOCAL.ToList();

            foreach (var i in datosPlan)
            {

                col = 1;
                sheet.Cells[rowindex, col++].Value = i.Tipo;
                sheet.Cells[rowindex, col++].Value = i.IdTienda;
                sheet.Cells[rowindex, col++].Value = i.CEN_Nombre;
                sheet.Cells[rowindex, col++].Value = i.Folio;
                sheet.Cells[rowindex, col++].Value = i.Fecha_Emision.ToString("dd-MM-yyyy");
                sheet.Cells[rowindex, col++].Value = i.Hora_Emision;
                sheet.Cells[rowindex, col++].Value = i.Caja;
                sheet.Cells[rowindex, col++].Value = i.Cliente;
                sheet.Cells[rowindex, col++].Value = Math.Abs(i.Total).ToString("N0");
                sheet.Cells[rowindex, col++].Value = i.Nro_Boleta;
                sheet.Cells[rowindex, col++].Value = i.FechaEmisionBoletaOrigen.ToString("dd-MM-yyyy");
                sheet.Cells[rowindex, col++].Value = i.TiendaBoletaOrigen;
                sheet.Cells[rowindex, col++].Value = i.CajaBoletaOrigen;
                sheet.Cells[rowindex, col++].Value = Math.Abs(i.Monto.Value).ToString("N0");// i.boletaOrigen.GroupBy(x => x.Monto).Select(x => x.Key).SingleOrDefault().Value.ToString("N0");
                sheet.Cells[rowindex, col++].Value = i.Tipo_Pago;
                sheet.Cells[rowindex, col++].Value = i.Descripcion;
                sheet.Cells[rowindex, col++].Value = i.CajeroNotaCredito;
                sheet.Cells[rowindex, col++].Value = i.EmpleadoNombre + " " + i.EmpleadoApellido;//i.Nombre_trabajador;
                sheet.Cells[rowindex, col++].Value = i.NcRutSupervisor;
                sheet.Cells[rowindex, col++].Value = i.SupervisorNombre + " " + i.SupervisorApellido;// i.Nombre_Jefe;
                sheet.Cells[rowindex, col++].Value = i.Hora_Aprobacion;
               

                //sheet.Cells[rowindex, col++].Value = i.boletaOrigen.Where(x => x.Tipo_Pago != 113).GroupBy(x => x.Tipo_Pago).Select(g => g.Sum(x => x.Monto));
                //sheet.Cells[rowindex, col++].Value = i.boletaOrigen.Where(x => x.Tipo_Pago == 113).GroupBy(x => x.Tipo_Pago).Select(g => g.Sum(x => x.Monto));
                //sheet.Cells[rowindex, col++].Value = i.boletaOrigen.Sum(x => x.Monto);
                // FALTA AGREGAR COLUMNAS QUE AGREGARIA PATRICIO A LA TABLA DE BD
                //hora_emision_nc 
                //Monto_Medio_de_Pago 
                //id_Cajero 
                //nombre_Jefatura_Autoriza 
                //hora_autorizacion_nota_credito 
                rowindex++;
            }

            return excelPackage;
        }

        #region Antiguo NCFULL
        //public ExcelPackage ReportefullNCContraloria(List<fnReporteNCContraloria_Result> listaDataNC)
        //{
        //    //NotaCredito NCDAC = new NotaCredito();
        //    ExcelPackage.LicenseContext = LicenseContext.Commercial;
        //    var excelPackage = new ExcelPackage();

        //    var sheet = excelPackage.Workbook.Worksheets.Add("NC");

        //    var rowindex = 1;
        //    //sheet.Cells[rowindex, 1].Value = "Trx";
        //    sheet.Cells[rowindex, 1].Value = "Tipo";
        //    sheet.Cells[rowindex, 2].Value = "IdTienda";
        //    sheet.Cells[rowindex, 3].Value = "Descripción Tienda";
        //    sheet.Cells[rowindex, 4].Value = "N° Nota de Crédito";
        //    sheet.Cells[rowindex, 5].Value = "Fecha NC";
        //    sheet.Cells[rowindex, 6].Value = "Hora emision Nota de Credito";
        //    sheet.Cells[rowindex, 7].Value = "Caja emisión NC";
        //    sheet.Cells[rowindex, 8].Value = "Rut Cliente NC ";
        //    sheet.Cells[rowindex, 9].Value = "Monto Nota de crédito en $";
        //    sheet.Cells[rowindex, 10].Value = "N° boleta";
        //    sheet.Cells[rowindex, 11].Value = "Fecha_boleta";
        //    sheet.Cells[rowindex, 12].Value = "Tienda_Origen";
        //    sheet.Cells[rowindex, 13].Value = "Caja_Origen";
        //    sheet.Cells[rowindex, 14].Value = "Monto_boleta";
        //    sheet.Cells[rowindex, 15].Value = "Forma de Pago_OrigenD";
        //    sheet.Cells[rowindex, 16].Value = "Monto_Medio de Pago";
        //    sheet.Cells[rowindex, 17].Value = "ID_Cajero";
        //    sheet.Cells[rowindex, 18].Value = "Rut  cajero";
        //    sheet.Cells[rowindex, 19].Value = "Nombre_Cajero";
        //    sheet.Cells[rowindex, 20].Value = "Rut  supervisor";
        //    sheet.Cells[rowindex, 21].Value = "Nombre Jefatura que autoriza";
        //    sheet.Cells[rowindex, 22].Value = "Hora autorización Nota de Credito";

        //    sheet.Column(1).AutoFit(20);
        //    sheet.Column(2).AutoFit(12);
        //    sheet.Column(3).AutoFit(13);
        //    sheet.Column(4).AutoFit(15);
        //    sheet.Column(5).AutoFit(13);
        //    sheet.Column(6).AutoFit(13);
        //    sheet.Column(7).AutoFit(15);
        //    sheet.Column(8).AutoFit(20);
        //    sheet.Column(9).AutoFit(20);
        //    sheet.Column(10).AutoFit(20);
        //    sheet.Column(11).AutoFit(20);
        //    sheet.Column(12).AutoFit(20);
        //    sheet.Column(13).AutoFit(20);
        //    sheet.Column(14).AutoFit(20);
        //    sheet.Column(15).AutoFit(20);
        //    sheet.Column(16).AutoFit(20);
        //    sheet.Column(17).AutoFit(20);
        //    sheet.Column(18).AutoFit(20);
        //    sheet.Column(19).AutoFit(20);
        //    sheet.Column(20).AutoFit(20);
        //    sheet.Column(21).AutoFit(20);
        //    sheet.Column(22).AutoFit(20);

        //    // SE CARGAN LOS DATOS DE TODO EL DOCUMENTO
        //    var datosPlan = listaDataNC;
        //    var col = 0;

        //    //Empezamos a escribir sobre ella
        //    rowindex = 2;

        //    var tiendas = db.CENTROS_LOCAL.ToList();

        //    foreach (var i in datosPlan)
        //    {
        //        col = 1;
        //        sheet.Cells[rowindex, col++].Value = i.TIPONOTACREDITO;
        //        sheet.Cells[rowindex, col++].Value = i.IdTienda_notacredito;
        //        sheet.Cells[rowindex, col++].Value = tiendas.Where(x=>x.CEN_Codigo == i.IdTienda_notacredito).Select(x=>x.CEN_Nombre).SingleOrDefault().ToString();
        //        sheet.Cells[rowindex, col++].Value = i.Folio_notacredito;
        //        sheet.Cells[rowindex, col++].Value = i.Fecha_Emision_Nota_Credito.ToString("dd-MM-yyyy");
        //        sheet.Cells[rowindex, col++].Value = i.Hora_Emision_Nota_Credito;
        //        sheet.Cells[rowindex, col++].Value = i.Caja_notacredito;
        //        sheet.Cells[rowindex, col++].Value = i.Cliente_notacredito;
        //        sheet.Cells[rowindex, col++].Value = i.Total_notacredito.Value.ToString("N0");
        //        sheet.Cells[rowindex, col++].Value = i.Nro_Boleta;
        //        sheet.Cells[rowindex, col++].Value = i.Fecha_Boleta.Value.ToString("dd-MM-yyyy"); ;
        //        sheet.Cells[rowindex, col++].Value = i.IdTienda_Origen_Boleta;
        //        sheet.Cells[rowindex, col++].Value = i.Caja_Origen_Boleta;
        //        sheet.Cells[rowindex, col++].Value = i.Monto_Boleta.Value.ToString("N0");
        //        sheet.Cells[rowindex, col++].Value = i.Descripcion_pago;
        //        sheet.Cells[rowindex, col++].Value = ""; // i.Montomediodepago
        //        sheet.Cells[rowindex, col++].Value = ""; //i.id_Cajero
        //        sheet.Cells[rowindex, col++].Value = i.Rut_Cajero;
        //        sheet.Cells[rowindex, col++].Value = i.Nombre_trabajador;
        //        sheet.Cells[rowindex, col++].Value = i.Rut_Supervisor;
        //        sheet.Cells[rowindex, col++].Value = i.Nombre_Jefe;
        //        sheet.Cells[rowindex, col++].Value = i.Hora_Aprov_Nota_Credito;
        //        // FALTA AGREGAR COLUMNAS QUE AGREGARIA PATRICIO A LA TABLA DE BD
        //        //hora_emision_nc 
        //        //Monto_Medio_de_Pago 
        //        //id_Cajero 
        //        //nombre_Jefatura_Autoriza 
        //        //hora_autorizacion_nota_credito 
        //        rowindex++;
        //    }

        //    return excelPackage;
        //}
        #endregion

        public ExcelPackage ReporteTarjetaFSP(List<fnReporte_PagosFSP_Result> lst)
        {
            //*************************************************************
            //FileInfo newFile = new FileInfo("C:\\excel\\Centros de costos.xlsx");
            //BORRAMOS EL ARCHIVO SI EXISTE, SI NO CREA UNO NUEVO.
            //newFile.Delete();
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            var excelPackage = new ExcelPackage();
            //Propiedades Hoja de excel
            //var zona = lst.Select(x => x.nombreZona).Distinct();


            var sheet = excelPackage.Workbook.Worksheets.Add("Lista Pagos FSP");
            //sheet.Name = item;

            //********************************************************************************************************
            //encabezados tabla de datos
            var rowindex = 1;

            sheet.Cells[rowindex, 1].Value = "Vigencia";
            sheet.Cells[rowindex, 2].Value = "Nombre TRX";
            sheet.Cells[rowindex, 3].Value = "Local";
            sheet.Cells[rowindex, 4].Value = "Caja";
            sheet.Cells[rowindex, 5].Value = "TRX GL";
            sheet.Cells[rowindex, 6].Value = "TRX SW";
            sheet.Cells[rowindex, 7].Value = "Fecha TRX";
            sheet.Cells[rowindex, 8].Value = "Codigo Autorizacion";
            sheet.Cells[rowindex, 9].Value = "N° Tarjeta";
            sheet.Cells[rowindex, 10].Value = "Montos Global";
            sheet.Cells[rowindex, 11].Value = "Montos SW";
            sheet.Cells[rowindex, 12].Value = "N° Cuota";
            sheet.Cells[rowindex, 13].Value = "Cuenta Cliente";
            sheet.Cells[rowindex, 14].Value = "Rut Cliente";
            sheet.Cells[rowindex, 15].Value = "Vigencia";

            sheet.Column(1).AutoFit(20);
            sheet.Column(2).AutoFit(18);
            sheet.Column(3).AutoFit(13);
            sheet.Column(4).AutoFit(13);
            sheet.Column(5).AutoFit(10);
            sheet.Column(6).AutoFit(30);
            sheet.Column(7).AutoFit(13);
            sheet.Column(8).AutoFit(30);
            sheet.Column(9).AutoFit(30);
            sheet.Column(10).AutoFit(30);
            sheet.Column(11).AutoFit(30);
            sheet.Column(12).AutoFit(30);
            sheet.Column(13).AutoFit(30);
            sheet.Column(14).AutoFit(30);
            sheet.Column(15).AutoFit(30);


            sheet.Cells[rowindex, 1, rowindex, 15].Style.Font.Bold = true;
            sheet.Cells[rowindex, 1, rowindex, 15].AutoFilter = true;

            var bordesEncabezados = sheet.Cells[rowindex, 1, rowindex, 15].Style.Border;
            bordesEncabezados.Top.Style = bordesEncabezados.Right.Style = bordesEncabezados.Bottom.Style = bordesEncabezados.Left.Style = ExcelBorderStyle.Medium;


            // SE CARGAN LOS DATOS DE TODO EL DOCUMENTO
            //var datosPlan = lst;
            var col = 0;
            //Empezamos a escribir sobre ella
            rowindex = 2;

            col = 1;
            //sheet.Cells[rowindex++, col].Value = item;
            foreach (var itemDatos in lst)
            {

                col = 1;
                sheet.Cells[rowindex, col++].Value = itemDatos.VIGENCIA;
                sheet.Cells[rowindex, col++].Value = itemDatos.NOMBRETRX;
                sheet.Cells[rowindex, col++].Value = itemDatos.LOCAL;
                sheet.Cells[rowindex, col++].Value = itemDatos.CAJA;
                sheet.Cells[rowindex, col++].Value = itemDatos.GLOBALTRX;
                sheet.Cells[rowindex, col++].Value = itemDatos.SWITCHTRX;
                sheet.Cells[rowindex, col++].Value = itemDatos.FECHA;
                sheet.Cells[rowindex, col++].Value = itemDatos.CODIGOAUTORIZACION;
                sheet.Cells[rowindex, col++].Value = itemDatos.NROTARJETA;
                sheet.Cells[rowindex, col++].Value = Math.Round(decimal.Parse(itemDatos.MONTOGLOBAL.Replace(".", ",")));
                sheet.Cells[rowindex, col++].Value = Math.Round(decimal.Parse(itemDatos.MONTOSWITCH.Replace(".", ",")));
                sheet.Cells[rowindex, col++].Value = itemDatos.NRO_CUOTAS;
                sheet.Cells[rowindex, col++].Value = itemDatos.CUENTA;
                sheet.Cells[rowindex, col++].Value = itemDatos.RUT_CLIENTE;
                sheet.Cells[rowindex, col++].Value = itemDatos.VIGENCIA2;
                rowindex++;
            }


            //rowindex = 2;
            sheet.Column(2).Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
            var bordes = sheet.Cells[2, 1, rowindex + 1, 15].Style.Border;
            bordes.Top.Style = bordes.Right.Style = bordes.Bottom.Style = bordes.Left.Style = ExcelBorderStyle.Thin;

            return excelPackage;
        }

        public ExcelPackage ReportConciliadasM(Conciliacion lst)
        {
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            var excelPackage = new ExcelPackage();

            var sheet = excelPackage.Workbook.Worksheets.Add("Lista Conciliadas Manual ");
            //sheet.Name = item.ToString();
            var rowindex = 1;

            sheet.Cells[rowindex, 1].Value = "Autorizador";
            sheet.Cells[rowindex, 2].Value = "Fecha Transanccion";
            sheet.Cells[rowindex, 3].Value = "CodigoLocal";
            sheet.Cells[rowindex, 4].Value = "Tienda";
            sheet.Cells[rowindex, 5].Value = "Monto MP";
            sheet.Cells[rowindex, 6].Value = "Monto TBK";
            sheet.Cells[rowindex, 7].Value = "Fecha Conciliacion";
            sheet.Cells[rowindex, 8].Value = "Tarjeta MP";
            sheet.Cells[rowindex, 9].Value = "Tarjeta TBK";

            sheet.Column(1).AutoFit(20);
            sheet.Column(2).AutoFit(12);
            sheet.Column(3).AutoFit(13);
            sheet.Column(4).AutoFit(7);
            sheet.Column(5).AutoFit(13);
            sheet.Column(6).AutoFit(13);
            sheet.Column(7).AutoFit(15);
            sheet.Column(8).AutoFit(20);
            sheet.Column(9).AutoFit(20);

            sheet.Cells[rowindex, 1, rowindex, 9].Style.Font.Bold = true;
            sheet.Cells[rowindex, 1, rowindex, 9].AutoFilter = true;

            var bordesEncabezados = sheet.Cells[rowindex, 1, rowindex, 9].Style.Border;
            bordesEncabezados.Top.Style = bordesEncabezados.Right.Style = bordesEncabezados.Bottom.Style = bordesEncabezados.Left.Style = ExcelBorderStyle.Medium;


            // SE CARGAN LOS DATOS DE TODO EL DOCUMENTO
            var datosPlan = lst;
            var col = 0;
            //Empezamos a escribir sobre ella
            rowindex = 2;
            col = 1;
            //sheet.Cells[rowindex++, col].Value = item;
            foreach (var itemDatos in datosPlan.DetalleConciliacion.Where(x => x.TipoConciliacion == "M"))
            {
                col = 1;
                sheet.Cells[rowindex, col++].Value = itemDatos.AutorizadorTbk;
                sheet.Cells[rowindex, col++].Value = itemDatos.FechaTbk.Value.ToString("dd-MM-yyyy");
                sheet.Cells[rowindex, col++].Value = itemDatos.CodigoLocal;
                sheet.Cells[rowindex, col++].Value = itemDatos.IdTienda;
                sheet.Cells[rowindex, col++].Value = itemDatos.MontoMp.Value;
                sheet.Cells[rowindex, col++].Value = itemDatos.MontoTbk.Value;
                sheet.Cells[rowindex, col++].Value = datosPlan.FechaConciliar.Value;
                sheet.Cells[rowindex, col++].Value = itemDatos.TarjetaTBk;
                sheet.Cells[rowindex, col++].Value = itemDatos.TarjetaMp;

                rowindex++;
            }
            sheet.Cells[rowindex, 4].Value = "Total:";
            sheet.Cells[rowindex, 4].Style.Font.Bold = true;
            sheet.Cells[rowindex, 5].Formula = "=Sum(" + (object)sheet.Cells[2, 5] + ":" + (object)sheet.Cells[rowindex - 1, 5] + ")";
            sheet.Cells[rowindex, 6].Formula = "=Sum(" + sheet.Cells[2, 6] + ":" + sheet.Cells[rowindex - 1, 6] + ")";

            sheet.Column(2).Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
            sheet.Column(7).Style.Numberformat.Format = "dd-MM-yyyy hh:mm";

            var startRow = Math.Max(2, rowindex - 1);
            var endRow = Math.Max(2, rowindex - 1);

            var bordes = sheet.Cells[startRow, 1, endRow, 9].Style.Border;
            bordes.Top.Style = bordes.Right.Style = bordes.Bottom.Style = bordes.Left.Style = ExcelBorderStyle.Thin;

            //var bordes = sheet.Cells[2, 1, rowindex - 1, 9].Style.Border;
            //bordes.Top.Style = bordes.Right.Style = bordes.Bottom.Style = bordes.Left.Style = ExcelBorderStyle.Thin;
            var bordesTotal = sheet.Cells[rowindex, 4, rowindex, 6].Style.Border;
            bordesTotal.Top.Style = bordesTotal.Right.Style = bordesTotal.Bottom.Style = bordesTotal.Left.Style = ExcelBorderStyle.Thin;
            return excelPackage;
        }


        public ExcelPackage ReportConciliadasA(Conciliacion lst)
        {
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            var excelPackage = new ExcelPackage();

            var sheet = excelPackage.Workbook.Worksheets.Add("Lista Conciliadas Automatica ");
            //sheet.Name = item.ToString();
            var rowindex = 1;

            sheet.Cells[rowindex, 1].Value = "Autorizador";
            sheet.Cells[rowindex, 2].Value = "Fecha Transanccion";
            sheet.Cells[rowindex, 3].Value = "CodigoLocal";
            sheet.Cells[rowindex, 4].Value = "Tienda";
            sheet.Cells[rowindex, 5].Value = "Monto MP";
            sheet.Cells[rowindex, 6].Value = "Monto TBK";
            sheet.Cells[rowindex, 7].Value = "Fecha Conciliacion";
            sheet.Cells[rowindex, 8].Value = "Tarjeta MP";
            sheet.Cells[rowindex, 9].Value = "Tarjeta TBK";

            sheet.Column(1).AutoFit(20);
            sheet.Column(2).AutoFit(12);
            sheet.Column(3).AutoFit(13);
            sheet.Column(4).AutoFit(7);
            sheet.Column(5).AutoFit(13);
            sheet.Column(6).AutoFit(13);
            sheet.Column(7).AutoFit(15);
            sheet.Column(8).AutoFit(20);
            sheet.Column(9).AutoFit(20);

            sheet.Cells[rowindex, 1, rowindex, 9].Style.Font.Bold = true;
            sheet.Cells[rowindex, 1, rowindex, 9].AutoFilter = true;

            var bordesEncabezados = sheet.Cells[rowindex, 1, rowindex, 9].Style.Border;
            bordesEncabezados.Top.Style = bordesEncabezados.Right.Style = bordesEncabezados.Bottom.Style = bordesEncabezados.Left.Style = ExcelBorderStyle.Medium;


            // SE CARGAN LOS DATOS DE TODO EL DOCUMENTO
            var datosPlan = lst;
            var col = 0;
            //Empezamos a escribir sobre ella
            rowindex = 2;
            col = 1;
            //sheet.Cells[rowindex++, col].Value = item;
            foreach (var itemDatos in datosPlan.DetalleConciliacion.Where(x => x.TipoConciliacion == "A"))
            {
                col = 1;
                sheet.Cells[rowindex, col++].Value = itemDatos.AutorizadorTbk;
                sheet.Cells[rowindex, col++].Value = itemDatos.FechaTbk.Value;
                sheet.Cells[rowindex, col++].Value = itemDatos.CodigoLocal;
                sheet.Cells[rowindex, col++].Value = itemDatos.IdTienda;
                sheet.Cells[rowindex, col++].Value = itemDatos.MontoTbk.Value;
                sheet.Cells[rowindex, col++].Value = itemDatos.MontoTbk.Value;
                sheet.Cells[rowindex, col++].Value = datosPlan.FechaConciliar;
                sheet.Cells[rowindex, col++].Value = itemDatos.TarjetaMp;
                sheet.Cells[rowindex, col++].Value = itemDatos.TarjetaTBk;

                rowindex++;
            }
            sheet.Cells[rowindex, 4].Value = "Total:";
            sheet.Cells[rowindex, 4].Style.Font.Bold = true;
            sheet.Cells[rowindex, 5].Formula = "=Sum(" + (object)sheet.Cells[2, 5] + ":" + (object)sheet.Cells[rowindex - 1, 5] + ")";
            sheet.Cells[rowindex, 6].Formula = "=Sum(" + sheet.Cells[2, 6] + ":" + sheet.Cells[rowindex - 1, 6] + ")";

            sheet.Column(2).Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
            sheet.Column(7).Style.Numberformat.Format = "dd-MM-yyyy hh:mm";
            var bordes = sheet.Cells[2, 1, rowindex - 1, 9].Style.Border;
            bordes.Top.Style = bordes.Right.Style = bordes.Bottom.Style = bordes.Left.Style = ExcelBorderStyle.Thin;
            var bordesTotal = sheet.Cells[rowindex, 4, rowindex, 6].Style.Border;
            bordesTotal.Top.Style = bordesTotal.Right.Style = bordesTotal.Bottom.Style = bordesTotal.Left.Style = ExcelBorderStyle.Thin;
            return excelPackage;
        }

        public ExcelPackage IdocMPDif(Conciliacion Dif)
        {
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            var excelPackage = new ExcelPackage();

            var sheet = excelPackage.Workbook.Worksheets.Add("FI");
            //sheet.Name = item.ToString();
            var rowindex = 1;

            //sheet.Cells[rowindex, 1].Value = "Autorizador";
            //sheet.Cells[rowindex, 2].Value = "Fecha Transanccion";
            //sheet.Cells[rowindex, 3].Value = "CodigoLocal";
            //sheet.Cells[rowindex, 4].Value = "Tienda";
            //sheet.Cells[rowindex, 5].Value = "Monto MP";
            //sheet.Cells[rowindex, 6].Value = "Monto TBK";
            //sheet.Cells[rowindex, 7].Value = "Fecha Conciliacion";
            //sheet.Cells[rowindex, 8].Value = "Tarjeta MP";
            //sheet.Cells[rowindex, 9].Value = "Tarjeta TBK";

            //sheet.Column(1).AutoFit(20);
            //sheet.Column(2).AutoFit(12);
            //sheet.Column(3).AutoFit(13);
            //sheet.Column(4).AutoFit(7);
            //sheet.Column(5).AutoFit(13);
            //sheet.Column(6).AutoFit(13);
            //sheet.Column(7).AutoFit(15);
            //sheet.Column(8).AutoFit(20);
            //sheet.Column(9).AutoFit(20);

            //sheet.Cells[rowindex, 1, rowindex, 9].Style.Font.Bold = true;
            //sheet.Cells[rowindex, 1, rowindex, 9].AutoFilter = true;

            //var bordesEncabezados = sheet.Cells[rowindex, 1, rowindex, 9].Style.Border;
            //bordesEncabezados.Top.Style = bordesEncabezados.Right.Style = bordesEncabezados.Bottom.Style = bordesEncabezados.Left.Style = ExcelBorderStyle.Medium;


            // SE CARGAN LOS DATOS DE TODO EL DOCUMENTO
            var datosPlan = Dif;
            var col = 0;
            //var NITEM = ;

            //Empezamos a escribir sobre ella
            rowindex = 1;
            col = 1;
            //sheet.Cells[rowindex++, col].Value = item;
            foreach (var itemDatos in datosPlan.DetalleConciliacion.Where(x => x.TipoConciliacion == "D"))
            {

                col = 1;
                sheet.Cells[rowindex, col++].Value = 400;
                sheet.Cells[rowindex, col++].Value = "CI";

                sheet.Cells[rowindex, col++].Value = itemDatos.FechaTbk.HasValue ? itemDatos.FechaTbk.Value.ToString("ddMMyyyy") : itemDatos.FechaMp.Value.ToString("ddMMyyyy") /* itemDatos.FechaTbk.Value.ToString("ddMMyyyy")*/;
                sheet.Cells[rowindex, col++].Value = 1;
                sheet.Cells[rowindex, col++].Value = rowindex;
                sheet.Cells[rowindex, col++].Value = "BP02";
                sheet.Cells[rowindex, col++].Value = "MP";
                sheet.Cells[rowindex, col++].Value = "CLP";
                sheet.Cells[rowindex, col++].Value = itemDatos.FechaTbk.HasValue ? itemDatos.FechaTbk.Value.ToString("ddMMyyyy") : itemDatos.FechaMp.Value.ToString("ddMMyyyy");
                sheet.Cells[rowindex, col++].Value = itemDatos.FechaTbk.HasValue ? itemDatos.FechaTbk.Value.ToString("ddMMyyyy") : itemDatos.FechaMp.Value.ToString("ddMMyyyy");
                sheet.Cells[rowindex, col++].Value = itemDatos.FechaTbk.HasValue ? itemDatos.FechaTbk.Value.Month : itemDatos.FechaMp.Value.Month;
                sheet.Cells[rowindex, col++].Value = itemDatos.ACREEDOR_REL.Local;
                sheet.Cells[rowindex, col++].Value = itemDatos.Cuenta.Descripcion;
                sheet.Cells[rowindex, col++].Value = "";
                sheet.Cells[rowindex, col++].Value = itemDatos.TipoAsiento;
                sheet.Cells[rowindex, col++].Value = itemDatos.Cuenta.NumeroCuenta.Trim();
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "";
                sheet.Cells[rowindex, col++].Value = itemDatos.MontoTbk.HasValue ? itemDatos.MontoTbk.Value : Math.Abs(itemDatos.MontoMp.Value);
                sheet.Cells[rowindex, col++].Value = "";
                sheet.Cells[rowindex, col++].Value = "C0";
                sheet.Cells[rowindex, col++].Value = "";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "";
                sheet.Cells[rowindex, col++].Value = "";
                sheet.Cells[rowindex, col++].Value = "";
                sheet.Cells[rowindex, col++].Value = itemDatos.Cuenta.Descripcion;
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = itemDatos.FechaTbk.HasValue ? itemDatos.FechaTbk.Value.ToString("ddMMyyyy") : itemDatos.FechaMp.Value.ToString("ddMMyyyy");
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                if (itemDatos.Cuenta.NumeroCuenta.Trim() == "3203000006")
                {
                    sheet.Cells[rowindex, col++].Value = itemDatos.ACREEDOR_REL.CeCo;
                }
                else
                {
                    sheet.Cells[rowindex, col++].Value = "/";
                }
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "T" + (itemDatos.ACREEDOR_REL.Local.ToString().Length == 3 ? itemDatos.ACREEDOR_REL.Local.ToString() : itemDatos.ACREEDOR_REL.Local.ToString().Length == 2 ? "0" + itemDatos.ACREEDOR_REL.Local.ToString() : itemDatos.ACREEDOR_REL.Local.ToString().Length == 1 ? "00" : "000");
                rowindex++;
            }

            return excelPackage;
        }

        public ExcelPackage IdocNCPremisas(NotaCreditoView NcList)
        {
            NotaCredito NCDAC = new NotaCredito();
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            var excelPackage = new ExcelPackage();

            var sheet = excelPackage.Workbook.Worksheets.Add("NC");

            // FILTRAMOS LOS DATOS QUE SE VAN AL DOCUMENTO 
            var dataFinal = new List<notaCreditoDTO>();
            dataFinal.AddRange(NCDAC.getNCListCheck(NcList.ListPremisa1));
            dataFinal.AddRange(NCDAC.getNCListCheck(NcList.ListPremisa2));
            dataFinal.AddRange(NCDAC.getNCListCheck(NcList.ListPremisa3));
            dataFinal.AddRange(NCDAC.getNCListCheck(NcList.ListPremisa4));

            var dataFinalAgrupada = dataFinal.GroupBy(i => new
            {
                TRX = i.TRX,
                Fecha_Emision_Nota_Credito = i.Fecha_Emision_Nota_Credito,
                IdTienda_notacredito = i.IdTienda_notacredito,
                Caja_notacredito = i.Caja_notacredito,
                TIPONOTACREDITO = i.TIPONOTACREDITO,
                Folio_notacredito = i.Folio_notacredito,
                Cliente_notacredito = i.Cliente_notacredito,
                Documento_Origen = i.Documento_Origen,
                //Total_notacredito = i.Total_notacredito.Value.sum
                Trx_Origen = i.Trx_Origen,
                Nro_Boleta = i.Nro_Boleta,
                IdTienda_Origen_Boleta = i.IdTienda_Origen_Boleta,
                Caja_Origen_Boleta = i.Caja_Origen_Boleta,
                Fecha_Boleta = i.Fecha_Boleta,
                Monto_Boleta = i.Monto_Boleta,
                Rut_Cajero = i.Rut_Cajero,
                Rut_Supervisor = i.Rut_Supervisor,
                Nombre_trabajador = i.Nombre_trabajador,
                //Descripcion_pago = i.Descripcion_pago,
                tipo_premisa = i.tipo_premisa
            }).Select(x => new notaCreditoDTO()
            {
                TRX = x.Key.TRX,
                Total_notacredito = x.Sum(y => y.Total_notacredito),
                Fecha_Emision_Nota_Credito = x.Key.Fecha_Emision_Nota_Credito,
                IdTienda_notacredito = x.Key.IdTienda_notacredito,
                Caja_notacredito = x.Key.Caja_notacredito,
                TIPONOTACREDITO = x.Key.TIPONOTACREDITO,
                Folio_notacredito = x.Key.Folio_notacredito,
                Cliente_notacredito = x.Key.Cliente_notacredito,
                Documento_Origen = x.Key.Documento_Origen,
                //Total_notacredito = i.Total_notacredito.Value.sum
                Trx_Origen = x.Key.Trx_Origen,
                Nro_Boleta = x.Key.Nro_Boleta,
                IdTienda_Origen_Boleta = x.Key.IdTienda_Origen_Boleta,
                Caja_Origen_Boleta = x.Key.Caja_Origen_Boleta,
                Fecha_Boleta = x.Key.Fecha_Boleta,
                Monto_Boleta = x.Key.Monto_Boleta,
                Rut_Cajero = x.Key.Rut_Cajero,
                Rut_Supervisor = x.Key.Rut_Supervisor,
                Nombre_trabajador = x.Key.Nombre_trabajador,
                Tipo_Pago_Origen = dataFinal.Where(w => w.TRX == x.Key.TRX && w.Tipo_Pago_Origen != 113).Select(w => w.Tipo_Pago_Origen).FirstOrDefault(),
                //Descripcion_pago = i.Descripcion_pago,
                tipo_premisa = x.Key.tipo_premisa


            }).ToList();

            var cuenta = db.Cuenta.ToList();
            var rowindex = 1;
            // SE CARGAN LOS DATOS DE TODO EL DOCUMENTO
            var datosPlan = dataFinal;
            var col = 0;
            //var NITEM = ;

            //Empezamos a escribir sobre ella
            rowindex = 1;
            col = 1;
            //sheet.Cells[rowindex++, col].Value = item;
            var nombrePago = string.Empty;
            var acreedorList = db.ACREEDOR_REL.ToList();

            int contadorCorrelativo = 1;
            foreach (var itemDatos in dataFinalAgrupada)
            {
                nombrePago = cuenta.Where(x => x.TipoPago == itemDatos.Tipo_Pago_Origen).Select(x => x.Descripcion).SingleOrDefault();
                //Reconocemos Nota Credito (tienda)
                col = 1;
                sheet.Cells[rowindex, col++].Value = 400;
                sheet.Cells[rowindex, col++].Value = "CI";
                sheet.Cells[rowindex, col++].Value = itemDatos.Fecha_Emision_Nota_Credito != null ? itemDatos.Fecha_Emision_Nota_Credito.Value.ToString("ddMMyyyy") : "";
                sheet.Cells[rowindex, col++].Value = contadorCorrelativo;
                sheet.Cells[rowindex, col++].Value = 1;
                sheet.Cells[rowindex, col++].Value = "BP02";
                sheet.Cells[rowindex, col++].Value = "AB";
                sheet.Cells[rowindex, col++].Value = "CLP";
                sheet.Cells[rowindex, col++].Value = itemDatos.Fecha_Emision_Nota_Credito != null ? itemDatos.Fecha_Emision_Nota_Credito.Value.ToString("ddMMyyyy") : "";
                sheet.Cells[rowindex, col++].Value = itemDatos.Fecha_Emision_Nota_Credito != null ? itemDatos.Fecha_Emision_Nota_Credito.Value.ToString("ddMMyyyy") : "";
                sheet.Cells[rowindex, col++].Value = itemDatos.Fecha_Emision_Nota_Credito != null ? itemDatos.Fecha_Emision_Nota_Credito.Value.ToString("MM") : "";
                sheet.Cells[rowindex, col++].Value = itemDatos.Folio_notacredito;
                sheet.Cells[rowindex, col++].Value = "NOTA CREDITO " + nombrePago;//cuenta.Where(x => x.TipoPago == itemDatos.Tipo_Pago_Origen).Select(x => x.Descripcion).ToString();//itemDatos.Tipo_Pago_Origen == 40 ? "NOTA CREDITO TFP" : "NOTA CREDITO TBK"; //
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "01";
                sheet.Cells[rowindex, col++].Value = "T" + (itemDatos.IdTienda_notacredito.ToString().LongCount() == 1 ? "00" + itemDatos.IdTienda_notacredito : itemDatos.IdTienda_notacredito.ToString().LongCount() == 2 ? "0" + itemDatos.IdTienda_notacredito : "000");//TIenda con formato T000
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = Math.Round(Math.Abs(itemDatos.Total_notacredito.Value));
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "NOTA CREDITO " + nombrePago;//cuenta.Where(x => x.TipoPago == itemDatos.Tipo_Pago_Origen).Select(x => x.Descripcion).ToString();//itemDatos.Tipo_Pago_Origen == 40 ? "NOTA CREDITO TFP" : "NOTA CREDITO TBK";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "T" + (itemDatos.IdTienda_notacredito.ToString().LongCount() == 1 ? "00" + itemDatos.IdTienda_notacredito : itemDatos.IdTienda_notacredito.ToString().LongCount() == 2 ? "0" + itemDatos.IdTienda_notacredito : "000");//TIenda con formato T000

                rowindex++;
                contadorCorrelativo++;


            }
            contadorCorrelativo = 1;
            foreach (var itemDatos in dataFinalAgrupada)
            {
                nombrePago = cuenta.Where(x => x.TipoPago == itemDatos.Tipo_Pago_Origen).Select(x => x.Descripcion).SingleOrDefault();
                //SE imputa al local (numero cuenta)
                col = 1;
                sheet.Cells[rowindex, col++].Value = 400;
                sheet.Cells[rowindex, col++].Value = "CI";
                sheet.Cells[rowindex, col++].Value = itemDatos.Fecha_Emision_Nota_Credito != null ? itemDatos.Fecha_Emision_Nota_Credito.Value.ToString("ddMMyyyy") : "";
                sheet.Cells[rowindex, col++].Value = contadorCorrelativo;
                sheet.Cells[rowindex, col++].Value = 1;
                sheet.Cells[rowindex, col++].Value = "BP02";
                sheet.Cells[rowindex, col++].Value = "AB";
                sheet.Cells[rowindex, col++].Value = "CLP";
                sheet.Cells[rowindex, col++].Value = itemDatos.Fecha_Emision_Nota_Credito != null ? itemDatos.Fecha_Emision_Nota_Credito.Value.ToString("ddMMyyyy") : "";
                sheet.Cells[rowindex, col++].Value = itemDatos.Fecha_Emision_Nota_Credito != null ? itemDatos.Fecha_Emision_Nota_Credito.Value.ToString("ddMMyyyy") : "";
                sheet.Cells[rowindex, col++].Value = itemDatos.Fecha_Emision_Nota_Credito != null ? itemDatos.Fecha_Emision_Nota_Credito.Value.ToString("MM") : "";
                sheet.Cells[rowindex, col++].Value = itemDatos.Folio_notacredito;
                sheet.Cells[rowindex, col++].Value = "NOTA CREDITO " + nombrePago;//cuenta.Where(x => x.TipoPago == itemDatos.Tipo_Pago_Origen).Select(x => x.Descripcion).ToString(); //itemDatos.Tipo_Pago_Origen == 40 ? "NOTA CREDITO TFP" : "NOTA CREDITO TBK"; //
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "50";
                sheet.Cells[rowindex, col++].Value = cuenta.Where(x => x.TipoPago == itemDatos.Tipo_Pago_Origen).Select(x => x.NumeroCuenta);//itemDatos.Tipo_Pago_Origen == 40 ? cuenta.Where(x=> x.Codigo == 3).Select(x=>x.NumeroCuenta) : cuenta.Where(x => x.Codigo == 2).Select(x => x.NumeroCuenta);//Se busca CUenta dependiendo del tipo pago origen
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = Math.Round(Math.Abs(itemDatos.Total_notacredito.Value));
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "NOTA CREDITO " + nombrePago;//cuenta.Where(x => x.TipoPago == itemDatos.Tipo_Pago_Origen).Select(x => x.Descripcion).ToString();//itemDatos.Tipo_Pago_Origen == 40 ? "NOTA CREDITO TFP" : "NOTA CREDITO TBK";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "T" + (itemDatos.IdTienda_notacredito.ToString().LongCount() == 1 ? "00" + itemDatos.IdTienda_notacredito : itemDatos.IdTienda_notacredito.ToString().LongCount() == 2 ? "0" + itemDatos.IdTienda_notacredito : "000");//TIenda con formato T000



                rowindex++;
                contadorCorrelativo++;
            }

            return excelPackage;
        }

        public ExcelPackage IdocMPDifPagos(ConciliaciondePagos Dif)
        {
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            var excelPackage = new ExcelPackage();

            var sheet = excelPackage.Workbook.Worksheets.Add("FI");
            //sheet.Name = item.ToString();
            var rowindex = 1;




            // SE CARGAN LOS DATOS DE TODO EL DOCUMENTO
            var datosPlan = Dif;
            var col = 0;
            //var NITEM = ;

            //Empezamos a escribir sobre ella
            rowindex = 1;
            col = 1;
            //sheet.Cells[rowindex++, col].Value = item;
            foreach (var itemDatos in datosPlan.DetalleConciliacionPagos.Where(x => x.TipoConciliacion == "D"))
            {
                col = 1;
                sheet.Cells[rowindex, col++].Value = 400;
                sheet.Cells[rowindex, col++].Value = "CI";
                sheet.Cells[rowindex, col++].Value = itemDatos.FechaTbk.HasValue ? itemDatos.FechaTbk.Value.ToString("ddMMyyyy") : itemDatos.FechaMp.Value.ToString("ddMMyyyy");
                sheet.Cells[rowindex, col++].Value = 1;
                sheet.Cells[rowindex, col++].Value = rowindex;
                sheet.Cells[rowindex, col++].Value = "BP04";
                sheet.Cells[rowindex, col++].Value = "MP";
                sheet.Cells[rowindex, col++].Value = "CLP";
                sheet.Cells[rowindex, col++].Value = itemDatos.FechaTbk.HasValue ? itemDatos.FechaTbk.Value.ToString("ddMMyyyy") : itemDatos.FechaMp.Value.ToString("ddMMyyyy");
                sheet.Cells[rowindex, col++].Value = itemDatos.FechaTbk.HasValue ? itemDatos.FechaTbk.Value.ToString("ddMMyyyy") : itemDatos.FechaMp.Value.ToString("ddMMyyyy");
                sheet.Cells[rowindex, col++].Value = itemDatos.FechaTbk.HasValue ? itemDatos.FechaTbk.Value.Month : itemDatos.FechaMp.Value.Month;
                sheet.Cells[rowindex, col++].Value = itemDatos.ACREEDOR_REL.Local;
                sheet.Cells[rowindex, col++].Value = itemDatos.Cuenta.Descripcion;
                sheet.Cells[rowindex, col++].Value = "";
                sheet.Cells[rowindex, col++].Value = itemDatos.TipoAsiento;
                sheet.Cells[rowindex, col++].Value = itemDatos.Cuenta.NumeroCuenta.Trim();
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = itemDatos.MontoTbk.HasValue ? itemDatos.MontoTbk.Value : Math.Abs(itemDatos.MontoMp.Value);
                sheet.Cells[rowindex, col++].Value = "";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "";
                sheet.Cells[rowindex, col++].Value = "";
                sheet.Cells[rowindex, col++].Value = itemDatos.FechaTbk.HasValue ? itemDatos.FechaTbk.Value.ToString("ddMMyyyy") : itemDatos.FechaMp.Value.ToString("ddMMyyyy");
                sheet.Cells[rowindex, col++].Value = itemDatos.Cuenta.Descripcion;
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                if (itemDatos.Cuenta.NumeroCuenta.Trim() == "3203000006")
                {
                    sheet.Cells[rowindex, col++].Value = itemDatos.ACREEDOR_REL.CeCo;
                }
                else if (itemDatos.Cuenta.NumeroCuenta.Trim() == "1104000133")
                {
                    sheet.Cells[rowindex, col++].Value = itemDatos.ACREEDOR_REL.CeCo;
                }
                else
                {
                    sheet.Cells[rowindex, col++].Value = "/";
                }
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "/";
                sheet.Cells[rowindex, col++].Value = "T" + (itemDatos.ACREEDOR_REL.Local.ToString().Length == 3 ? itemDatos.ACREEDOR_REL.Local.ToString() : itemDatos.ACREEDOR_REL.Local.ToString().Length == 2 ? "0" + itemDatos.ACREEDOR_REL.Local.ToString() : itemDatos.ACREEDOR_REL.Local.ToString().Length == 1 ? "00" : "000");
                rowindex++;
            }


            return excelPackage;
        }


        #region ConciliacionPagos
        public ExcelPackage ReportConciliadasAPagos(ConciliaciondePagos Pago)
        {
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            var excelPackage = new ExcelPackage();
            var sheet = excelPackage.Workbook.Worksheets.Add("Conciliadas Automaticas Pago");
            //sheet.Name = item.ToString();
            var rowindex = 1;
            sheet.Cells[rowindex, 1].Value = "Autorizador";
            sheet.Cells[rowindex, 2].Value = "Fecha Transanccion";
            sheet.Cells[rowindex, 3].Value = "CodigoLocal";
            sheet.Cells[rowindex, 4].Value = "Tienda";
            sheet.Cells[rowindex, 5].Value = "Monto MP";
            sheet.Cells[rowindex, 6].Value = "Monto TBK";
            sheet.Cells[rowindex, 7].Value = "Fecha Conciliacion";
            sheet.Cells[rowindex, 8].Value = "Tarjeta MP";
            sheet.Cells[rowindex, 9].Value = "Tarjeta TBK";

            sheet.Column(1).AutoFit(20);
            sheet.Column(2).AutoFit(12);
            sheet.Column(3).AutoFit(13);
            sheet.Column(4).AutoFit(7);
            sheet.Column(5).AutoFit(13);
            sheet.Column(6).AutoFit(13);
            sheet.Column(7).AutoFit(15);
            sheet.Column(8).AutoFit(20);
            sheet.Column(9).AutoFit(20);

            sheet.Cells[rowindex, 1, rowindex, 9].Style.Font.Bold = true;
            sheet.Cells[rowindex, 1, rowindex, 9].AutoFilter = true;

            var bordesEncabezados = sheet.Cells[rowindex, 1, rowindex, 8].Style.Border;
            bordesEncabezados.Top.Style = bordesEncabezados.Right.Style = bordesEncabezados.Bottom.Style = bordesEncabezados.Left.Style = ExcelBorderStyle.Medium;

            // SE CARGAN LOS DATOS DE TODO EL DOCUMENTO
            var datosPlan = Pago;
            var col = 0;
            //Empezamos a escribir sobre ella
            rowindex = 2;
            col = 1;
            //sheet.Cells[rowindex++, col].Value = item;
            foreach (var itemDatos in datosPlan.DetalleConciliacionPagos.Where(x => x.TipoConciliacion == "A"))
            {
                col = 1;
                sheet.Cells[rowindex, col++].Value = itemDatos.AutorizadorTbk;
                sheet.Cells[rowindex, col++].Value = itemDatos.FechaTbk.Value.ToString("dd-MM-yyyy");
                sheet.Cells[rowindex, col++].Value = itemDatos.CodigoLocal;
                sheet.Cells[rowindex, col++].Value = itemDatos.IdTienda;
                sheet.Cells[rowindex, col++].Value = itemDatos.MontoTbk.Value;
                sheet.Cells[rowindex, col++].Value = itemDatos.MontoTbk.Value;
                sheet.Cells[rowindex, col++].Value = datosPlan.FechaConciliar.Value.ToString("dd-MM-yyyy");
                sheet.Cells[rowindex, col++].Value = itemDatos.TarjetaMp;
                sheet.Cells[rowindex, col++].Value = itemDatos.TarjetaTBk;


                rowindex++;
            }
            sheet.Cells[rowindex, 4].Value = "Total:";
            sheet.Cells[rowindex, 4].Style.Font.Bold = true;
            sheet.Cells[rowindex, 6].Formula = "=Sum(" + (object)sheet.Cells[2, 6] + ":" + (object)sheet.Cells[rowindex - 1, 6] + ")";
            //sheet.Cells[rowindex, 6].Formula = "=Sum(" + sheet.Cells[2, 6] + ":" + sheet.Cells[rowindex - 1, 6] + ")";
            sheet.Column(7).Style.Numberformat.Format = "dd-MM-yyyy hh:mm";
            var bordes = sheet.Cells[2, 1, rowindex, 9].Style.Border;
            bordes.Top.Style = bordes.Right.Style = bordes.Bottom.Style = bordes.Left.Style = ExcelBorderStyle.Thin;
            var bordesTotal = sheet.Cells[rowindex, 4, rowindex, 9].Style.Border;
            bordesTotal.Top.Style = bordesTotal.Right.Style = bordesTotal.Bottom.Style = bordesTotal.Left.Style = ExcelBorderStyle.Thin;
            return excelPackage;
        }

        public ExcelPackage ReportConciliadasMPagos(ConciliaciondePagos lst)
        {
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            var excelPackage = new ExcelPackage();

            var sheet = excelPackage.Workbook.Worksheets.Add("Lista Conciliadas Manual ");
            //sheet.Name = item.ToString();
            var rowindex = 1;

            sheet.Cells[rowindex, 1].Value = "Autorizador";
            sheet.Cells[rowindex, 2].Value = "Fecha Transanccion";
            sheet.Cells[rowindex, 3].Value = "CodigoLocal";
            sheet.Cells[rowindex, 4].Value = "Tienda";
            sheet.Cells[rowindex, 5].Value = "Monto MP";
            sheet.Cells[rowindex, 6].Value = "Monto TBK";
            sheet.Cells[rowindex, 7].Value = "Fecha Conciliacion";
            sheet.Cells[rowindex, 8].Value = "Tarjeta MP";
            sheet.Cells[rowindex, 9].Value = "Tarjeta TBK";

            sheet.Column(1).AutoFit(20);
            sheet.Column(2).AutoFit(12);
            sheet.Column(3).AutoFit(13);
            sheet.Column(4).AutoFit(7);
            sheet.Column(5).AutoFit(13);
            sheet.Column(6).AutoFit(13);
            sheet.Column(7).AutoFit(15);
            sheet.Column(8).AutoFit(20);
            sheet.Column(9).AutoFit(20);

            sheet.Cells[rowindex, 1, rowindex, 9].Style.Font.Bold = true;
            sheet.Cells[rowindex, 1, rowindex, 9].AutoFilter = true;

            var bordesEncabezados = sheet.Cells[rowindex, 1, rowindex, 9].Style.Border;
            bordesEncabezados.Top.Style = bordesEncabezados.Right.Style = bordesEncabezados.Bottom.Style = bordesEncabezados.Left.Style = ExcelBorderStyle.Medium;


            // SE CARGAN LOS DATOS DE TODO EL DOCUMENTO
            var datosPlan = lst;
            var col = 0;
            //Empezamos a escribir sobre ella
            rowindex = 2;
            col = 1;
            //sheet.Cells[rowindex++, col].Value = item;
            foreach (var itemDatos in datosPlan.DetalleConciliacionPagos.Where(x => x.TipoConciliacion == "M"))
            {
                col = 1;
                sheet.Cells[rowindex, col++].Value = itemDatos.AutorizadorTbk;
                sheet.Cells[rowindex, col++].Value = itemDatos.FechaTbk.Value.ToString("dd-MM-yyyy");
                sheet.Cells[rowindex, col++].Value = itemDatos.CodigoLocal;
                sheet.Cells[rowindex, col++].Value = itemDatos.IdTienda;
                sheet.Cells[rowindex, col++].Value = itemDatos.MontoMp.Value;
                sheet.Cells[rowindex, col++].Value = itemDatos.MontoTbk.Value;
                sheet.Cells[rowindex, col++].Value = datosPlan.FechaConciliar.Value;
                sheet.Cells[rowindex, col++].Value = itemDatos.TarjetaTBk;
                sheet.Cells[rowindex, col++].Value = itemDatos.TarjetaMp;

                rowindex++;
            }
            sheet.Cells[rowindex, 4].Value = "Total:";
            sheet.Cells[rowindex, 4].Style.Font.Bold = true;
            sheet.Cells[rowindex, 5].Formula = "=Sum(" + (object)sheet.Cells[2, 5] + ":" + (object)sheet.Cells[rowindex - 1, 5] + ")";
            sheet.Cells[rowindex, 6].Formula = "=Sum(" + sheet.Cells[2, 6] + ":" + sheet.Cells[rowindex - 1, 6] + ")";

            sheet.Column(2).Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
            sheet.Column(7).Style.Numberformat.Format = "dd-MM-yyyy hh:mm";
            var bordes = sheet.Cells[2, 1, rowindex - 1, 9].Style.Border;
            bordes.Top.Style = bordes.Right.Style = bordes.Bottom.Style = bordes.Left.Style = ExcelBorderStyle.Thin;
            var bordesTotal = sheet.Cells[rowindex, 4, rowindex, 6].Style.Border;
            bordesTotal.Top.Style = bordesTotal.Right.Style = bordesTotal.Bottom.Style = bordesTotal.Left.Style = ExcelBorderStyle.Thin;
            return excelPackage;
        }

        #endregion

        #region Generacion de MP y FI


        public ExcelPackage IdocMpPago(List<PAGOS_MP> Pagos)
        {
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            var excelPackage = new ExcelPackage();

            var sheet = excelPackage.Workbook.Worksheets.Add("MP");
            //sheet.Name = item.ToString();
            var rowindex = 1;
            // SE CARGAN LOS DATOS DE TODO EL DOCUMENTO
            var datosPlan = Pagos;
            var col = 0;
            //var NITEM = ;

            //Empezamos a escribir sobre ella
            rowindex = 1;
            col = 1;
            //sheet.Cells[rowindex++, col].Value = item;
            foreach (var itemDatos in datosPlan.Where(x => x.WRBTR != 0))
            {
                col = 1;
                sheet.Cells[rowindex, col++].Value = 400;
                sheet.Cells[rowindex, col++].Value = "CI";
                sheet.Cells[rowindex, col++].Value = itemDatos.FECHAINT;
                sheet.Cells[rowindex, col++].Value = itemDatos.CORRELAT;
                sheet.Cells[rowindex, col++].Value = itemDatos.NITEM;
                sheet.Cells[rowindex, col++].Value = itemDatos.BUKRS;
                sheet.Cells[rowindex, col++].Value = itemDatos.BLART;
                sheet.Cells[rowindex, col++].Value = itemDatos.WAERS;
                sheet.Cells[rowindex, col++].Value = itemDatos.BUDAT;
                sheet.Cells[rowindex, col++].Value = itemDatos.BLDAT;
                sheet.Cells[rowindex, col++].Value = itemDatos.MONAT;
                sheet.Cells[rowindex, col++].Value = itemDatos.XBLNR;
                sheet.Cells[rowindex, col++].Value = itemDatos.BKTXT;
                sheet.Cells[rowindex, col++].Value = itemDatos.BUPLA;
                sheet.Cells[rowindex, col++].Value = itemDatos.NEWBS;
                sheet.Cells[rowindex, col++].Value = itemDatos.NEWKO;
                sheet.Cells[rowindex, col++].Value = itemDatos.NEWUM;
                sheet.Cells[rowindex, col++].Value = itemDatos.NEWBK;
                sheet.Cells[rowindex, col++].Value = itemDatos.WRBTR;
                sheet.Cells[rowindex, col++].Value = itemDatos.FWBAS;
                sheet.Cells[rowindex, col++].Value = itemDatos.MWSKZ;
                sheet.Cells[rowindex, col++].Value = itemDatos.GSBER;
                sheet.Cells[rowindex, col++].Value = itemDatos.KOSTL_PRCTR;
                sheet.Cells[rowindex, col++].Value = itemDatos.AUFNR;
                sheet.Cells[rowindex, col++].Value = itemDatos.ZTERM;
                sheet.Cells[rowindex, col++].Value = itemDatos.ZUONR;
                sheet.Cells[rowindex, col++].Value = itemDatos.SGTXT;
                sheet.Cells[rowindex, col++].Value = itemDatos.VBUND;
                sheet.Cells[rowindex, col++].Value = itemDatos.XREF1;
                sheet.Cells[rowindex, col++].Value = itemDatos.XREF2;
                sheet.Cells[rowindex, col++].Value = itemDatos.XREF3;
                sheet.Cells[rowindex, col++].Value = itemDatos.VALUT;
                sheet.Cells[rowindex, col++].Value = itemDatos.XMWST;
                sheet.Cells[rowindex, col++].Value = itemDatos.ZLSPR;
                sheet.Cells[rowindex, col++].Value = itemDatos.ZFBDT;
                sheet.Cells[rowindex, col++].Value = itemDatos.MANSP;
                sheet.Cells[rowindex, col++].Value = itemDatos.NEWBW;
                sheet.Cells[rowindex, col++].Value = itemDatos.MENGE;
                sheet.Cells[rowindex, col++].Value = itemDatos.KURSF;
                sheet.Cells[rowindex, col++].Value = itemDatos.WWERT;
                sheet.Cells[rowindex, col++].Value = itemDatos.PRCTR;
                sheet.Cells[rowindex, col++].Value = itemDatos.SKFBT;
                sheet.Cells[rowindex, col++].Value = itemDatos.NAME1;
                sheet.Cells[rowindex, col++].Value = itemDatos.ORT01;
                sheet.Cells[rowindex, col++].Value = itemDatos.STCD1;
                sheet.Cells[rowindex, col++].Value = itemDatos.EBELN;
                sheet.Cells[rowindex, col++].Value = itemDatos.WERKS;
                rowindex++;
            }
            return excelPackage;
        }

        public ExcelPackage IdocMpVentas(List<fnIdocVentas_MP_Result> Pagos)
        {
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            var excelPackage = new ExcelPackage();

            var sheet = excelPackage.Workbook.Worksheets.Add("MP");
            //sheet.Name = item.ToString();
            var rowindex = 1;
            // SE CARGAN LOS DATOS DE TODO EL DOCUMENTO
            var datosPlan = Pagos;
            var col = 0;
            //var NITEM = ;

            //Empezamos a escribir sobre ella
            rowindex = 1;
            col = 1;
            //sheet.Cells[rowindex++, col].Value = item;
            foreach (var itemDatos in datosPlan.Where(x => x.WRBTR != 0))
            {
                col = 1;
                sheet.Cells[rowindex, col++].Value = 400;
                sheet.Cells[rowindex, col++].Value = "CI";
                sheet.Cells[rowindex, col++].Value = itemDatos.FECHAINT;
                sheet.Cells[rowindex, col++].Value = itemDatos.CORRELAT;
                sheet.Cells[rowindex, col++].Value = itemDatos.NITEM;
                sheet.Cells[rowindex, col++].Value = itemDatos.BUKRS;
                sheet.Cells[rowindex, col++].Value = itemDatos.BLART;
                sheet.Cells[rowindex, col++].Value = itemDatos.WAERS;
                sheet.Cells[rowindex, col++].Value = itemDatos.BUDAT;
                sheet.Cells[rowindex, col++].Value = itemDatos.BLDAT;
                sheet.Cells[rowindex, col++].Value = itemDatos.MONAT;
                sheet.Cells[rowindex, col++].Value = itemDatos.XBLNR;
                sheet.Cells[rowindex, col++].Value = itemDatos.BKTXT;
                sheet.Cells[rowindex, col++].Value = itemDatos.BUPLA;
                sheet.Cells[rowindex, col++].Value = itemDatos.NEWBS;
                sheet.Cells[rowindex, col++].Value = itemDatos.NEWKO;
                sheet.Cells[rowindex, col++].Value = itemDatos.NEWUM;
                sheet.Cells[rowindex, col++].Value = itemDatos.NEWBK;
                sheet.Cells[rowindex, col++].Value = itemDatos.WRBTR;
                sheet.Cells[rowindex, col++].Value = itemDatos.FWBAS;
                sheet.Cells[rowindex, col++].Value = itemDatos.MWSKZ;
                sheet.Cells[rowindex, col++].Value = itemDatos.GSBER;
                sheet.Cells[rowindex, col++].Value = itemDatos.KOSTL_PRCTR;
                sheet.Cells[rowindex, col++].Value = itemDatos.AUFNR;
                sheet.Cells[rowindex, col++].Value = itemDatos.ZTERM;
                sheet.Cells[rowindex, col++].Value = itemDatos.ZUONR;
                sheet.Cells[rowindex, col++].Value = itemDatos.SGTXT;
                sheet.Cells[rowindex, col++].Value = itemDatos.VBUND;
                sheet.Cells[rowindex, col++].Value = itemDatos.XREF1;
                sheet.Cells[rowindex, col++].Value = itemDatos.XREF2;
                sheet.Cells[rowindex, col++].Value = itemDatos.XREF3;
                sheet.Cells[rowindex, col++].Value = itemDatos.VALUT;
                sheet.Cells[rowindex, col++].Value = itemDatos.XMWST;
                sheet.Cells[rowindex, col++].Value = itemDatos.ZLSPR;
                sheet.Cells[rowindex, col++].Value = itemDatos.ZFBDT;
                sheet.Cells[rowindex, col++].Value = itemDatos.MANSP;
                sheet.Cells[rowindex, col++].Value = itemDatos.NEWBW;
                sheet.Cells[rowindex, col++].Value = itemDatos.MENGE;
                sheet.Cells[rowindex, col++].Value = itemDatos.KURSF;
                sheet.Cells[rowindex, col++].Value = itemDatos.WWERT;
                sheet.Cells[rowindex, col++].Value = itemDatos.PRCTR;
                sheet.Cells[rowindex, col++].Value = itemDatos.SKFBT;
                sheet.Cells[rowindex, col++].Value = itemDatos.NAME1;
                sheet.Cells[rowindex, col++].Value = itemDatos.ORT01;
                sheet.Cells[rowindex, col++].Value = itemDatos.STCD1;
                sheet.Cells[rowindex, col++].Value = itemDatos.EBELN;
                sheet.Cells[rowindex, col++].Value = itemDatos.WERKS;
                rowindex++;
            }
            return excelPackage;
        }

        public ExcelPackage IdocFI(List<IDOC_PAGOS_FI> FI)
        {
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            var excelPackage = new ExcelPackage();

            var sheet = excelPackage.Workbook.Worksheets.Add("FIPagos");
            //sheet.Name = item.ToString();
            var rowindex = 1;
            // SE CARGAN LOS DATOS DE TODO EL DOCUMENTO
            var datosPlan = FI;
            var col = 0;
            //var NITEM = ;

            //Empezamos a escribir sobre ella
            rowindex = 1;
            col = 1;
            //sheet.Cells[rowindex++, col].Value = item;
            foreach (var itemDatos in datosPlan.Where(x => x.WRBTR != 0))
            {
                col = 1;
                sheet.Cells[rowindex, col++].Value = itemDatos.MANDT;
                sheet.Cells[rowindex, col++].Value = itemDatos.INTERFAZ;
                sheet.Cells[rowindex, col++].Value = itemDatos.FECHAINT;
                sheet.Cells[rowindex, col++].Value = 1;
                sheet.Cells[rowindex, col++].Value = itemDatos.NITEM;
                sheet.Cells[rowindex, col++].Value = itemDatos.BUKRS;
                sheet.Cells[rowindex, col++].Value = itemDatos.BLART;
                sheet.Cells[rowindex, col++].Value = itemDatos.WAERS;
                sheet.Cells[rowindex, col++].Value = itemDatos.BUDAT;
                sheet.Cells[rowindex, col++].Value = itemDatos.BLDAT;
                sheet.Cells[rowindex, col++].Value = itemDatos.MONAT;
                sheet.Cells[rowindex, col++].Value = itemDatos.XBLNR;
                sheet.Cells[rowindex, col++].Value = itemDatos.BKTXT;
                sheet.Cells[rowindex, col++].Value = itemDatos.BUPLA;
                sheet.Cells[rowindex, col++].Value = itemDatos.NEWBS;
                sheet.Cells[rowindex, col++].Value = itemDatos.NEWKO;
                sheet.Cells[rowindex, col++].Value = itemDatos.NEWUM;
                sheet.Cells[rowindex, col++].Value = itemDatos.NEWBK;
                sheet.Cells[rowindex, col++].Value = itemDatos.WRBTR;
                sheet.Cells[rowindex, col++].Value = itemDatos.FWBAS;
                sheet.Cells[rowindex, col++].Value = itemDatos.MWSKZ;
                sheet.Cells[rowindex, col++].Value = itemDatos.GSBER;
                sheet.Cells[rowindex, col++].Value = itemDatos.KOSTL_PRCTR;
                sheet.Cells[rowindex, col++].Value = itemDatos.AUFNR;
                sheet.Cells[rowindex, col++].Value = itemDatos.ZTERM;
                sheet.Cells[rowindex, col++].Value = itemDatos.ZUONR;
                sheet.Cells[rowindex, col++].Value = itemDatos.SGTXT;
                sheet.Cells[rowindex, col++].Value = itemDatos.VBUND;
                sheet.Cells[rowindex, col++].Value = itemDatos.XREF1;
                sheet.Cells[rowindex, col++].Value = itemDatos.XREF2;
                sheet.Cells[rowindex, col++].Value = itemDatos.XREF3;
                sheet.Cells[rowindex, col++].Value = itemDatos.VALUT;
                sheet.Cells[rowindex, col++].Value = itemDatos.XMWST;
                sheet.Cells[rowindex, col++].Value = itemDatos.ZLSPR;
                sheet.Cells[rowindex, col++].Value = itemDatos.ZFBDT;
                sheet.Cells[rowindex, col++].Value = itemDatos.MANSP;
                sheet.Cells[rowindex, col++].Value = itemDatos.NEWBW;
                sheet.Cells[rowindex, col++].Value = itemDatos.MENGE;
                sheet.Cells[rowindex, col++].Value = itemDatos.KURSF;
                sheet.Cells[rowindex, col++].Value = itemDatos.WWERT;
                sheet.Cells[rowindex, col++].Value = itemDatos.PRCTR;
                sheet.Cells[rowindex, col++].Value = itemDatos.SKFBT;
                sheet.Cells[rowindex, col++].Value = itemDatos.NAME1;
                sheet.Cells[rowindex, col++].Value = itemDatos.ORT01;
                sheet.Cells[rowindex, col++].Value = itemDatos.STCD1;
                sheet.Cells[rowindex, col++].Value = itemDatos.EBELN;
                sheet.Cells[rowindex, col++].Value = itemDatos.WERKS;
                rowindex++;
            }
            return excelPackage;
        }

        public ExcelPackage IdocFI(List<IDOC_FI> fi)
        {
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            var excelPackage = new ExcelPackage();

            var sheet = excelPackage.Workbook.Worksheets.Add("FIVentas");
            //sheet.Name = item.ToString();
            var rowindex = 1;

            //sheet.Cells[rowindex, 1].Value = "Autorizador";
            //sheet.Cells[rowindex, 2].Value = "Fecha Transanccion";
            //sheet.Cells[rowindex, 3].Value = "CodigoLocal";
            //sheet.Cells[rowindex, 4].Value = "Tienda";
            //sheet.Cells[rowindex, 5].Value = "Monto MP";
            //sheet.Cells[rowindex, 6].Value = "Monto TBK";
            //sheet.Cells[rowindex, 7].Value = "Fecha Conciliacion";
            //sheet.Cells[rowindex, 8].Value = "Tarjeta MP";
            //sheet.Cells[rowindex, 9].Value = "Tarjeta TBK";

            //sheet.Column(1).AutoFit(20);
            //sheet.Column(2).AutoFit(12);
            //sheet.Column(3).AutoFit(13);
            //sheet.Column(4).AutoFit(7);
            //sheet.Column(5).AutoFit(13);
            //sheet.Column(6).AutoFit(13);
            //sheet.Column(7).AutoFit(15);
            //sheet.Column(8).AutoFit(20);
            //sheet.Column(9).AutoFit(20);

            //sheet.Cells[rowindex, 1, rowindex, 9].Style.Font.Bold = true;
            //sheet.Cells[rowindex, 1, rowindex, 9].AutoFilter = true;

            //var bordesEncabezados = sheet.Cells[rowindex, 1, rowindex, 9].Style.Border;
            //bordesEncabezados.Top.Style = bordesEncabezados.Right.Style = bordesEncabezados.Bottom.Style = bordesEncabezados.Left.Style = ExcelBorderStyle.Medium;


            // SE CARGAN LOS DATOS DE TODO EL DOCUMENTO
            var datosPlan = fi;
            var col = 0;
            //Empezamos a escribir sobre ella
            rowindex = 1;
            col = 1;
            //sheet.Cells[rowindex++, col].Value = item;
            foreach (var itemDatos in datosPlan.Where(x => x.WRBTR != 0))
            {
                col = 1;
                sheet.Cells[rowindex, col++].Value = itemDatos.MANDT;
                sheet.Cells[rowindex, col++].Value = itemDatos.INTERFAZ;
                sheet.Cells[rowindex, col++].Value = itemDatos.FECHAINT;
                sheet.Cells[rowindex, col++].Value = itemDatos.CORRELAT;
                sheet.Cells[rowindex, col++].Value = itemDatos.NITEM;
                sheet.Cells[rowindex, col++].Value = itemDatos.BUKRS;
                sheet.Cells[rowindex, col++].Value = itemDatos.BLART;
                sheet.Cells[rowindex, col++].Value = itemDatos.WAERS;
                sheet.Cells[rowindex, col++].Value = itemDatos.BUDAT;
                sheet.Cells[rowindex, col++].Value = itemDatos.BLDAT;
                sheet.Cells[rowindex, col++].Value = itemDatos.MONAT;
                sheet.Cells[rowindex, col++].Value = itemDatos.XBLNR;
                sheet.Cells[rowindex, col++].Value = itemDatos.BKTXT;
                sheet.Cells[rowindex, col++].Value = itemDatos.BUPLA;
                sheet.Cells[rowindex, col++].Value = itemDatos.NEWBS;
                sheet.Cells[rowindex, col++].Value = itemDatos.NEWKO;
                sheet.Cells[rowindex, col++].Value = itemDatos.NEWUM;
                sheet.Cells[rowindex, col++].Value = itemDatos.NEWBK;
                sheet.Cells[rowindex, col++].Value = itemDatos.WRBTR;
                sheet.Cells[rowindex, col++].Value = itemDatos.FWBAS;
                sheet.Cells[rowindex, col++].Value = itemDatos.MWSKZ;
                sheet.Cells[rowindex, col++].Value = itemDatos.GSBER;
                sheet.Cells[rowindex, col++].Value = itemDatos.KOSTL_PRCTR;
                sheet.Cells[rowindex, col++].Value = itemDatos.AUFNR;
                sheet.Cells[rowindex, col++].Value = itemDatos.ZTERM;
                sheet.Cells[rowindex, col++].Value = itemDatos.ZUONR;
                sheet.Cells[rowindex, col++].Value = itemDatos.SGTXT;
                sheet.Cells[rowindex, col++].Value = itemDatos.VBUND;
                sheet.Cells[rowindex, col++].Value = itemDatos.XREF1;
                sheet.Cells[rowindex, col++].Value = itemDatos.XREF2;
                sheet.Cells[rowindex, col++].Value = itemDatos.XREF3;
                sheet.Cells[rowindex, col++].Value = itemDatos.VALUT;
                sheet.Cells[rowindex, col++].Value = itemDatos.XMWST;
                sheet.Cells[rowindex, col++].Value = itemDatos.ZLSPR;
                sheet.Cells[rowindex, col++].Value = itemDatos.ZFBDT;
                sheet.Cells[rowindex, col++].Value = itemDatos.MANSP;
                sheet.Cells[rowindex, col++].Value = itemDatos.NEWBW;
                sheet.Cells[rowindex, col++].Value = itemDatos.MENGE;
                sheet.Cells[rowindex, col++].Value = itemDatos.KURSF;
                sheet.Cells[rowindex, col++].Value = itemDatos.WWERT;
                sheet.Cells[rowindex, col++].Value = itemDatos.PRCTR;
                sheet.Cells[rowindex, col++].Value = itemDatos.SKFBT;
                sheet.Cells[rowindex, col++].Value = itemDatos.NAME1;
                sheet.Cells[rowindex, col++].Value = itemDatos.ORT01;
                sheet.Cells[rowindex, col++].Value = itemDatos.STCD1;
                sheet.Cells[rowindex, col++].Value = itemDatos.EBELN;
                sheet.Cells[rowindex, col++].Value = itemDatos.WERKS;
                rowindex++;
            }

            //sheet.Column(2).Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
            //sheet.Column(7).Style.Numberformat.Format = "dd-MM-yyyy hh:mm";
            //var bordes = sheet.Cells[2, 1, rowindex - 1, 9].Style.Border;
            //bordes.Top.Style = bordes.Right.Style = bordes.Bottom.Style = bordes.Left.Style = ExcelBorderStyle.Thin;
            //var bordesTotal = sheet.Cells[rowindex, 4, rowindex, 6].Style.Border;
            //bordesTotal.Top.Style = bordesTotal.Right.Style = bordesTotal.Bottom.Style = bordesTotal.Left.Style = ExcelBorderStyle.Thin;
            return excelPackage;
        }

        public ExcelPackage ExecelPagos(List<PAGOS> Pago)
        {
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            var excelPackage = new ExcelPackage();

            var sheet = excelPackage.Workbook.Worksheets.Add("Pagos Tienda");
            //sheet.Name = item.ToString();
            var rowindex = 1;
            sheet.Cells[rowindex, 1].Value = "Tipo_Trx";
            sheet.Cells[rowindex, 2].Value = "idTienda";
            sheet.Cells[rowindex, 3].Value = "Trx_Global";
            sheet.Cells[rowindex, 4].Value = "Caja";
            sheet.Cells[rowindex, 5].Value = "Tipo";
            sheet.Cells[rowindex, 6].Value = "TRX";
            sheet.Cells[rowindex, 7].Value = "Fecha";
            sheet.Cells[rowindex, 8].Value = "Cajero";
            sheet.Cells[rowindex, 9].Value = "Monto";
            sheet.Cells[rowindex, 10].Value = "Autorizador";
            sheet.Cells[rowindex, 11].Value = "AutorizadorTBK";
            sheet.Cells[rowindex, 12].Value = "NumeroReferencia";
            sheet.Cells[rowindex, 13].Value = "RutCliente";
            sheet.Cells[rowindex, 14].Value = "Tipo_Pago";
            sheet.Cells[rowindex, 15].Value = "Pago";
            sheet.Cells[rowindex, 16].Value = "Monto_Pago";
            sheet.Cells[rowindex, 17].Value = "Tarjeta";
            sheet.Cells[rowindex, 18].Value = "NCheque";
            sheet.Cells[rowindex, 19].Value = "RutCheque";
            sheet.Cells[rowindex, 20].Value = "Banco";
            sheet.Cells[rowindex, 21].Value = "FechaCheque";
            sheet.Cells[rowindex, 22].Value = "Estado";

            sheet.Column(1).AutoFit(8);
            sheet.Column(2).AutoFit(3);
            sheet.Column(3).AutoFit(10);
            sheet.Column(4).AutoFit(3);
            sheet.Column(5).AutoFit(5);
            sheet.Column(6).AutoFit(10);
            sheet.Column(7).AutoFit(11);
            sheet.Column(8).AutoFit(11);
            sheet.Column(9).AutoFit(11);
            sheet.Column(10).AutoFit(20);
            sheet.Column(11).AutoFit(10);
            sheet.Column(12).AutoFit(20);
            sheet.Column(13).AutoFit(11);
            sheet.Column(14).AutoFit(4);
            sheet.Column(15).AutoFit(10);
            sheet.Column(16).AutoFit(30);
            sheet.Column(17).AutoFit(30);
            sheet.Column(18).AutoFit(10);
            sheet.Column(19).AutoFit(10);
            sheet.Column(20).AutoFit(10);
            sheet.Column(21).AutoFit(10);
            sheet.Column(22).AutoFit(10);

            sheet.Cells[rowindex, 1, rowindex, 22].Style.Font.Bold = true;
            sheet.Cells[rowindex, 1, rowindex, 22].AutoFilter = true;

            var bordesEncabezados = sheet.Cells[rowindex, 1, rowindex, 22].Style.Border;
            bordesEncabezados.Top.Style = bordesEncabezados.Right.Style = bordesEncabezados.Bottom.Style = bordesEncabezados.Left.Style = ExcelBorderStyle.Medium;


            // SE CARGAN LOS DATOS DE TODO EL DOCUMENTO
            var datosPlan = Pago;
            var col = 0;
            //Empezamos a escribir sobre ella
            rowindex = 2;
            col = 1;
            //sheet.Cells[rowindex++, col].Value = item;
            foreach (var itemDatos in datosPlan)
            {
                col = 1;
                sheet.Cells[rowindex, col++].Value = itemDatos.Tipo_Trx;
                sheet.Cells[rowindex, col++].Value = itemDatos.IdTienda;
                sheet.Cells[rowindex, col++].Value = itemDatos.Trx_Global;
                sheet.Cells[rowindex, col++].Value = itemDatos.Caja;
                sheet.Cells[rowindex, col++].Value = itemDatos.Tipo;
                sheet.Cells[rowindex, col++].Value = itemDatos.TRX;
                sheet.Cells[rowindex, col++].Value = itemDatos.Fecha.ToString("dd-MM-yyyy");
                sheet.Cells[rowindex, col++].Value = itemDatos.Cajero;
                sheet.Cells[rowindex, col++].Value = itemDatos.Monto;
                sheet.Cells[rowindex, col++].Value = itemDatos.Autorizador;
                sheet.Cells[rowindex, col++].Value = itemDatos.Autorizador_TBK;
                sheet.Cells[rowindex, col++].Value = itemDatos.NumeroReferencia;
                sheet.Cells[rowindex, col++].Value = itemDatos.RutCliente;
                sheet.Cells[rowindex, col++].Value = itemDatos.Tipo_Pago;
                sheet.Cells[rowindex, col++].Value = itemDatos.Pago;
                sheet.Cells[rowindex, col++].Value = itemDatos.Monto_Pago;
                sheet.Cells[rowindex, col++].Value = itemDatos.Tarjeta;
                sheet.Cells[rowindex, col++].Value = itemDatos.NCheque;
                sheet.Cells[rowindex, col++].Value = itemDatos.RutCheque;
                sheet.Cells[rowindex, col++].Value = itemDatos.Banco;
                sheet.Cells[rowindex, col++].Value = itemDatos.FechaCheque;
                sheet.Cells[rowindex, col++].Value = itemDatos.Estado;



                rowindex++;
            }
            var bordes = sheet.Cells[2, 1, rowindex - 1, 22].Style.Border;
            bordes.Top.Style = bordes.Right.Style = bordes.Bottom.Style = bordes.Left.Style = ExcelBorderStyle.Thin;

            return excelPackage;
        }

        #endregion

        #region Informes
        public ExcelPackage ReporteVentas(List<Proc_ListaDetalleVenta_Result> Detalle)
        {
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            var excelPackage = new ExcelPackage();
            var sheet = excelPackage.Workbook.Worksheets.Add("DetalleVenta");
            //sheet.Name = item.ToString();
            var rowindex = 1;


            sheet.Cells[rowindex, 1].Value = "Centro";
            sheet.Cells[rowindex, 2].Value = "Caja";
            sheet.Cells[rowindex, 3].Value = "Fecha";
            sheet.Cells[rowindex, 4].Value = "Documento";
            sheet.Cells[rowindex, 5].Value = "Folio";
            sheet.Cells[rowindex, 6].Value = "Total";


            sheet.Column(1).AutoFit(10);
            sheet.Column(2).AutoFit(10);
            sheet.Column(3).AutoFit(30);
            sheet.Column(4).AutoFit(11);
            sheet.Column(5).AutoFit(10);
            sheet.Column(6).AutoFit(20);


            sheet.Cells[rowindex, 1, rowindex, 6].Style.Font.Bold = true;
            sheet.Cells[rowindex, 1, rowindex, 6].AutoFilter = true;

            var bordesEncabezados = sheet.Cells[rowindex, 1, rowindex, 6].Style.Border;
            bordesEncabezados.Top.Style = bordesEncabezados.Right.Style = bordesEncabezados.Bottom.Style = bordesEncabezados.Left.Style = ExcelBorderStyle.Medium;

            // SE CARGAN LOS DATOS DE TODO EL DOCUMENTO
            var datosPlan = Detalle;
            var col = 0;
            //var NITEM = ;

            //Empezamos a escribir sobre ella
            rowindex = 2;
            col = 1;

            foreach (var itemDatos in datosPlan)
            {
                col = 1;
                sheet.Cells[rowindex, col++].Value = itemDatos.Centro;
                sheet.Cells[rowindex, col++].Value = itemDatos.Caja;
                sheet.Cells[rowindex, col++].Value = itemDatos.Fecha;
                sheet.Cells[rowindex, col++].Value = itemDatos.Tipo_Documento;
                sheet.Cells[rowindex, col++].Value = itemDatos.Folio_Documento;
                sheet.Cells[rowindex, col++].Value = itemDatos.Total;
                rowindex++;

            }
            sheet.Column(3).Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
            var bordes = sheet.Cells[2, 1, rowindex - 1, 6].Style.Border;
            bordes.Top.Style = bordes.Right.Style = bordes.Bottom.Style = bordes.Left.Style = ExcelBorderStyle.Thin;
            return excelPackage;

        }
        // INFORME NOTAS DE CREDITO DETALLE 
        public ExcelPackage ReporteNCDetalle(List<NcDetailsByCenco> listaDataNC)
        {
            NotaCredito NCDAC = new NotaCredito();
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            var excelPackage = new ExcelPackage();

            var sheet = excelPackage.Workbook.Worksheets.Add("NC");

            var rowindex = 1;
            //sheet.Cells[rowindex, 1].Value = "Trx";
            sheet.Cells[rowindex, 1].Value = "Tipo";
            sheet.Cells[rowindex, 2].Value = "IdTienda";
            sheet.Cells[rowindex, 3].Value = "Descripción Tienda";
            sheet.Cells[rowindex, 4].Value = "N° Nota de Crédito";
            sheet.Cells[rowindex, 5].Value = "Fecha NC";
            sheet.Cells[rowindex, 6].Value = "Hora emision Nota de Credito";
            sheet.Cells[rowindex, 7].Value = "Caja emisión NC";
            sheet.Cells[rowindex, 8].Value = "Rut Cliente NC ";
            sheet.Cells[rowindex, 9].Value = "Monto Nota de crédito en $";
            sheet.Cells[rowindex, 10].Value = "N° boleta";
            sheet.Cells[rowindex, 11].Value = "Fecha_boleta";
            sheet.Cells[rowindex, 12].Value = "Tienda_Origen";
            sheet.Cells[rowindex, 13].Value = "Caja_Origen";
            sheet.Cells[rowindex, 14].Value = "Monto_boleta";
            sheet.Cells[rowindex, 15].Value = "Forma de Pago_OrigenD";
            sheet.Cells[rowindex, 16].Value = "Monto_Medio de Pago";
            sheet.Cells[rowindex, 17].Value = "ID_Cajero";
            sheet.Cells[rowindex, 18].Value = "Rut  cajero";
            sheet.Cells[rowindex, 19].Value = "Nombre_Cajero";
            sheet.Cells[rowindex, 20].Value = "Rut  supervisor";
            sheet.Cells[rowindex, 21].Value = "Nombre Jefatura que autoriza";
            sheet.Cells[rowindex, 22].Value = "Hora autorización Nota de Credito";

            sheet.Column(1).AutoFit(20);
            sheet.Column(2).AutoFit(12);
            sheet.Column(3).AutoFit(13);
            sheet.Column(4).AutoFit(15);
            sheet.Column(5).AutoFit(13);
            sheet.Column(6).AutoFit(13);
            sheet.Column(7).AutoFit(15);
            sheet.Column(8).AutoFit(20);
            sheet.Column(9).AutoFit(20);
            sheet.Column(10).AutoFit(20);
            sheet.Column(11).AutoFit(20);
            sheet.Column(12).AutoFit(20);
            sheet.Column(13).AutoFit(20);
            sheet.Column(14).AutoFit(20);
            sheet.Column(15).AutoFit(20);
            sheet.Column(16).AutoFit(20);
            sheet.Column(17).AutoFit(20);
            sheet.Column(18).AutoFit(20);
            sheet.Column(19).AutoFit(20);
            sheet.Column(20).AutoFit(20);
            sheet.Column(21).AutoFit(20);
            sheet.Column(22).AutoFit(20);

            // SE CARGAN LOS DATOS DE TODO EL DOCUMENTO
            var datosPlan = listaDataNC;
            var col = 0;

            //Empezamos a escribir sobre ella
            rowindex = 2;


            foreach (var i in datosPlan)
            {
                col = 1;
                sheet.Cells[rowindex, col++].Value = i.TIPONOTACREDITO;
                sheet.Cells[rowindex, col++].Value = i.IdTienda_notacredito;
                sheet.Cells[rowindex, col++].Value = i.descTienda;
                sheet.Cells[rowindex, col++].Value = i.Folio_notacredito;
                sheet.Cells[rowindex, col++].Value = i.Fecha_Emision_Nota_Credito.Value.ToString("dd-MM-yyyy");
                sheet.Cells[rowindex, col++].Value = i.hora_emision_nc;
                sheet.Cells[rowindex, col++].Value = i.Caja_notacredito;
                sheet.Cells[rowindex, col++].Value = i.Cliente_notacredito;
                sheet.Cells[rowindex, col++].Value = i.Total_notacredito.Value.ToString("N0");
                sheet.Cells[rowindex, col++].Value = i.Nro_Boleta;
                sheet.Cells[rowindex, col++].Value = i.Fecha_Boleta.Value.ToString("dd-MM-yyyy"); ;
                sheet.Cells[rowindex, col++].Value = i.IdTienda_Origen_Boleta;
                sheet.Cells[rowindex, col++].Value = i.Caja_Origen_Boleta;
                sheet.Cells[rowindex, col++].Value = i.Monto_Boleta.Value.ToString("N0");
                sheet.Cells[rowindex, col++].Value = i.Descripcion_pago;
                sheet.Cells[rowindex, col++].Value = i.Monto_Medio_de_Pago;
                sheet.Cells[rowindex, col++].Value = i.id_Cajero;
                sheet.Cells[rowindex, col++].Value = i.Rut_Cajero;
                sheet.Cells[rowindex, col++].Value = i.Nombre_trabajador;
                sheet.Cells[rowindex, col++].Value = i.Rut_Supervisor;
                sheet.Cells[rowindex, col++].Value = i.nombre_Jefatura_Autoriza;
                sheet.Cells[rowindex, col++].Value = i.hora_autorizacion_nota_credito;
                // FALTA AGREGAR COLUMNAS QUE AGREGARIA PATRICIO A LA TABLA DE BD
                //hora_emision_nc 
                //Monto_Medio_de_Pago 
                //id_Cajero 
                //nombre_Jefatura_Autoriza 
                //hora_autorizacion_nota_credito 
                rowindex++;
            }

            return excelPackage;
        }


        public ExcelPackage ReporteParticipacion(List<fnReporte_Participacion_Result> Detalle)
        {
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            var excelPackage = new ExcelPackage();
            var sheet = excelPackage.Workbook.Worksheets.Add("Participacion");
            //sheet.Name = item.ToString();
            var rowindex = 1;




            sheet.Cells[rowindex, 1].Value = "Centro";
            sheet.Cells[rowindex, 2].Value = "TipoPago";
            sheet.Cells[rowindex, 3].Value = "TotalVenta";
            sheet.Cells[rowindex, 4].Value = "Porcentaje";



            sheet.Column(1).AutoFit(10);
            sheet.Column(2).AutoFit(10);
            sheet.Column(3).AutoFit(30);
            sheet.Column(4).AutoFit(11);



            sheet.Cells[rowindex, 1, rowindex, 4].Style.Font.Bold = true;
            sheet.Cells[rowindex, 1, rowindex, 4].AutoFilter = true;

            var bordesEncabezados = sheet.Cells[rowindex, 1, rowindex, 4].Style.Border;
            bordesEncabezados.Top.Style = bordesEncabezados.Right.Style = bordesEncabezados.Bottom.Style = bordesEncabezados.Left.Style = ExcelBorderStyle.Medium;

            // SE CARGAN LOS DATOS DE TODO EL DOCUMENTO
            var datosPlan = Detalle;
            var col = 0;
            //var NITEM = ;

            //Empezamos a escribir sobre ella
            rowindex = 2;
            col = 1;

            foreach (var itemDatos in datosPlan)
            {
                col = 1;
                if (itemDatos.idTienda == 0)
                {
                    sheet.Cells[rowindex, col++].Value = "TODAS";
                }
                else
                {
                    sheet.Cells[rowindex, col++].Value = itemDatos.idTienda;
                }

                sheet.Cells[rowindex, col++].Value = itemDatos.TipodePago;
                sheet.Cells[rowindex, col++].Value = itemDatos.TotalVenta.Value;
                sheet.Cells[rowindex, col++].Value = itemDatos.Porcentaje;

                rowindex++;

            }
            sheet.Column(3).Style.Numberformat.Format = "$#,##0";
            sheet.Column(4).Style.Numberformat.Format = "0.00";
            sheet.Cells[rowindex, 2].Value = "Totales:";
            sheet.Cells[rowindex, 2].Style.Font.Bold = true;
            sheet.Cells[rowindex, 4].Formula = "=Sum(" + (object)sheet.Cells[2, 4] + ":" + (object)sheet.Cells[rowindex - 1, 4] + ")";
            sheet.Cells[rowindex, 4].Style.Font.Bold = true;
            sheet.Cells[rowindex, 3].Formula = "=Sum(" + (object)sheet.Cells[2, 3] + ":" + (object)sheet.Cells[rowindex - 1, 3] + ")";
            sheet.Cells[rowindex, 3].Style.Font.Bold = true;

            var bordes = sheet.Cells[2, 1, rowindex - 1, 4].Style.Border;
            bordes.Top.Style = bordes.Right.Style = bordes.Bottom.Style = bordes.Left.Style = ExcelBorderStyle.Thin;
            var bordesTotal = sheet.Cells[rowindex, 1, rowindex, 4].Style.Border;
            bordesTotal.Top.Style = bordesTotal.Right.Style = bordesTotal.Bottom.Style = bordesTotal.Left.Style = ExcelBorderStyle.Thin;
            return excelPackage;

        }

        public ExcelPackage ReporteAvance(List<fnReporte_Avance_Result> Detalle)
        {
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            var excelPackage = new ExcelPackage();
            var sheet = excelPackage.Workbook.Worksheets.Add("Avance");
            //sheet.Name = item.ToString();
            var rowindex = 1;




            sheet.Cells[rowindex, 1].Value = "Tienda";
            sheet.Cells[rowindex, 2].Value = "Mes";
            sheet.Cells[rowindex, 3].Value = "Monto";
            sheet.Cells[rowindex, 4].Value = "TRX";



            sheet.Column(1).AutoFit(10);
            sheet.Column(2).AutoFit(10);
            sheet.Column(3).AutoFit(30);
            sheet.Column(4).AutoFit(11);



            sheet.Cells[rowindex, 1, rowindex, 4].Style.Font.Bold = true;
            sheet.Cells[rowindex, 1, rowindex, 4].AutoFilter = true;

            var bordesEncabezados = sheet.Cells[rowindex, 1, rowindex, 4].Style.Border;
            bordesEncabezados.Top.Style = bordesEncabezados.Right.Style = bordesEncabezados.Bottom.Style = bordesEncabezados.Left.Style = ExcelBorderStyle.Medium;

            // SE CARGAN LOS DATOS DE TODO EL DOCUMENTO
            var datosPlan = Detalle;
            var col = 0;
            //var NITEM = ;

            //Empezamos a escribir sobre ella
            rowindex = 2;
            col = 1;

            foreach (var itemDatos in datosPlan)
            {
                col = 1;
                if (itemDatos.Local == 0)
                {
                    sheet.Cells[rowindex, col++].Value = "TODAS";
                }
                else
                {
                    sheet.Cells[rowindex, col++].Value = itemDatos.Local;
                }

                sheet.Cells[rowindex, col++].Value = itemDatos.Mes;
                sheet.Cells[rowindex, col++].Value = itemDatos.Monto.Value;
                sheet.Cells[rowindex, col++].Value = itemDatos.Trx;

                rowindex++;

            }
            sheet.Column(3).Style.Numberformat.Format = "$#,##0";
            sheet.Cells[rowindex, 2].Value = "Totales:";
            sheet.Cells[rowindex, 2].Style.Font.Bold = true;
            sheet.Cells[rowindex, 4].Formula = "=Sum(" + (object)sheet.Cells[2, 4] + ":" + (object)sheet.Cells[rowindex - 1, 4] + ")";
            sheet.Cells[rowindex, 4].Style.Font.Bold = true;
            sheet.Cells[rowindex, 3].Formula = "=Sum(" + (object)sheet.Cells[2, 3] + ":" + (object)sheet.Cells[rowindex - 1, 3] + ")";
            sheet.Cells[rowindex, 3].Style.Font.Bold = true;
            //var bordes = sheet.Cells[2, 1, rowindex - 1, 4].Style.Border;
            //bordes.Top.Style = bordes.Right.Style = bordes.Bottom.Style = bordes.Left.Style = ExcelBorderStyle.Thin;
            var bordes = sheet.Cells[2, 1, rowindex - 1, 4].Style.Border;
            bordes.Top.Style = bordes.Right.Style = bordes.Bottom.Style = bordes.Left.Style = ExcelBorderStyle.Thin;
            var bordesTotal = sheet.Cells[rowindex, 1, rowindex, 4].Style.Border;
            bordesTotal.Top.Style = bordesTotal.Right.Style = bordesTotal.Bottom.Style = bordesTotal.Left.Style = ExcelBorderStyle.Thin;
            return excelPackage;

        }

        public ExcelPackage ReporteTicketPromedio(List<fnReporte_TicketPromedio_Result> Detalle)
        {
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            var excelPackage = new ExcelPackage();
            var sheet = excelPackage.Workbook.Worksheets.Add("TicketPromedio");
            //sheet.Name = item.ToString();
            var rowindex = 1;





            sheet.Cells[rowindex, 1].Value = "TipoPago";
            sheet.Cells[rowindex, 2].Value = "TotalVenta";
            sheet.Cells[rowindex, 3].Value = "Porcentaje";
            sheet.Cells[rowindex, 4].Value = "TRX";



            sheet.Column(1).AutoFit(15);
            sheet.Column(2).AutoFit(30);
            sheet.Column(3).AutoFit(10);
            sheet.Column(4).AutoFit(11);



            sheet.Cells[rowindex, 1, rowindex, 4].Style.Font.Bold = true;
            sheet.Cells[rowindex, 1, rowindex, 4].AutoFilter = true;

            var bordesEncabezados = sheet.Cells[rowindex, 1, rowindex, 4].Style.Border;
            bordesEncabezados.Top.Style = bordesEncabezados.Right.Style = bordesEncabezados.Bottom.Style = bordesEncabezados.Left.Style = ExcelBorderStyle.Medium;

            // SE CARGAN LOS DATOS DE TODO EL DOCUMENTO
            var datosPlan = Detalle;
            var col = 0;
            //var NITEM = ;

            //Empezamos a escribir sobre ella
            rowindex = 2;
            col = 1;

            foreach (var itemDatos in datosPlan)
            {
                col = 1;

                sheet.Cells[rowindex, col++].Value = itemDatos.TipodePago;
                sheet.Cells[rowindex, col++].Value = Math.Abs(itemDatos.TotalVenta.Value);
                sheet.Cells[rowindex, col++].Value = itemDatos.Porcentaje;
                sheet.Cells[rowindex, col++].Value = itemDatos.TRX;

                rowindex++;

            }
            sheet.Column(2).Style.Numberformat.Format = "$#,##0";
            sheet.Column(3).Style.Numberformat.Format = "0.00";
            sheet.Cells[rowindex, 1].Value = "Totales:";
            sheet.Cells[rowindex, 1].Style.Font.Bold = true;
            sheet.Cells[rowindex, 2].Formula = "=Sum(" + (object)sheet.Cells[2, 2] + ":" + (object)sheet.Cells[rowindex - 1, 2] + ")";
            sheet.Cells[rowindex, 2].Style.Font.Bold = true;
            sheet.Cells[rowindex, 3].Formula = "=Sum(" + (object)sheet.Cells[2, 3] + ":" + (object)sheet.Cells[rowindex - 1, 3] + ")";
            sheet.Cells[rowindex, 3].Style.Font.Bold = true;
            sheet.Cells[rowindex, 4].Formula = "=Sum(" + (object)sheet.Cells[2, 4] + ":" + (object)sheet.Cells[rowindex - 1, 4] + ")";
            sheet.Cells[rowindex, 4].Style.Font.Bold = true;

            var bordes = sheet.Cells[2, 1, rowindex - 1, 4].Style.Border;
            bordes.Top.Style = bordes.Right.Style = bordes.Bottom.Style = bordes.Left.Style = ExcelBorderStyle.Thin;
            var bordesTotal = sheet.Cells[rowindex, 1, rowindex, 4].Style.Border;
            bordesTotal.Top.Style = bordesTotal.Right.Style = bordesTotal.Bottom.Style = bordesTotal.Left.Style = ExcelBorderStyle.Thin;
            return excelPackage;

        }

        public ExcelPackage ReporteVenta(List<fnReporte_Ventas_Result> Detalle, int mes)
        {
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            var excelPackage = new ExcelPackage();
            var sheet = excelPackage.Workbook.Worksheets.Add("ReporteVenta");
            //sheet.Name = item.ToString();
            var rowindex = 1;
            var col = 0;
            DateTime MesName = new DateTime(DateTime.Now.Year, mes, DateTime.Now.Day);


            sheet.Cells[rowindex, 1].Value = "Ventas";
            rowindex = 2;

            sheet.Cells[rowindex, 2].Value = "Internet";
            sheet.Cells[rowindex, 2].Style.Font.Color.SetColor(Color.White);
            sheet.Cells[rowindex, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
            sheet.Cells[rowindex, 2].Style.Font.Bold = true;
            sheet.Cells[rowindex, 2].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#ac0133"));
            sheet.Cells[rowindex, 3].Value = "MediosPago";
            sheet.Cells[rowindex, 3].Style.Font.Color.SetColor(Color.White);
            sheet.Cells[rowindex, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
            sheet.Cells[rowindex, 3].Style.Font.Bold = true;
            sheet.Cells[rowindex, 3].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#ac0133"));
            sheet.Cells[rowindex, 4].Value = "TRX";
            sheet.Cells[rowindex, 4].Style.Font.Color.SetColor(Color.White);
            sheet.Cells[rowindex, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
            sheet.Cells[rowindex, 4].Style.Font.Bold = true;
            sheet.Cells[rowindex, 4].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#ac0133"));
            sheet.Cells[rowindex, 5].Value = "VentaNeta";
            sheet.Cells[rowindex, 5].Style.Font.Color.SetColor(Color.White);
            sheet.Cells[rowindex, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
            sheet.Cells[rowindex, 5].Style.Font.Bold = true;
            sheet.Cells[rowindex, 5].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#ac0133"));
            sheet.Cells[rowindex, 6].Value = "VentaBruta";
            sheet.Cells[rowindex, 6].Style.Font.Color.SetColor(Color.White);
            sheet.Cells[rowindex, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
            sheet.Cells[rowindex, 6].Style.Font.Bold = true;
            sheet.Cells[rowindex, 6].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#ac0133"));

            sheet.Column(1).AutoFit(30);
            sheet.Column(2).AutoFit(10);
            sheet.Column(3).AutoFit(15);
            sheet.Column(4).AutoFit(15);
            sheet.Column(5).AutoFit(20);
            sheet.Column(6).AutoFit(20);


            rowindex = 3;
            int InicioSum = rowindex;
            foreach (var itemDatos in Detalle.Select(x => x.Pago).Distinct())
            {
                col = 3;

                sheet.Cells[rowindex, col++].Value = itemDatos;
                sheet.Cells[rowindex, col++].Value = Detalle.Where(x => x.idTienda == 26 && x.Pago == itemDatos).Count();
                sheet.Cells[rowindex, col++].Value = Detalle.Where(x => x.idTienda == 26 && x.Pago == itemDatos && x.TipodePago != "NOTACREDITO").Sum(x => x.Monto);
                sheet.Cells[rowindex, col++].Value = Detalle.Where(x => x.idTienda == 26 && x.Pago == itemDatos && x.TipodePago != "NOTACREDITO").Sum(x => x.Monto) - (Detalle.Where(x => x.idTienda == 26 && x.Pago == itemDatos && x.TipodePago == "NOTACREDITO").Sum(x => x.Monto));



                rowindex++;
            }


            sheet.Cells[rowindex, 2].Value = "Totales General:";

            sheet.Column(4).Style.Numberformat.Format = "0";

            sheet.Cells[rowindex, 2].Style.Font.Bold = true;
            sheet.Cells[rowindex, 4].Formula = "=Sum(" + (object)sheet.Cells[InicioSum, 4] + ":" + (object)sheet.Cells[rowindex - 1, 4] + ")";
            sheet.Cells[rowindex, 4].Style.Font.Bold = true;
            sheet.Cells[rowindex, 5].Formula = "=Sum(" + (object)sheet.Cells[InicioSum, 5] + ":" + (object)sheet.Cells[rowindex - 1, 5] + ")";
            sheet.Cells[rowindex, 5].Style.Font.Bold = true;
            sheet.Cells[rowindex, 6].Formula = "=Sum(" + (object)sheet.Cells[InicioSum, 6] + ":" + (object)sheet.Cells[rowindex - 1, 6] + ")";
            sheet.Cells[rowindex, 6].Style.Font.Bold = true;


            rowindex = rowindex + 2;
            sheet.Cells[rowindex, 2].Value = "Retail";
            sheet.Cells[rowindex, 2].Style.Font.Color.SetColor(Color.White);
            sheet.Cells[rowindex, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
            sheet.Cells[rowindex, 2].Style.Font.Bold = true;
            sheet.Cells[rowindex, 2].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#ac0133"));
            sheet.Cells[rowindex, 3].Value = "MediosPago";
            sheet.Cells[rowindex, 3].Style.Font.Color.SetColor(Color.White);
            sheet.Cells[rowindex, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
            sheet.Cells[rowindex, 3].Style.Font.Bold = true;
            sheet.Cells[rowindex, 3].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#ac0133"));
            sheet.Cells[rowindex, 4].Value = "TRX";
            sheet.Cells[rowindex, 4].Style.Font.Color.SetColor(Color.White);
            sheet.Cells[rowindex, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
            sheet.Cells[rowindex, 4].Style.Font.Bold = true;
            sheet.Cells[rowindex, 4].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#ac0133"));
            sheet.Cells[rowindex, 5].Value = "VentaNeta";
            sheet.Cells[rowindex, 5].Style.Font.Color.SetColor(Color.White);
            sheet.Cells[rowindex, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
            sheet.Cells[rowindex, 5].Style.Font.Bold = true;
            sheet.Cells[rowindex, 5].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#ac0133"));
            sheet.Cells[rowindex, 6].Value = "VentaBruta";
            sheet.Cells[rowindex, 6].Style.Font.Color.SetColor(Color.White);
            sheet.Cells[rowindex, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
            sheet.Cells[rowindex, 6].Style.Font.Bold = true;
            sheet.Cells[rowindex, 6].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#ac0133"));



            rowindex = rowindex + 1;
            InicioSum = rowindex;
            foreach (var itemDatos in Detalle.Select(x => x.Pago).Distinct())
            {
                col = 3;

                sheet.Cells[rowindex, col++].Value = itemDatos;
                sheet.Cells[rowindex, col++].Value = Detalle.Where(x => x.idTienda != 26 && x.Pago == itemDatos).Count();
                sheet.Cells[rowindex, col++].Value = Detalle.Where(x => x.idTienda != 26 && x.Pago == itemDatos && x.TipodePago != "NOTACREDITO").Sum(x => x.Monto);
                sheet.Cells[rowindex, col++].Value = Detalle.Where(x => x.idTienda != 26 && x.Pago == itemDatos && x.TipodePago != "NOTACREDITO").Sum(x => x.Monto) - (Detalle.Where(x => x.idTienda != 26 && x.Pago == itemDatos && x.TipodePago == "NOTACREDITO").Sum(x => x.Monto));

                rowindex++;
            }



            sheet.Cells[rowindex, 2].Value = "Totales General:";
            sheet.Column(5).Style.Numberformat.Format = "$#,##0";
            sheet.Cells[rowindex, 2].Style.Font.Bold = true;
            sheet.Cells[rowindex, 4].Formula = "=Sum(" + (object)sheet.Cells[InicioSum, 4] + ":" + (object)sheet.Cells[rowindex - 1, 4] + ")";
            sheet.Cells[rowindex, 4].Style.Font.Bold = true;
            sheet.Cells[rowindex, 5].Formula = "=Sum(" + (object)sheet.Cells[InicioSum, 5] + ":" + (object)sheet.Cells[rowindex - 1, 5] + ")";
            sheet.Cells[rowindex, 5].Style.Font.Bold = true;
            sheet.Cells[rowindex, 6].Formula = "=Sum(" + (object)sheet.Cells[InicioSum, 6] + ":" + (object)sheet.Cells[rowindex - 1, 6] + ")";
            sheet.Cells[rowindex, 6].Style.Font.Bold = true;


            rowindex = rowindex + 2;
            sheet.Cells[rowindex, 2].Value = "Ecomerce/Retail";

            sheet.Cells[rowindex, 2].Style.Font.Color.SetColor(Color.White);
            sheet.Cells[rowindex, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
            sheet.Cells[rowindex, 2].Style.Font.Bold = true;
            sheet.Cells[rowindex, 2].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#ac0133"));
            sheet.Cells[rowindex, 3].Value = "MediosPago";
            sheet.Cells[rowindex, 3].Style.Font.Color.SetColor(Color.White);
            sheet.Cells[rowindex, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
            sheet.Cells[rowindex, 3].Style.Font.Bold = true;
            sheet.Cells[rowindex, 3].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#ac0133"));
            sheet.Cells[rowindex, 4].Value = "TRX";
            sheet.Cells[rowindex, 4].Style.Font.Color.SetColor(Color.White);
            sheet.Cells[rowindex, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
            sheet.Cells[rowindex, 4].Style.Font.Bold = true;
            sheet.Cells[rowindex, 4].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#ac0133"));
            sheet.Cells[rowindex, 5].Value = "VentaNeta";
            sheet.Cells[rowindex, 5].Style.Font.Color.SetColor(Color.White);
            sheet.Cells[rowindex, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
            sheet.Cells[rowindex, 5].Style.Font.Bold = true;
            sheet.Cells[rowindex, 5].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#ac0133"));
            sheet.Cells[rowindex, 6].Value = "VentaBruta";
            sheet.Cells[rowindex, 6].Style.Font.Color.SetColor(Color.White);
            sheet.Cells[rowindex, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
            sheet.Cells[rowindex, 6].Style.Font.Bold = true;
            sheet.Cells[rowindex, 6].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#ac0133"));


            rowindex = rowindex + 1;
            InicioSum = rowindex;
            foreach (var itemDatos in Detalle.Select(x => x.Pago).Distinct())
            {
                col = 3;

                sheet.Cells[rowindex, col++].Value = itemDatos;
                sheet.Cells[rowindex, col++].Value = Detalle.Where(x => x.Pago == itemDatos).Count();
                sheet.Cells[rowindex, col++].Value = Detalle.Where(x => x.Pago == itemDatos && x.TipodePago != "NOTACREDITO").Sum(x => x.Monto);
                sheet.Cells[rowindex, col++].Value = Detalle.Where(x => x.Pago == itemDatos && x.TipodePago != "NOTACREDITO").Sum(x => x.Monto) - (Detalle.Where(x => x.Pago == itemDatos && x.TipodePago == "NOTACREDITO").Sum(x => x.Monto));

                rowindex++;
            }


            sheet.Column(5).Style.Numberformat.Format = "$#,##0";
            sheet.Column(6).Style.Numberformat.Format = "$#,##0";
            sheet.Cells[rowindex, 2].Value = "Totales General:";

            sheet.Cells[rowindex, 2].Style.Font.Bold = true;
            sheet.Cells[rowindex, 4].Formula = "=Sum(" + (object)sheet.Cells[InicioSum, 4] + ":" + (object)sheet.Cells[rowindex - 1, 4] + ")";
            sheet.Cells[rowindex, 4].Style.Font.Bold = true;
            sheet.Cells[rowindex, 5].Formula = "=Sum(" + (object)sheet.Cells[InicioSum, 5] + ":" + (object)sheet.Cells[rowindex - 1, 5] + ")";
            sheet.Cells[rowindex, 5].Style.Font.Bold = true;
            sheet.Cells[rowindex, 6].Formula = "=Sum(" + (object)sheet.Cells[InicioSum, 6] + ":" + (object)sheet.Cells[rowindex - 1, 6] + ")";
            sheet.Cells[rowindex, 6].Style.Font.Bold = true;


            return excelPackage;

        }

        public ExcelPackage ReporteBoleasContraloria(List<fnReporteBoletasContraloria_Result> Detalle)
        {
            NotaCredito NCDAC = new NotaCredito();
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            var excelPackage = new ExcelPackage();

            var sheet = excelPackage.Workbook.Worksheets.Add("Boletas");

            var rowindex = 1;
            var col = 1;
            //sheet.Cells[rowindex, 1].Value = "Trx";
            sheet.Cells[rowindex, col++].Value = "TRX";
            sheet.Cells[rowindex, col++].Value = "TRX_Impreso";
            sheet.Cells[rowindex, col++].Value = "Tipo_Trx";
            sheet.Cells[rowindex, col++].Value = "Local";
            sheet.Cells[rowindex, col++].Value = "Nombre_Tienda";
            sheet.Cells[rowindex, col++].Value = "Cajero";
            sheet.Cells[rowindex, col++].Value = "NOMBRES";
            sheet.Cells[rowindex, col++].Value = "APELLIDOS";
            sheet.Cells[rowindex, col++].Value = "Caja";
            sheet.Cells[rowindex, col++].Value = "Fecha_Venta";
            sheet.Cells[rowindex, col++].Value = "Hora_Venta";
            sheet.Cells[rowindex, col++].Value = "Fecha_Vta_Sistema";
            sheet.Cells[rowindex, col++].Value = "Fecha_Venta_Linea";
            sheet.Cells[rowindex, col++].Value = "Caja_Fuera_Linea";
            sheet.Cells[rowindex, col++].Value = "Modo_Entrenamiento";
            sheet.Cells[rowindex, col++].Value = "Codigo_Anula";
            sheet.Cells[rowindex, col++].Value = "Cliente";
            sheet.Cells[rowindex, col++].Value = "Vendedor";
            sheet.Cells[rowindex, col++].Value = "Venta_Iniciada";
            sheet.Cells[rowindex, col++].Value = "Total_Venta";
            sheet.Cells[rowindex, col++].Value = "Caja2";
            sheet.Cells[rowindex, col++].Value = "Folio_Documento";
            sheet.Cells[rowindex, col++].Value = "Tipo_Documento";
            sheet.Cells[rowindex, col++].Value = "Tipo_Impto";
            sheet.Cells[rowindex, col++].Value = "Impto";
            sheet.Cells[rowindex, col++].Value = "Neto";
            sheet.Cells[rowindex, col++].Value = "Bruto";



            for (int i = 1; i > col; i++)
            {
                sheet.Column(i).AutoFit(10);
            }
            //sheet.Column(1).AutoFit(10);
            //sheet.Column(2).AutoFit(3);
            //sheet.Column(3).AutoFit(3);
            //sheet.Column(4).AutoFit(15);
            //sheet.Column(5).AutoFit(13);
            //sheet.Column(6).AutoFit(10);
            //sheet.Column(7).AutoFit(8);
            //sheet.Column(8).AutoFit(20);
            //sheet.Column(9).AutoFit(3);
            //sheet.Column(10).AutoFit(10);
            //sheet.Column(11).AutoFit(20);
            //sheet.Column(12).AutoFit(20);
            //sheet.Column(13).AutoFit(15);
            //sheet.Column(14).AutoFit(20);
            //sheet.Column(15).AutoFit(15);
            //sheet.Column(16).AutoFit(15);
            sheet.Cells[rowindex, 1, rowindex, col].Style.Font.Bold = true;
            sheet.Cells[rowindex, 1, rowindex, col].AutoFilter = true;

            var bordesEncabezados = sheet.Cells[rowindex, 1, rowindex, col].Style.Border;
            bordesEncabezados.Top.Style = bordesEncabezados.Right.Style = bordesEncabezados.Bottom.Style = bordesEncabezados.Left.Style = ExcelBorderStyle.Medium;

            // SE CARGAN LOS DATOS DE TODO EL DOCUMENTO
            var datosPlan = Detalle;
            col = 0;

            //Empezamos a escribir sobre ella
            rowindex = 2;


            foreach (var i in datosPlan)
            {
                col = 1; // reiniciar el valor de la columna para cada fila

                sheet.Cells[rowindex, col++].Value = i.TRX;
                sheet.Cells[rowindex, col++].Value = i.TRX_Impreso;
                sheet.Cells[rowindex, col++].Value = i.Tipo_Trx;
                sheet.Cells[rowindex, col++].Value = i.Local;
                sheet.Cells[rowindex, col++].Value = i.Nombre_Tienda;
                sheet.Cells[rowindex, col++].Value = i.Cajero;
                sheet.Cells[rowindex, col++].Value = i.NOMBRES;
                sheet.Cells[rowindex, col++].Value = i.APELLIDOS;
                sheet.Cells[rowindex, col++].Value = i.Caja;
                sheet.Cells[rowindex, col++].Value = i.Fecha_Venta.Value.ToString("MM/dd/yyyy");
                sheet.Cells[rowindex, col++].Value = i.Fecha_Venta.Value.ToString("hh:mm:ss tt");
                sheet.Cells[rowindex, col++].Value = i.Fecha_Vta_Sistema;
                sheet.Cells[rowindex, col++].Value = i.Fecha_Venta_Linea;
                sheet.Cells[rowindex, col++].Value = i.Caja_Fuera_Linea;
                sheet.Cells[rowindex, col++].Value = i.Modo_Entrenamiento;
                sheet.Cells[rowindex, col++].Value = i.Codigo_Anula;
                sheet.Cells[rowindex, col++].Value = i.Cliente;
                sheet.Cells[rowindex, col++].Value = i.Vendedor;
                sheet.Cells[rowindex, col++].Value = i.Venta_Iniciada;
                sheet.Cells[rowindex, col++].Value = i.Total_Venta;
                sheet.Cells[rowindex, col++].Value = i.Caja2;
                sheet.Cells[rowindex, col++].Value = i.Folio_Documento;
                sheet.Cells[rowindex, col++].Value = i.Tipo_Documento;
                sheet.Cells[rowindex, col++].Value = i.Tipo_Impto;
                sheet.Cells[rowindex, col++].Value = i.Impto;
                sheet.Cells[rowindex, col++].Value = i.Neto;
                sheet.Cells[rowindex, col++].Value = i.Bruto;

                rowindex++;
            }


            return excelPackage;

        }

        public ExcelPackage ReporteMP(List<MEDIOSPAGOS> MP)
        {
            NotaCredito NCDAC = new NotaCredito();
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            var excelPackage = new ExcelPackage();

            var sheet = excelPackage.Workbook.Worksheets.Add("MP");

            var rowindex = 1;
            //sheet.Cells[rowindex, 1].Value = "Trx";
            sheet.Cells[rowindex, 1].Value = "TRX";
            sheet.Cells[rowindex, 2].Value = "IdTienda";
            sheet.Cells[rowindex, 3].Value = "Caja";
            sheet.Cells[rowindex, 4].Value = "Fecha";
            sheet.Cells[rowindex, 5].Value = "TipoDocumento";
            sheet.Cells[rowindex, 6].Value = "Folio";
            sheet.Cells[rowindex, 7].Value = "Trans";
            sheet.Cells[rowindex, 8].Value = "Monto";
            sheet.Cells[rowindex, 9].Value = "Tipo_Pago";
            sheet.Cells[rowindex, 10].Value = "Pago";
            sheet.Cells[rowindex, 11].Value = "Tarjeta";
            sheet.Cells[rowindex, 12].Value = "Autorizador";
            sheet.Cells[rowindex, 13].Value = "MontoAprobado";
            sheet.Cells[rowindex, 14].Value = "Auth_Resp";


            sheet.Column(1).AutoFit(10);
            sheet.Column(2).AutoFit(3);
            sheet.Column(3).AutoFit(3);
            sheet.Column(4).AutoFit(15);
            sheet.Column(5).AutoFit(13);
            sheet.Column(6).AutoFit(10);
            sheet.Column(7).AutoFit(8);
            sheet.Column(8).AutoFit(20);
            sheet.Column(9).AutoFit(3);
            sheet.Column(10).AutoFit(10);
            sheet.Column(11).AutoFit(20);
            sheet.Column(12).AutoFit(20);
            sheet.Column(13).AutoFit(15);
            sheet.Column(14).AutoFit(20);

            sheet.Cells[rowindex, 1, rowindex, 14].Style.Font.Bold = true;
            sheet.Cells[rowindex, 1, rowindex, 14].AutoFilter = true;

            var bordesEncabezados = sheet.Cells[rowindex, 1, rowindex, 14].Style.Border;
            bordesEncabezados.Top.Style = bordesEncabezados.Right.Style = bordesEncabezados.Bottom.Style = bordesEncabezados.Left.Style = ExcelBorderStyle.Medium;

            // SE CARGAN LOS DATOS DE TODO EL DOCUMENTO
            var datosPlan = MP;
            var col = 0;

            //Empezamos a escribir sobre ella
            rowindex = 2;


            foreach (var i in datosPlan)
            {
                col = 1;
                sheet.Cells[rowindex, col++].Value = i.TRX;
                sheet.Cells[rowindex, col++].Value = i.IdTienda;
                sheet.Cells[rowindex, col++].Value = i.Caja;
                sheet.Cells[rowindex, col++].Value = i.Fecha.ToString("dd-MM-yyyy");
                sheet.Cells[rowindex, col++].Value = i.Tipo;
                sheet.Cells[rowindex, col++].Value = i.Folio;
                sheet.Cells[rowindex, col++].Value = i.Trans;
                sheet.Cells[rowindex, col++].Value = i.Monto.Value.ToString("C");
                sheet.Cells[rowindex, col++].Value = i.Tipo_Pago;
                sheet.Cells[rowindex, col++].Value = i.Pago;
                sheet.Cells[rowindex, col++].Value = i.Tarjeta;
                sheet.Cells[rowindex, col++].Value = i.Autorizador;
                sheet.Cells[rowindex, col++].Value = i.Monto_Aprobado;
                sheet.Cells[rowindex, col++].Value = i.AUTH_RESP_CD;

                // FALTA AGREGAR COLUMNAS QUE AGREGARIA PATRICIO A LA TABLA DE BD
                //hora_emision_nc 
                //Monto_Medio_de_Pago 
                //id_Cajero 
                //nombre_Jefatura_Autoriza 
                //hora_autorizacion_nota_credito 
                rowindex++;
            }

            return excelPackage;
        }

        public ExcelPackage ReporteMPCuenta(List<Fn_MP_Cuenta_Rut_Result> MP)
        {
            NotaCredito NCDAC = new NotaCredito();
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            var excelPackage = new ExcelPackage();

            var sheet = excelPackage.Workbook.Worksheets.Add("MP");

            var rowindex = 1;
            //sheet.Cells[rowindex, 1].Value = "Trx";
            sheet.Cells[rowindex, 1].Value = "TRX";
            sheet.Cells[rowindex, 2].Value = "IdTienda";
            sheet.Cells[rowindex, 3].Value = "Caja";
            sheet.Cells[rowindex, 4].Value = "Fecha";
            sheet.Cells[rowindex, 5].Value = "TipoDocumento";
            sheet.Cells[rowindex, 6].Value = "Folio";
            sheet.Cells[rowindex, 7].Value = "Trans";
            sheet.Cells[rowindex, 8].Value = "Monto";
            sheet.Cells[rowindex, 9].Value = "Tipo_Pago";
            sheet.Cells[rowindex, 10].Value = "Pago";
            sheet.Cells[rowindex, 11].Value = "Tarjeta";
            sheet.Cells[rowindex, 12].Value = "Autorizador";
            sheet.Cells[rowindex, 13].Value = "MontoAprobado";
            sheet.Cells[rowindex, 14].Value = "Auth_Resp";
            sheet.Cells[rowindex, 15].Value = "Cuenta";
            sheet.Cells[rowindex, 16].Value = "Rut";



            sheet.Column(1).AutoFit(10);
            sheet.Column(2).AutoFit(3);
            sheet.Column(3).AutoFit(3);
            sheet.Column(4).AutoFit(15);
            sheet.Column(5).AutoFit(13);
            sheet.Column(6).AutoFit(10);
            sheet.Column(7).AutoFit(8);
            sheet.Column(8).AutoFit(20);
            sheet.Column(9).AutoFit(3);
            sheet.Column(10).AutoFit(10);
            sheet.Column(11).AutoFit(20);
            sheet.Column(12).AutoFit(20);
            sheet.Column(13).AutoFit(15);
            sheet.Column(14).AutoFit(20);
            sheet.Column(15).AutoFit(15);
            sheet.Column(16).AutoFit(15);
            sheet.Cells[rowindex, 1, rowindex, 16].Style.Font.Bold = true;
            sheet.Cells[rowindex, 1, rowindex, 16].AutoFilter = true;

            var bordesEncabezados = sheet.Cells[rowindex, 1, rowindex, 16].Style.Border;
            bordesEncabezados.Top.Style = bordesEncabezados.Right.Style = bordesEncabezados.Bottom.Style = bordesEncabezados.Left.Style = ExcelBorderStyle.Medium;

            // SE CARGAN LOS DATOS DE TODO EL DOCUMENTO
            var datosPlan = MP;
            var col = 0;

            //Empezamos a escribir sobre ella
            rowindex = 2;


            foreach (var i in datosPlan)
            {
                col = 1;
                sheet.Cells[rowindex, col++].Value = i.TRX;
                sheet.Cells[rowindex, col++].Value = i.IdTienda;
                sheet.Cells[rowindex, col++].Value = i.Caja;
                sheet.Cells[rowindex, col++].Value = i.Fecha.Value.ToString("dd-MM-yyyy");
                sheet.Cells[rowindex, col++].Value = i.Tipo;
                sheet.Cells[rowindex, col++].Value = i.Folio;
                sheet.Cells[rowindex, col++].Value = i.Trans;
                sheet.Cells[rowindex, col++].Value = i.Monto.Value.ToString("C");
                sheet.Cells[rowindex, col++].Value = i.Tipo_Pago;
                sheet.Cells[rowindex, col++].Value = i.Pago;
                sheet.Cells[rowindex, col++].Value = i.Tarjeta;
                sheet.Cells[rowindex, col++].Value = i.Autorizador;
                sheet.Cells[rowindex, col++].Value = i.Monto_Aprobado;
                sheet.Cells[rowindex, col++].Value = i.AUTH_RESP_CD;
                sheet.Cells[rowindex, col++].Value = i.cuenta;
                sheet.Cells[rowindex, col++].Value = i.rut_cliente;

                // FALTA AGREGAR COLUMNAS QUE AGREGARIA PATRICIO A LA TABLA DE BD
                //hora_emision_nc 
                //Monto_Medio_de_Pago 
                //id_Cajero 
                //nombre_Jefatura_Autoriza 
                //hora_autorizacion_nota_credito 
                rowindex++;
            }

            return excelPackage;
        }

        public ExcelPackage ReportePagosCuenta(List<Fn_PAGOS_Cuenta_Rut_Result> MP)
        {
            NotaCredito NCDAC = new NotaCredito();
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            var excelPackage = new ExcelPackage();

            var sheet = excelPackage.Workbook.Worksheets.Add("MP");

            var rowindex = 1;
            //sheet.Cells[rowindex, 1].Value = "Trx";
            sheet.Cells[rowindex, 1].Value = "TipoTRX";
            sheet.Cells[rowindex, 2].Value = "IdTienda";
            sheet.Cells[rowindex, 3].Value = "TrxGlobal";
            sheet.Cells[rowindex, 4].Value = "Caja";
            sheet.Cells[rowindex, 5].Value = "Tipo";
            sheet.Cells[rowindex, 6].Value = "TRX";
            sheet.Cells[rowindex, 7].Value = "Fecha";
            sheet.Cells[rowindex, 8].Value = "Cajero";
            sheet.Cells[rowindex, 9].Value = "Monto";
            sheet.Cells[rowindex, 10].Value = "Autorizador";
            sheet.Cells[rowindex, 11].Value = "Autorizador_TBK";
            sheet.Cells[rowindex, 12].Value = "Referencia";
            sheet.Cells[rowindex, 13].Value = "Rut";
            sheet.Cells[rowindex, 14].Value = "Tipo_Pago";
            sheet.Cells[rowindex, 15].Value = "Pago";
            sheet.Cells[rowindex, 16].Value = "MontoPago";
            sheet.Cells[rowindex, 17].Value = "Estado";
            sheet.Cells[rowindex, 18].Value = "Cuenta";
            sheet.Cells[rowindex, 19].Value = "Rut";


            sheet.Column(1).AutoFit(10);
            sheet.Column(2).AutoFit(3);
            sheet.Column(3).AutoFit(3);
            sheet.Column(4).AutoFit(5);
            sheet.Column(5).AutoFit(13);
            sheet.Column(6).AutoFit(10);
            sheet.Column(7).AutoFit(8);
            sheet.Column(8).AutoFit(20);
            sheet.Column(9).AutoFit(15);
            sheet.Column(10).AutoFit(10);
            sheet.Column(11).AutoFit(20);
            sheet.Column(12).AutoFit(20);
            sheet.Column(13).AutoFit(15);
            sheet.Column(14).AutoFit(20);
            sheet.Column(15).AutoFit(15);
            sheet.Column(16).AutoFit(15);
            sheet.Column(17).AutoFit(15);
            sheet.Column(18).AutoFit(15);
            sheet.Column(19).AutoFit(15);
            sheet.Cells[rowindex, 1, rowindex, 19].Style.Font.Bold = true;
            sheet.Cells[rowindex, 1, rowindex, 19].AutoFilter = true;

            var bordesEncabezados = sheet.Cells[rowindex, 1, rowindex, 19].Style.Border;
            bordesEncabezados.Top.Style = bordesEncabezados.Right.Style = bordesEncabezados.Bottom.Style = bordesEncabezados.Left.Style = ExcelBorderStyle.Medium;

            // SE CARGAN LOS DATOS DE TODO EL DOCUMENTO
            var datosPlan = MP;
            var col = 0;

            //Empezamos a escribir sobre ella
            rowindex = 2;


            foreach (var i in datosPlan)
            {
                col = 1;
                sheet.Cells[rowindex, col++].Value = i.Tipo_Trx;
                sheet.Cells[rowindex, col++].Value = i.IdTienda;
                sheet.Cells[rowindex, col++].Value = i.Trx_Global;
                sheet.Cells[rowindex, col++].Value = i.Caja;
                sheet.Cells[rowindex, col++].Value = i.Tipo;
                sheet.Cells[rowindex, col++].Value = i.TRX;
                sheet.Cells[rowindex, col++].Value = i.Fecha.ToString("dd-MM-yyyy");
                sheet.Cells[rowindex, col++].Value = i.Cajero;
                sheet.Cells[rowindex, col++].Value = i.Monto.Value.ToString("C");
                sheet.Cells[rowindex, col++].Value = i.Autorizador;
                sheet.Cells[rowindex, col++].Value = i.Autorizador_TBK;
                sheet.Cells[rowindex, col++].Value = i.NumeroReferencia;
                sheet.Cells[rowindex, col++].Value = i.RutCliente;
                sheet.Cells[rowindex, col++].Value = i.Tipo_Pago;
                sheet.Cells[rowindex, col++].Value = i.Pago;
                sheet.Cells[rowindex, col++].Value = i.Monto_Pago.ToString("C");
                sheet.Cells[rowindex, col++].Value = i.Estado;
                sheet.Cells[rowindex, col++].Value = i.cuenta;
                sheet.Cells[rowindex, col++].Value = i.RutCliente;







                // FALTA AGREGAR COLUMNAS QUE AGREGARIA PATRICIO A LA TABLA DE BD
                //hora_emision_nc 
                //Monto_Medio_de_Pago 
                //id_Cajero 
                //nombre_Jefatura_Autoriza 
                //hora_autorizacion_nota_credito 
                rowindex++;
            }

            return excelPackage;
        }

        #endregion

        #region ConcliacionEFo

        public ExcelPackage ReportConciliadasAutoEfo(List<LibroMayorDEP> lst)
        {
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            var excelPackage = new ExcelPackage();

            var sheet = excelPackage.Workbook.Worksheets.Add("Lista Conciliadas");
            //sheet.Name = item.ToString();
            var rowindex = 1;

            sheet.Cells[rowindex, 1].Value = "TextoFI";
            sheet.Cells[rowindex, 2].Value = "SocFI";
            sheet.Cells[rowindex, 3].Value = "AsignacionFI";
            sheet.Cells[rowindex, 4].Value = "ReferenciaFI";
            sheet.Cells[rowindex, 5].Value = "NDocumentoFI";
            sheet.Cells[rowindex, 6].Value = "ClaseFI";
            sheet.Cells[rowindex, 7].Value = "FechaDocumentoFI";
            sheet.Cells[rowindex, 8].Value = "CT";
            sheet.Cells[rowindex, 9].Value = "ImporteMDFI";
            sheet.Cells[rowindex, 10].Value = "ImporteMLFI";
            sheet.Cells[rowindex, 11].Value = "DocCompesacionFI";
            sheet.Cells[rowindex, 12].Value = "UsuarioFI";
            sheet.Cells[rowindex, 13].Value = "DescripcionFI";
            sheet.Cells[rowindex, 14].Value = "libMayorFI";
            sheet.Cells[rowindex, 15].Value = "TextoZQ";
            sheet.Cells[rowindex, 16].Value = "SocZQ";
            sheet.Cells[rowindex, 17].Value = "AsignacionZQ";
            sheet.Cells[rowindex, 18].Value = "ReferenciaZQ";
            sheet.Cells[rowindex, 19].Value = "NDocumentoZQ";
            sheet.Cells[rowindex, 20].Value = "ClaseZQ";
            sheet.Cells[rowindex, 21].Value = "FechaDocumentoZQ";
            sheet.Cells[rowindex, 22].Value = "CT";
            sheet.Cells[rowindex, 23].Value = "ImporteMDZQ";
            sheet.Cells[rowindex, 24].Value = "ImporteMLZQ";
            sheet.Cells[rowindex, 25].Value = "DocCompesacionZQ";
            sheet.Cells[rowindex, 26].Value = "UsuarioZQ";
            sheet.Cells[rowindex, 27].Value = "DescripcionZQ";
            sheet.Cells[rowindex, 28].Value = "libMayorZQ";

            sheet.Column(1).AutoFit(20);
            sheet.Column(2).AutoFit(5);
            sheet.Column(3).AutoFit(10);
            sheet.Column(4).AutoFit(5);
            sheet.Column(5).AutoFit(20);
            sheet.Column(6).AutoFit(3);
            sheet.Column(7).AutoFit(10);
            sheet.Column(8).AutoFit(3);
            sheet.Column(9).AutoFit(10);
            sheet.Column(10).AutoFit(10);
            sheet.Column(11).AutoFit(5);
            sheet.Column(12).AutoFit(10);
            sheet.Column(13).AutoFit(20);
            sheet.Column(14).AutoFit(10);
            sheet.Column(15).AutoFit(20);
            sheet.Column(16).AutoFit(5);
            sheet.Column(17).AutoFit(10);
            sheet.Column(18).AutoFit(5);
            sheet.Column(19).AutoFit(3);
            sheet.Column(20).AutoFit(10);
            sheet.Column(21).AutoFit(3);
            sheet.Column(22).AutoFit(10);
            sheet.Column(23).AutoFit(3);
            sheet.Column(24).AutoFit(5);
            sheet.Column(25).AutoFit(10);
            sheet.Column(26).AutoFit(20);
            sheet.Column(27).AutoFit(10);
            sheet.Column(28).AutoFit(20);


            sheet.Cells[rowindex, 1, rowindex, 28].Style.Font.Bold = true;
            sheet.Cells[rowindex, 1, rowindex, 28].AutoFilter = true;

            var bordesEncabezados = sheet.Cells[rowindex, 1, rowindex, 28].Style.Border;
            bordesEncabezados.Top.Style = bordesEncabezados.Right.Style = bordesEncabezados.Bottom.Style = bordesEncabezados.Left.Style = ExcelBorderStyle.Medium;


            // SE CARGAN LOS DATOS DE TODO EL DOCUMENTO
            var datosPlan = lst;
            var col = 0;
            //Empezamos a escribir sobre ella
            rowindex = 2;
            col = 1;
            //sheet.Cells[rowindex++, col].Value = item;
            foreach (var itemDatos in datosPlan.Where(x => x.Estado.Equals("CA")))
            {
                col = 1;
                sheet.Cells[rowindex, col++].Value = itemDatos.Texto_cab_documento;
                sheet.Cells[rowindex, col++].Value = itemDatos.Soc;
                sheet.Cells[rowindex, col++].Value = itemDatos.Asignacion;
                sheet.Cells[rowindex, col++].Value = itemDatos.Referencia;
                sheet.Cells[rowindex, col++].Value = itemDatos.N_Documento;
                sheet.Cells[rowindex, col++].Value = itemDatos.Clase;
                sheet.Cells[rowindex, col++].Value = itemDatos.Fecha_Documento.ToString("dd-MM-yyyy");
                sheet.Cells[rowindex, col++].Value = itemDatos.CT;
                sheet.Cells[rowindex, col++].Value = itemDatos.Importe_MD;
                sheet.Cells[rowindex, col++].Value = itemDatos.Importe_ML;
                sheet.Cells[rowindex, col++].Value = itemDatos.Doc_Compensacion;
                sheet.Cells[rowindex, col++].Value = itemDatos.Usuario;
                sheet.Cells[rowindex, col++].Value = itemDatos.Descripcion;
                sheet.Cells[rowindex, col++].Value = itemDatos.Lib_mayor;
                sheet.Cells[rowindex, col++].Value = itemDatos.Texto_cab_documento1;
                sheet.Cells[rowindex, col++].Value = itemDatos.Soc1;
                sheet.Cells[rowindex, col++].Value = itemDatos.Asignacion1;
                sheet.Cells[rowindex, col++].Value = itemDatos.Referencia1;
                sheet.Cells[rowindex, col++].Value = itemDatos.N_Documento1;
                sheet.Cells[rowindex, col++].Value = itemDatos.Clase1;
                sheet.Cells[rowindex, col++].Value = itemDatos.Fecha_Documento1.ToString("dd-MM-yyyy");
                sheet.Cells[rowindex, col++].Value = itemDatos.CT1;
                sheet.Cells[rowindex, col++].Value = itemDatos.Importe_MD1;
                sheet.Cells[rowindex, col++].Value = itemDatos.Importe_ML1;
                sheet.Cells[rowindex, col++].Value = itemDatos.Doc_Compensacion1;
                sheet.Cells[rowindex, col++].Value = itemDatos.Usuario1;
                sheet.Cells[rowindex, col++].Value = itemDatos.Descripcion1;
                sheet.Cells[rowindex, col++].Value = itemDatos.Lib_mayor1;


                rowindex++;
            }

            foreach (var itemDatos in datosPlan.Where(x => x.Estado.Equals("CM")))
            {
                col = 1;
                sheet.Cells[rowindex, col++].Value = itemDatos.Texto_cab_documento;
                sheet.Cells[rowindex, col++].Value = itemDatos.Soc;
                sheet.Cells[rowindex, col++].Value = itemDatos.Asignacion;
                sheet.Cells[rowindex, col++].Value = itemDatos.Referencia;
                sheet.Cells[rowindex, col++].Value = itemDatos.N_Documento;
                sheet.Cells[rowindex, col++].Value = itemDatos.Clase;
                sheet.Cells[rowindex, col++].Value = itemDatos.Fecha_Documento.ToString("dd-MM-yyyy");
                sheet.Cells[rowindex, col++].Value = itemDatos.CT;
                sheet.Cells[rowindex, col++].Value = itemDatos.Importe_MD;
                sheet.Cells[rowindex, col++].Value = itemDatos.Importe_ML;
                sheet.Cells[rowindex, col++].Value = itemDatos.Doc_Compensacion;
                sheet.Cells[rowindex, col++].Value = itemDatos.Usuario;
                sheet.Cells[rowindex, col++].Value = itemDatos.Descripcion;
                sheet.Cells[rowindex, col++].Value = itemDatos.Lib_mayor;
                sheet.Cells[rowindex, col++].Value = itemDatos.Texto_cab_documento1;
                sheet.Cells[rowindex, col++].Value = itemDatos.Soc1;
                sheet.Cells[rowindex, col++].Value = itemDatos.Asignacion1;
                sheet.Cells[rowindex, col++].Value = itemDatos.Referencia1;
                sheet.Cells[rowindex, col++].Value = itemDatos.N_Documento1;
                sheet.Cells[rowindex, col++].Value = itemDatos.Clase1;
                sheet.Cells[rowindex, col++].Value = itemDatos.Fecha_Documento1.ToString("dd-MM-yyyy");
                sheet.Cells[rowindex, col++].Value = itemDatos.CT1;
                sheet.Cells[rowindex, col++].Value = itemDatos.Importe_MD1;
                sheet.Cells[rowindex, col++].Value = itemDatos.Importe_ML1;
                sheet.Cells[rowindex, col++].Value = itemDatos.Doc_Compensacion1;
                sheet.Cells[rowindex, col++].Value = itemDatos.Usuario1;
                sheet.Cells[rowindex, col++].Value = itemDatos.Descripcion1;
                sheet.Cells[rowindex, col++].Value = itemDatos.Lib_mayor1;


                rowindex++;
            }
            //sheet.Cells[rowindex, 4].Value = "Total:";
            //sheet.Cells[rowindex, 4].Style.Font.Bold = true;
            //sheet.Cells[rowindex, 5].Formula = "=Sum(" + (object)sheet.Cells[2, 5] + ":" + (object)sheet.Cells[rowindex - 1, 5] + ")";
            //sheet.Cells[rowindex, 6].Formula = "=Sum(" + sheet.Cells[2, 6] + ":" + sheet.Cells[rowindex - 1, 6] + ")";

            //sheet.Column(2).Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
            //sheet.Column(7).Style.Numberformat.Format = "dd-MM-yyyy hh:mm";
            //var bordes = sheet.Cells[2, 1, rowindex - 1, 9].Style.Border;
            //bordes.Top.Style = bordes.Right.Style = bordes.Bottom.Style = bordes.Left.Style = ExcelBorderStyle.Thin;
            //var bordesTotal = sheet.Cells[rowindex, 4, rowindex, 6].Style.Border;
            //bordesTotal.Top.Style = bordesTotal.Right.Style = bordesTotal.Bottom.Style = bordesTotal.Left.Style = ExcelBorderStyle.Thin;
            return excelPackage;
        }

        public ExcelPackage ReporteNoConciliadas(List<LibroMayor> lst)
        {
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            var excelPackage = new ExcelPackage();

            var sheet = excelPackage.Workbook.Worksheets.Add("Lista NoConciliadas");
            //sheet.Name = item.ToString();
            var rowindex = 1;

            sheet.Cells[rowindex, 1].Value = "Texto";
            sheet.Cells[rowindex, 2].Value = "Soc";
            sheet.Cells[rowindex, 3].Value = "Asignacion";
            sheet.Cells[rowindex, 4].Value = "Referencia";
            sheet.Cells[rowindex, 5].Value = "NDocumento";
            sheet.Cells[rowindex, 6].Value = "Clase";
            sheet.Cells[rowindex, 7].Value = "FechaDocumento";
            sheet.Cells[rowindex, 8].Value = "CT";
            sheet.Cells[rowindex, 9].Value = "ImporteMD";
            sheet.Cells[rowindex, 10].Value = "ImporteML";
            sheet.Cells[rowindex, 11].Value = "DocCompesacion";
            sheet.Cells[rowindex, 12].Value = "Usuario";
            sheet.Cells[rowindex, 13].Value = "Descripcion";
            sheet.Cells[rowindex, 14].Value = "libMayor";


            sheet.Column(1).AutoFit(20);
            sheet.Column(2).AutoFit(5);
            sheet.Column(3).AutoFit(10);
            sheet.Column(4).AutoFit(5);
            sheet.Column(5).AutoFit(20);
            sheet.Column(6).AutoFit(3);
            sheet.Column(7).AutoFit(10);
            sheet.Column(8).AutoFit(3);
            sheet.Column(9).AutoFit(10);
            sheet.Column(10).AutoFit(10);
            sheet.Column(11).AutoFit(5);
            sheet.Column(12).AutoFit(10);
            sheet.Column(13).AutoFit(20);
            sheet.Column(14).AutoFit(10);



            sheet.Cells[rowindex, 1, rowindex, 14].Style.Font.Bold = true;
            sheet.Cells[rowindex, 1, rowindex, 14].AutoFilter = true;

            var bordesEncabezados = sheet.Cells[rowindex, 1, rowindex, 14].Style.Border;
            bordesEncabezados.Top.Style = bordesEncabezados.Right.Style = bordesEncabezados.Bottom.Style = bordesEncabezados.Left.Style = ExcelBorderStyle.Medium;


            // SE CARGAN LOS DATOS DE TODO EL DOCUMENTO
            var datosPlan = lst;
            var col = 0;
            //Empezamos a escribir sobre ella
            rowindex = 2;
            col = 1;
            //sheet.Cells[rowindex++, col].Value = item;
            foreach (var itemDatos in datosPlan)
            {
                col = 1;
                sheet.Cells[rowindex, col++].Value = itemDatos.Texto_cab_documento;
                sheet.Cells[rowindex, col++].Value = itemDatos.Soc;
                sheet.Cells[rowindex, col++].Value = itemDatos.Asignacion;
                sheet.Cells[rowindex, col++].Value = itemDatos.Referencia;
                sheet.Cells[rowindex, col++].Value = itemDatos.N_Documento;
                sheet.Cells[rowindex, col++].Value = itemDatos.Clase;
                sheet.Cells[rowindex, col++].Value = itemDatos.Fecha_Documento.ToString("dd-MM-yyyy");
                sheet.Cells[rowindex, col++].Value = itemDatos.CT;
                sheet.Cells[rowindex, col++].Value = itemDatos.Importe_MD;
                sheet.Cells[rowindex, col++].Value = itemDatos.Importe_ML;
                sheet.Cells[rowindex, col++].Value = itemDatos.Doc_Compensacion;
                sheet.Cells[rowindex, col++].Value = itemDatos.Usuario;
                sheet.Cells[rowindex, col++].Value = itemDatos.Descripcion;
                sheet.Cells[rowindex, col++].Value = itemDatos.Lib_mayor;



                rowindex++;
            }


            //sheet.Cells[rowindex, 4].Value = "Total:";
            //sheet.Cells[rowindex, 4].Style.Font.Bold = true;
            //sheet.Cells[rowindex, 5].Formula = "=Sum(" + (object)sheet.Cells[2, 5] + ":" + (object)sheet.Cells[rowindex - 1, 5] + ")";
            //sheet.Cells[rowindex, 6].Formula = "=Sum(" + sheet.Cells[2, 6] + ":" + sheet.Cells[rowindex - 1, 6] + ")";

            //sheet.Column(2).Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
            //sheet.Column(7).Style.Numberformat.Format = "dd-MM-yyyy hh:mm";
            //var bordes = sheet.Cells[2, 1, rowindex - 1, 9].Style.Border;
            //bordes.Top.Style = bordes.Right.Style = bordes.Bottom.Style = bordes.Left.Style = ExcelBorderStyle.Thin;
            //var bordesTotal = sheet.Cells[rowindex, 4, rowindex, 6].Style.Border;
            //bordesTotal.Top.Style = bordesTotal.Right.Style = bordesTotal.Bottom.Style = bordesTotal.Left.Style = ExcelBorderStyle.Thin;
            return excelPackage;
        }
        #endregion

        #region ConciliacionCAAUCAPA
        public ExcelPackage ReportConciliadasAutoCaau(List<fnReporte_CAAU_Result> lst)
        {
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            var excelPackage = new ExcelPackage();

            var sheet = excelPackage.Workbook.Worksheets.Add("Lista Conciliadas");
            var sheet2 = excelPackage.Workbook.Worksheets.Add("Lista Global");
            var sheet3 = excelPackage.Workbook.Worksheets.Add("Lista CAAU");

            //sheet.Name = item.ToString();
            #region Conciliadas
            var rowindex = 1;

            sheet.Cells[rowindex, 1].Value = "Referencia";
            sheet.Cells[rowindex, 2].Value = "Codigo Empresa";
            sheet.Cells[rowindex, 3].Value = "Fecha Autorizacion";
            sheet.Cells[rowindex, 4].Value = "Codigo Agencia";
            sheet.Cells[rowindex, 5].Value = "Numero Terminal";
            sheet.Cells[rowindex, 6].Value = "Numero Boleta";
            sheet.Cells[rowindex, 7].Value = "Tipo Transaccion";
            sheet.Cells[rowindex, 8].Value = "Codigo Usuario";
            sheet.Cells[rowindex, 9].Value = "Numero Autorizacion";
            sheet.Cells[rowindex, 10].Value = "Tipo Archivo";
            sheet.Cells[rowindex, 11].Value = "Codigo Cliente";
            sheet.Cells[rowindex, 12].Value = "Numero Tarjeta";
            sheet.Cells[rowindex, 13].Value = "Monto";
            sheet.Cells[rowindex, 14].Value = "Monto Pie";
            sheet.Cells[rowindex, 15].Value = "Meses Diferido";
            sheet.Cells[rowindex, 16].Value = "Meses Gracia";
            sheet.Cells[rowindex, 17].Value = "Fecha Autoriza";
            sheet.Cells[rowindex, 18].Value = "Numero Cuotas";
            sheet.Cells[rowindex, 19].Value = "Extrafinanciamiento";
            sheet.Cells[rowindex, 20].Value = "Codigo Plan Credito";
            sheet.Cells[rowindex, 21].Value = "Monto Afecto";
            sheet.Cells[rowindex, 22].Value = "Monto no Afecto";
            sheet.Cells[rowindex, 23].Value = "Codigo Comercio";
            sheet.Cells[rowindex, 24].Value = "Codigo Rubro";
            sheet.Cells[rowindex, 25].Value = "Rut Vendedor";
            sheet.Cells[rowindex, 26].Value = "Codigo Promocion Venta";
            sheet.Cells[rowindex, 27].Value = "Monto Cuota";
            sheet.Cells[rowindex, 28].Value = "Fecha Primer Vencimiento";
            sheet.Cells[rowindex, 29].Value = "Fecha Ultimo Vencimiento";
            sheet.Cells[rowindex, 30].Value = "Confirmada";
            sheet.Cells[rowindex, 31].Value = "Modo Proceso";
            sheet.Cells[rowindex, 32].Value = "Nro Trx Pos";
            sheet.Cells[rowindex, 33].Value = "Nro Ref";
            sheet.Cells[rowindex, 34].Value = "Nro Caja";
            sheet.Cells[rowindex, 35].Value = "Tipo Diferencia";
            sheet.Cells[rowindex, 36].Value = "Estado";
            sheet.Cells[rowindex, 37].Value = "Cuenta GL";
            sheet.Cells[rowindex, 38].Value = "Rut GL";
            sheet.Cells[rowindex, 39].Value = "Monto GL";
            sheet.Cells[rowindex, 40].Value = "Monto Aprobado GL";

            sheet.Column(1).AutoFit(20);
            sheet.Column(2).AutoFit(10);
            sheet.Column(3).AutoFit(20);
            sheet.Column(4).AutoFit(5);
            sheet.Column(5).AutoFit(20);
            sheet.Column(6).AutoFit(3);
            sheet.Column(7).AutoFit(10);
            sheet.Column(8).AutoFit(6);
            sheet.Column(9).AutoFit(10);
            sheet.Column(10).AutoFit(10);
            sheet.Column(11).AutoFit(15);
            sheet.Column(12).AutoFit(10);
            sheet.Column(13).AutoFit(20);
            sheet.Column(14).AutoFit(10);
            sheet.Column(15).AutoFit(20);
            sheet.Column(16).AutoFit(5);
            sheet.Column(17).AutoFit(10);
            sheet.Column(18).AutoFit(5);
            sheet.Column(19).AutoFit(3);
            sheet.Column(20).AutoFit(10);
            sheet.Column(21).AutoFit(3);
            sheet.Column(22).AutoFit(10);
            sheet.Column(23).AutoFit(3);
            sheet.Column(24).AutoFit(5);
            sheet.Column(25).AutoFit(10);
            sheet.Column(26).AutoFit(20);
            sheet.Column(27).AutoFit(10);
            sheet.Column(28).AutoFit(20);
            sheet.Column(29).AutoFit(20);
            sheet.Column(30).AutoFit(20);
            sheet.Column(31).AutoFit(20);
            sheet.Column(32).AutoFit(20);
            sheet.Column(33).AutoFit(20);
            sheet.Column(34).AutoFit(20);
            sheet.Column(35).AutoFit(20);
            sheet.Column(36).AutoFit(20);
            sheet.Column(37).AutoFit(20);
            sheet.Column(38).AutoFit(20);
            sheet.Column(39).AutoFit(20);
            sheet.Column(40).AutoFit(20);

            sheet.Cells[rowindex, 1, rowindex, 40].Style.Font.Bold = true;
            sheet.Cells[rowindex, 1, rowindex, 40].AutoFilter = true;

            var bordesEncabezados = sheet.Cells[rowindex, 1, rowindex, 40].Style.Border;
            bordesEncabezados.Top.Style = bordesEncabezados.Right.Style = bordesEncabezados.Bottom.Style = bordesEncabezados.Left.Style = ExcelBorderStyle.Medium;


            // SE CARGAN LOS DATOS DE TODO EL DOCUMENTO
            var datosPlan = lst;
            var col = 0;
            //Empezamos a escribir sobre ella
            rowindex = 2;
            col = 1;
            //sheet.Cells[rowindex++, col].Value = item;
            foreach (var itemDatos in datosPlan.Where(x => x.Estado.Equals("AV")))
            {
                col = 1;
                sheet.Cells[rowindex, col++].Value = itemDatos.Referencia;
                sheet.Cells[rowindex, col++].Value = itemDatos.CodigoEmpresa;
                sheet.Cells[rowindex, col++].Value = itemDatos.FechaAutorizacion;
                sheet.Cells[rowindex, col++].Value = itemDatos.CodigoAgencia;
                sheet.Cells[rowindex, col++].Value = itemDatos.NumeroTerminal;
                sheet.Cells[rowindex, col++].Value = itemDatos.NumeroBoleta;
                sheet.Cells[rowindex, col++].Value = itemDatos.TipoTransaccion;
                sheet.Cells[rowindex, col++].Value = itemDatos.CodigoUsuario;
                sheet.Cells[rowindex, col++].Value = itemDatos.NumeroAutorizacion;
                sheet.Cells[rowindex, col++].Value = itemDatos.TipoArchivo;
                sheet.Cells[rowindex, col++].Value = itemDatos.CodigoCliente;
                sheet.Cells[rowindex, col++].Value = itemDatos.NumeroTarjeta;
                sheet.Cells[rowindex, col++].Value = itemDatos.Monto;
                sheet.Cells[rowindex, col++].Value = itemDatos.MontoPie;
                sheet.Cells[rowindex, col++].Value = itemDatos.MesesDiferido;
                sheet.Cells[rowindex, col++].Value = itemDatos.MesesGracia;
                sheet.Cells[rowindex, col++].Value = itemDatos.FechaAutoriza.Value.ToString("dd-MM-yyyy");
                sheet.Cells[rowindex, col++].Value = itemDatos.NumeroCuotas;
                sheet.Cells[rowindex, col++].Value = itemDatos.Extrafinanciamiento;
                sheet.Cells[rowindex, col++].Value = itemDatos.CodigoPlanCredito;
                sheet.Cells[rowindex, col++].Value = itemDatos.MontoAfecto;
                sheet.Cells[rowindex, col++].Value = itemDatos.MontonoAfecto;
                sheet.Cells[rowindex, col++].Value = itemDatos.CodigoComercio;
                sheet.Cells[rowindex, col++].Value = itemDatos.CodigoRubro;
                sheet.Cells[rowindex, col++].Value = itemDatos.RutVendedor;
                sheet.Cells[rowindex, col++].Value = itemDatos.CodigoPromocionVenta;
                sheet.Cells[rowindex, col++].Value = itemDatos.MontoCuota;
                sheet.Cells[rowindex, col++].Value = itemDatos.FechaPrimerVencimiento.Value.ToString("dd-MM-yyyy");
                sheet.Cells[rowindex, col++].Value = itemDatos.FechaUltimoVencimiento.Value.ToString("dd-MM-yyyy");
                sheet.Cells[rowindex, col++].Value = itemDatos.Confirmada;
                sheet.Cells[rowindex, col++].Value = itemDatos.ModoProceso;
                sheet.Cells[rowindex, col++].Value = itemDatos.NroTrxPos;
                sheet.Cells[rowindex, col++].Value = itemDatos.NroRef;
                sheet.Cells[rowindex, col++].Value = itemDatos.NroCaja;
                sheet.Cells[rowindex, col++].Value = itemDatos.TipoDiferencia;
                sheet.Cells[rowindex, col++].Value = itemDatos.Estado;
                sheet.Cells[rowindex, col++].Value = itemDatos.Cuenta_GL;
                sheet.Cells[rowindex, col++].Value = itemDatos.Rut_GL;
                sheet.Cells[rowindex, col++].Value = itemDatos.Monto_GL;
                sheet.Cells[rowindex, col++].Value = itemDatos.MontoAprobado_GL;

                rowindex++;
            }

            sheet.Column(3).Style.Numberformat.Format = "dd-MM-yyyy HH:mm:ss";
            #endregion

            #region Global
            rowindex = 1;

            sheet2.Cells[rowindex, 1].Value = "Referencia";
            sheet2.Cells[rowindex, 2].Value = "Codigo Empresa";
            sheet2.Cells[rowindex, 3].Value = "Fecha Autorizacion";
            sheet2.Cells[rowindex, 4].Value = "Codigo Agencia";
            sheet2.Cells[rowindex, 5].Value = "Numero Terminal";
            sheet2.Cells[rowindex, 6].Value = "Numero Boleta";
            sheet2.Cells[rowindex, 7].Value = "Tipo Transaccion";
            sheet2.Cells[rowindex, 8].Value = "Codigo Usuario";
            sheet2.Cells[rowindex, 9].Value = "Numero Autorizacion";
            sheet2.Cells[rowindex, 10].Value = "Tipo Archivo";
            sheet2.Cells[rowindex, 11].Value = "Codigo Cliente";
            sheet2.Cells[rowindex, 12].Value = "Numero Tarjeta";
            sheet2.Cells[rowindex, 13].Value = "Monto";
            sheet2.Cells[rowindex, 14].Value = "Monto Pie";
            sheet2.Cells[rowindex, 15].Value = "Meses Diferido";
            sheet2.Cells[rowindex, 16].Value = "Meses Gracia";
            sheet2.Cells[rowindex, 17].Value = "Fecha Autoriza";
            sheet2.Cells[rowindex, 18].Value = "Numero Cuotas";
            sheet2.Cells[rowindex, 19].Value = "Extrafinanciamiento";
            sheet2.Cells[rowindex, 20].Value = "Codigo Plan Credito";
            sheet2.Cells[rowindex, 21].Value = "Monto Afecto";
            sheet2.Cells[rowindex, 22].Value = "Monto no Afecto";
            sheet2.Cells[rowindex, 23].Value = "Codigo Comercio";
            sheet2.Cells[rowindex, 24].Value = "Codigo Rubro";
            sheet2.Cells[rowindex, 25].Value = "Rut Vendedor";
            sheet2.Cells[rowindex, 26].Value = "Codigo Promocion Venta";
            sheet2.Cells[rowindex, 27].Value = "Monto Cuota";
            sheet2.Cells[rowindex, 28].Value = "Fecha Primer Vencimiento";
            sheet2.Cells[rowindex, 29].Value = "Fecha Ultimo Vencimiento";
            sheet2.Cells[rowindex, 30].Value = "Confirmada";
            sheet2.Cells[rowindex, 31].Value = "Modo Proceso";
            sheet2.Cells[rowindex, 32].Value = "Nro Trx Pos";
            sheet2.Cells[rowindex, 33].Value = "Nro Ref";
            sheet2.Cells[rowindex, 34].Value = "Nro Caja";
            sheet2.Cells[rowindex, 35].Value = "Tipo Diferencia";
            sheet2.Cells[rowindex, 36].Value = "Estado";
            sheet2.Cells[rowindex, 37].Value = "Cuenta GL";
            sheet2.Cells[rowindex, 38].Value = "Rut GL";
            sheet2.Cells[rowindex, 39].Value = "Monto GL";
            sheet2.Cells[rowindex, 40].Value = "Monto Aprobado GL";

            sheet2.Column(1).AutoFit(20);
            sheet2.Column(2).AutoFit(10);
            sheet2.Column(3).AutoFit(20);
            sheet2.Column(4).AutoFit(5);
            sheet2.Column(5).AutoFit(20);
            sheet2.Column(6).AutoFit(3);
            sheet2.Column(7).AutoFit(10);
            sheet2.Column(8).AutoFit(6);
            sheet2.Column(9).AutoFit(10);
            sheet2.Column(10).AutoFit(10);
            sheet2.Column(11).AutoFit(15);
            sheet2.Column(12).AutoFit(10);
            sheet2.Column(13).AutoFit(20);
            sheet2.Column(14).AutoFit(10);
            sheet2.Column(15).AutoFit(20);
            sheet2.Column(16).AutoFit(5);
            sheet2.Column(17).AutoFit(10);
            sheet2.Column(18).AutoFit(5);
            sheet2.Column(19).AutoFit(3);
            sheet2.Column(20).AutoFit(10);
            sheet2.Column(21).AutoFit(3);
            sheet2.Column(22).AutoFit(10);
            sheet2.Column(23).AutoFit(3);
            sheet2.Column(24).AutoFit(5);
            sheet2.Column(25).AutoFit(10);
            sheet2.Column(26).AutoFit(20);
            sheet2.Column(27).AutoFit(10);
            sheet2.Column(28).AutoFit(20);
            sheet2.Column(29).AutoFit(20);
            sheet2.Column(30).AutoFit(20);
            sheet2.Column(31).AutoFit(20);
            sheet2.Column(32).AutoFit(20);
            sheet2.Column(33).AutoFit(20);
            sheet2.Column(34).AutoFit(20);
            sheet2.Column(35).AutoFit(20);
            sheet2.Column(36).AutoFit(20);
            sheet2.Column(37).AutoFit(20);
            sheet2.Column(38).AutoFit(20);
            sheet2.Column(39).AutoFit(20);
            sheet2.Column(40).AutoFit(20);

            sheet2.Cells[rowindex, 1, rowindex, 40].Style.Font.Bold = true;
            sheet2.Cells[rowindex, 1, rowindex, 40].AutoFilter = true;

            bordesEncabezados = sheet2.Cells[rowindex, 1, rowindex, 40].Style.Border;
            bordesEncabezados.Top.Style = bordesEncabezados.Right.Style = bordesEncabezados.Bottom.Style = bordesEncabezados.Left.Style = ExcelBorderStyle.Medium;


            // SE CARGAN LOS DATOS DE TODO EL DOCUMENTO
            col = 0;
            //Empezamos a escribir sobre ella
            rowindex = 2;
            col = 1;
            //sheet2.Cells[rowindex++, col].Value = item;
            foreach (var itemDatos in datosPlan.Where(x => x.Estado.Equals("CV")))
            {
                col = 1;
                sheet2.Cells[rowindex, col++].Value = itemDatos.Referencia;
                sheet2.Cells[rowindex, col++].Value = itemDatos.CodigoEmpresa;
                sheet2.Cells[rowindex, col++].Value = itemDatos.FechaAutorizacion;
                sheet2.Cells[rowindex, col++].Value = itemDatos.CodigoAgencia;
                sheet2.Cells[rowindex, col++].Value = itemDatos.NumeroTerminal;
                sheet2.Cells[rowindex, col++].Value = itemDatos.NumeroBoleta;
                sheet2.Cells[rowindex, col++].Value = itemDatos.TipoTransaccion;
                sheet2.Cells[rowindex, col++].Value = itemDatos.CodigoUsuario;
                sheet2.Cells[rowindex, col++].Value = itemDatos.NumeroAutorizacion;
                sheet2.Cells[rowindex, col++].Value = itemDatos.TipoArchivo;
                sheet2.Cells[rowindex, col++].Value = itemDatos.CodigoCliente;
                sheet2.Cells[rowindex, col++].Value = itemDatos.NumeroTarjeta;
                sheet2.Cells[rowindex, col++].Value = itemDatos.Monto;
                sheet2.Cells[rowindex, col++].Value = itemDatos.MontoPie;
                sheet2.Cells[rowindex, col++].Value = itemDatos.MesesDiferido;
                sheet2.Cells[rowindex, col++].Value = itemDatos.MesesGracia;
                sheet2.Cells[rowindex, col++].Value = itemDatos.FechaAutoriza;
                sheet2.Cells[rowindex, col++].Value = itemDatos.NumeroCuotas;
                sheet2.Cells[rowindex, col++].Value = itemDatos.Extrafinanciamiento;
                sheet2.Cells[rowindex, col++].Value = itemDatos.CodigoPlanCredito;
                sheet2.Cells[rowindex, col++].Value = itemDatos.MontoAfecto;
                sheet2.Cells[rowindex, col++].Value = itemDatos.MontonoAfecto;
                sheet2.Cells[rowindex, col++].Value = itemDatos.CodigoComercio;
                sheet2.Cells[rowindex, col++].Value = itemDatos.CodigoRubro;
                sheet2.Cells[rowindex, col++].Value = itemDatos.RutVendedor;
                sheet2.Cells[rowindex, col++].Value = itemDatos.CodigoPromocionVenta;
                sheet2.Cells[rowindex, col++].Value = itemDatos.MontoCuota;
                sheet2.Cells[rowindex, col++].Value = itemDatos.FechaPrimerVencimiento;
                sheet2.Cells[rowindex, col++].Value = itemDatos.FechaUltimoVencimiento;
                sheet2.Cells[rowindex, col++].Value = itemDatos.Confirmada;
                sheet2.Cells[rowindex, col++].Value = itemDatos.ModoProceso;
                sheet2.Cells[rowindex, col++].Value = itemDatos.NroTrxPos;
                sheet2.Cells[rowindex, col++].Value = itemDatos.NroRef;
                sheet2.Cells[rowindex, col++].Value = itemDatos.NroCaja;
                sheet2.Cells[rowindex, col++].Value = itemDatos.TipoDiferencia;
                sheet2.Cells[rowindex, col++].Value = itemDatos.Estado;
                sheet2.Cells[rowindex, col++].Value = itemDatos.Cuenta_GL;
                sheet2.Cells[rowindex, col++].Value = itemDatos.Rut_GL;
                sheet2.Cells[rowindex, col++].Value = itemDatos.Monto_GL;
                sheet2.Cells[rowindex, col++].Value = itemDatos.MontoAprobado_GL;

                rowindex++;
            }

            sheet2.Column(3).Style.Numberformat.Format = "dd-MM-yyyy HH:mm:ss";

            #endregion

            #region CAAU
            rowindex = 1;

            sheet3.Cells[rowindex, 1].Value = "Referencia";
            sheet3.Cells[rowindex, 2].Value = "Codigo Empresa";
            sheet3.Cells[rowindex, 3].Value = "Fecha Autorizacion";
            sheet3.Cells[rowindex, 4].Value = "Codigo Agencia";
            sheet3.Cells[rowindex, 5].Value = "Numero Terminal";
            sheet3.Cells[rowindex, 6].Value = "Numero Boleta";
            sheet3.Cells[rowindex, 7].Value = "Tipo Transaccion";
            sheet3.Cells[rowindex, 8].Value = "Codigo Usuario";
            sheet3.Cells[rowindex, 9].Value = "Numero Autorizacion";
            sheet3.Cells[rowindex, 10].Value = "Tipo Archivo";
            sheet3.Cells[rowindex, 11].Value = "Codigo Cliente";
            sheet3.Cells[rowindex, 12].Value = "Numero Tarjeta";
            sheet3.Cells[rowindex, 13].Value = "Monto";
            sheet3.Cells[rowindex, 14].Value = "Monto Pie";
            sheet3.Cells[rowindex, 15].Value = "Meses Diferido";
            sheet3.Cells[rowindex, 16].Value = "Meses Gracia";
            sheet3.Cells[rowindex, 17].Value = "Fecha Autoriza";
            sheet3.Cells[rowindex, 18].Value = "Numero Cuotas";
            sheet3.Cells[rowindex, 19].Value = "Extrafinanciamiento";
            sheet3.Cells[rowindex, 20].Value = "Codigo Plan Credito";
            sheet3.Cells[rowindex, 21].Value = "Monto Afecto";
            sheet3.Cells[rowindex, 22].Value = "Monto no Afecto";
            sheet3.Cells[rowindex, 23].Value = "Codigo Comercio";
            sheet3.Cells[rowindex, 24].Value = "Codigo Rubro";
            sheet3.Cells[rowindex, 25].Value = "Rut Vendedor";
            sheet3.Cells[rowindex, 26].Value = "Codigo Promocion Venta";
            sheet3.Cells[rowindex, 27].Value = "Monto Cuota";
            sheet3.Cells[rowindex, 28].Value = "Fecha Primer Vencimiento";
            sheet3.Cells[rowindex, 29].Value = "Fecha Ultimo Vencimiento";
            sheet3.Cells[rowindex, 30].Value = "Confirmada";
            sheet3.Cells[rowindex, 31].Value = "Modo Proceso";
            sheet3.Cells[rowindex, 32].Value = "Nro Trx Pos";
            sheet3.Cells[rowindex, 33].Value = "Nro Ref";
            sheet3.Cells[rowindex, 34].Value = "Nro Caja";
            sheet3.Cells[rowindex, 35].Value = "Tipo Diferencia";
            sheet3.Cells[rowindex, 36].Value = "Estado";
            sheet3.Cells[rowindex, 37].Value = "Cuenta GL";
            sheet3.Cells[rowindex, 38].Value = "Rut GL";
            sheet3.Cells[rowindex, 39].Value = "Monto GL";
            sheet3.Cells[rowindex, 40].Value = "Monto Aprobado GL";

            sheet3.Column(1).AutoFit(20);
            sheet3.Column(2).AutoFit(10);
            sheet3.Column(3).AutoFit(20);
            sheet3.Column(4).AutoFit(5);
            sheet3.Column(5).AutoFit(20);
            sheet3.Column(6).AutoFit(3);
            sheet3.Column(7).AutoFit(10);
            sheet3.Column(8).AutoFit(6);
            sheet3.Column(9).AutoFit(10);
            sheet3.Column(10).AutoFit(10);
            sheet3.Column(11).AutoFit(15);
            sheet3.Column(12).AutoFit(10);
            sheet3.Column(13).AutoFit(20);
            sheet3.Column(14).AutoFit(10);
            sheet3.Column(15).AutoFit(20);
            sheet3.Column(16).AutoFit(5);
            sheet3.Column(17).AutoFit(10);
            sheet3.Column(18).AutoFit(5);
            sheet3.Column(19).AutoFit(3);
            sheet3.Column(20).AutoFit(10);
            sheet3.Column(21).AutoFit(3);
            sheet3.Column(22).AutoFit(10);
            sheet3.Column(23).AutoFit(3);
            sheet3.Column(24).AutoFit(5);
            sheet3.Column(25).AutoFit(10);
            sheet3.Column(26).AutoFit(20);
            sheet3.Column(27).AutoFit(10);
            sheet3.Column(28).AutoFit(20);
            sheet3.Column(29).AutoFit(20);
            sheet3.Column(30).AutoFit(20);
            sheet3.Column(31).AutoFit(20);
            sheet3.Column(32).AutoFit(20);
            sheet3.Column(33).AutoFit(20);
            sheet3.Column(34).AutoFit(20);
            sheet3.Column(35).AutoFit(20);
            sheet3.Column(36).AutoFit(20);
            sheet3.Column(37).AutoFit(20);
            sheet3.Column(38).AutoFit(20);
            sheet3.Column(39).AutoFit(20);
            sheet3.Column(40).AutoFit(20);

            sheet3.Cells[rowindex, 1, rowindex, 40].Style.Font.Bold = true;
            sheet3.Cells[rowindex, 1, rowindex, 40].AutoFilter = true;

            bordesEncabezados = sheet3.Cells[rowindex, 1, rowindex, 40].Style.Border;
            bordesEncabezados.Top.Style = bordesEncabezados.Right.Style = bordesEncabezados.Bottom.Style = bordesEncabezados.Left.Style = ExcelBorderStyle.Medium;

            // SE CARGAN LOS DATOS DE TODO EL DOCUMENTO
            col = 0;
            //Empezamos a escribir sobre ella
            rowindex = 2;
            col = 1;
            //sheet3.Cells[rowindex++, col].Value = item;
            foreach (var itemDatos in datosPlan.Where(x => x.Estado.Equals("BV")))
            {
                col = 1;
                sheet3.Cells[rowindex, col++].Value = itemDatos.Referencia;
                sheet3.Cells[rowindex, col++].Value = itemDatos.CodigoEmpresa;
                sheet3.Cells[rowindex, col++].Value = itemDatos.FechaAutorizacion;
                sheet3.Cells[rowindex, col++].Value = itemDatos.CodigoAgencia;
                sheet3.Cells[rowindex, col++].Value = itemDatos.NumeroTerminal;
                sheet3.Cells[rowindex, col++].Value = itemDatos.NumeroBoleta;
                sheet3.Cells[rowindex, col++].Value = itemDatos.TipoTransaccion;
                sheet3.Cells[rowindex, col++].Value = itemDatos.CodigoUsuario;
                sheet3.Cells[rowindex, col++].Value = itemDatos.NumeroAutorizacion;
                sheet3.Cells[rowindex, col++].Value = itemDatos.TipoArchivo;
                sheet3.Cells[rowindex, col++].Value = itemDatos.CodigoCliente;
                sheet3.Cells[rowindex, col++].Value = itemDatos.NumeroTarjeta;
                sheet3.Cells[rowindex, col++].Value = itemDatos.Monto;
                sheet3.Cells[rowindex, col++].Value = itemDatos.MontoPie;
                sheet3.Cells[rowindex, col++].Value = itemDatos.MesesDiferido;
                sheet3.Cells[rowindex, col++].Value = itemDatos.MesesGracia;
                sheet3.Cells[rowindex, col++].Value = itemDatos.FechaAutoriza;
                sheet3.Cells[rowindex, col++].Value = itemDatos.NumeroCuotas;
                sheet3.Cells[rowindex, col++].Value = itemDatos.Extrafinanciamiento;
                sheet3.Cells[rowindex, col++].Value = itemDatos.CodigoPlanCredito;
                sheet3.Cells[rowindex, col++].Value = itemDatos.MontoAfecto;
                sheet3.Cells[rowindex, col++].Value = itemDatos.MontonoAfecto;
                sheet3.Cells[rowindex, col++].Value = itemDatos.CodigoComercio;
                sheet3.Cells[rowindex, col++].Value = itemDatos.CodigoRubro;
                sheet3.Cells[rowindex, col++].Value = itemDatos.RutVendedor;
                sheet3.Cells[rowindex, col++].Value = itemDatos.CodigoPromocionVenta;
                sheet3.Cells[rowindex, col++].Value = itemDatos.MontoCuota;
                sheet3.Cells[rowindex, col++].Value = itemDatos.FechaPrimerVencimiento;
                sheet3.Cells[rowindex, col++].Value = itemDatos.FechaUltimoVencimiento;
                sheet3.Cells[rowindex, col++].Value = itemDatos.Confirmada;
                sheet3.Cells[rowindex, col++].Value = itemDatos.ModoProceso;
                sheet3.Cells[rowindex, col++].Value = itemDatos.NroTrxPos;
                sheet3.Cells[rowindex, col++].Value = itemDatos.NroRef;
                sheet3.Cells[rowindex, col++].Value = itemDatos.NroCaja;
                sheet3.Cells[rowindex, col++].Value = itemDatos.TipoDiferencia;
                sheet3.Cells[rowindex, col++].Value = itemDatos.Estado;
                sheet3.Cells[rowindex, col++].Value = itemDatos.Cuenta_GL;
                sheet3.Cells[rowindex, col++].Value = itemDatos.Rut_GL;
                sheet3.Cells[rowindex, col++].Value = itemDatos.Monto_GL;
                sheet3.Cells[rowindex, col++].Value = itemDatos.MontoAprobado_GL;

                rowindex++;
            }

            sheet3.Column(3).Style.Numberformat.Format = "dd-MM-yyyy HH:mm:ss";

            #endregion            
            //sheet.Cells[rowindex, 4].Value = "Total:";
            //sheet.Cells[rowindex, 4].Style.Font.Bold = true;
            //sheet.Cells[rowindex, 5].Formula = "=Sum(" + (object)sheet.Cells[2, 5] + ":" + (object)sheet.Cells[rowindex - 1, 5] + ")";
            //sheet.Cells[rowindex, 6].Formula = "=Sum(" + sheet.Cells[2, 6] + ":" + sheet.Cells[rowindex - 1, 6] + ")";

            //sheet.Column(2).Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
            //sheet.Column(7).Style.Numberformat.Format = "dd-MM-yyyy hh:mm";
            //var bordes = sheet.Cells[2, 1, rowindex - 1, 9].Style.Border;
            //bordes.Top.Style = bordes.Right.Style = bordes.Bottom.Style = bordes.Left.Style = ExcelBorderStyle.Thin;
            //var bordesTotal = sheet.Cells[rowindex, 4, rowindex, 6].Style.Border;
            //bordesTotal.Top.Style = bordesTotal.Right.Style = bordesTotal.Bottom.Style = bordesTotal.Left.Style = ExcelBorderStyle.Thin;
            return excelPackage;
        }

        public ExcelPackage ReportConciliadasAutoCaauAvances(List<fnReporte_CAAU_AVANCES_Result> lst)
        {
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            var excelPackage = new ExcelPackage();

            var sheet = excelPackage.Workbook.Worksheets.Add("Lista Conciliadas");
            var sheet2 = excelPackage.Workbook.Worksheets.Add("Lista Global");
            var sheet3 = excelPackage.Workbook.Worksheets.Add("Lista CAAUAvances");

            //sheet.Name = item.ToString();
            #region Conciliadas
            var rowindex = 1;

            sheet.Cells[rowindex, 1].Value = "Referencia";
            sheet.Cells[rowindex, 2].Value = "Codigo Empresa";
            sheet.Cells[rowindex, 3].Value = "Fecha Autorizacion";
            sheet.Cells[rowindex, 4].Value = "Codigo Agencia";
            sheet.Cells[rowindex, 5].Value = "Numero Terminal";
            sheet.Cells[rowindex, 6].Value = "Numero Boleta";
            sheet.Cells[rowindex, 7].Value = "Tipo Transaccion";
            sheet.Cells[rowindex, 8].Value = "Codigo Usuario";
            sheet.Cells[rowindex, 9].Value = "Numero Autorizacion";
            sheet.Cells[rowindex, 10].Value = "Tipo Archivo";
            sheet.Cells[rowindex, 11].Value = "Codigo Cliente";
            sheet.Cells[rowindex, 12].Value = "Numero Tarjeta";
            sheet.Cells[rowindex, 13].Value = "Monto";
            sheet.Cells[rowindex, 14].Value = "Monto Pie";
            sheet.Cells[rowindex, 15].Value = "Meses Diferido";
            sheet.Cells[rowindex, 16].Value = "Meses Gracia";
            sheet.Cells[rowindex, 17].Value = "Fecha Autoriza";
            sheet.Cells[rowindex, 18].Value = "Numero Cuotas";
            sheet.Cells[rowindex, 19].Value = "Extrafinanciamiento";
            sheet.Cells[rowindex, 20].Value = "Codigo Plan Credito";
            sheet.Cells[rowindex, 21].Value = "Monto Afecto";
            sheet.Cells[rowindex, 22].Value = "Monto no Afecto";
            sheet.Cells[rowindex, 23].Value = "Codigo Comercio";
            sheet.Cells[rowindex, 24].Value = "Codigo Rubro";
            sheet.Cells[rowindex, 25].Value = "Rut Vendedor";
            sheet.Cells[rowindex, 26].Value = "Codigo Promocion Venta";
            sheet.Cells[rowindex, 27].Value = "Monto Cuota";
            sheet.Cells[rowindex, 28].Value = "Fecha Primer Vencimiento";
            sheet.Cells[rowindex, 29].Value = "Fecha Ultimo Vencimiento";
            sheet.Cells[rowindex, 30].Value = "Confirmada";
            sheet.Cells[rowindex, 31].Value = "Modo Proceso";
            sheet.Cells[rowindex, 32].Value = "Nro Trx Pos";
            sheet.Cells[rowindex, 33].Value = "Nro Ref";
            sheet.Cells[rowindex, 34].Value = "Nro Caja";
            sheet.Cells[rowindex, 35].Value = "Tipo Diferencia";
            sheet.Cells[rowindex, 36].Value = "Estado";
            sheet.Cells[rowindex, 37].Value = "Cuenta GL";
            sheet.Cells[rowindex, 38].Value = "Rut GL";
            sheet.Cells[rowindex, 39].Value = "Monto GL";
            sheet.Cells[rowindex, 40].Value = "Monto Aprobado GL";

            sheet.Column(1).AutoFit(20);
            sheet.Column(2).AutoFit(10);
            sheet.Column(3).AutoFit(20);
            sheet.Column(4).AutoFit(5);
            sheet.Column(5).AutoFit(20);
            sheet.Column(6).AutoFit(3);
            sheet.Column(7).AutoFit(10);
            sheet.Column(8).AutoFit(6);
            sheet.Column(9).AutoFit(10);
            sheet.Column(10).AutoFit(10);
            sheet.Column(11).AutoFit(15);
            sheet.Column(12).AutoFit(10);
            sheet.Column(13).AutoFit(20);
            sheet.Column(14).AutoFit(10);
            sheet.Column(15).AutoFit(20);
            sheet.Column(16).AutoFit(5);
            sheet.Column(17).AutoFit(10);
            sheet.Column(18).AutoFit(5);
            sheet.Column(19).AutoFit(3);
            sheet.Column(20).AutoFit(10);
            sheet.Column(21).AutoFit(3);
            sheet.Column(22).AutoFit(10);
            sheet.Column(23).AutoFit(3);
            sheet.Column(24).AutoFit(5);
            sheet.Column(25).AutoFit(10);
            sheet.Column(26).AutoFit(20);
            sheet.Column(27).AutoFit(10);
            sheet.Column(28).AutoFit(20);
            sheet.Column(29).AutoFit(20);
            sheet.Column(30).AutoFit(20);
            sheet.Column(31).AutoFit(20);
            sheet.Column(32).AutoFit(20);
            sheet.Column(33).AutoFit(20);
            sheet.Column(34).AutoFit(20);
            sheet.Column(35).AutoFit(20);
            sheet.Column(36).AutoFit(20);
            sheet.Column(37).AutoFit(20);
            sheet.Column(38).AutoFit(20);
            sheet.Column(39).AutoFit(20);
            sheet.Column(40).AutoFit(20);

            sheet.Cells[rowindex, 1, rowindex, 40].Style.Font.Bold = true;
            sheet.Cells[rowindex, 1, rowindex, 40].AutoFilter = true;

            var bordesEncabezados = sheet.Cells[rowindex, 1, rowindex, 40].Style.Border;
            bordesEncabezados.Top.Style = bordesEncabezados.Right.Style = bordesEncabezados.Bottom.Style = bordesEncabezados.Left.Style = ExcelBorderStyle.Medium;


            // SE CARGAN LOS DATOS DE TODO EL DOCUMENTO
            var datosPlan = lst;
            var col = 0;
            //Empezamos a escribir sobre ella
            rowindex = 2;
            col = 1;
            //sheet.Cells[rowindex++, col].Value = item;
            foreach (var itemDatos in datosPlan.Where(x => x.Estado.Equals("AA")))
            {
                col = 1;
                sheet.Cells[rowindex, col++].Value = itemDatos.Referencia;
                sheet.Cells[rowindex, col++].Value = itemDatos.CodigoEmpresa;
                sheet.Cells[rowindex, col++].Value = itemDatos.FechaAutorizacion;
                sheet.Cells[rowindex, col++].Value = itemDatos.CodigoAgencia;
                sheet.Cells[rowindex, col++].Value = itemDatos.NumeroTerminal;
                sheet.Cells[rowindex, col++].Value = itemDatos.NumeroBoleta;
                sheet.Cells[rowindex, col++].Value = itemDatos.TipoTransaccion;
                sheet.Cells[rowindex, col++].Value = itemDatos.CodigoUsuario;
                sheet.Cells[rowindex, col++].Value = itemDatos.NumeroAutorizacion;
                sheet.Cells[rowindex, col++].Value = itemDatos.TipoArchivo;
                sheet.Cells[rowindex, col++].Value = itemDatos.CodigoCliente;
                sheet.Cells[rowindex, col++].Value = itemDatos.NumeroTarjeta;
                sheet.Cells[rowindex, col++].Value = itemDatos.Monto;
                sheet.Cells[rowindex, col++].Value = itemDatos.MontoPie;
                sheet.Cells[rowindex, col++].Value = itemDatos.MesesDiferido;
                sheet.Cells[rowindex, col++].Value = itemDatos.MesesGracia;
                sheet.Cells[rowindex, col++].Value = itemDatos.FechaAutoriza.Value.ToString("dd-MM-yyyy");
                sheet.Cells[rowindex, col++].Value = itemDatos.NumeroCuotas;
                sheet.Cells[rowindex, col++].Value = itemDatos.Extrafinanciamiento;
                sheet.Cells[rowindex, col++].Value = itemDatos.CodigoPlanCredito;
                sheet.Cells[rowindex, col++].Value = itemDatos.MontoAfecto;
                sheet.Cells[rowindex, col++].Value = itemDatos.MontonoAfecto;
                sheet.Cells[rowindex, col++].Value = itemDatos.CodigoComercio;
                sheet.Cells[rowindex, col++].Value = itemDatos.CodigoRubro;
                sheet.Cells[rowindex, col++].Value = itemDatos.RutVendedor;
                sheet.Cells[rowindex, col++].Value = itemDatos.CodigoPromocionVenta;
                sheet.Cells[rowindex, col++].Value = itemDatos.MontoCuota;
                sheet.Cells[rowindex, col++].Value = itemDatos.FechaPrimerVencimiento.Value.ToString("dd-MM-yyyy");
                sheet.Cells[rowindex, col++].Value = itemDatos.FechaUltimoVencimiento.Value.ToString("dd-MM-yyyy");
                sheet.Cells[rowindex, col++].Value = itemDatos.Confirmada;
                sheet.Cells[rowindex, col++].Value = itemDatos.ModoProceso;
                sheet.Cells[rowindex, col++].Value = itemDatos.NroTrxPos;
                sheet.Cells[rowindex, col++].Value = itemDatos.NroRef;
                sheet.Cells[rowindex, col++].Value = itemDatos.NroCaja;
                sheet.Cells[rowindex, col++].Value = itemDatos.TipoDiferencia;
                sheet.Cells[rowindex, col++].Value = itemDatos.Estado;
                sheet.Cells[rowindex, col++].Value = itemDatos.Cuenta_GL;
                sheet.Cells[rowindex, col++].Value = itemDatos.Rut_GL;
                sheet.Cells[rowindex, col++].Value = itemDatos.Monto_GL;
                sheet.Cells[rowindex, col++].Value = itemDatos.MontoPago_GL;

                rowindex++;
            }

            sheet.Column(3).Style.Numberformat.Format = "dd-MM-yyyy HH:mm:ss";
            #endregion

            #region Global
            rowindex = 1;

            sheet2.Cells[rowindex, 1].Value = "Referencia";
            sheet2.Cells[rowindex, 2].Value = "Codigo Empresa";
            sheet2.Cells[rowindex, 3].Value = "Fecha Autorizacion";
            sheet2.Cells[rowindex, 4].Value = "Codigo Agencia";
            sheet2.Cells[rowindex, 5].Value = "Numero Terminal";
            sheet2.Cells[rowindex, 6].Value = "Numero Boleta";
            sheet2.Cells[rowindex, 7].Value = "Tipo Transaccion";
            sheet2.Cells[rowindex, 8].Value = "Codigo Usuario";
            sheet2.Cells[rowindex, 9].Value = "Numero Autorizacion";
            sheet2.Cells[rowindex, 10].Value = "Tipo Archivo";
            sheet2.Cells[rowindex, 11].Value = "Codigo Cliente";
            sheet2.Cells[rowindex, 12].Value = "Numero Tarjeta";
            sheet2.Cells[rowindex, 13].Value = "Monto";
            sheet2.Cells[rowindex, 14].Value = "Monto Pie";
            sheet2.Cells[rowindex, 15].Value = "Meses Diferido";
            sheet2.Cells[rowindex, 16].Value = "Meses Gracia";
            sheet2.Cells[rowindex, 17].Value = "Fecha Autoriza";
            sheet2.Cells[rowindex, 18].Value = "Numero Cuotas";
            sheet2.Cells[rowindex, 19].Value = "Extrafinanciamiento";
            sheet2.Cells[rowindex, 20].Value = "Codigo Plan Credito";
            sheet2.Cells[rowindex, 21].Value = "Monto Afecto";
            sheet2.Cells[rowindex, 22].Value = "Monto no Afecto";
            sheet2.Cells[rowindex, 23].Value = "Codigo Comercio";
            sheet2.Cells[rowindex, 24].Value = "Codigo Rubro";
            sheet2.Cells[rowindex, 25].Value = "Rut Vendedor";
            sheet2.Cells[rowindex, 26].Value = "Codigo Promocion Venta";
            sheet2.Cells[rowindex, 27].Value = "Monto Cuota";
            sheet2.Cells[rowindex, 28].Value = "Fecha Primer Vencimiento";
            sheet2.Cells[rowindex, 29].Value = "Fecha Ultimo Vencimiento";
            sheet2.Cells[rowindex, 30].Value = "Confirmada";
            sheet2.Cells[rowindex, 31].Value = "Modo Proceso";
            sheet2.Cells[rowindex, 32].Value = "Nro Trx Pos";
            sheet2.Cells[rowindex, 33].Value = "Nro Ref";
            sheet2.Cells[rowindex, 34].Value = "Nro Caja";
            sheet2.Cells[rowindex, 35].Value = "Tipo Diferencia";
            sheet2.Cells[rowindex, 36].Value = "Estado";
            sheet2.Cells[rowindex, 37].Value = "Cuenta GL";
            sheet2.Cells[rowindex, 38].Value = "Rut GL";
            sheet2.Cells[rowindex, 39].Value = "Monto GL";
            sheet2.Cells[rowindex, 40].Value = "Monto Aprobado GL";

            sheet2.Column(1).AutoFit(20);
            sheet2.Column(2).AutoFit(10);
            sheet2.Column(3).AutoFit(20);
            sheet2.Column(4).AutoFit(5);
            sheet2.Column(5).AutoFit(20);
            sheet2.Column(6).AutoFit(3);
            sheet2.Column(7).AutoFit(10);
            sheet2.Column(8).AutoFit(6);
            sheet2.Column(9).AutoFit(10);
            sheet2.Column(10).AutoFit(10);
            sheet2.Column(11).AutoFit(15);
            sheet2.Column(12).AutoFit(10);
            sheet2.Column(13).AutoFit(20);
            sheet2.Column(14).AutoFit(10);
            sheet2.Column(15).AutoFit(20);
            sheet2.Column(16).AutoFit(5);
            sheet2.Column(17).AutoFit(10);
            sheet2.Column(18).AutoFit(5);
            sheet2.Column(19).AutoFit(3);
            sheet2.Column(20).AutoFit(10);
            sheet2.Column(21).AutoFit(3);
            sheet2.Column(22).AutoFit(10);
            sheet2.Column(23).AutoFit(3);
            sheet2.Column(24).AutoFit(5);
            sheet2.Column(25).AutoFit(10);
            sheet2.Column(26).AutoFit(20);
            sheet2.Column(27).AutoFit(10);
            sheet2.Column(28).AutoFit(20);
            sheet2.Column(29).AutoFit(20);
            sheet2.Column(30).AutoFit(20);
            sheet2.Column(31).AutoFit(20);
            sheet2.Column(32).AutoFit(20);
            sheet2.Column(33).AutoFit(20);
            sheet2.Column(34).AutoFit(20);
            sheet2.Column(35).AutoFit(20);
            sheet2.Column(36).AutoFit(20);
            sheet2.Column(37).AutoFit(20);
            sheet2.Column(38).AutoFit(20);
            sheet2.Column(39).AutoFit(20);
            sheet2.Column(40).AutoFit(20);

            sheet2.Cells[rowindex, 1, rowindex, 40].Style.Font.Bold = true;
            sheet2.Cells[rowindex, 1, rowindex, 40].AutoFilter = true;

            bordesEncabezados = sheet2.Cells[rowindex, 1, rowindex, 40].Style.Border;
            bordesEncabezados.Top.Style = bordesEncabezados.Right.Style = bordesEncabezados.Bottom.Style = bordesEncabezados.Left.Style = ExcelBorderStyle.Medium;


            // SE CARGAN LOS DATOS DE TODO EL DOCUMENTO
            col = 0;
            //Empezamos a escribir sobre ella
            rowindex = 2;
            col = 1;
            //sheet2.Cells[rowindex++, col].Value = item;
            foreach (var itemDatos in datosPlan.Where(x => x.Estado.Equals("CA")))
            {
                col = 1;
                sheet2.Cells[rowindex, col++].Value = itemDatos.Referencia;
                sheet2.Cells[rowindex, col++].Value = itemDatos.CodigoEmpresa;
                sheet2.Cells[rowindex, col++].Value = itemDatos.FechaAutorizacion;
                sheet2.Cells[rowindex, col++].Value = itemDatos.CodigoAgencia;
                sheet2.Cells[rowindex, col++].Value = itemDatos.NumeroTerminal;
                sheet2.Cells[rowindex, col++].Value = itemDatos.NumeroBoleta;
                sheet2.Cells[rowindex, col++].Value = itemDatos.TipoTransaccion;
                sheet2.Cells[rowindex, col++].Value = itemDatos.CodigoUsuario;
                sheet2.Cells[rowindex, col++].Value = itemDatos.NumeroAutorizacion;
                sheet2.Cells[rowindex, col++].Value = itemDatos.TipoArchivo;
                sheet2.Cells[rowindex, col++].Value = itemDatos.CodigoCliente;
                sheet2.Cells[rowindex, col++].Value = itemDatos.NumeroTarjeta;
                sheet2.Cells[rowindex, col++].Value = itemDatos.Monto;
                sheet2.Cells[rowindex, col++].Value = itemDatos.MontoPie;
                sheet2.Cells[rowindex, col++].Value = itemDatos.MesesDiferido;
                sheet2.Cells[rowindex, col++].Value = itemDatos.MesesGracia;
                sheet2.Cells[rowindex, col++].Value = itemDatos.FechaAutoriza;
                sheet2.Cells[rowindex, col++].Value = itemDatos.NumeroCuotas;
                sheet2.Cells[rowindex, col++].Value = itemDatos.Extrafinanciamiento;
                sheet2.Cells[rowindex, col++].Value = itemDatos.CodigoPlanCredito;
                sheet2.Cells[rowindex, col++].Value = itemDatos.MontoAfecto;
                sheet2.Cells[rowindex, col++].Value = itemDatos.MontonoAfecto;
                sheet2.Cells[rowindex, col++].Value = itemDatos.CodigoComercio;
                sheet2.Cells[rowindex, col++].Value = itemDatos.CodigoRubro;
                sheet2.Cells[rowindex, col++].Value = itemDatos.RutVendedor;
                sheet2.Cells[rowindex, col++].Value = itemDatos.CodigoPromocionVenta;
                sheet2.Cells[rowindex, col++].Value = itemDatos.MontoCuota;
                sheet2.Cells[rowindex, col++].Value = itemDatos.FechaPrimerVencimiento;
                sheet2.Cells[rowindex, col++].Value = itemDatos.FechaUltimoVencimiento;
                sheet2.Cells[rowindex, col++].Value = itemDatos.Confirmada;
                sheet2.Cells[rowindex, col++].Value = itemDatos.ModoProceso;
                sheet2.Cells[rowindex, col++].Value = itemDatos.NroTrxPos;
                sheet2.Cells[rowindex, col++].Value = itemDatos.NroRef;
                sheet2.Cells[rowindex, col++].Value = itemDatos.NroCaja;
                sheet2.Cells[rowindex, col++].Value = itemDatos.TipoDiferencia;
                sheet2.Cells[rowindex, col++].Value = itemDatos.Estado;
                sheet2.Cells[rowindex, col++].Value = itemDatos.Cuenta_GL;
                sheet2.Cells[rowindex, col++].Value = itemDatos.Rut_GL;
                sheet2.Cells[rowindex, col++].Value = itemDatos.Monto_GL;
                sheet2.Cells[rowindex, col++].Value = itemDatos.MontoPago_GL;

                rowindex++;
            }

            sheet2.Column(3).Style.Numberformat.Format = "dd-MM-yyyy HH:mm:ss";

            #endregion

            #region CAAU
            rowindex = 1;

            sheet3.Cells[rowindex, 1].Value = "Referencia";
            sheet3.Cells[rowindex, 2].Value = "Codigo Empresa";
            sheet3.Cells[rowindex, 3].Value = "Fecha Autorizacion";
            sheet3.Cells[rowindex, 4].Value = "Codigo Agencia";
            sheet3.Cells[rowindex, 5].Value = "Numero Terminal";
            sheet3.Cells[rowindex, 6].Value = "Numero Boleta";
            sheet3.Cells[rowindex, 7].Value = "Tipo Transaccion";
            sheet3.Cells[rowindex, 8].Value = "Codigo Usuario";
            sheet3.Cells[rowindex, 9].Value = "Numero Autorizacion";
            sheet3.Cells[rowindex, 10].Value = "Tipo Archivo";
            sheet3.Cells[rowindex, 11].Value = "Codigo Cliente";
            sheet3.Cells[rowindex, 12].Value = "Numero Tarjeta";
            sheet3.Cells[rowindex, 13].Value = "Monto";
            sheet3.Cells[rowindex, 14].Value = "Monto Pie";
            sheet3.Cells[rowindex, 15].Value = "Meses Diferido";
            sheet3.Cells[rowindex, 16].Value = "Meses Gracia";
            sheet3.Cells[rowindex, 17].Value = "Fecha Autoriza";
            sheet3.Cells[rowindex, 18].Value = "Numero Cuotas";
            sheet3.Cells[rowindex, 19].Value = "Extrafinanciamiento";
            sheet3.Cells[rowindex, 20].Value = "Codigo Plan Credito";
            sheet3.Cells[rowindex, 21].Value = "Monto Afecto";
            sheet3.Cells[rowindex, 22].Value = "Monto no Afecto";
            sheet3.Cells[rowindex, 23].Value = "Codigo Comercio";
            sheet3.Cells[rowindex, 24].Value = "Codigo Rubro";
            sheet3.Cells[rowindex, 25].Value = "Rut Vendedor";
            sheet3.Cells[rowindex, 26].Value = "Codigo Promocion Venta";
            sheet3.Cells[rowindex, 27].Value = "Monto Cuota";
            sheet3.Cells[rowindex, 28].Value = "Fecha Primer Vencimiento";
            sheet3.Cells[rowindex, 29].Value = "Fecha Ultimo Vencimiento";
            sheet3.Cells[rowindex, 30].Value = "Confirmada";
            sheet3.Cells[rowindex, 31].Value = "Modo Proceso";
            sheet3.Cells[rowindex, 32].Value = "Nro Trx Pos";
            sheet3.Cells[rowindex, 33].Value = "Nro Ref";
            sheet3.Cells[rowindex, 34].Value = "Nro Caja";
            sheet3.Cells[rowindex, 35].Value = "Tipo Diferencia";
            sheet3.Cells[rowindex, 36].Value = "Estado";
            sheet3.Cells[rowindex, 37].Value = "Cuenta GL";
            sheet3.Cells[rowindex, 38].Value = "Rut GL";
            sheet3.Cells[rowindex, 39].Value = "Monto GL";
            sheet3.Cells[rowindex, 40].Value = "Monto Aprobado GL";

            sheet3.Column(1).AutoFit(20);
            sheet3.Column(2).AutoFit(10);
            sheet3.Column(3).AutoFit(20);
            sheet3.Column(4).AutoFit(5);
            sheet3.Column(5).AutoFit(20);
            sheet3.Column(6).AutoFit(3);
            sheet3.Column(7).AutoFit(10);
            sheet3.Column(8).AutoFit(6);
            sheet3.Column(9).AutoFit(10);
            sheet3.Column(10).AutoFit(10);
            sheet3.Column(11).AutoFit(15);
            sheet3.Column(12).AutoFit(10);
            sheet3.Column(13).AutoFit(20);
            sheet3.Column(14).AutoFit(10);
            sheet3.Column(15).AutoFit(20);
            sheet3.Column(16).AutoFit(5);
            sheet3.Column(17).AutoFit(10);
            sheet3.Column(18).AutoFit(5);
            sheet3.Column(19).AutoFit(3);
            sheet3.Column(20).AutoFit(10);
            sheet3.Column(21).AutoFit(3);
            sheet3.Column(22).AutoFit(10);
            sheet3.Column(23).AutoFit(3);
            sheet3.Column(24).AutoFit(5);
            sheet3.Column(25).AutoFit(10);
            sheet3.Column(26).AutoFit(20);
            sheet3.Column(27).AutoFit(10);
            sheet3.Column(28).AutoFit(20);
            sheet3.Column(29).AutoFit(20);
            sheet3.Column(30).AutoFit(20);
            sheet3.Column(31).AutoFit(20);
            sheet3.Column(32).AutoFit(20);
            sheet3.Column(33).AutoFit(20);
            sheet3.Column(34).AutoFit(20);
            sheet3.Column(35).AutoFit(20);
            sheet3.Column(36).AutoFit(20);
            sheet3.Column(37).AutoFit(20);
            sheet3.Column(38).AutoFit(20);
            sheet3.Column(39).AutoFit(20);
            sheet3.Column(40).AutoFit(20);

            sheet3.Cells[rowindex, 1, rowindex, 40].Style.Font.Bold = true;
            sheet3.Cells[rowindex, 1, rowindex, 40].AutoFilter = true;

            bordesEncabezados = sheet3.Cells[rowindex, 1, rowindex, 40].Style.Border;
            bordesEncabezados.Top.Style = bordesEncabezados.Right.Style = bordesEncabezados.Bottom.Style = bordesEncabezados.Left.Style = ExcelBorderStyle.Medium;

            // SE CARGAN LOS DATOS DE TODO EL DOCUMENTO
            col = 0;
            //Empezamos a escribir sobre ella
            rowindex = 2;
            col = 1;
            //sheet3.Cells[rowindex++, col].Value = item;
            foreach (var itemDatos in datosPlan.Where(x => x.Estado.Equals("BA")))
            {
                col = 1;
                sheet3.Cells[rowindex, col++].Value = itemDatos.Referencia;
                sheet3.Cells[rowindex, col++].Value = itemDatos.CodigoEmpresa;
                sheet3.Cells[rowindex, col++].Value = itemDatos.FechaAutorizacion;
                sheet3.Cells[rowindex, col++].Value = itemDatos.CodigoAgencia;
                sheet3.Cells[rowindex, col++].Value = itemDatos.NumeroTerminal;
                sheet3.Cells[rowindex, col++].Value = itemDatos.NumeroBoleta;
                sheet3.Cells[rowindex, col++].Value = itemDatos.TipoTransaccion;
                sheet3.Cells[rowindex, col++].Value = itemDatos.CodigoUsuario;
                sheet3.Cells[rowindex, col++].Value = itemDatos.NumeroAutorizacion;
                sheet3.Cells[rowindex, col++].Value = itemDatos.TipoArchivo;
                sheet3.Cells[rowindex, col++].Value = itemDatos.CodigoCliente;
                sheet3.Cells[rowindex, col++].Value = itemDatos.NumeroTarjeta;
                sheet3.Cells[rowindex, col++].Value = itemDatos.Monto;
                sheet3.Cells[rowindex, col++].Value = itemDatos.MontoPie;
                sheet3.Cells[rowindex, col++].Value = itemDatos.MesesDiferido;
                sheet3.Cells[rowindex, col++].Value = itemDatos.MesesGracia;
                sheet3.Cells[rowindex, col++].Value = itemDatos.FechaAutoriza;
                sheet3.Cells[rowindex, col++].Value = itemDatos.NumeroCuotas;
                sheet3.Cells[rowindex, col++].Value = itemDatos.Extrafinanciamiento;
                sheet3.Cells[rowindex, col++].Value = itemDatos.CodigoPlanCredito;
                sheet3.Cells[rowindex, col++].Value = itemDatos.MontoAfecto;
                sheet3.Cells[rowindex, col++].Value = itemDatos.MontonoAfecto;
                sheet3.Cells[rowindex, col++].Value = itemDatos.CodigoComercio;
                sheet3.Cells[rowindex, col++].Value = itemDatos.CodigoRubro;
                sheet3.Cells[rowindex, col++].Value = itemDatos.RutVendedor;
                sheet3.Cells[rowindex, col++].Value = itemDatos.CodigoPromocionVenta;
                sheet3.Cells[rowindex, col++].Value = itemDatos.MontoCuota;
                sheet3.Cells[rowindex, col++].Value = itemDatos.FechaPrimerVencimiento;
                sheet3.Cells[rowindex, col++].Value = itemDatos.FechaUltimoVencimiento;
                sheet3.Cells[rowindex, col++].Value = itemDatos.Confirmada;
                sheet3.Cells[rowindex, col++].Value = itemDatos.ModoProceso;
                sheet3.Cells[rowindex, col++].Value = itemDatos.NroTrxPos;
                sheet3.Cells[rowindex, col++].Value = itemDatos.NroRef;
                sheet3.Cells[rowindex, col++].Value = itemDatos.NroCaja;
                sheet3.Cells[rowindex, col++].Value = itemDatos.TipoDiferencia;
                sheet3.Cells[rowindex, col++].Value = itemDatos.Estado;
                sheet3.Cells[rowindex, col++].Value = itemDatos.Cuenta_GL;
                sheet3.Cells[rowindex, col++].Value = itemDatos.Rut_GL;
                sheet3.Cells[rowindex, col++].Value = itemDatos.Monto_GL;
                sheet3.Cells[rowindex, col++].Value = itemDatos.MontoPago_GL;

                rowindex++;
            }

            sheet3.Column(3).Style.Numberformat.Format = "dd-MM-yyyy HH:mm:ss";

            #endregion            
            //sheet.Cells[rowindex, 4].Value = "Total:";
            //sheet.Cells[rowindex, 4].Style.Font.Bold = true;
            //sheet.Cells[rowindex, 5].Formula = "=Sum(" + (object)sheet.Cells[2, 5] + ":" + (object)sheet.Cells[rowindex - 1, 5] + ")";
            //sheet.Cells[rowindex, 6].Formula = "=Sum(" + sheet.Cells[2, 6] + ":" + sheet.Cells[rowindex - 1, 6] + ")";

            //sheet.Column(2).Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
            //sheet.Column(7).Style.Numberformat.Format = "dd-MM-yyyy hh:mm";
            //var bordes = sheet.Cells[2, 1, rowindex - 1, 9].Style.Border;
            //bordes.Top.Style = bordes.Right.Style = bordes.Bottom.Style = bordes.Left.Style = ExcelBorderStyle.Thin;
            //var bordesTotal = sheet.Cells[rowindex, 4, rowindex, 6].Style.Border;
            //bordesTotal.Top.Style = bordesTotal.Right.Style = bordesTotal.Bottom.Style = bordesTotal.Left.Style = ExcelBorderStyle.Thin;
            return excelPackage;
        }

        #endregion

        #region ConciliacionCAPA


        public ExcelPackage ReportConciliadasCAPA(List<fnReporte_CAPA_PAGOS_Result> lst)
        {
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            var excelPackage = new ExcelPackage();

            var sheet = excelPackage.Workbook.Worksheets.Add("Lista Conciliadas");
            var sheet2 = excelPackage.Workbook.Worksheets.Add("Lista Global");
            var sheet3 = excelPackage.Workbook.Worksheets.Add("Lista CAAUAvances");

            //sheet.Name = item.ToString();
            #region Conciliadas
            var rowindex = 1;

            sheet.Cells[rowindex, 1].Value = "CodigoEmpresa";
            sheet.Cells[rowindex, 2].Value = "FechaAutorizacion";
            sheet.Cells[rowindex, 3].Value = "CodigoAgencia";
            sheet.Cells[rowindex, 4].Value = "NumeroTerminal";
            sheet.Cells[rowindex, 5].Value = "CodigoUsuario";
            sheet.Cells[rowindex, 6].Value = "NumeroAutorizacion";
            sheet.Cells[rowindex, 7].Value = "NumeroBoleta";
            sheet.Cells[rowindex, 8].Value = "TipoArchivo";
            sheet.Cells[rowindex, 9].Value = "CodigoCliente";
            sheet.Cells[rowindex, 10].Value = "Numerocuenta";
            sheet.Cells[rowindex, 11].Value = "Monto";
            sheet.Cells[rowindex, 12].Value = "CodigoMedioPago";
            sheet.Cells[rowindex, 13].Value = "DetalleMovimiento";
            sheet.Cells[rowindex, 14].Value = "NroTrxPos";
            sheet.Cells[rowindex, 15].Value = "NroRef";
            sheet.Cells[rowindex, 16].Value = "NroCaja";
            sheet.Cells[rowindex, 17].Value = "Rut_GL";
            sheet.Cells[rowindex, 18].Value = "Estado";
            sheet.Cells[rowindex, 19].Value = "Cuenta_GL";
            sheet.Cells[rowindex, 20].Value = "Monto_GL";
            sheet.Cells[rowindex, 21].Value = "MontoPago_GL";


            sheet.Column(1).AutoFit(20);
            sheet.Column(2).AutoFit(10);
            sheet.Column(3).AutoFit(20);
            sheet.Column(4).AutoFit(5);
            sheet.Column(5).AutoFit(20);
            sheet.Column(6).AutoFit(3);
            sheet.Column(7).AutoFit(10);
            sheet.Column(8).AutoFit(6);
            sheet.Column(9).AutoFit(10);
            sheet.Column(10).AutoFit(10);
            sheet.Column(11).AutoFit(15);
            sheet.Column(12).AutoFit(10);
            sheet.Column(13).AutoFit(20);
            sheet.Column(14).AutoFit(10);
            sheet.Column(15).AutoFit(20);
            sheet.Column(16).AutoFit(5);
            sheet.Column(17).AutoFit(10);
            sheet.Column(18).AutoFit(5);
            sheet.Column(19).AutoFit(3);
            sheet.Column(20).AutoFit(10);
            sheet.Column(21).AutoFit(3);



            sheet.Cells[rowindex, 1, rowindex, 21].Style.Font.Bold = true;
            sheet.Cells[rowindex, 1, rowindex, 21].AutoFilter = true;

            var bordesEncabezados = sheet.Cells[rowindex, 1, rowindex, 21].Style.Border;
            bordesEncabezados.Top.Style = bordesEncabezados.Right.Style = bordesEncabezados.Bottom.Style = bordesEncabezados.Left.Style = ExcelBorderStyle.Medium;


            // SE CARGAN LOS DATOS DE TODO EL DOCUMENTO
            var datosPlan = lst;
            var col = 0;
            //Empezamos a escribir sobre ella
            rowindex = 2;
            col = 1;
            //sheet.Cells[rowindex++, col].Value = item;
            foreach (var itemDatos in datosPlan.Where(x => x.Estado.Equals("AP")))
            {
                col = 1;
                sheet.Cells[rowindex, col++].Value = itemDatos.CodigoEmpresa;
                sheet.Cells[rowindex, col++].Value = itemDatos.FechaAutorizacion;
                sheet.Cells[rowindex, col++].Value = itemDatos.CodigoAgencia;
                sheet.Cells[rowindex, col++].Value = itemDatos.NumeroTerminal;
                sheet.Cells[rowindex, col++].Value = itemDatos.CodigoUsuario;
                sheet.Cells[rowindex, col++].Value = itemDatos.NumeroAutorizacion;
                sheet.Cells[rowindex, col++].Value = itemDatos.NumeroBoleta;
                sheet.Cells[rowindex, col++].Value = itemDatos.TipoArchivo;
                sheet.Cells[rowindex, col++].Value = itemDatos.CodigoCliente;
                sheet.Cells[rowindex, col++].Value = itemDatos.Numerocuenta;
                sheet.Cells[rowindex, col++].Value = itemDatos.Monto;
                sheet.Cells[rowindex, col++].Value = itemDatos.CodigoMedioPago;
                sheet.Cells[rowindex, col++].Value = itemDatos.DetalleMovimiento;
                sheet.Cells[rowindex, col++].Value = itemDatos.NroTrxPos;
                sheet.Cells[rowindex, col++].Value = itemDatos.NroRef;
                sheet.Cells[rowindex, col++].Value = itemDatos.NroCaja;
                sheet.Cells[rowindex, col++].Value = itemDatos.Rut_GL;
                sheet.Cells[rowindex, col++].Value = itemDatos.Estado;
                sheet.Cells[rowindex, col++].Value = itemDatos.Cuenta_GL;
                sheet.Cells[rowindex, col++].Value = itemDatos.Monto_GL;
                sheet.Cells[rowindex, col++].Value = itemDatos.MontoPago_GL;
                rowindex++;
            }

            sheet.Column(3).Style.Numberformat.Format = "dd-MM-yyyy HH:mm:ss";
            #endregion

            #region Global
            rowindex = 1;
            sheet2.Cells[rowindex, 1].Value = "CodigoAgencia";
            sheet2.Cells[rowindex, 2].Value = "Fecha";
            sheet2.Cells[rowindex, 3].Value = "DetalleMovimiento";
            sheet2.Cells[rowindex, 4].Value = "NroTrxPos";
            sheet2.Cells[rowindex, 5].Value = "NroRef";
            sheet2.Cells[rowindex, 6].Value = "NroCaja";
            sheet2.Cells[rowindex, 7].Value = "Rut_GL";
            sheet2.Cells[rowindex, 8].Value = "Estado";
            sheet2.Cells[rowindex, 9].Value = "Cuenta_GL";
            sheet2.Cells[rowindex, 10].Value = "Monto_GL";
            sheet2.Cells[rowindex, 11].Value = "MontoPago_GL";

            sheet2.Column(1).AutoFit(4);
            sheet2.Column(2).AutoFit(10);
            sheet2.Column(3).AutoFit(6);
            sheet2.Column(4).AutoFit(10);
            sheet2.Column(5).AutoFit(20);
            sheet2.Column(6).AutoFit(3);
            sheet2.Column(7).AutoFit(11);
            sheet2.Column(8).AutoFit(3);
            sheet2.Column(9).AutoFit(15);
            sheet2.Column(10).AutoFit(15);
            sheet2.Column(11).AutoFit(15);




            //sheet2.Cells[rowindex, 1].Value = "CodigoEmpresa";
            //sheet2.Cells[rowindex, 2].Value = "FechaAutorizacion";
            //sheet2.Cells[rowindex, 3].Value = "CodigoAgencia";
            //sheet2.Cells[rowindex, 4].Value = "NumeroTerminal";
            //sheet2.Cells[rowindex, 5].Value = "CodigoUsuario";
            //sheet2.Cells[rowindex, 6].Value = "NumeroAutorizacion";
            //sheet2.Cells[rowindex, 7].Value = "NumeroBoleta";
            //sheet2.Cells[rowindex, 8].Value = "TipoArchivo";
            //sheet2.Cells[rowindex, 9].Value = "CodigoCliente";
            //sheet2.Cells[rowindex, 10].Value = "Numerocuenta";
            //sheet2.Cells[rowindex, 11].Value = "Monto";
            //sheet2.Cells[rowindex, 12].Value = "CodigoMedioPago";
            //sheet2.Cells[rowindex, 13].Value = "DetalleMovimiento";
            //sheet2.Cells[rowindex, 14].Value = "NroTrxPos";
            //sheet2.Cells[rowindex, 15].Value = "NroRef";
            //sheet2.Cells[rowindex, 16].Value = "NroCaja";
            //sheet2.Cells[rowindex, 17].Value = "Rut_GL";
            //sheet2.Cells[rowindex, 18].Value = "Estado";
            //sheet2.Cells[rowindex, 19].Value = "Cuenta_GL";
            //sheet2.Cells[rowindex, 20].Value = "Monto_GL";
            //sheet2.Cells[rowindex, 21].Value = "MontoPago_GL";


            //sheet2.Column(1).AutoFit(20);
            //sheet2.Column(2).AutoFit(10);
            //sheet2.Column(3).AutoFit(20);
            //sheet2.Column(4).AutoFit(5);
            //sheet2.Column(5).AutoFit(20);
            //sheet2.Column(6).AutoFit(3);
            //sheet2.Column(7).AutoFit(10);
            //sheet2.Column(8).AutoFit(6);
            //sheet2.Column(9).AutoFit(10);
            //sheet2.Column(10).AutoFit(10);
            //sheet2.Column(11).AutoFit(15);
            //sheet2.Column(12).AutoFit(10);
            //sheet2.Column(13).AutoFit(20);
            //sheet2.Column(14).AutoFit(10);
            //sheet2.Column(15).AutoFit(20);
            //sheet2.Column(16).AutoFit(5);
            //sheet2.Column(17).AutoFit(10);
            //sheet2.Column(18).AutoFit(5);
            //sheet2.Column(19).AutoFit(3);
            //sheet2.Column(20).AutoFit(10);
            //sheet2.Column(21).AutoFit(3);


            sheet2.Cells[rowindex, 1, rowindex, 11].Style.Font.Bold = true;
            sheet2.Cells[rowindex, 1, rowindex, 11].AutoFilter = true;

            bordesEncabezados = sheet2.Cells[rowindex, 1, rowindex, 11].Style.Border;
            bordesEncabezados.Top.Style = bordesEncabezados.Right.Style = bordesEncabezados.Bottom.Style = bordesEncabezados.Left.Style = ExcelBorderStyle.Medium;


            // SE CARGAN LOS DATOS DE TODO EL DOCUMENTO
            col = 0;
            //Empezamos a escribir sobre ella
            rowindex = 2;
            col = 1;
            //sheet2.Cells[rowindex++, col].Value = item;
            foreach (var itemDatos in datosPlan.Where(x => x.Estado.Equals("CP")))
            {
                col = 1;
                //sheet2.Cells[rowindex, col++].Value = itemDatos.CodigoEmpresa;
                //sheet2.Cells[rowindex, col++].Value = itemDatos.FechaAutorizacion;
                sheet2.Cells[rowindex, col++].Value = itemDatos.CodigoAgencia;
                sheet2.Cells[rowindex, col++].Value = itemDatos.FechaAutorizacion.Value.ToString("dd-MM-yyyy");
                //sheet2.Cells[rowindex, col++].Value = itemDatos.NumeroTerminal;
                //sheet2.Cells[rowindex, col++].Value = itemDatos.CodigoUsuario;
                //sheet2.Cells[rowindex, col++].Value = itemDatos.NumeroAutorizacion;
                //sheet2.Cells[rowindex, col++].Value = itemDatos.NumeroBoleta;
                //sheet2.Cells[rowindex, col++].Value = itemDatos.TipoArchivo;
                //sheet2.Cells[rowindex, col++].Value = itemDatos.CodigoCliente;
                //sheet2.Cells[rowindex, col++].Value = itemDatos.Numerocuenta;
                //sheet2.Cells[rowindex, col++].Value = itemDatos.Monto;
                //sheet2.Cells[rowindex, col++].Value = itemDatos.CodigoMedioPago;
                sheet2.Cells[rowindex, col++].Value = itemDatos.DetalleMovimiento;
                sheet2.Cells[rowindex, col++].Value = itemDatos.NroTrxPos;
                sheet2.Cells[rowindex, col++].Value = itemDatos.NroRef;
                sheet2.Cells[rowindex, col++].Value = itemDatos.NroCaja;
                sheet2.Cells[rowindex, col++].Value = itemDatos.Rut_GL;
                sheet2.Cells[rowindex, col++].Value = itemDatos.Estado;
                sheet2.Cells[rowindex, col++].Value = itemDatos.Cuenta_GL;
                sheet2.Cells[rowindex, col++].Value = itemDatos.Monto_GL;
                sheet2.Cells[rowindex, col++].Value = itemDatos.MontoPago_GL;

                rowindex++;
            }

            //sheet2.Column(3).Style.Numberformat.Format = "dd-MM-yyyy HH:mm:ss";

            #endregion

            #region CAPA
            rowindex = 1;

            sheet3.Cells[rowindex, 1].Value = "CodigoEmpresa";
            sheet3.Cells[rowindex, 2].Value = "FechaAutorizacion";
            sheet3.Cells[rowindex, 3].Value = "CodigoAgencia";
            sheet3.Cells[rowindex, 4].Value = "NumeroTerminal";
            sheet3.Cells[rowindex, 5].Value = "CodigoUsuario";
            sheet3.Cells[rowindex, 6].Value = "NumeroAutorizacion";
            sheet3.Cells[rowindex, 7].Value = "NumeroBoleta";
            sheet3.Cells[rowindex, 8].Value = "TipoArchivo";
            sheet3.Cells[rowindex, 9].Value = "CodigoCliente";
            sheet3.Cells[rowindex, 10].Value = "Numerocuenta";
            sheet3.Cells[rowindex, 11].Value = "Monto";
            sheet3.Cells[rowindex, 12].Value = "CodigoMedioPago";
            sheet3.Cells[rowindex, 13].Value = "DetalleMovimiento";
            sheet3.Cells[rowindex, 14].Value = "NroTrxPos";
            sheet3.Cells[rowindex, 15].Value = "NroRef";
            sheet3.Cells[rowindex, 16].Value = "NroCaja";
            sheet3.Cells[rowindex, 17].Value = "Rut_GL";
            sheet3.Cells[rowindex, 18].Value = "Estado";
            sheet3.Cells[rowindex, 19].Value = "Cuenta_GL";
            sheet3.Cells[rowindex, 20].Value = "Monto_GL";
            sheet3.Cells[rowindex, 21].Value = "MontoPago_GL";


            sheet3.Column(1).AutoFit(20);
            sheet3.Column(2).AutoFit(10);
            sheet3.Column(3).AutoFit(20);
            sheet3.Column(4).AutoFit(5);
            sheet3.Column(5).AutoFit(20);
            sheet3.Column(6).AutoFit(3);
            sheet3.Column(7).AutoFit(10);
            sheet3.Column(8).AutoFit(6);
            sheet3.Column(9).AutoFit(10);
            sheet3.Column(10).AutoFit(10);
            sheet3.Column(11).AutoFit(15);
            sheet3.Column(12).AutoFit(10);
            sheet3.Column(13).AutoFit(20);
            sheet3.Column(14).AutoFit(10);
            sheet3.Column(15).AutoFit(20);
            sheet3.Column(16).AutoFit(5);
            sheet3.Column(17).AutoFit(10);
            sheet3.Column(18).AutoFit(5);
            sheet3.Column(19).AutoFit(3);
            sheet3.Column(20).AutoFit(10);
            sheet3.Column(21).AutoFit(3);

            sheet3.Cells[rowindex, 1, rowindex, 21].Style.Font.Bold = true;
            sheet3.Cells[rowindex, 1, rowindex, 21].AutoFilter = true;

            bordesEncabezados = sheet3.Cells[rowindex, 1, rowindex, 21].Style.Border;
            bordesEncabezados.Top.Style = bordesEncabezados.Right.Style = bordesEncabezados.Bottom.Style = bordesEncabezados.Left.Style = ExcelBorderStyle.Medium;

            // SE CARGAN LOS DATOS DE TODO EL DOCUMENTO
            col = 0;
            //Empezamos a escribir sobre ella
            rowindex = 2;
            col = 1;
            //sheet3.Cells[rowindex++, col].Value = item;
            foreach (var itemDatos in datosPlan.Where(x => x.Estado.Equals("BP")))
            {
                col = 1;
                sheet3.Cells[rowindex, col++].Value = itemDatos.CodigoEmpresa;
                sheet3.Cells[rowindex, col++].Value = itemDatos.FechaAutorizacion;
                sheet3.Cells[rowindex, col++].Value = itemDatos.CodigoAgencia;
                sheet3.Cells[rowindex, col++].Value = itemDatos.NumeroTerminal;
                sheet3.Cells[rowindex, col++].Value = itemDatos.CodigoUsuario;
                sheet3.Cells[rowindex, col++].Value = itemDatos.NumeroAutorizacion;
                sheet3.Cells[rowindex, col++].Value = itemDatos.NumeroBoleta;
                sheet3.Cells[rowindex, col++].Value = itemDatos.TipoArchivo;
                sheet3.Cells[rowindex, col++].Value = itemDatos.CodigoCliente;
                sheet3.Cells[rowindex, col++].Value = itemDatos.Numerocuenta;
                sheet3.Cells[rowindex, col++].Value = itemDatos.Monto;
                sheet3.Cells[rowindex, col++].Value = itemDatos.CodigoMedioPago;
                sheet3.Cells[rowindex, col++].Value = itemDatos.DetalleMovimiento;
                sheet3.Cells[rowindex, col++].Value = itemDatos.NroTrxPos;
                sheet3.Cells[rowindex, col++].Value = itemDatos.NroRef;
                sheet3.Cells[rowindex, col++].Value = itemDatos.NroCaja;
                sheet3.Cells[rowindex, col++].Value = itemDatos.Rut_GL;
                sheet3.Cells[rowindex, col++].Value = itemDatos.Estado;
                sheet3.Cells[rowindex, col++].Value = itemDatos.Cuenta_GL;
                sheet3.Cells[rowindex, col++].Value = itemDatos.Monto_GL;
                sheet3.Cells[rowindex, col++].Value = itemDatos.MontoPago_GL;

                rowindex++;
            }

            //sheet3.Column(3).Style.Numberformat.Format = "dd-MM-yyyy HH:mm:ss";

            #endregion            
            //sheet.Cells[rowindex, 4].Value = "Total:";
            //sheet.Cells[rowindex, 4].Style.Font.Bold = true;
            //sheet.Cells[rowindex, 5].Formula = "=Sum(" + (object)sheet.Cells[2, 5] + ":" + (object)sheet.Cells[rowindex - 1, 5] + ")";
            //sheet.Cells[rowindex, 6].Formula = "=Sum(" + sheet.Cells[2, 6] + ":" + sheet.Cells[rowindex - 1, 6] + ")";

            //sheet.Column(2).Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
            //sheet.Column(7).Style.Numberformat.Format = "dd-MM-yyyy hh:mm";
            //var bordes = sheet.Cells[2, 1, rowindex - 1, 9].Style.Border;
            //bordes.Top.Style = bordes.Right.Style = bordes.Bottom.Style = bordes.Left.Style = ExcelBorderStyle.Thin;
            //var bordesTotal = sheet.Cells[rowindex, 4, rowindex, 6].Style.Border;
            //bordesTotal.Top.Style = bordesTotal.Right.Style = bordesTotal.Bottom.Style = bordesTotal.Left.Style = ExcelBorderStyle.Thin;
            return excelPackage;
        }

        #endregion





        #region ConciliacionSencillosTesoreria
        public ExcelPackage ReporteTesoreria(ReporteTesoreriaView lst)
        {
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            var excelPackage = new ExcelPackage();

            int sheetIndex = 1; // Índice de la hoja
            var sheet = excelPackage.Workbook.Worksheets.Add("Lista Conciliadas " + sheetIndex.ToString()); // Agregar una nueva hoja con nombre
            sheetIndex++; // Incrementar el índice de la hoja
            //sheet.Name = item.ToString();
            var rowindex = 1;
            int col = 1;
            sheet.Cells[rowindex, col++].Value = "idTienda";
            sheet.Cells[rowindex, col++].Value = "MontoTesoreria";
            sheet.Cells[rowindex, col++].Value = "MontoSAP";
            sheet.Cells[rowindex, col++].Value = "Banco";
            sheet.Cells[rowindex, col++].Value = "DiaLiberacion";
            sheet.Cells[rowindex, col++].Value = "DiaEntrega";
            sheet.Cells[rowindex, col++].Value = "Estado";
            sheet.Cells[rowindex, col++].Value = "Asignacion";
            sheet.Cells[rowindex, col++].Value = "FechaDoc";


            sheet.Column(1).AutoFit(2);
            sheet.Column(2).AutoFit(5);
            sheet.Column(3).AutoFit(10);
            sheet.Column(4).AutoFit(10);
            sheet.Column(5).AutoFit(7);
            sheet.Column(6).AutoFit(3);
            sheet.Column(7).AutoFit(5);
            sheet.Column(8).AutoFit(10);
            sheet.Column(9).AutoFit(10);



            sheet.Cells[rowindex, 1, rowindex, 9].Style.Font.Bold = true;
            sheet.Cells[rowindex, 1, rowindex, 9].AutoFilter = true;

            var bordesEncabezados = sheet.Cells[rowindex, 1, rowindex, 9].Style.Border;
            bordesEncabezados.Top.Style = bordesEncabezados.Right.Style = bordesEncabezados.Bottom.Style = bordesEncabezados.Left.Style = ExcelBorderStyle.Medium;


            // SE CARGAN LOS DATOS DE TODO EL DOCUMENTO
            var datosPlan = lst;
            col = 0;
            //Empezamos a escribir sobre ella
            rowindex = 2;
            col = 1;
            //sheet.Cells[rowindex++, col].Value = item;
            foreach (var datos in datosPlan.SAP.OrderBy(x => x.idTienda))
            {
                col = 1;
                sheet.Cells[rowindex, col++].Value = datos.idTienda;
                sheet.Cells[rowindex, col++].Value = datos.MontoTesoreria.Value.ToString("C");
                sheet.Cells[rowindex, col++].Value = datos.MontoSAP.Value.ToString("C");
                sheet.Cells[rowindex, col++].Value = datos.Banco;
                sheet.Cells[rowindex, col++].Value = datos.DiaLiberacion;
                sheet.Cells[rowindex, col++].Value = datos.DiaEntrega;
                sheet.Cells[rowindex, col++].Value = datos.Estado;
                sheet.Cells[rowindex, col++].Value = datos.Asignacion;
                sheet.Cells[rowindex, col++].Value = datos.FechaDoc.Value.ToString("yyyy-MM-dd");


                rowindex++;
            }

            sheet = excelPackage.Workbook.Worksheets.Add("No Conciliadas " + sheetIndex.ToString()); // Agregar una nueva hoja con nombre diferente
            sheetIndex++; // Incrementar el índice de la hoja
            rowindex = 1;
            col = 1;
            sheet.Cells[rowindex, col++].Value = "idtienda";
            sheet.Cells[rowindex, col++].Value = "NuevoTotal";
            sheet.Cells[rowindex, col++].Value = "Banco";
            sheet.Cells[rowindex, col++].Value = "DiaLiberacion";
            sheet.Cells[rowindex, col++].Value = "DiaEntrega";
            sheet.Cells[rowindex, col++].Value = "FechaInicio";
            sheet.Cells[rowindex, col++].Value = "FechaFin";


            sheet.Cells[rowindex, 1, rowindex, 7].Style.Font.Bold = true;
            sheet.Cells[rowindex, 1, rowindex, 7].AutoFilter = true;

            var bordesEncabezados2 = sheet.Cells[rowindex, 1, rowindex, 7].Style.Border;
            bordesEncabezados2.Top.Style = bordesEncabezados2.Right.Style = bordesEncabezados2.Bottom.Style = bordesEncabezados2.Left.Style = ExcelBorderStyle.Medium;
            col = 0;
            //Empezamos a escribir sobre ella
            rowindex = 2;
            col = 1;

            foreach (var datos in datosPlan.NoConciliadas)
            {
                col = 1;

                sheet.Cells[rowindex, col++].Value = datos.idtienda;
                sheet.Cells[rowindex, col++].Value = datos.NuevoTotal;
                sheet.Cells[rowindex, col++].Value = datos.Banco;
                sheet.Cells[rowindex, col++].Value = datos.DiaLiberacion;
                sheet.Cells[rowindex, col++].Value = datos.DiaEntrega;
                sheet.Cells[rowindex, col++].Value = datos.FechaInicio.Value.ToString("dd-MM-yyyy");
                sheet.Cells[rowindex, col++].Value = datos.FechaFin.Value.ToString("dd-MM-yyyy");




                rowindex++;
            }


            return excelPackage;
        }

        public ExcelPackage ReporteSencilloCuadratura(ReporteTesoreriaView lst, List<Sencillos_Tiendas> lstTiendas)
        {
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            var excelPackage = new ExcelPackage();
            #region Conciliadas
            int sheetIndex = 1; // Índice de la hoja
            var sheet = excelPackage.Workbook.Worksheets.Add("Lista Conciliadas " + sheetIndex.ToString()); // Agregar una nueva hoja con nombre
            sheetIndex++; // Incrementar el índice de la hoja
            //sheet.Name = item.ToString();
            var rowindex = 1;
            int col = 1;
            sheet.Cells[rowindex, col++].Value = "idTienda";
            sheet.Cells[rowindex, col++].Value = "MontoTesoreria";
            sheet.Cells[rowindex, col++].Value = "MontoSAP";
            sheet.Cells[rowindex, col++].Value = "Banco";
            sheet.Cells[rowindex, col++].Value = "DiaLiberacion";
            sheet.Cells[rowindex, col++].Value = "DiaEntrega";
            sheet.Cells[rowindex, col++].Value = "Estado";
            sheet.Cells[rowindex, col++].Value = "Asignacion";
            sheet.Cells[rowindex, col++].Value = "FechaDoc";


            sheet.Column(1).AutoFit(2);
            sheet.Column(2).AutoFit(5);
            sheet.Column(3).AutoFit(10);
            sheet.Column(4).AutoFit(10);
            sheet.Column(5).AutoFit(7);
            sheet.Column(6).AutoFit(3);
            sheet.Column(7).AutoFit(5);
            sheet.Column(8).AutoFit(10);
            sheet.Column(9).AutoFit(10);



            sheet.Cells[rowindex, 1, rowindex, 9].Style.Font.Bold = true;
            sheet.Cells[rowindex, 1, rowindex, 9].AutoFilter = true;

            var bordesEncabezados = sheet.Cells[rowindex, 1, rowindex, 9].Style.Border;
            bordesEncabezados.Top.Style = bordesEncabezados.Right.Style = bordesEncabezados.Bottom.Style = bordesEncabezados.Left.Style = ExcelBorderStyle.Medium;


            // SE CARGAN LOS DATOS DE TODO EL DOCUMENTO
            var datosPlan = lst;
            col = 0;
            //Empezamos a escribir sobre ella
            rowindex = 2;
            col = 1;
            //sheet.Cells[rowindex++, col].Value = item;
            foreach (var datos in datosPlan.SAP.OrderBy(x => x.idTienda).OrderBy(x => x.idTienda))
            {
                col = 1;
                sheet.Cells[rowindex, col++].Value = datos.idTienda;
                sheet.Cells[rowindex, col++].Value = datos.MontoTesoreria.Value.ToString("C");
                sheet.Cells[rowindex, col++].Value = datos.MontoSAP.Value.ToString("C");
                sheet.Cells[rowindex, col++].Value = datos.Banco;
                sheet.Cells[rowindex, col++].Value = datos.DiaLiberacion;
                sheet.Cells[rowindex, col++].Value = datos.DiaEntrega;
               sheet.Cells[rowindex, col++].Value = datos.Estado.Value ? "Conciliado" : "No Conciliado";

                sheet.Cells[rowindex, col++].Value = datos.Asignacion;
                sheet.Cells[rowindex, col++].Value = datos.FechaDoc.Value.ToString("yyyy-MM-dd");


                rowindex++;
            }
            #endregion

            #region Noconciliadas
            sheet = excelPackage.Workbook.Worksheets.Add("No Conciliadas " + sheetIndex.ToString()); // Agregar una nueva hoja con nombre diferente
            sheetIndex++; // Incrementar el índice de la hoja
            rowindex = 1;
            col = 1;
            sheet.Cells[rowindex, col++].Value = "idtienda";
            sheet.Cells[rowindex, col++].Value = "NuevoTotal";
            sheet.Cells[rowindex, col++].Value = "Banco";
            sheet.Cells[rowindex, col++].Value = "DiaLiberacion";
            sheet.Cells[rowindex, col++].Value = "DiaEntrega";
            sheet.Cells[rowindex, col++].Value = "FechaInicio";
            sheet.Cells[rowindex, col++].Value = "FechaFin";


            sheet.Cells[rowindex, 1, rowindex, 7].Style.Font.Bold = true;
            sheet.Cells[rowindex, 1, rowindex, 7].AutoFilter = true;

            var bordesEncabezados2 = sheet.Cells[rowindex, 1, rowindex, 7].Style.Border;
            bordesEncabezados2.Top.Style = bordesEncabezados2.Right.Style = bordesEncabezados2.Bottom.Style = bordesEncabezados2.Left.Style = ExcelBorderStyle.Medium;
            col = 0;
            //Empezamos a escribir sobre ella
            rowindex = 2;
            col = 1;

            foreach (var datos in datosPlan.NoConciliadas.OrderBy(x=>x.idtienda))
            {
                col = 1;

                sheet.Cells[rowindex, col++].Value = datos.idtienda;
                sheet.Cells[rowindex, col++].Value = datos.NuevoTotal;
                sheet.Cells[rowindex, col++].Value = datos.Banco;
                sheet.Cells[rowindex, col++].Value = datos.DiaLiberacion;
                sheet.Cells[rowindex, col++].Value = datos.DiaEntrega;
                sheet.Cells[rowindex, col++].Value = datos.FechaInicio.Value.ToString("dd-MM-yyyy");
                sheet.Cells[rowindex, col++].Value = datos.FechaFin.Value.ToString("dd-MM-yyyy");




                rowindex++;
            }
            #endregion

            #region Transito
            sheet = excelPackage.Workbook.Worksheets.Add("Transito" + sheetIndex.ToString()); // Agregar una nueva hoja con nombre diferente
            sheetIndex++; // Incrementar el índice de la hoja
            rowindex = 1;
            col = 1;
            sheet.Cells[rowindex, col++].Value = "idtienda";
            sheet.Cells[rowindex, col++].Value = "NuevoTotal";
            sheet.Cells[rowindex, col++].Value = "Banco";
            sheet.Cells[rowindex, col++].Value = "DiaLiberacion";
            sheet.Cells[rowindex, col++].Value = "DiaEntrega";
            sheet.Cells[rowindex, col++].Value = "FechaInicio";
            sheet.Cells[rowindex, col++].Value = "FechaFin";


            sheet.Cells[rowindex, 1, rowindex, 7].Style.Font.Bold = true;
            sheet.Cells[rowindex, 1, rowindex, 7].AutoFilter = true;

            var bordesEncabezados3 = sheet.Cells[rowindex, 1, rowindex, 7].Style.Border;
            bordesEncabezados2.Top.Style = bordesEncabezados3.Right.Style = bordesEncabezados3.Bottom.Style = bordesEncabezados3.Left.Style = ExcelBorderStyle.Medium;
            col = 0;
            //Empezamos a escribir sobre ella
            rowindex = 2;
            col = 1;

            foreach (var datos in lstTiendas.Where(x=>x.CodigoEstadoSencillo == 2).OrderBy(x => x.idTienda))
            {
                col = 1;

                sheet.Cells[rowindex, col++].Value = datos.idTienda;
                sheet.Cells[rowindex, col++].Value = datos.DetalleSencillo.NuevoTotal;
                sheet.Cells[rowindex, col++].Value = datos.DetalleSencillo.Banco;
                sheet.Cells[rowindex, col++].Value = datos.DetalleSencillo.FechaLiberacion.Value.ToString("dd-MM-yyyy");
                sheet.Cells[rowindex, col++].Value = datos.DetalleSencillo.FechaEntrega.HasValue ? datos.DetalleSencillo.FechaEntrega.Value.ToString("dd-MM-yyyy") : "0";
                sheet.Cells[rowindex, col++].Value = datos.DetalleSencillo.Sencillos.FechaInicio.Value.ToString("dd-MM-yyyy");
                sheet.Cells[rowindex, col++].Value = datos.DetalleSencillo.Sencillos.FechaFin.Value.ToString("dd-MM-yyyy");




                rowindex++;
            }
            #endregion

            #region Aceptado
            sheet = excelPackage.Workbook.Worksheets.Add("Aceptado" + sheetIndex.ToString()); // Agregar una nueva hoja con nombre diferente
            sheetIndex++; // Incrementar el índice de la hoja
            rowindex = 1;
            col = 1;
            sheet.Cells[rowindex, col++].Value = "Correlativo";
            sheet.Cells[rowindex, col++].Value = "idtienda";
            sheet.Cells[rowindex, col++].Value = "NuevoTotal";
            sheet.Cells[rowindex, col++].Value = "Banco";
            sheet.Cells[rowindex, col++].Value = "DiaLiberacion";
            sheet.Cells[rowindex, col++].Value = "DiaEntrega";
            sheet.Cells[rowindex, col++].Value = "FechaInicio";
            sheet.Cells[rowindex, col++].Value = "FechaFin";
            sheet.Cells[rowindex, col++].Value = "RemitoAceptacion";
            sheet.Cells[rowindex, col++].Value = "FechaAceptacion";



            sheet.Cells[rowindex, 1, rowindex, 10].Style.Font.Bold = true;
            sheet.Cells[rowindex, 1, rowindex, 10].AutoFilter = true;

            var bordesEncabezados4 = sheet.Cells[rowindex, 1, rowindex, 10].Style.Border;
            bordesEncabezados2.Top.Style = bordesEncabezados4.Right.Style = bordesEncabezados4.Bottom.Style = bordesEncabezados4.Left.Style = ExcelBorderStyle.Medium;
            col = 0;
            //Empezamos a escribir sobre ella
            rowindex = 2;
            col = 1;

            foreach (var datos in lstTiendas.Where(x => x.CodigoEstadoSencillo == 3).OrderBy(x=>x.idTienda))
            {
                col = 1;
                Remito remito = new Remito();

                if (datos.Remito == null)
                {
                      remito = db.Remito.Where(x => x.idSencillosTienda == datos.DetalleSencillo.Id && x.CodigoTipoRemito == 1).SingleOrDefault();
                }

                sheet.Cells[rowindex, col++].Value = datos.DetalleSencillo.Sencillos.Correlativo;
                sheet.Cells[rowindex, col++].Value = datos.idTienda;
                sheet.Cells[rowindex, col++].Value = datos.DetalleSencillo.NuevoTotal;
                sheet.Cells[rowindex, col++].Value = datos.DetalleSencillo.Banco;
                sheet.Cells[rowindex, col++].Value = datos.DetalleSencillo.FechaLiberacion.Value.ToString("dd-MM-yyyy");
                sheet.Cells[rowindex, col++].Value = datos.DetalleSencillo.FechaEntrega.HasValue ? datos.DetalleSencillo.FechaEntrega.Value.ToString("dd-MM-yyyy") : "0";
                sheet.Cells[rowindex, col++].Value = datos.DetalleSencillo.Sencillos.FechaInicio.Value.ToString("dd-MM-yyyy");
                sheet.Cells[rowindex, col++].Value = datos.DetalleSencillo.Sencillos.FechaFin.Value.ToString("dd-MM-yyyy");
                sheet.Cells[rowindex, col++].Value = datos.Remito.Select(x => x.Codigo).SingleOrDefault()?.ToString() ?? remito.Codigo;
                sheet.Cells[rowindex, col++].Value = datos.Remito.Select(x=>x.Fecha.Value.ToString("dd-MM-yyyy")).SingleOrDefault().ToString() ?? remito.Fecha.Value.ToString("dd-MM-yyyy");

                rowindex++;
            }
            #endregion
            #region Devolucion
            sheet = excelPackage.Workbook.Worksheets.Add("Devolucion" + sheetIndex.ToString()); // Agregar una nueva hoja con nombre diferente
            sheetIndex++; // Incrementar el índice de la hoja
            rowindex = 1;
            col = 1;
            sheet.Cells[rowindex, col++].Value = "Correlativo";
            sheet.Cells[rowindex, col++].Value = "idtienda";
            sheet.Cells[rowindex, col++].Value = "NuevoTotal";
            sheet.Cells[rowindex, col++].Value = "Banco";
            sheet.Cells[rowindex, col++].Value = "DiaLiberacion";
            sheet.Cells[rowindex, col++].Value = "DiaEntrega";
            sheet.Cells[rowindex, col++].Value = "FechaInicio";
            sheet.Cells[rowindex, col++].Value = "FechaFin";
            sheet.Cells[rowindex, col++].Value = "RemitoAceptacion";
            sheet.Cells[rowindex, col++].Value = "FechaAceptacion";
            sheet.Cells[rowindex, col++].Value = "RemitoDevolucion";
            sheet.Cells[rowindex, col++].Value = "FechaDevolucion";
            sheet.Cells[rowindex, col++].Value = "N° Deposito";



            sheet.Cells[rowindex, 1, rowindex, 13].Style.Font.Bold = true;
            sheet.Cells[rowindex, 1, rowindex, 13].AutoFilter = true;

            var bordesEncabezados5 = sheet.Cells[rowindex, 1, rowindex, 13].Style.Border;
            bordesEncabezados2.Top.Style = bordesEncabezados5.Right.Style = bordesEncabezados5.Bottom.Style = bordesEncabezados5.Left.Style = ExcelBorderStyle.Medium;
            col = 0;
            //Empezamos a escribir sobre ella
            rowindex = 2;
            col = 1;

            foreach (var datos in lstTiendas.Where(x => x.CodigoEstadoSencillo == 4).OrderBy(x => x.idTienda))
            {
                col = 1;


                sheet.Cells[rowindex, col++].Value = datos.DetalleSencillo.Sencillos.Correlativo;
                sheet.Cells[rowindex, col++].Value = datos.idTienda;
                sheet.Cells[rowindex, col++].Value = datos.DetalleSencillo.NuevoTotal;
                sheet.Cells[rowindex, col++].Value = datos.DetalleSencillo.Banco;
                sheet.Cells[rowindex, col++].Value = datos.DetalleSencillo.FechaLiberacion.Value.ToString("dd-MM-yyyy");
                sheet.Cells[rowindex, col++].Value = datos.DetalleSencillo.FechaEntrega.HasValue ? datos.DetalleSencillo.FechaEntrega.Value.ToString("dd-MM-yyyy") : "0";
                sheet.Cells[rowindex, col++].Value = datos.DetalleSencillo.Sencillos.FechaInicio.Value.ToString("dd-MM-yyyy");
                sheet.Cells[rowindex, col++].Value = datos.DetalleSencillo.Sencillos.FechaFin.Value.ToString("dd-MM-yyyy");
                sheet.Cells[rowindex, col++].Value = datos.Remito.Where(x => x.CodigoTipoRemito == 1).Select(x => x.Codigo).SingleOrDefault();
                sheet.Cells[rowindex, col++].Value = datos.Remito.Where(x => x.CodigoTipoRemito == 1).Select(x => x.Fecha.Value.ToString("dd-MM-yyyy")).SingleOrDefault();
                sheet.Cells[rowindex, col++].Value = datos.Remito.Where(x => x.CodigoTipoRemito == 2).Select(x => x.Codigo).SingleOrDefault();
                sheet.Cells[rowindex, col++].Value = datos.Remito.Where(x => x.CodigoTipoRemito == 2).Select(x => x.Fecha.Value.ToString("dd-MM-yyyy")).SingleOrDefault();
                sheet.Cells[rowindex, col++].Value = datos.Remito.Where(x => x.CodigoTipoRemito == 2).Select(x => x.NumeroDepostio).SingleOrDefault();

                rowindex++;
            }
            #endregion
            return excelPackage;
        }

        #endregion




    }
}
