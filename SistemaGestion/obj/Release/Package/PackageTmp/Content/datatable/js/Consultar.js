var tabla_data;
$(document).ready(function () {
tabla_data = $("#tbReporte").DataTable({
    "processing": true,
    
    "ajax": {
        "url": $.MisUrls.url._ObtenerCompras + "?local=" + "&documento=" + "&fechaCon=" ,
        "type": "GET",
    "datatype": "json"
                    },
                    
                    "columns": [

                            { "data": "Centro", "name": "Centro", "autoWidth": true },
                            { "data": "Caja", "name": "Caja", "autoWidth": true },
                            {
                                "data": "Fecha",
                                "name": "Fecha",
                                "autoWidth": true,
                                
                            },
                            { "data": "Tipo_Documento", "name": "Tipo_Documento", "autoWidth": true },
                            { "data": "Folio_Documento", "name": "Folio_Documento", "autoWidth": true },
                            { "data": "Total", "name": "Total", "autoWidth": true },

                         

                    ],
                    "language": {
                        "url": $.MisUrls.url.Url_datatable_spanish
                    }
                    

        });
})
function buscar() {



    tabla_data.ajax.url($.MisUrls.url._ObtenerCompras + "?" +
        "&documento=" + $("#txtTipo").val() +
        "&fechaCon=" + $("#txtFecha").val() +
        "&local=" + $("#txtLocal").val()).load();
}

