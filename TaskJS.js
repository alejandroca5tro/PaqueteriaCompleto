var Id;
var ArrayDatos = [];
var ArrayDatosNotas = [];
var ArrayDatosDoc = [];
var ArrayDatosHistorico = [];
var NDocFiltro = 4;
var Npaginas;
var PaginaActual = 1;
var jsonObject = "";
var notification;
var nuevaNota = "true";
var fileSelector = $('<input type="file">');
var Documento;
var DocumentosASubir = [];
var SubidoDocumento = false;
var errorWS = "Por favor espere y vuelva a lanzar la peticion,si el problema persiste contacte con el administrador del sistema.";
var DocByes;

$(document).ready(function () {
    CargarBase64();
    ObtenerId();
    Init();
});

function Init() {
    notification = $("#notification").kendoNotification({
        position: {
            pinned: true,
            top: 30,
            right: 30
        },
        autoHideAfter: 0,
        stacking: "down",
        templates: [{
            type: "error",
            template: $("#errorTemplate").html()
        }]
    }).data("kendoNotification");
    var data = ({ "CasoId": Id });

    LoadAbogado();
    $.ajax({
        type: 'POST',
        url: "/api/todolist/loadDocumentos",
        data: JSON.stringify(data),
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            ArrayDatosDoc = response;
            LoadUsuario();
            PintarDocumentos();
            LoadHistorico();
            LoadNotas();
            PaginaActual = 1;

        },
        error: function (e) {
            notification.show({
                title: "Error en el sistema",
                message: errorWS
            }, "error");
        },
        beforeSend: function () {
        },
    });
}

function LoadDocumentos() {
    var data = ({ "CasoId": Id });
    $.ajax({
        type: 'POST',
        url: "/api/todolist/loadDocumentos",
        data: JSON.stringify(data),
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            ArrayDatosDoc = response;
            PintarDocumentos();
        },
        error: function (e) {
            notification.show({
                title: "Error en el sistema",
                message: errorWS
            }, "error");
        },
        beforeSend: function () {
        },
    });
}

function LoadAbogado() {
    var Abogado;
    $.ajax({
        type: 'GET',
        url: "/api/todolist/loadAbog",
        success: function (response) {
            Abogado = response;
            PintarDetalles(Abogado[0][3]);
        }
    });
}

function LoadNotas() {

    var data = ({ "CasoId": Id });

    $.ajax({
        type: 'POST',
        url: "/api/todolist/loadNotas",
        data: JSON.stringify(data),
        cache: false,
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            ArrayDatosNotas = response;
            PintarNotas();
            CargarNotas();
            PaginaActual = 1;
        },
        error: function (e) {
            notification.show({
                title: "Error en el sistema",
                message: errorWS
            }, "error");
        },
        beforeSend: function () {
        },
    });
}

function LoadUsuario() {
    $.ajax({
        type: 'GET',
        url: "/api/todolist/user",
        success: function (response) {
            $('#Usuario').text(response);
            BloquearDesbloquearTarea(response, true);
        },
    });
}

function LoadHistorico() {
    var data = ({ "CasoId": Id });

    data = ({ "CasoId": Id });
    $.ajax({
        type: 'POST',
        url: "/api/todolist/loadHistorico",
        data: JSON.stringify(data),
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            ArrayDatosHistorico = response;
            PintarHistorico();

        },
        error: function (e) {
            notification.show({
                title: "Error en la peticion con el WebService",
                message: errorWS
            }, "error");
        },
        beforeSend: function () {
        },
    });
}

