function onChangeTipoGridNewOH(e) {
    var value = e.newValue;
    if (value == "CL") {
        //$("#divClientesRadio").show() //--COMENTADO POR JOSE DURAND - 08/03/2024
        $("#divDestinatariosRadio").show(); //--AGREGADO POR JOSE DURAND - 08/03/2024
        $("#divGruposRadio").hide()
        $("#selectClientesOHGrid").data("kendoGrid").dataSource.read()
    } else {
        $("#divGruposRadio").show()
        //$("#divClientesRadio").hide() //--COMENTADO POR JOSE DURAND - 08/03/2024
        $("#divDestinatariosRadio").hide() //--AGREGADO POR JOSE DURAND - 08/03/2024
        $("#selectGruposOHGrid").data("kendoGrid").dataSource.read()
    }
}

//REVISAR JOSE DURAND - 20/02/2024 - ESTA FUNCIÓN PERMITE ELEGIR SOLO UN CLIENTE DE LA LISTA

$(document).on('click', ".selectCheckClientesGridNewOH", function (ev) {
    var sType = $("#typeSelectClienteOH").val()
    var sTypeTable = $("#tipoGridNewOH").data("kendoRadioGroup").value();
    var bOnlyOne = false


    if (sType == "copyOH")
        if (sTypeTable != "CL")
            bOnlyOne = true;
        else
            bOnlyOne = false;
    else
        bOnlyOne = true;

    if (bOnlyOne) {
        var grid = $("#" + $(this).closest('.tableGridKendo')[0].id).data("kendoGrid")
        /*var item = grid.dataItem($(this).closest("tr"))*/
        grid.clearSelection();
        grid.select($(this).closest("tr"))
    }
});

//AGREGADO POR JOSE DURAND - INICIO - 04/03/2024

$(document).on('click', ".selectCheckDestinatariosGridNewOH", function (ev) {
    var sType = $("#typeSelectClienteOH").val()
    var sTypeTable = $("#tipoGridNewOH").data("kendoRadioGroup").value();
    var bOnlyOne = false

    if (sType == "copyOH")
        if (sTypeTable != "CL")
            bOnlyOne = true;
        else
            bOnlyOne = false;
    else
        bOnlyOne = true;

    if (bOnlyOne) {

        var grid = $("#" + $(this).closest('.tableGridKendo')[0].id).data("kendoGrid");
        var selectedRow = $(this).closest("tr");
        var checkbox = selectedRow.find('.k-checkbox');
        var isChecked = checkbox.prop('checked');
        var isSelected = selectedRow.hasClass("k-state-selected");
        var isCheckedBefore;
        // Si solo se permite una selección, limpiar todas las selecciones y seleccionar la fila clicada
        grid.clearSelection();

        if (isChecked === true) {
            isCheckedBefore = false
        } else {
            isCheckedBefore = true
        }

        if (!isCheckedBefore) {
            grid.select(selectedRow);
        }
    }
});

function onChangeTipoGridCopyOH(e) {
    var value = e.newValue;
    if (value == "CL") {
        $("#divClientesCopyRadio").show()
        $("#divGruposCopyRadio").hide()
        $("#selectClientesCopyOHGrid").data("kendoGrid").dataSource.read()
    } else {
        $("#divGruposCopyRadio").show()
        $("#divClientesCopyRadio").hide()
        $("#selectGruposCopyOHGrid").data("kendoGrid").dataSource.read()
    }
}

$(document).on('click', ".selectCheckClientesGridCopyOH", function (ev) {
    var sType = $("#typeSelectClienteCopyOH").val()
    var sTypeTable = $("#tipoGridCopyOH").data("kendoRadioGroup").value();

    var bOnlyOne = false


    if (sType == "copyOH")
        if (sTypeTable != "CL")
            bOnlyOne = true;
        else
            bOnlyOne = false;
    else
        bOnlyOne = true;

    if (bOnlyOne) {
        var grid = $("#" + $(this).closest('.tableGridKendo')[0].id).data("kendoGrid")
        /*var item = grid.dataItem($(this).closest("tr"))*/
        grid.clearSelection();
        grid.select($(this).closest("tr"))
    }

});

