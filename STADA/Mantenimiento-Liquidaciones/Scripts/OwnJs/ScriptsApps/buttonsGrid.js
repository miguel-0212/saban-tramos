function onDownloadPDF(e) {
    e.preventDefault();

    var iIdOffer = this.dataItem($(e.currentTarget).closest("tr")).idOffer;

    var href = $('#pdfButton').get(0).href;
    if (!href.includes("iIdOffer")) {
        $('#pdfButton').get(0).href = $('#pdfButton').get(0).href + "?iIdOffer=" + iIdOffer
    } else {
        $('#pdfButton').get(0).href = href.substring(0, href.indexOf("?iIdOffer"))
        $('#pdfButton').get(0).href = $('#pdfButton').get(0).href + "?iIdOffer=" + iIdOffer

    }
    $('#pdfButton').get(0).click();
}

function onDownloadPDFIndex() {
    var iIdOffer = $("#iIdOffer").val();

    var href = $('#pdfButton').get(0).href;
    if (!href.includes("iIdOffer")) {
        $('#pdfButton').get(0).href = $('#pdfButton').get(0).href + "?iIdOffer=" + iIdOffer
    } else {
        $('#pdfButton').get(0).href = href.substring(0, href.indexOf("?iIdOffer"))
        $('#pdfButton').get(0).href = $('#pdfButton').get(0).href + "?iIdOffer=" + iIdOffer

    }
    $('#pdfButton').get(0).click();
}

function goToDetail(e) {
    showLoadingIco();
    e.preventDefault();
    var iIdOffer = this.dataItem($(e.currentTarget).closest("tr")).idOffer;
    window.location.href = route + "/Ofertas/Detail?iIdOffer=" + iIdOffer + "&iIdUser=" + $("#iIdUser").val();
}

function goToDetailForValidated(e) {
    showLoadingIco();
    e.preventDefault();
    var iIdOffer = this.dataItem($(e.currentTarget).closest("tr")).idOffer;
    window.location.href = route + "/Ofertas/DetailForValidated?iIdOffer=" + iIdOffer + "&iIdUser=" + $("#iIdUser").val();
}

function goToDetailRefused(e) {
    showLoadingIco();
    e.preventDefault();
    var iIdOffer = this.dataItem($(e.currentTarget).closest("tr")).idOffer;
    window.location.href = route + "/Ofertas/DetailRefused?iIdOffer=" + iIdOffer + "&iIdUser=" + $("#iIdUser").val();
}

function goToDetailValidated(e) {
    showLoadingIco();
    e.preventDefault();
    var iIdOffer = this.dataItem($(e.currentTarget).closest("tr")).idOffer;
    window.location.href = route + "/Ofertas/DetailValidated?iIdOffer=" + iIdOffer + "&iIdUser=" + $("#iIdUser").val();
}

function goToDetailSearchOffer(e) {
    showLoadingIco();
    e.preventDefault();
    var iIdOffer = this.dataItem($(e.currentTarget).closest("tr")).idOffer;
    window.location.href = route + "/Ofertas/DetailSearchOffer?iIdOffer=" + iIdOffer + "&iIdUser=" + $("#iIdUser").val();
}

function copyOffer(e) {
    e.preventDefault();

    var iIdOffer = this.dataItem($(e.currentTarget).closest("tr")).idOffer;
    
    $("#iIdOfferToCopy").val(iIdOffer)
    //$('#unidosisNewOH').parent().parent().hide() //COMENTADO POR JOSE DURAND - 22/02/2024
    $('#unidosisCopyOH').parent().parent().hide()
    //$("#typeSelectClienteOH").val("copyOH") //COMENTADO POR JOSE DURAND - 22/02/2024
    $("#typeSelectClienteCopyOH").val("copyOH")
    //$("#titleSelectClienteOH").html("Selecciona Cliente para copiar oferta"); //COMENTADO POR JOSE DURAND - 22/02/2024
    $("#titleSelectClienteCopyOH").html("Selecciona Cliente para copiar oferta");
    //$("#selectClientesOHGrid").data("kendoGrid").clearSelection(); //COMENTADO POR JOSE DURAND - 22/02/2024
    $("#selectClientesCopyOHGrid").data("kendoGrid").clearSelection();
    //$("#selectGruposOHGrid").data("kendoGrid").clearSelection(); //COMENTADO POR JOSE DURAND - 22/02/2024
    $("#selectGruposCopyOHGrid").data("kendoGrid").clearSelection();
    //$("#dialogSelectClienteOH").data("kendoDialog").open(); //COMENTADO POR JOSE DURAND - 22/02/2024
    $("#dialogSelectClienteCopyOH").data("kendoDialog").open();
}

function removeOffer(e) {
    e.preventDefault();

    var iIdOffer = this.dataItem($(e.currentTarget).closest("tr")).idOffer;
    $("#hiddenIdOfertaForRemove").val(iIdOffer);
    $("#dialogRemoveToValidateOfertaHospital").data("kendoDialog").open();

}

function messageOffers(e) {
    e.preventDefault();

    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var msg = "";

    if (dataItem.isvalidate)
        msg = "Oferta Validada";
    else if (dataItem.isSendToValidate && !dataItem.isRefused)
        msg = "Oferta pendiente de valicación por " + dataItem.userValidate;
    else if (dataItem.isRefused)
        msg = "Oferta rechazada por " + dataItem.userValidate;

    $('#msgInfoOferta').html(msg);
    $("#dialegInfoOferta").data("kendoDialog").open();
}

function setVisibleValidatedIndex(dataItem) {
    return dataItem.isvalidate ;
}

function setVisibleRefusedBalidatedIndex(dataItem) {
    return dataItem.isRefused ;
}