function PintarDetalles(Abogado) {
    var data = ({ "CasoId": Id });
    $.ajax({
        type: 'POST',
        url: "/api/todolist/loadDetalle/" + data + "/" + Abogado,
        data: JSON.stringify(data),
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            ArrayDatos = response;
            if (ArrayDatos.length > 0) {
                $("#NombreCentro").text(ArrayDatos[0][0]);
                $("#Centro").text(ArrayDatos[0][1]);
                $("#CasoId").text(ArrayDatos[0][2]);
                $("#FCreacion").text(ArrayDatos[0][3]);
                $("#FSLA").text(ArrayDatos[0][4]);
                $("#NumeroSgl").text(ArrayDatos[0][5]);
                $("#RUT").text(ArrayDatos[0][6]);
                $("#AbogadoAsignado").text(ArrayDatos[0][7]);
            }
        },
        error: function (e) {
            notification.show({
                title: "Error en el sistema",
                message: errorWS
            }, "error");
        }
    });
}

function ObtenerId() {
    var direccion = self.location.href;
    var Datos = direccion.split("?");
    var IdFinal = Datos[1].split("#")
    Id = IdFinal[0];
}
function BloquearDesbloquearTarea(Usuario, Bloquear) {
    // var Abogado = Abogado[0][3];
    var data = ({ "CasoId": Id });
    $.ajax({
        type: 'POST',
        url: "/api/todolist/BloquearDesbloquearTarea/" + data + "/" + Usuario + "/" + Bloquear,
        data: JSON.stringify(data),
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            if (Bloquear == false) {

                var link = document.createElement("a");

                link.href = "POSOCH_ToDoList.html";
                link.style = "visibility:hidden";
                document.body.appendChild(link);
                link.click();
                document.body.removeChild(link);
            }
        },
        error: function (e) {
            notification.show({
                title: "Error en el sistema",
                message: errorWS
            }, "error");
        }
    });
}

$("#Login2").click(function (e) {
    if (document.getElementById('Progreso').style.visibility == "visible") {
        stopEvent(e);
    }
});
$("#Atras").click(function (e) {

    var Usuario = $('#Usuario').text();
    BloquearDesbloquearTarea(Usuario, false);

});
/*
$("#sidebar_secondary_toggle").click(function (e) {
    if (document.getElementById('Progreso').style.visibility == "visible") {
        stopEvent(e);
    }
});*/
function CambiarEstado(tipo) {
    var deferred = $.Deferred();

    var idTipo = Id;
  
    $.ajax({
        type: 'POST',
        url: "/api/todolist/setReparo/" + idTipo + "/" + tipo,
        data: JSON.stringify(jsonObject),
        contentType: "application/json; charset=utf-8",
        success: function (response) {

            deferred.resolve();
        },

        error: function (e) {
            notification.show({
                title: "Error en la peticion con el WebService",
                message: errorWS
            }, "error");
        },
    });
    return deferred.promise();
}
$("#reparo").click(function (e) {

    var Promesa = CambiarEstado("1");

    Promesa.done(CalbackOK);


});
$("#avanzar").click(function (e) {

    var Promesa = CambiarEstado("2");

    Promesa.done(CalbackOK);

});
function CalbackOK() {
   
    var link = document.createElement("a");
    link.href = "POSOCH_ToDoList.html";
    link.style = "visibility:hidden";
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}



