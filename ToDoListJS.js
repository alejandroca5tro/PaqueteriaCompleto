var ArrayDatos = [];
var ArrayProceso = [];
var FiltroIdCaso = "";
var FiltroNumeroSGL = "";
var FiltroFechaInicio = "";
var FiltroFechaSLA = "";
var jsonObject = "";
var jsonObjectAbog = "";
var notification;
var Abogado;
var ArrayTerr = [];
var ArraySucur = [];
var errorWS = "Por favor espere y vuelva a lanzar la peticion,si el problema persiste contacte con el administrador del sistema.";
var ComboFiltroProc="";
var ComboFiltroSucur = "";
var ComboFiltroTerr = "";
var Inicio = true;
var errorFiltro = false;

$(document).ready(function () {
    Init();
});

function Init() {
    if (Inicio == true) {
        PintarTablaInicio();
    }
     notification = $("#notification").kendoNotification({
        position: {
            pinned: true,
            top: 30,
            right: 30
        },
        autoHideAfter: 0,
        stacking: "down",
        templates: [ {
            type: "error",
            template: $("#errorTemplate").html()
        }]
    }).data("kendoNotification");

    $("#customerID").attr('maxlength', '50');
        $("#centre").attr('maxlength', '50');
        $("#reference").attr('maxlength', '50');
        $("#contract").attr('maxlength', '50');
        $("#taskID").attr('maxlength', '50');

        $("#IconCalendarA").click(function () {
            $('#datepicker_a').datepicker('show');
        });
        $("#IconCalendarB").click(function () {
            $('#datepicker_b').datepicker('show');
        });
           CargarCombos();
}

function CargarComboAbogado() {
    $.ajax({
        type: 'GET',
        url: "/api/todolist/loadAbog",
        success: function (response) {
            Abogado = response;
            if (Inicio == true) {
                CargarComboTerr();
            }
             CargarUsuario();
             CalLang();
             var data =[];
             var param = jsonObject;
             if (param == "")
             { data.push({ "concepto": "", "valor":"", "signo": "" });
             jsonObject =  {"Parametros": data};
             param= data
             }
             $.ajax({
                 type: 'POST',
                 url: "/api/todolist/GetToDoList/" + param + "/" + Abogado[0][3],
                 dataType: 'json',
                 data: JSON.stringify(jsonObject),
                 cache: false,
                 contentType: "application/json; charset=utf-8",
                 success: function (response) {
                     ArrayDatos = response;
                     PintarTabla();
                 },
                 beforeSend: function () {
                 },
                 error: function (e) {
                        notification.show({
                         title: "Error en el sistema",
                         message: errorWS
                     }, "error");
                 }
             });
        },
        error: function (e) {
              notification.show({
                title: "Error en el sistema",
                message: errorWS
            }, "error");
        }
    });
}

function CargarComboTerr() {
    var data = ({ "idAbogado": Abogado[0][3] });
    data = ({ "idAbogado": Abogado[0][3] });

    $.ajax({
        type: 'POST',
        url: "/api/todolist/loadTerr",
        data: JSON.stringify(data),
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            ArrayTerr = response;
            PintarCombosTerr();
            InitCombo();
        },
        error: function (e) {
              notification.show({
                title: "Error en el sistema",
                message: errorWS
            }, "error");
        }
    });
}

function CargarComboSucur(indice) {

            PintarCombosSucur(indice);
            InitCombo();
}

function CargarCombos() {
    $.ajax({
        type: 'GET',
        url: "/api/todolist/load",
        success: function (response) {
            ArrayProceso = response;
            if (Inicio == true) {
                PintarCombo();
                InitCombo();
            }
            CargarComboAbogado();

        },
        error: function (e) {
            $('#loading').hide();
            notification.show({
                title: "Error en el sistema",
                message: errorWS
            }, "error");
        }
    });
}