function additionalDataForTableCliente() {
    return {
        valueIdTypeDestinatario: $("#valueIdDestinatario").val()
    }
}

function selectDestinatarioOHButton() {
    showLoadingIco();
    var sTypeTable = $("#tipoGridNewOH").data("kendoRadioGroup").value();
    var valueIdType;
    var valueIdTypeDestinatario;
    var msg = "";
    var idGrid = $("#chargeFilters").attr("data-idTable");
    var grid = $('#' + idGrid).data("kendoGrid")
    var sType = $("#typeSelectDestinatarioOH").val();

    if (sTypeTable == "CL") {
        if ($("#selectDestinatariosOHGrid").data("kendoGrid").selectedKeyNames().length >= 1) {
            //valueIdType = $("#valueIdCliente").val();
            valueIdTypeDestinatario = $("#selectDestinatariosOHGrid").data("kendoGrid").selectedKeyNames().toString();
        } else {
            msg = "Has de escoger un destinatario"
        }
    } else {

        if ($("#selectGruposOHGrid").data("kendoGrid").selectedKeyNames().length >= 1) {
            value = $("#selectGruposOHGrid").data("kendoGrid").selectedKeyNames().toString()
        } else {
            msg = "Has de escoger un grupo";
            stopLoadingIco();
        }
    }

    if (msg == "") {

        if (sType == "newOH") {

            if (sTypeTable == "CL") {

                $.ajax({
                    type: 'POST',
                    url: route + "/FuncionesOfertasTelerik/getDadesClients",
                    data: {
                        valueIdTypeDestinatario: valueIdTypeDestinatario
                    },
                    dataType: "json",
                    success: function (data) {

                        if (data == -1) {
                            $('#divErrorTitle').html("<p style='font-weight: bold;'>Oferta Hospitalaria:</p>")
                            $('#divErrorImage').html('<img src="' + route + '/Content/img/warning.png" style="width: 72px;height: 72px;">');
                            $('#divErrorBody').html("Hubo un error al cargar los destinatarios")
                            $("#dialogError").data("kendoDialog").open();
                            stopLoadingIco();

                        } else if (data.Data.length == 0) {
                            $('#divErrorTitle').html("<p style='font-weight: bold;'>Oferta Hospitalaria:</p>")
                            $('#divErrorImage').html('<img src="' + route + '/Content/img/warning.png" style="width: 72px;height: 72px;">');
                            $('#divErrorBody').html("No existen Clientes para este destinatario")
                            $("#dialogError").data("kendoDialog").open();
                            stopLoadingIco();
                        } else if (data.Data.length == 1) {
                            //enviamos oferta para validar
                            window.location.href = route + "/Ofertas/Create?typeClienteGrupo=" + sTypeTable + "&valueIdType=" + data.Data[0].codigoSap + "&valueIdTypeDestinatario="
                                + valueIdTypeDestinatario
                                + "&unidosis=" + $("#unidosisNewOH").val() + "&sNameUsuario=" + $("#sNameUsuario").val() + "&iIdUser=" + $("#iIdUser").val();

                        } else if (data.Data.length > 1) {
                            //$('#divErrorTitle').html("<p style='font-weight: bold;'>Oferta Hospitalaria:</p>")
                            //$('#divErrorImage').html('<img src="' + route + '/Content/img/warning.png" style="width: 72px;height: 72px;">');
                            //$('#divErrorBody').html("Existe más de un Cliente para el Destinatario seleccionado, contactar a HelpDesk")
                            //$("#dialogError").data("kendoDialog").open();

                            //stopLoadingIco();
                            $("#dialogSelectDestinatarioOH").data("kendoDialog").close();
                            $("#valueIdDestinatario").val(valueIdTypeDestinatario);//Guardamos el codSap del Hospital para luego cargar la tabla de destinatarios                    
                            $("#titleSelectClienteOH").html("Selecciona Cliente");
                            $("#selectClientesOHGrid").data("kendoGrid").dataSource.read()
                            $("#dialogSelectClienteOH").data("kendoDialog").open();
                            stopLoadingIco();
                        }
                    }
                })

            } else {

                window.location.href = route + "/Ofertas/Create?typeClienteGrupo=" + sTypeTable + "&valueIdType=" + value
                    + "&unidosis=" + $("#unidosisNewOH").val() + "&sNameUsuario=" + $("#sNameUsuario").val() + "&iIdUser=" + $("#iIdUser").val();

            }
        }

        ////enviamos oferta para validar
        //window.location.href = route + "/Ofertas/Create?typeClienteGrupo=" + sTypeTable + "&valueIdType=" + valueIdType + "&valueIdTypeDestinatario="
        //    + valueIdTypeDestinatario
        //    + "&unidosis=" + $("#unidosisNewOH").val() + "&sNameUsuario=" + $("#sNameUsuario").val() + "&iIdUser=" + $("#iIdUser").val();
    } else {
        $('#divErrorTitle').html("<p style='font-weight: bold;'>Selecciona Destinatario:</p>")
        $('#divErrorImage').html('<img src="' + route + '/Content/img/warning.png" style="width: 72px;height: 72px;">');
        $('#divErrorBody').html(msg);
        $("#dialogError").data("kendoDialog").open();
        return false;
        stopLoadingIco();
    }

    //if (msg == "") {

    //    //enviamos oferta para validar
    //    window.location.href = route + "/Ofertas/Create?typeClienteGrupo=" + sTypeTable + "&valueIdType=" + valueIdType + "&valueIdTypeDestinatario="
    //        + valueIdTypeDestinatario
    //        + "&unidosis=" + $("#unidosisNewOH").val() + "&sNameUsuario=" + $("#sNameUsuario").val() + "&iIdUser=" + $("#iIdUser").val();
    //} else {
    //    $('#divErrorTitle').html("<p style='font-weight: bold;'>Selecciona Destinatario:</p>")
    //    $('#divErrorImage').html('<img src="' + route + '/Content/img/warning.png" style="width: 72px;height: 72px;">');
    //    $('#divErrorBody').html(msg);
    //    $("#dialogError").data("kendoDialog").open();
    //    return false;
    //    stopLoadingIco();
    //}
}