function stopEvent(e) {
    if (!e) e = window.event;
    if (e.stopPropagation) {
        e.stopPropagation();
    } else {
        e.cancelBubble = true;
    }
}
var OcultarMenu = true;
function PintarDocumentos() {
    if (ArrayDatosDoc.length == 0 && SubidoDocumento == true) {
        SubidoDocumento = false;
        $('#loading').hide();

    }
    if (ArrayDatosDoc.length > 0) {
        $('#DocumentosList').html("");
        Npaginas = ArrayDatosDoc.length / NDocFiltro;
        var HTML = "";
        var Contador = "0";
        var PaginadoFinal = PaginaActual * NDocFiltro;
        var PaginadoInicio = (PaginaActual - 1) * (NDocFiltro);
        for (var i = PaginadoInicio ; i <= PaginadoFinal - 1; i++) {
            if (i < ArrayDatosDoc.length) {

                HTML += "<li>";
                HTML += '<div class="md-list-addon-element"> ';
                HTML += '<a id="idDoc1">';
                HTML += '<i id=' + ArrayDatosDoc[i][0] + '  class="uk-icon-file-code-o uk-icon-small"></i>'
                HTML += '</div> ';
                HTML += '<div class="md-list-content">';
                HTML += ' <span id="Documento' + Contador + '" class="md-list-heading" valor="' + ArrayDatosDoc[i][0] + '" tipo="' + ArrayDatosDoc[i][3].trim() + '"  usuario="' + ArrayDatosDoc[i][4] + '"  nombre="' + ArrayDatosDoc[i][1] + '"  >' + ArrayDatosDoc[i][1] + '</span> ';
                HTML += ' <span class="uk-text-small uk-text-muted">' + ArrayDatosDoc[i][2] + '</span> ';
                HTML += '</div> ';
                HTML += '</li> ';
                Contador++;
            }
        }
        $('#DocumentosList').html(HTML);
        $('#Paginacion').html('');
        MontarPaginado();
        for (var i = 0; i < ArrayDatosDoc.length; i++) {
            $("#Documento" + i).click(function (e) {
                //$("#menu").css({ 'display': 'block', 'left': e.pageX, 'top': e.pageY });
                OcultarMenu = false;
                $('#loading').show();
                var id = e.target.getAttribute("valor");
                var tipo = e.target.getAttribute("tipo");
                var usuario = e.target.getAttribute("usuario");
                var nombre = e.target.getAttribute("nombre");
                ConseguirDocumento(id, tipo, usuario, nombre);

            });
        }
    }
};

//cuando hagamos click, el menú desaparecerá
/*
$(document).click(function (e) {
    stopEvent(e);
    if (e.target.id != "Visualizar" || e.target.id != "Descargar") {
        if (OcultarMenu == true) {
            if (e.button == 0) {
                $("#menu").css("display", "none");
            }
        }
    }
    switch (e.target.id) {
        case "Visualizar":
            OpenDocumento(DocumentoDoc, responseDoc);
            break;
        case "Descargar":
            downloadFile(DocumentoDoc, nombreDoc, responseDoc)
            break;
    }
    OcultarMenu = true;
});
*/
function PintarHistorico() {
    if (ArrayDatosHistorico.length > 0) {
        var HTML = "";

        for (var i = 0; i < ArrayDatosHistorico.length; i++) {
            HTML += '<li>';
            HTML += '<div class="md-list-content">';
            HTML += ' <div class="uk-margin-small-top">';
            HTML += '<span class="uk-margin-right"> <i class="material-icons">&#xE192;</i> <span class="uk-text-muted uk-text-small">' + ArrayDatosHistorico[i][0] + '</span> </span>';
            HTML += '<span class="uk-margin-right"> <i class="material-icons">&#xE7FD;</i> <span class="uk-text-muted uk-text-small">' + ArrayDatosHistorico[i][1] + '</span> </span>';
            HTML += '<span class="uk-margin-right"> <i class="material-icons">&#xE0B9;</i> <span class="uk-text-muted uk-text-small">' + ArrayDatosHistorico[i][2] + '</span> </span>';
            HTML += '  </div> ';
            HTML += ' </div> ';
            HTML += '</li> ';
        }
        $('#TimeJ').append(HTML);
    }
};

function ConseguirDocumento(IdDoc, Tipo, Usuario, nombre) {
    $.ajax({
        type: 'POST',
        url: "/api/todolist/getDocumento/" + IdDoc + "/" + Tipo + "/" + Usuario,
        dataType: 'json',
        success: function (response) {
            ConseguirTipo(response, IdDoc, Tipo, Usuario, nombre);

        },
        contentType: "application/json; charset=utf-8",
        error: function (e) {
            notification.show({
                title: "Error en la peticion con el WebService",
                message: errorWS
            }, "error");
        },
    });
}
var DocumentoDoc;
var responseDoc;
var nombreDoc;