function CargarUsuario() {
    $.ajax({
        type: 'GET',
        url: "/api/todolist/user",
        success: function (response) {
            $('#Usuario').html(response);
        },
            error: function (e) {
                notification.show({
                    title: "Error en el sistema",
                    message: errorWS
                }, "error");
        }
    });
}

function PintarCombo() {
    var HTML="<label class='uk-form-label' for='val_select'>Proceso</label>";
        HTML+=" <select id='select_demo_proc' data-md-selectize><option value=''>Selecciona...</option>";
    for (var i =0; i< ArrayProceso[0].length;i++)
    {
       HTML += "<option value='" + ArrayProceso[1][i]+ "'>" + ArrayProceso[0][i] + "</option>";
    }
    HTML += "<option value=' '></option>";
    HTML += "</select>";
    $('#pruebase').empty();
    $('#pruebase').append(HTML);
}

function PintarCombosTerr() {

    var HTML = "<label class='uk-form-label' for='val_select'>Territorio</label>";
    HTML += " <select id='select_demo_terr' data-md-selectize><option value=''>Selecciona...</option>";
    for (var i = 0; i < ArrayTerr[0].length; i++) {
        HTML += "<option value='" + ArrayTerr[0][i] + "'>" + ArrayTerr[1][i] + "</option>";
    }
    HTML += "<option value=' '></option>";
    HTML += "</select>";
    $('#terrIdp').empty();
    $('#terrIdp').append(HTML);
}

function PintarCombosSucur(indice) {
    var indices = [];
    var completo = [];
    $('#centrep').html("");
    completo = indice.split(',');
    for (var i=0;i<completo.length;i++)
    {
        indices.push(completo[i].split('-')[1]);
    }
    var HTML = "<label class='uk-form-label' for='val_select'>Sucursal</label>";
    HTML += " <select id='select_demo_sucur' data-md-selectize><option value=''>Selecciona...</option>";
    for (var j = 0; j < indices.length; j++)
    {
        for (var i = 0; i < ArrayTerr[2].length; i++) {

            if ((indices[j] == ArrayTerr[2][i].split("-")[1]) == true)
            {
                HTML += "<option value='" + ArrayTerr[2][i] + "'>" + ArrayTerr[3][i] + "</option>";
            }
        }
    }
     HTML += "<option value=' '></option>";
        HTML += "</select>";
    $('#centrep').empty();
    $('#centrep').append(HTML);
}

function PintarTablaInicio() {
           a = $('#dt_colVis').DataTable({
            paging: false,
            searching: false,
            columns: [
           { title: "Acciones", },
           { title: "Bloqueado" },
           { title: "Etapas" },
           { title: "Materia" },
           { title: "RUT." },
           { title: "Nombre Cliente" },
           { title: "Numero Sucursal" },
           { title: "Nombre Sucursal" },
           { title: "Caso ID" },
           { title: "Nº SGL" },
           { title: "Inicio" },
           { title: "SLA" }],
            "oLanguage": {
                "sEmptyTable": "No hay registros disponibles",
            },
            destroy: true,
        });
}