function setVisiblePendingValidatedIndex(dataItem) {
    return dataItem.isSendToValidate && !dataItem.isRefused && !dataItem.isvalidate;
}

function setVisibleRemoveOfferIndex(dataItem) {
    return !dataItem.isvalidate && !dataItem.isRefused && !dataItem.isSendToValidate;
}

function goToEditGrupo(e) {
    showLoadingIco();
    e.preventDefault();
    var iIdGrupo = this.dataItem($(e.currentTarget).closest("tr")).idGrupo;
    location.replace(route + "/Grupos/EditGrupos?iIdGrupo=" + iIdGrupo + "&iIdUsuario=" + $("#iIdUsuario").val())
}

function changeStatusGrupo(e) {
    e.preventDefault();
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var newEstado = dataItem.estado == "Activo" ? "Inactivo" : "Activo"

    $("#hiddenIdGrupo").val(dataItem.idGrupo);
    $("#typeEstadoGrupo").val(dataItem.estado);

    $('#changeStatusActivoGrupoDialogTitle').html("<p style='font-weight: bold;'>Cambio Estado del Activo</p>")
    if (newEstado == "Inactivo") {
        $('#changeStatusActivoGrupoDialogBody').html("Está desactivando el grupo " + dataItem.nombreGrupo + ". Desea continuar?")
    } else {
        $('#changeStatusActivoGrupoDialogBody').html("Está activando el grupo " + dataItem.nombreGrupo + ".  Desea continuar?")
    }
    $("#dialogChangeStatusGrupo").data("kendoDialog").open();
    stopLoadingIco();
}

function setVisibleActivateButtonGrupo(dataItem) {
    return dataItem.estado == "Inactivo";
}

function setVisibleInactivateButtonGrupo(dataItem) {
    return dataItem.estado == "Activo";
}

//AGREGADO POR JOSE DURAND - INICIO - 28/02/2024

function setVisibleEditButtonGrupo(dataItem) {
    return dataItem.estado == "Activo";
}

//AGREGADO POR JOSE DURAND - FIN - 28/02/2024

//REVISAR JOSE DURAND - 19/02/2024 - CUANDO LE DAS AL BOTON PARA CREAR UNA OFERTA APARECE EL MODAL
$(document).on('click', "#addNewOfertaHospitalaria", function (ev) {
    $("#typeSelectClienteOH").val("newOH")
    $("#typeSelectDestinatarioOH").val("newOH") //AGREGADO POR JOSE DURAND - 11/03/2024  
    $("#iIdOfferToCopy").val("")
    $('#unidosisNewOH').parent().parent().show()
    //$("#titleSelectClienteOH").html("Selecciona Cliente"); //--COMENTADO POR JOSE DURAND - 11/03/2024
    $("#titleSelectDestinatarioOH").html("Selecciona Destinatario"); //AGREGADO POR JOSE DURAND - 11/03/2024
    //$("#selectClientesOHGrid").data("kendoGrid").clearSelection(); //--COMENTADO POR JOSE DURAND - 11/03/2024
    $("#selectDestinatariosOHGrid").data("kendoGrid").clearSelection(); //AGREGADO POR JOSE DURAND - 11/03/2024 
    $("#selectGruposOHGrid").data("kendoGrid").clearSelection();
    //$("#dialogSelectClienteOH").data("kendoDialog").open(); //--COMENTADO POR JOSE DURAND - 08/03/2024
    $("#dialogSelectDestinatarioOH").data("kendoDialog").open();
});

$(document).on('click', "#sendMultiplesOfertasToValidate", function (ev) {
    $("#dialogSendToValidateOfertaHospital").data("kendoDialog").open();
});

$(document).on('click', "#sentToValidate", function (ev) {
    showLoadingIco();
    //enviamos oferta para validar
    $.ajax({
        type: 'POST',
        url: route + "/Ofertas/SendToValidate/",
        data: {
            iIdOffer: $('#iIdOffer').val(),
            iIdUsuario: $('#iIdUser').val()
        },
        success: function (data) {
            if (data == -1) {
                $('#divErrorTitle').html("<p style='font-weight: bold;'>Falta introducir los siguientes datos:</p>")
                $('#divErrorImage').html('<img src="' + route + '/Content/img/warning.png" style="width: 72px;height: 72px;">');
                $('#divErrorBody').html(msg)
                $("#dialogError").data("kendoDialog").open();
                stopLoadingIco();
            } else {
                location.replace(route + "/Ofertas/Index");
            }
        }, complete: function () {
        }
    })
});

$(document).on('click', "#editOffer", function (ev) {
    showLoadingIco();
    location.replace(route + "/Ofertas/Edit?iIdOffer=" + $("#iIdOffer").val() + "&iIdUsuario=" + $("#iIdUser").val());
});

$(document).on('click', "#validateOffer", function (ev) {
    $("#dialogValidateOfferDC").data("kendoDialog").open();
});

$(document).on('click', "#addNewGrupoHospitales", function (ev) {
    showLoadingIco();
    location.replace(route + "/Grupos/CreateGrupo?iIdUsuario=" + $("#iIdUsuario").val());
});

$(document).on('click', "#validateOfferForValidated", function (ev) {
    $("#dialogValidateOfferForValidated").data("kendoDialog").open();
});

$(document).on('click', "#refusedOfferForValidated", function (ev) {
    $('#textAreaRefused').val("");
    $("#dialogRefusedOfferForValidated").data("kendoDialog").open();
});

$(document).on('click', "#editOfferForValidated", function (ev) {
    showLoadingIco();
    var idOffer = $("#iIdOffer").val();
    location.replace(route + "/Ofertas/EditForValidated?iIdOffer=" + idOffer + "&iIdUsuario=" + $("#iIdUser").val());
});