function ConseguirTipo(DocByes, IdDoc, Tipo, Usuario, nombre) {
    $.ajax({
        type: 'POST',
        url: "/api/todolist/getTipo/" + IdDoc + "/" + Tipo + "/" + Usuario,
        success: function (response) {

            if (DocByes != null) {
                responseDoc = response;
                nombreDoc = nombre;
                // OpenDocumento(DocumentoDoc, responseDoc);
                downloadFile(DocByes, nombreDoc, responseDoc)
                //  OpenDocumento(DocumentoDoc, responseDoc);
            }
            else {
                notification.show({
                    title: "Error al descargar el documento",
                    message: errorWS
                }, "error");
            }
        },
        error: function (e) {
            notification.show({
                title: "Error en la peticion con el WebService",
                message: errorWS
            }, "error");
        },
    });
}

//previsuaizacion en la misma pantalla
//function OpenDocumento(data, type) {
//  var binary = atob(data)
//    var array = new Uint8Array(binary.length)
//    for (var i = 0; i < binary.length; i++) { array[i] = binary.charCodeAt(i) }
//    var csvData = array;
//    /* var blob = new Blob([csvData], {
//         type: type
//     });
//     var fileURL = URL.createObjectURL(blob);
//     window.open(fileURL);*/
//    var reader = new FileReader();
//    var out = new Blob([csvData], { type: type });
//    var fileURL = URL.createObjectURL(out);
//    var selector = document.getElementById('ObjetoDocumento');
//    //var selector = document.getElementById('sidebar_secondary');
//    //  var iframe = document.createElement('iframe');
//    //propiedades iframe
//    //  iframe.setAttribute('src', fileURL);
//    //  iframe.setAttribute('data', fileURL);
//    // selector.appendChild(iframe);
//   // var iframe = document.createElement('object');
//    selector.setAttribute('data', fileURL);
//    $('#sidebar_secondary_toggle').click();
//    //window.open(fileURL);
//    // reader.onload = function (e) {
//    // window.location.href = reader.result;
//    // $('#ObjetoDocumento').attr('data', csvData);
//    // $('#ObjetoDocumento').attr('type', type);
//    /* var link = $('#ObjetoDocumento');
//     link.href = reader.result;
//     link.style = "visibility:hidden";*/
//    //document.body.appendChild(link);
//    //  link.click();
//    //document.body.removeChild(link);
//    //}
//    // reader.readAsDataURL(out);
//    //var file = new Blob([data], { type: 'application/pdf' });
//}

function downloadFile(DocByes, fileName, type) {
    if (window.navigator.msSaveBlob) {

        var filenameext = fileName + type.split(",")[1];
        var array = new Uint8Array(DocByes)

        var blob = new Blob([array], {
            type: type.split(",")[0]
        });
        navigator.msSaveBlob(blob, filenameext);
    }
    else {

        var filenameext = fileName + type.split(",")[1];
        var array = new Uint8Array(DocByes)

        var blob = new Blob([array], {
            type: type.split(",")[0]
        });
        var link = document.createElement("a");
        var csvUrl = URL.createObjectURL(blob);
        link.href = csvUrl;
        link.style = "visibility:hidden";
        link.download = fileName;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    }
    $('#loading').hide();
}

function PintarNotas() {
    $('#NotasImportantes').html("");
    var HTMLI = '';
    var HTMLO = '';
    var Contador = "0";

    for (var i = 0; i < ArrayDatosNotas.length; i++) {
        HTMLI += "<li>";
        HTMLI += '<a data-note-cuerpo="' + ArrayDatosNotas[i][1] + '" data-note-id="' + ArrayDatosNotas[i][2] + '" class="md-list-content note_link" href="#">';
        HTMLI += ' <span class="md-list-heading uk-text-truncate">' + ArrayDatosNotas[i][0] + '</span>'
        HTMLI += ' <span class="uk-text-small uk-text-muted">' + '' + '</span>'
        HTMLI += ' </a>';
        HTMLI += '</li> ';
    }
    $('#NotasImportantes').append(HTMLI);
    $('#loading').hide();
};