function selectClienteCopyOHButton() {
    showLoadingIco();
    var sTypeTable = $("#tipoGridCopyOH").data("kendoRadioGroup").value();
    var value;
    //var valueDestinatario;//AGREGADO POR JOSE DURAND - 22/02/2024
    var msg = "";
    //var idGrid = $("#chargeFilters").attr("data-idTable");
    //var grid = $('#' + idGrid).data("kendoGrid")
    var grid = $("#selectClientesCopyOHGrid").getKendoGrid();
    var sType = $("#typeSelectClienteCopyOH").val();
    let clientesDestinatarios = [];
    let clienteDestinatario = new Object();
    var keyNames = grid.selectedKeyNames();
    var gridData = grid.dataSource.data();

    if (sTypeTable == "CL") {
        if ($("#selectClientesCopyOHGrid").data("kendoGrid").selectedKeyNames().length >= 1) {           
            keyNames.forEach(function (number) {
                gridData.forEach(function (dataItem) {
                    if (number == dataItem.RowNum) {
                        //selectedClientes.push(dataItem.sSAPCliente) 
                        //selectedDestinatarios.push(dataItem.sSAPDestinatario)
                        clienteDestinatario = {
                            "codSapCliente": dataItem.codigoSap,
                            "codSapDestinatario": dataItem.codigoSap_Destinatario
                        }
                        clientesDestinatarios.push(clienteDestinatario);          
                    }
                })
            });

            //COMENTADO POR JOSE DURAND - INICIO - 19/04/2024
            //var allSelected = $("#selectClientesCopyOHGrid tr.k-state-selected");
            //var allSelectedModels = [];
            //var allSelectedModelsClientes = [];
            //$.each(allSelected, function (e) {
            //    var row = $(this);
            //    var grid = row.closest(".k-grid").data("kendoGrid");
            //    var dataItem = grid.dataItem(row);
            //    allSelectedModels.push(dataItem.codigoSap_Destinatario);
            //    allSelectedModelsClientes.push(dataItem.codigoSap);
            //    clienteDestinatario = {
            //        "codSapCliente": dataItem.codigoSap,
            //        "codSapDestinatario": dataItem.codigoSap_Destinatario
            //    }
            //    clientesDestinatarios.push(clienteDestinatario);
            //});
            //COMENTADO POR JOSE DURAND - FIN - 19/04/2024

            //valueDestinatario = allSelectedModels.toString(); --COMENTADO POR JOSE DURAND - 21/03/2024
            ////value = $("#selectClientesCopyOHGrid").data("kendoGrid").selectedKeyNames().toString();
            //value = allSelectedModelsClientes.toString(); --COMENTADO POR JOSE DURAND - 21/03/2024
        } else {
            msg = "Has de escoger un cliente"
            stopLoadingIco();
        }
    } else {

        if ($("#selectGruposCopyOHGrid").data("kendoGrid").selectedKeyNames().length >= 1) {

            value = $("#selectGruposCopyOHGrid").data("kendoGrid").selectedKeyNames().toString()

            clienteDestinatario = {
                "codSapCliente": value,
                "codSapDestinatario": null
            }
            clientesDestinatarios.push(clienteDestinatario);
        } else {
            msg = "Has de escoger un grupo";
            stopLoadingIco();
        }
    }

    if (msg == "") {

        if (sType == "copyOH") {

            $.ajax({
                type: 'POST',
                url: route + "/Ofertas/CopyOffer/",
                data: {
                    typeClienteGrupo: sTypeTable,
                    //valueIdType: value,
                    //valueIdTypeDestinatario: valueDestinatario,
                    oClientesDestinatarios: clientesDestinatarios,
                    iIdOffer: $("#iIdOfferToCopy").val()
                },
                success: function (data) {
                    if (data == -1) {
                        $('#divErrorTitle').html("<p style='font-weight: bold;'>Oferta Hospitalaria:</p>")
                        $('#divErrorImage').html('<img src="' + route + '/Content/img/warning.png" style="width: 72px;height: 72px;">');
                        $('#divErrorBody').html("Ha ocurrido un error.")
                        $("#dialogError").data("kendoDialog").open();
                        stopLoadingIco();
                    } else {
                        $('#messageDialogTitle').html("<p style='font-weight: bold;'>Oferta Hospitalaria:</p>")
                        $('#messageDialogImage').html('');
                        $('#messageDialogBody').html("Oferta copiada correctamente.")
                        $("#dialogMessage").data("kendoDialog").open();
                        grid.dataSource.read();
                        grid.clearSelection();
                        stopLoadingIco();
                    }
                    //stopLoadingIco();
                }, complete: function () {
                }
            })
        }
    } else {
        $('#divErrorTitle').html("<p style='font-weight: bold;'>Selecciona cliente:</p>")
        $('#divErrorImage').html('<img src="' + route + '/Content/img/warning.png" style="width: 72px;height: 72px;">');
        $('#divErrorBody').html(msg);
        $("#dialogError").data("kendoDialog").open();
        return false;
        stopLoadingIco();
    }

}

