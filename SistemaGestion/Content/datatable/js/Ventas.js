/*/*/// <reference path="Ventas.js" />*/*/
var tabla_data;
$(document).ready(function () {
    tabla_data = $("#tbReporte").DataTable({
        responsive: true,
        "processing": true,

        "ajax": {
            "url": $.MisUrls.url._ObtenerVenta + "?local=" + "&fechaCon=",
            "type": "GET",
            "datatype": "json"
        },

        "columns": [

                { "data": "Local", "name": "Local", "autoWidth": true, },
                { "data": "Local", "name": "Local", "autoWidth": true, },
                { "data": "Estado", "name": "Estado", "autoWidth": true },
                { "data": "Observacion", "name": "Observacion", "autoWidth": true },
                { "data": "Fecha", "name": "Fecha", "autoWidth": true },
                { "data": "Archivo_IDOC", "name": "Archivo_IDOC", "autoWidth": true },
                { "data": "ID_Proceso", "name": "ID_Proceso", "autoWidth": true },
                { "data": "FechaHora", "name": "FechaHora", "autoWidth": true },

               

        ],

        "language": {
            url: $.MisUrls.url.Url_datatable_spanish
        }


    });





    jQuery.ajax({
        url: $.MisUrls.url._TiendaPorNombre,
        type: "GET",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            $("<option>").attr({ "value": 0 }).text("--  Seleccionar Tienda--").appendTo("#cboTienda");
            $.each(data.data, function (i, item) {
                $("<option>").attr({ "value": item.Codigo }).text(item.Nombre_Centro).appendTo("#cboTienda");

            })
        },
        error: function (error) {
            console.log(error)
        },

    });

    



    $.datepicker.regional['es'] = {
        closeText: 'Cerrar',
        prevText: '< Ant',
        nextText: 'Sig >',
        currentText: 'Hoy',
        monthNames: ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio', 'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'],
        monthNamesShort: ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'],
        dayNames: ['Domingo', 'Lunes', 'Martes', 'Miércoles', 'Jueves', 'Viernes', 'Sábado'],
        dayNamesShort: ['Dom', 'Lun', 'Mar', 'Mié', 'Juv', 'Vie', 'Sáb'],
        dayNamesMin: ['Do', 'Lu', 'Ma', 'Mi', 'Ju', 'Vi', 'Sá'],
        weekHeader: 'Sm',
        dateFormat: 'dd-mm-yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    $.datepicker.setDefaults($.datepicker.regional['es']);
    $("#txtFecha").datepicker();
    $("#txtFecha").val(ObtenerFecha());

})





function buscar() {
    if ($("#cboTienda").val().trim() == "") {
        swal("Mensaje", "Debe ingresar Todos los campos", "warning")
        return;
    }

    tabla_data.ajax.url($.MisUrls.url._ObtenerVenta + "?" +
        "&fechaCon=" + $("#txtFecha").val().trim() +
        "&local=" + $("#cboTienda").val()).load();
}
function ObtenerFecha() {

    var d = new Date();
    var month = d.getMonth() + 1;
    var day = d.getDate();
    var output = (('' + day).length < 2 ? '0' : '') + day + '-' + (('' + month).length < 2 ? '0' : '') + month + '-' + d.getFullYear();

    return output;
}

//function redireccion(){
//    location.href = "Gestion/GenerarIdoc"
//}