function MontarPaginado() {
    if (Npaginas != null && ArrayDatosDoc.length % NDocFiltro != 0) {
        Npaginas++;
        Npaginas = parseInt(Npaginas);
    }
    var list = $("#Paginacion");

    if (PaginaActual == 1) {
    }
    else {
        list.append("<li><a href='#'><i class='uk-icon-angle-double-left'></i></a></li>");
    }

    if (PaginaActual > NDocFiltro) {
        list.append("<li class='PaginaId'><a href='#'>" + 1 + "</a></li>");
        list.append("<li><span>…</span></li>");
    }
    var inicio = 1;
    if (PaginaActual > 3) {
        inicio = PaginaActual - 1
    }

    var final = inicio + 2;

    if (PaginaActual > Npaginas - 2) {
        final = Npaginas
    }

    for (var i = inicio ; i <= final; i++) {
        if (i == PaginaActual) {
            list.append("<li class='uk-active'><span>" + i + "</span></li>");
        }
        else {
            list.append("<li class='PaginaId'><a href='#'>" + i + "</a></li>");
        }
    }
    if (PaginaActual < Npaginas - 1) {
        list.append("<li><span>…</span></li>");
        list.append("<li class='PaginaId'><a href='#'>" + Npaginas + "</a></li>");
    }

    if (PaginaActual == Npaginas) {
    }
    else {
        list.append("<li><a href='#'><i class='uk-icon-angle-double-right'></i></a></li>");
    }
    if (SubidoDocumento == true) {
        $('#loading').hide();
        SubidoDocumento == false;
    }
}

$("#Paginacion").on("click", ".uk-icon-angle-double-right", function () {
    if (PaginaActual != ArrayDatosDoc.length / NDocFiltro) {
        PaginaActual++;
        PintarDocumentos();
    }
});

$("#Paginacion").on("click", ".uk-icon-angle-double-left", function () {
    if (PaginaActual != 1) {
        PaginaActual--;
        PintarDocumentos();
    }
});

$("#Paginacion").on("click", ".PaginaId", function () {
    PaginaActual = this.innerText;
    PintarDocumentos();
});

$(function () {
    altair_helpers.retina_images();
});
var status = 0;
$("#documentoAmpliar").click(function () {
    if (status == 0) {
        $("#sidebar_secondary").css("width", "100%");
        status = 1;
    } else {
        $("#sidebar_secondary").css("width", "50%");
        status = 0;
    }
});

$("#idDoc1").click(function () {
    $body.hasClass("sidebar_secondary_active") ? altair_secondary_sidebar.hide_sidebar() : altair_secondary_sidebar.show_sidebar();
    if ($("#idDoc1a").hasClass("uk-text-success")) {
        $("#idDoc1a").removeClass("uk-text-success");
    } else {
        $("#idDoc1a").addClass("uk-text-success");
    }
});

$("#adjuntar").click(function () {
    fileSelector.click();
    return false;
});

$("#agrenota").click(function () {
    $("#salvar").show();
    $("#notas").click();
    $("#note_title").val("");
    $("#note_content").val("");
    nuevaNota = "true";
    altair_notes.add_new_note();
    $('#note_title').attr('readonly', false);
    $('#note_content').attr('readonly', false);
});

$("#salvar").click(function () {
    var titulo = $("#note_title").val();
    var contenido;

    var indice;
    if (nuevaNota == "true") {
        indice = ArrayDatosNotas.length + 1;
        contenido = $("#note_content").val();
    }
    else {
        indice = $("#note_content").val().split(":")[0].replace("Nota ", '');
        indice = indice.replace(" Cuerpo", '');
        contenido = $("#note_content").val().split(":")[1];
    }
    SalvarNota(titulo, contenido, indice);
    nuevaNota = "false";

});