function PintarTabla() {
   a= $('#dt_colVis').DataTable({
                data: ArrayDatos,
                columns: [
                {title: "Acciones", },
                { title: "Bloqueado" },
                { title: "Etapas" },
                { title: "Materia" },
                { title: "RUT." },
                { title: "Nombre Cliente" },
                { title: "Numero Sucursal" },
                { title: "Nombre Sucursal" },
                { title: "Caso ID" },
                { title: "Nº SGL" },
                { title: "Inicio" },
                { title: "SLA" }],
                "columnDefs": [{
                    "targets": 0,
                    "render": function (data, type, full, meta) {
                        return "<a  href='POSOCH_Task.html?" + full[8] + "'><i class='material-icons' data-uk-tooltip title='Ir a la tarea'>&#xE154;</i></a>";
                    }
                }],
                "bDestroy": true,
                "oLanguage": {
                    "sEmptyTable": "No hay registros disponibles",
                }
    });

 $(function () {   altair_datatables.dt_colVis()}),
   altair_datatables = {
     dt_colVis: function () {
           var t = $("#dt_colVis");
           if (t.length) {
               var a = t.DataTable(), e = new $.fn.dataTable.ColVis(a, { buttonText: "Selecciona columnas", exclude: [0], restore: "Restaurar", showAll: "Mostrar Todas", showNone: "No mostrar" }), i = $(e.dom.button).off("click").attr("class", "md-btn md-btn-colVis"), l = $('<div class="uk-button-dropdown uk-text-left" data-uk-dropdown="{mode:\'click\'}"/>').append(i), o = $('<div class="md-colVis uk-text-right"/>').append(l), d = $(e.dom.collection);
               $(d).attr({ "class": "md-list-inputs", style: "" }).find("input").each(function (t) { var a = $(this).clone().hide(); $(this).attr({ "class": "data-md-icheck", id: "col_" + t }).css({ "float": "left" }).before(a) }).end().find("span").unwrap().each(function () { var t = $(this).text(), a = $(this).prev("input").attr("id"); $(this).after('<label for="' + a + '">' + t + "</label>").end() }).remove();
               var n = $('<div class="uk-dropdown uk-dropdown-flip"/>').append(d); l.append(n), t.closest(".dt-uikit").find(".dt-uikit-header").before(o), altair_md.checkbox_radio(), t.closest(".dt-uikit").find(".md-colVis .data-md-icheck").on("ifClicked", function () { $(this).closest("li").click() }), t.closest(".dt-uikit").find(".md-colVis .ColVis_ShowAll,.md-colVis .ColVis_Restore").on("click", function () { $(this).closest(".uk-dropdown").find(".data-md-icheck").prop("checked", !0).iCheck("update") }), t.closest(".dt-uikit").find(".md-colVis .ColVis_ShowNone").on("click", function () { $(this).closest(".uk-dropdown").find(".data-md-icheck").prop("checked", !1).iCheck("update") })
           }
       }
   };
 $('#loading').hide();
}

$("#page_content").click(function (e) {
    $('#style_switcher').removeClass('switcher_active');
})

$("#Limpiar").click(function (e) {
   $("#reference").val("")
   $("#customerID").val("")
   $("#contract").val("");
   $("#centre").val("");
   $("#taskID").val("");
   $('.item').text("");
   $("#datepicker_a").val("");
   $("#datepicker_b").val("");
})

