var tabla_data;
var tabla_data;
$(document).ready(function () {
    tabla_data = $('#tbReporte').DataTable({
        responsive: true,
        "processing": true,

        "ajax": {
            "url": $.MisUrls.url._ObtenerLibro + "?local=" + "&fechaCon=",
            "type": "GET",
            "datatype": "json"
        },

        "columns": [

                { "data": "Sociedad", "name": "Sociedad", "autoWidth": true },
                { "data": "Centro", "name": "Centro", "autoWidth": true },
                { "data": "ClaseDocumental", "name": "ClaseDocumental", "autoWidth": true, },
                { "data": "NumeroDocto", "name": "NumeroDocto", "autoWidth": true },
                { "data": "FechaContabilizacion", "name": "FechaContabilizacion", "autoWidth": true },
                { "data": "Referencia", "name": "Referencia", "autoWidth": true },
                { "data": "FechaDocumento", "name": "FechaDocumento", "autoWidth": true },
                { "data": "IndicadorImpuesto", "name": "IndicadorImpuesto", "autoWidth": true },
                { "data": "NombreCliente", "name": "NombreCliente", "autoWidth": true },
                { "data": "NumeroIdentFiscal", "name": "NumeroIdentFiscal", "autoWidth": true },
                { "data": "Exento", "name": "Exento", "autoWidth": true },
              {
                  "data": "Afecto", "autoWidth": true,
                  render: function (data, type) {
                      var number = $.fn.dataTable.render.number(',', '', '', '$').display(data);
                      return number;
                  }
              },
                {
                    "data": "Iva", "autoWidth": true,
                    render: function (data, type) {
                        var number = $.fn.dataTable.render.number(',', '', '', '$').display(data);
                        return number;
                    }
                },
                {
                    "data": "BaseImponibleMP", "autoWidth": true ,
                    render: function (data, type) {
                        var number = $.fn.dataTable.render.number(',', '', '', '$').display(data);
                        return number;
                    }
                },


        ],
        "language": {
            "url": $.MisUrls.url.Url_datatable_spanish
        }


    });
    jQuery.ajax({
        url: $.MisUrls.url._TiendaPorNombre,
        type: "GET",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            $("<option>").attr({ "value": 0 }).text("-- Seleccionar todas--").appendTo("#cboTienda");
            $.each(data.data, function (i, item) {
                $("<option>").attr({ "value": item.Centro }).text(item.Nombre_Centro).appendTo("#cboTienda");

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
        dateFormat: 'dd.mm.yy',
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
    if ($("#txtFecha").val().trim() == "") {
        swal("Mensaje", "Debe ingresar Todos los campos", "warning")
        return;
    }

    tabla_data.ajax.url($.MisUrls.url._ObtenerLibro + "?" +
        "&fechaCon=" + $("#txtFecha").val().trim() +
        "&local=" + $("#cboTienda").val()).load();
}

$(function (){
    $("#btcarga").click(function () {
        event.preventDefault();
        
        var url = "/Gestion/CargaLibro";
        var manejarErrorAjax = "Eror";
        alert("Cargando Libro de Venta");
        $.post(url).done(function (resultado) {
            if (resultado.error) {
                swal("Se Cargo Correctamente!", "Presiona OK Para Continuar", "success");
            }else{
                swal("Error no se cargo!", "Presiona OK Para Continuar", "warning");
            }
            $("mensaje-alerta").html(resultado.respuesta)
        }).fail(manejarErrorAjax);


    });

})


function ObtenerFecha() {

    var d = new Date();
    var month = d.getMonth() + 1;
    var day = d.getDate();
    var output = (('' + day).length < 2 ? '0' : '') + day + '.' + (('' + month).length < 2 ? '0' : '') + month + '.' + d.getFullYear();

    return output;
}