//AGREGADO POR JOSE DURAND - FIN - 04/03/2024

//REVISAR JOSE DURAND - 19/02/2024 - CUANDO LE DAS AL BOTON DE ACEPTAR DEL MODAL DE CREACION DE OFERTA SE EJECUTA ESTA FUNCIÓN.
function selectClienteOHButton() {
    showLoadingIco();
    var sTypeTable = $("#tipoGridNewOH").data("kendoRadioGroup").value();
    var value;
    var msg = "";
    var idGrid = $("#chargeFilters").attr("data-idTable");
    var grid = $('#' + idGrid).data("kendoGrid")
    var sType = $("#typeSelectClienteOH").val();

    if (sTypeTable == "CL") {
        if ($("#selectClientesOHGrid").data("kendoGrid").selectedKeyNames().length >= 1) {
            valueIdTypeDestinatario = $("#valueIdDestinatario").val();
            value = $("#selectClientesOHGrid").data("kendoGrid").selectedKeyNames().toString();
        } else {
            msg = "Has de escoger un cliente"
            stopLoadingIco();
        }
    } else {

        if ($("#selectGruposOHGrid").data("kendoGrid").selectedKeyNames().length >= 1) {
            value = $("#selectGruposOHGrid").data("kendoGrid").selectedKeyNames().toString()
        } else {
            msg = "Has de escoger un grupo";
            stopLoadingIco();
        }
    }

    if (msg == "") {

        if (sType == "newOH") {

            //AGREGADO POR JOSE DURAND - INICIO - 20/02/2024
            if (sTypeTable == "CL") {

                window.location.href = route + "/Ofertas/Create?typeClienteGrupo=" + sTypeTable + "&valueIdType=" + value + "&valueIdTypeDestinatario="
                    + valueIdTypeDestinatario
                    + "&unidosis=" + $("#unidosisNewOH").val() + "&sNameUsuario=" + $("#sNameUsuario").val() + "&iIdUser=" + $("#iIdUser").val();

                //$.ajax({
                //    type: 'POST',
                //    url: route + "/FuncionesOfertasTelerik/getDadesDestinatarios",
                //    data: {
                //        valueIdType: value
                //    },
                //    dataType: "json",
                //    success: function (data) {

                //        if (data == -1) {
                //            $('#divErrorTitle').html("<p style='font-weight: bold;'>Oferta Hospitalaria:</p>")
                //            $('#divErrorImage').html('<img src="' + route + '/Content/img/warning.png" style="width: 72px;height: 72px;">');
                //            $('#divErrorBody').html("Hubo un error al cargar los destinatarios")
                //            $("#dialogError").data("kendoDialog").open();
                //            stopLoadingIco();

                //        } else if (data.Data.length == 0) {
                //            $('#divErrorTitle').html("<p style='font-weight: bold;'>Oferta Hospitalaria:</p>")
                //            $('#divErrorImage').html('<img src="' + route + '/Content/img/warning.png" style="width: 72px;height: 72px;">');
                //            $('#divErrorBody').html("No existen Destinatarios para este cliente")
                //            $("#dialogError").data("kendoDialog").open();
                //            stopLoadingIco();
                //        } else if (data.Data.length == 1) {
                //            //enviamos oferta para validar
                //            window.location.href = route + "/Ofertas/Create?typeClienteGrupo=" + sTypeTable + "&valueIdType=" + value + "&valueIdTypeDestinatario="
                //                + data.Data[0].codigoSap
                //                + "&unidosis=" + $("#unidosisNewOH").val() + "&sNameUsuario=" + $("#sNameUsuario").val() + "&iIdUser=" + $("#iIdUser").val();

                //        } else if (data.Data.length > 1) {

                //            $("#dialogSelectClienteOH").data("kendoDialog").close();
                //            $("#valueIdCliente").val(value);//Guardamos el codSap del Hospital para luego cargar la tabla de destinatarios                    
                //            $("#titleSelectDestinatarioOH").html("Selecciona Destinatario");
                //            $("#selectDestinatariosOHGrid").data("kendoGrid").dataSource.read()
                //            $("#dialogSelectDestinatarioOH").data("kendoDialog").open();
                //            stopLoadingIco();
                //        }
                //        //stopLoadingIco();
                //    }
                //})
            } else {
                window.location.href = route + "/Ofertas/Create?typeClienteGrupo=" + sTypeTable + "&valueIdType=" + value
                    + "&unidosis=" + $("#unidosisNewOH").val() + "&sNameUsuario=" + $("#sNameUsuario").val() + "&iIdUser=" + $("#iIdUser").val();
            }

            //AGREGADO POR JOSE DURAND - FIN - 20/02/2024

            //window.location.href = route + "/Ofertas/Create?typeClienteGrupo=" + sTypeTable + "&valueIdType=" + value
            //    + "&unidosis=" + $("#unidosisNewOH").val() + "&sNameUsuario=" + $("#sNameUsuario").val() + "&iIdUser=" + $("#iIdUser").val();

        } else if (sType == "copyOH") {

            $.ajax({
                type: 'POST',
                url: route + "/Ofertas/CopyOffer/",
                data: {
                    typeClienteGrupo: sTypeTable,
                    valueIdType: value,
                    iIdOffer: $("#iIdOfferToCopy").val()
                },
                success: function (data) {
                    if (data == -1) {
                        $('#divErrorTitle').html("<p style='font-weight: bold;'>Oferta Hospitalaria:</p>")
                        $('#divErrorImage').html('<img src="' + route + '/Content/img/warning.png" style="width: 72px;height: 72px;">');
                        $('#divErrorBody').html("Ha ocurrido un error.")
                        $("#dialogError").data("kendoDialog").open();
                        stopLoadingIco();
                    } else {
                        $('#messageDialogTitle').html("<p style='font-weight: bold;'>Oferta Hospitalaria:</p>")
                        $('#messageDialogImage').html('');
                        $('#messageDialogBody').html("Oferta copiada correctamente.")
                        $("#dialogMessage").data("kendoDialog").open();
                        grid.dataSource.read();
                        grid.clearSelection();
                    }
                    stopLoadingIco();
                }, complete: function () {
                }
            })
        }
    } else {
        $('#divErrorTitle').html("<p style='font-weight: bold;'>Selecciona cliente:</p>")
        $('#divErrorImage').html('<img src="' + route + '/Content/img/warning.png" style="width: 72px;height: 72px;">');
        $('#divErrorBody').html(msg);
        $("#dialogError").data("kendoDialog").open();
        return false;
        stopLoadingIco();
    }

}