$("#Filtro").click(function (e) {
    $('#loading').show();
    Inicio = false;
    var Filtro;
    var data = [];
    if ($("#reference").val() != "") {
        Filtro = $("#reference").val();
        while (Filtro.indexOf("_") != -1) {
            Filtro = Filtro.replace("_", "");
        }
        data.push({ "concepto": "0110", "valor": Filtro, "signo": "" });
    }

    if ($("#customerID").val() != "") {
        Filtro = $("#customerID").val();
        while (Filtro.indexOf("_") != -1) {
            Filtro = Filtro.replace("_", "");
        }
        data.push({ "concepto": "0104", "valor": Filtro, "signo": "" });
    }

    if ($("#contract").val() != "") {
        Filtro = $("#contract").val();
        while (Filtro.indexOf("_") != -1) {
            Filtro = Filtro.replace("_", "");
        }
        data.push({ "concepto": "0118", "valor": Filtro, "signo": "" });
    }

    if ($("#select_demo_sucur").val() != " " && $("#select_demo_sucur").val() != "" && $("#select_demo_sucur").val()!=null) {
        Filtro = $("#select_demo_sucur").val();
        ComboFiltroSucur = $("#select_demo_sucur").val();
        data.push({ "concepto": "0002", "valor": Filtro.split('-')[0], "signo": "" });
    }

    if ($("#select_demo_terr").val() != " " && $("#select_demo_terr").val() != "" && $("#select_demo_terr").val() != null) {
        Filtro = $("#select_demo_terr").val();
        ComboFiltroTerr = $("#select_demo_terr").val();
        data.push({ "concepto": "0189", "valor": Filtro.split('-')[0], "signo": "" });
     }

    if ($("#select_demo_proc").val() != " " && $("#select_demo_proc").val() != "" && $("#select_demo_proc").val() != null)
     {
         Filtro = $("#select_demo_proc").val();
        ComboFiltroProc = $("#select_demo_proc").val();
        data.push({ "concepto": "0036", "valor": Filtro, "signo": "" });
    }

    //Se cambia el formato de la fecha por el problema al crear el mommento yyyy mm dd por yyyy dd mm
    if ($("#datepicker_a").val() != "")
    {
        Filtro = $("#datepicker_a").val();
        if (validarFormatoFecha(Filtro) == false) {
            errorFiltro = true;
            notification.show({
                title: "Formato Fecha",
                message: "La Fecha Inicio introducida es incorrecta"
            }, "error");
        }
        else {
            if (validarFechas() == false) {

            }
            else {
                errorFiltro = false;
                data.push({ "concepto": "0037", "valor": Filtro.split('/')[2] + "-" + Filtro.split('/')[1] + "-" + Filtro.split('/')[0], "signo": ">=" });
            }
        }
    }

    if($("#datepicker_b").val() != "")
    {
        Filtro = $("#datepicker_b").val();
        if (validarFormatoFecha(Filtro) == false) {
            errorFiltro = true;
            notification.show({
                title: "Formato Fecha",
                message: "La Fecha Fin introducida es incorrecta"
            }, "error");
        }
        else {
            if (validarFechas() == false)
            {
                errorFiltro = true;
                notification.show({
                    title: "Formato Fecha",
                    message: "La Fecha Inicio debe ser menor que la fecha fin"
                }, "error");

            }
            else
            {
                errorFiltro = false;
                data.push({ "concepto": "0037", "valor": Filtro.split('/')[2] + "-" + Filtro.split('/')[1] + "-" + Filtro.split('/')[0], "signo": "<=" });
            }
        }
    }
    if(($('input:radio[id=radio_demo_inline_2]:checked').val()=="on"))
    {
        data.push({ "concepto": "0184", "valor": $("#Usuario").text(), "signo": "" });
    }
    jsonObject = {
        "Parametros": data
    };
    Init();
    if (errorFiltro == false) {
        $('#style_switcher').removeClass('switcher_active');
    }
    errorFiltro = false;
});


function filterColumn() {
    $('#dt_colVis').DataTable().column(8).search(
        FiltroIdCaso
        );

    $('#dt_colVis').DataTable().column(9).search(
      FiltroNumeroSGL
     ).draw();
};

function CalLang() {
    $('#datepicker_a').datepicker();
      $('#datepicker_b').datepicker();
};