$("#NotasImportantes").click(function () {
    $("#salvar").hide();
    nuevaNota = "false";
    $('#note_title').attr('readonly', true);
    $('#note_content').attr('readonly', true);
});

function SalvarNota(titulo, contenido, indice) {
    if (titulo == "" || contenido == "") {
        if (titulo == "" && contenido != "") {
            notification.show({
                title: "Error nota",
                message: "El título de la nota debe tener un valor"
            }, "error");
        }
        if (titulo != "" && contenido == "") {
            notification.show({
                title: "Error nota",
                message: "El cuerpo de la nota debe tener un valor"
            }, "error");
        }
        if (titulo == "" && contenido == "") {
            notification.show({
                title: "Error nota",
                message: "El título y el cuerpo de la nota deben tener un valor"
            }, "error");
        }
    }
    else {
        $('#loading').show();
        $.ajax({
            type: 'POST',
            url: "/api/todolist/setNota/" + titulo + "/" + contenido + "/" + indice + "/" + Id,
            data: JSON.stringify(jsonObject),
            contentType: "application/json; charset=utf-8",
            success: function () {
                setTimeout(function () {
                    $("#salvar").hide();
                    $('#note_title').attr('readonly', true);
                    $('#note_content').attr('readonly', true);
                    nuevaNota = "true";
                    LoadNotas();
                }, 1100);
            },
            error: function (e) {
                notification.show({
                    title: "Error en la peticion con el WebService",
                    message: errorWS
                }, "error");
            },
        });
    }
}

fileSelector.change(function () {
    getBase64(this.files[0], $("#RUT").text(), $("#NumeroSgl").text());
    fileSelector.val("");
});

function getBase64(file, Rut, SGL) {
    var reader = new FileReader();
    if (file != null) {

    reader.readAsDataURL(file);
    reader.onload = function () {
        SubidoDocumento = true;
        $('#loading').show();
        if (reader.result != null) {
            EnviarDocumento(reader.result.substr(reader.result.indexOf(',') + 1), file.name.substring(0, file.name.lastIndexOf(".")), Rut, SGL, file.type);
        }
        else
        {
            $('#loading').hide();
            notification.show({
                title: "Error Documento",
                message: "No se puede subir un documento sin contenido"
            }, "error");
        }
    };
    reader.onerror = function (error) {
        console.log('Error: ', error);
    };
    }
}

function EnviarDocumento(Documento, Nombre, Rut, SGL, tipo) {
    var data = ({ "Documento": Documento, "Rut": Rut, "SGL": SGL, "CasoId": Id, "Nombre": Nombre, "Tipo": tipo });
    $.ajax({
        type: 'POST',
        url: "/api/todolist/setDocumento/",
        data: JSON.stringify(data),
        success: function (response) {
            if (response != null) {
                AsignarDocumentosAtarea(response, Nombre);
            }
            else {
                $('#loading').hide();
                notification.show({
                    title: "Error en la peticion con el WebService",
                    message: errorWS
                }, "error");
            }
        },
        contentType: "application/json; charset=utf-8",
        error: function (e) {
            $('#loading').hide();
            notification.show({
                title: "Error en la peticion con el WebService",
                message: errorWS
            }, "error");
        },
    });
}

function AsignarDocumentosAtarea(Documento, Nombre) {

    $.ajax({
        type: 'POST',
        url: "/api/todolist/SetAsignarDoc/" + Id + "/" + Documento + "/" + Nombre,
        data: JSON.stringify(jsonObject),
        contentType: "application/json; charset=utf-8",
        success: function () {
            SubidoDocumento = true;
            $('#loading').show();
            LoadDocumentos();
            setTimeout(function () { PintarDocumentos(); }, 3100);
        },
        error: function (e) {
            $('#loading').hide();
            notification.show({
                title: "Error en la peticion con el WebService",
                message: errorWS
            }, "error");
        },
    });
};