function sendToValidateMultipleOffer() {
    showLoadingIco();
    var idGrid = $("#chargeFilters").attr("data-idTable");
    var grid = $('#' + idGrid).data("kendoGrid")

    var listOffersSelected = grid.selectedKeyNames()

    //lo convertimos en string

    if (listOffersSelected.length >= 1) {
        //obtenemos la tabla de clientes mediante ajax 
        $.ajax({
            type: 'POST',
            url: route + "/Ofertas/SendMultipleToValidate/",
            data: {
                listOffers: listOffersSelected.toString(),
                iIdUsuario: $("#iIdUser").val()
            },
            success: function (data) {
                if (data == -1) {
                    $('#divErrorTitle').html("<p style='font-weight: bold;'>Oferta Hospitalaria:</p>")
                    $('#divErrorImage').html('<img src="' + route + '/Content/img/warning.png" style="width: 72px;height: 72px;">');
                    $('#divErrorBody').html("Ha ocurrido un error.")
                    $("#dialogError").data("kendoDialog").open();
                } else {
                    $('#messageDialogTitle').html("<p style='font-weight: bold;'>Oferta Hospitalaria:</p>")
                    $('#messageDialogImage').html('');
                    if ($("#sCodRole").val() == "DC")
                        $('#messageDialogBody').html("Validadas correctamente.")
                    else
                        $('#messageDialogBody').html("Enviadas a validar correctamente.")
                    $("#dialogMessage").data("kendoDialog").open();
                    grid.dataSource.read();
                    grid.clearSelection();
                    stopLoadingIco();
                }
            }, complete: function () {
            }
        })
    } else {
        $('#divErrorTitle').html("<p style='font-weight: bold;'>Validar multiples ofertas:</p>")
        $('#divErrorImage').html('<img src="' + route + '/Content/img/warning.png" style="width: 72px;height: 72px;">');
        $('#divErrorBody').html("Has de seleccionar mínimo 1 oferta.");
        $("#dialogError").data("kendoDialog").open();
        stopLoadingIco();
    }

}

function removeOH() {
    var idGrid = $("#chargeFilters").attr("data-idTable");
    var grid = $('#' + idGrid).data("kendoGrid")
    showLoadingIco();
    $.ajax({
        type: 'POST',
        url: route + "/Ofertas/DeleteOffer/",
        data: {
            iIdOffer: $('#hiddenIdOfertaForRemove').val()
        },
        success: function (data) {
            if (data == -1) {
                $('#divErrorTitle').html("<p style='font-weight: bold;'>Oferta Hospitalaria:</p>")
                $('#divErrorImage').html('<img src="' + route + '/Content/img/warning.png" style="width: 72px;height: 72px;">');
                $('#divErrorBody').html("Ha ocurrido un error.")
                $("#dialogError").data("kendoDialog").open();
            } else {
                $('#messageDialogTitle').html("<p style='font-weight: bold;'>Oferta Hospitalaria:</p>")
                $('#messageDialogImage').html('');
                $('#messageDialogBody').html("Oferta borrada correctamente.")
                $("#dialogMessage").data("kendoDialog").open();
                grid.dataSource.read();
                grid.clearSelection();
            }
            stopLoadingIco();
        }, complete: function () {
        }
    })
}