$(function($){
    $.datepicker.regional['es'] = {
        closeText: 'Cerrar',
        prevText: '<Ant',
        nextText: 'Sig>',
        currentText: 'Hoy',
        monthNames: ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio', 'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'],
        monthNamesShort: ['Ene','Feb','Mar','Abr', 'May','Jun','Jul','Ago','Sep', 'Oct','Nov','Dic'],
        dayNames: ['Domingo', 'Lunes', 'Martes', 'Miércoles', 'Jueves', 'Viernes', 'Sábado'],
        dayNamesShort: ['Dom','Lun','Mar','Mié','Juv','Vie','Sáb'],
        dayNamesMin: ['Do','Lu','Ma','Mi','Ju','Vi','Sá'],
        weekHeader: 'Sm',
        dateFormat: 'dd/mm/yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    $.datepicker.setDefaults($.datepicker.regional['es']);
});
function InitCombo() {
    "use strict"; Modernizr.touch && FastClick.attach(document.body), altair_page_onload.init(),
    altair_main_header.init(), altair_main_sidebar.init(), altair_secondary_sidebar.init(),
    altair_top_bar.init(), altair_md.init(), altair_forms.init(), altair_uikit.init(),
    altair_helpers.truncate_text($(".truncate-text"))
};

$(function () {
    altair_helpers.retina_images();
});

$("#customerID").kendoMaskedTextBox({
});

$("#terrId").kendoMaskedTextBox({
});
$("#taskID").kendoMaskedTextBox({
});
$("#contract").kendoMaskedTextBox({
});
$("#reference").kendoMaskedTextBox({
});
$("#centre").kendoMaskedTextBox({
});

$("#terrIdp").change(function () {
CargarComboSucur($("#select_demo_terr").val());
});

$(function () {
    var $switcher = $('#style_switcher'),
        $switcher_toggle = $('#style_switcher_toggle'),
        $theme_switcher = $('#theme_switcher'),
        $mini_sidebar_toggle = $('#style_sidebar_mini'),
        $boxed_layout_toggle = $('#style_layout_boxed'),
        $body = $('body');

    $switcher_toggle.click(function (e) {
        e.preventDefault();
        $switcher.toggleClass('switcher_active');
    });

    $theme_switcher.children('li').click(function (e) {
        e.preventDefault();
        var $this = $(this),
            this_theme = $this.attr('data-app-theme');

        $theme_switcher.children('li').removeClass('active_theme');
        $(this).addClass('active_theme');
        $body
            .removeClass('app_theme_a app_theme_b app_theme_c app_theme_d app_theme_e app_theme_f app_theme_g')
            .addClass(this_theme);

        if (this_theme == '') {
            localStorage.removeItem('altair_theme');
        } else {
            localStorage.setItem("altair_theme", this_theme);
        }
    });

    if (localStorage.getItem("altair_theme") !== null) {
        $theme_switcher.children('li[data-app-theme=' + localStorage.getItem("altair_theme") + ']').click();
    }

    if ((localStorage.getItem("altair_sidebar_mini") !== null && localStorage.getItem("altair_sidebar_mini") == '1') || $body.hasClass('sidebar_mini')) {
        $mini_sidebar_toggle.iCheck('check');
    }

    $mini_sidebar_toggle
        .on('ifChecked', function (event) {
            $switcher.removeClass('switcher_active');
            localStorage.setItem("altair_sidebar_mini", '1');
            location.reload(true);
        })
        .on('ifUnchecked', function (event) {
            $switcher.removeClass('switcher_active');
            localStorage.removeItem('altair_sidebar_mini');
            location.reload(true);
        });

    if ((localStorage.getItem("altair_layout") !== null && localStorage.getItem("altair_layout") == 'boxed') || $body.hasClass('boxed_layout')) {
        $boxed_layout_toggle.iCheck('check');
        $body.addClass('boxed_layout');
        $(window).resize();
    }

    $boxed_layout_toggle
        .on('ifChecked', function (event) {
            $switcher.removeClass('switcher_active');
            localStorage.setItem("altair_layout", 'boxed');
            location.reload(true);
        })
        .on('ifUnchecked', function (event) {
            $switcher.removeClass('switcher_active');
            localStorage.removeItem('altair_layout');
            location.reload(true);
        });
});

function validarFormatoFecha(campo) {
    var RegExPattern = /^\d{1,2}\/\d{1,2}\/\d{2,4}$/;
    if ((campo.match(RegExPattern)) && (campo != '')) {
        return true;
    } else {
        return false;
    }
}

function validarFechas() {
    var fechaInicio;
    var fechaFin;
    if ($("#datepicker_a").val() != "") {
        fechaInicio=$("#datepicker_a").val().split("/");
    }
    else
    {
        return true;
    }

    if ($("#datepicker_b").val() != "") {
        fechaFin = $("#datepicker_b").val().split("/");
    }
    else
    {
        return true;
    }
    var x = new Date();
    var y = new Date();
    x.setFullYear(fechaInicio[2], fechaInicio[1] - 1, fechaInicio[0]);
    y.setFullYear(fechaFin[2], fechaFin[1] - 1, fechaFin[0]);
    if (x >= y)
        return false;
    else
        return true;
}