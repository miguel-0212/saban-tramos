$(document).on('click', "#turnHomeOH", function (ev) {
    showLoadingIco();
    location.replace(route);
})

$(document).on('click', "#turnHomeGrupoOH", function (ev) {
    showLoadingIco();
    location.replace(route + "/Grupos/Index");
})

$(document).on('click', "#turnHomeOHForValidated", function (ev) {
    showLoadingIco();
    location.replace(route + "/Ofertas/IndexForValidate");
})

$(document).on('click', "#turnHomeOHRefused", function (ev) {
    showLoadingIco();
    location.replace(route + "/Ofertas/IndexRefused");
})

$(document).on('click', "#turnHomeOHSearchOffer", function (ev) {
    showLoadingIco();
    location.replace(route + "/Ofertas/SearchOfertas");
})

$(document).on('click', "#turnHomeOHValidated", function (ev) {
    showLoadingIco();
    location.replace(route + "/Ofertas/IndexValidated");
})

let itemsMedicamentos = [];

$(document).on('change', ".dtoGrid", function (ev) {
    showLoadingIco();
    var errorDTO = false;
    var showMSG = false;
    var msg = "";
    var grid = $("#tableMedicamentos").data("kendoGrid");
    var dto = $(this).val();
    var codAleman = $(this)[0].className.split('dtoGrid_')[1].split('_')[0];

    var dataItem = grid.dataItem($(this).closest('.k-master-row'));

    //obtenemos valor max y min de descuento
    var maxDto = dataItem.maxDesc;
    var minDto = dataItem.minDesc;

    if (isNaN(parseFloat(dto.replace(',', '.')))) {
        msg = "Has de añadir un número."
        errorDTO = true;
        showMSG = true;
    } else {
        dto = parseFloat(dto.replace(',', '.'));
        if (dto > 100 || dto < 0) {
            msg = dto > 100 ? "No se puede añadir un descuento superior al 100%." : "No se puede añadir un descuento inferior al 0%."
            errorDTO = true;
            showMSG = true;
        }
        else if ((maxDto != null && minDto != null) && (dto > maxDto || dto < minDto)) {
            msg = dto < minDto ? "Medicamento con descuento bloqueado. No se puede poner un descuento menor de " + minDto + "." : "Medicamento con descuento bloqueado.No se puede poner un descuento mayor de " + maxDto + "."
            errorDTO = true;
            showMSG = true;
        }
        else if (dto === "" || dto == null || dto == undefined) {
            errorDTO = true;
            showMSG = false;
        } else {
            grid.select($(this).closest('.k-master-row'));
            calcValuesGridRow(dto, dataItem);
        }
    }



    if (errorDTO) {
        $(this).val("");
        if ($(this).closest('.k-master-row').hasClass("k-state-selected")) {
            $(this).closest('.k-master-row').find('.k-checkbox').click();
            $(this).closest('.k-master-row').removeClass("k-state-selected");
        }

        dataItem.set("errorRenta", -1);
        dataItem.set("rentabilidad", "");
        dataItem.set("precio", "");
        dataItem.set("precioUnitario", "");
        dataItem.set("precioOfertado", "");
        dataItem.set("precioUnitarioOfertado", "");
        dataItem.set("dto", "");

        manageItemIntoListMedicamentos(dataItem, "Remove")
        delete grid._selectedIds[codAleman];
        if (showMSG) {
            $('#divErrorTitle').html("<p style='font-weight: bold;'>Error:</p>")
            $('#divErrorImage').html('<img src="' + route + '/Content/img/warning.png" style="width: 72px;height: 72px;">');
            $('#divErrorBody').html(msg);
            $("#dialogError").data("kendoDialog").open();
        }
    }

    stopLoadingIco();

})

function calcValuesGridRow(dto, dataItem) {
    var unidades = 1;
    var maxRentabilidad = 15
    var dtoGlobal = dataItem.fDeduccion != "" ? dataItem.fDeduccion : "0";

    //José Durand 15/02/2023
    if (parseFloat(dtoGlobal.split('%')[0].replace(",", ".")) > 0) {
        var Pi = (parseFloat(dataItem.fPVL.replace(",", ".")) * unidades) - (parseFloat(dataItem.fPVL.replace(",", ".")) * unidades) * (parseFloat(dtoGlobal.split('%')[0].replace(",", ".")) / 100)
            - (((parseFloat(dataItem.fPVL.replace(",", ".")) * unidades) - (parseFloat(dataItem.fPVL.replace(",", ".")) * unidades) * (parseFloat(dtoGlobal.split('%')[0].replace(",", ".")) / 100)) * (parseFloat(dto) / 100));
    }
    else {
        var Pi = (parseFloat(dataItem.fPVL.replace(",", ".")) * unidades) - ((parseFloat(dataItem.fPVL.replace(",", ".")) * unidades) * ((parseFloat(dto) / 100)));
    }

    //var Pi = (parseFloat(dataItem.fPVL.replace(",", ".")) * unidades) - ((parseFloat(dataItem.fPVL.replace(",", ".")) * unidades) * ((parseFloat(dto) + (parseFloat(dtoGlobal.split('%')[0].replace(",", ".")))) / 100));
    // Fin José Durand

    var Ni = Pi - (parseFloat(dataItem.fCoste.replace(",", ".")) * unidades);
    var Di = parseFloat(dataItem.fPVL.replace(",", ".")) * unidades;

    var rentabilidad = Ni / Di;
    var errorRenta = 0;
    if (rentabilidad > (maxRentabilidad / 100)) {
        errorRenta = 0;
        dataItem.set("errorRenta", 0);
    } else {
        errorRenta = 1;
        dataItem.set("errorRenta", 1);
    }
    dataItem.set("rentabilidad", Math.round(rentabilidad * 10000) / 100 + "%");

    //JOSE DURAND - 14/02/2023 - INICIO
    if ((parseFloat(dataItem.IVA.replace(',', '.'))) < 1) {
        var precioIVA = Pi + (Pi * (parseFloat(dataItem.IVA.replace(',', '.'))));
        dataItem.set("precio", Math.round(precioIVA * 100) / 100 + "€");
    } else {
        var precioIVA = Pi + (Pi * (parseFloat(dataItem.IVA.replace(',', '.')) / 100));
        dataItem.set("precio", Math.round(precioIVA * 100) / 100 + "€");
    }
    //JOSE DURAND - 14/02/2023 - FIN

    //var precioIVA = Pi + (Pi * (parseFloat(dataItem.IVA.replace(',', '.'))/100)); //---- GSG (07/02/2023)
    //dataItem.set("precio", Math.round(precioIVA * 100) / 100 + "€");

    var precioUnitario = precioIVA / parseInt(dataItem.iUnidades);
    dataItem.set("precioUnitario", Math.round(precioUnitario * 100000) / 100000);


    var precioOfertado = Pi;
    dataItem.set("precioOfertado", Math.round(precioOfertado * 100) / 100 + "€");


    var precioOfertadoUnitario = Pi / parseInt(dataItem.iUnidades);
    dataItem.set("precioUnitarioOfertado", Math.round(precioOfertadoUnitario * 100000) / 100000);

    dataItem.set("dto", dto);

    if (errorRenta == 0)
        $('.rentabilidad_' + dataItem.codAlemania).addClass("bg-green");
    else
        $('.rentabilidad_' + dataItem.codAlemania).addClass("bg-red");

    manageItemIntoListMedicamentos(dataItem, "Add")
}

function onDataBound(e) {
    var grid = $("#tableMedicamentos").data("kendoGrid");

    var listItems = $.map(this.select(), function (item) {
        return $(item)
    });

    var dataItem = null;

    if (listItems.length > 0) {
        for (var i = 0; i <= listItems.length - 1; i++) {
            dataItem = grid.dataItem(listItems[i]);
            if (dataItem.errorRenta == 0)
                $('.rentabilidad_' + dataItem.codAlemania).addClass("bg-green");
            else
                $('.rentabilidad_' + dataItem.codAlemania).addClass("bg-red");
        }
    }
}

$(document).on('click', '.orderGridSelectedItems', function () {
    var grid = $('#tableMedicamentos').data("kendoGrid")
    grid.dataSource.sort({ field: "dto", dir: "desc" });
    grid.dataSource.page(1)
})

$(document).on('click', ".onSaveOH", function (ev) {
    showLoadingIco();

    var error = false;
    var msg = "Se ha de seleccionar mínimo 1 medicamento."
    var grid = $("#tableMedicamentos").data("kendoGrid");
    var selected = grid.selectedKeyNames().join(",");
    var sTypeCreate = $("#typeClienteGrupo").val()

    if (grid.dataSource._total > 0) {
        if (selected.length < 1 || selected == null) {
            error = true;
        }
    }

    if (error) {
        $('#divErrorTitle').html("<p style='font-weight: bold;'>Falta introducir los siguientes datos:</p>")
        $('#divErrorImage').html('<img src="' + route + '/Content/img/warning.png" style="width: 72px;height: 72px;">');
        $('#divErrorBody').html(msg)
        $("#dialogError").data("kendoDialog").open();
        stopLoadingIco();
    } else {
        if (sTypeCreate == "CL") {
            if ($("#concursoOH").data("kendoRadioGroup").value() == null || $("#concursoOH").data("kendoRadioGroup").value() == undefined) {
                error = true;
                msg = "Se ha de seleccionar un valor del concurso";
            }
            else if ($("#concursoOH").data("kendoRadioGroup").value() == "C" && ($("#dConcursoInput").val() == "" || $("#dConcursoInput").val() == null || $("#dConcursoInput").val() == undefined)) {
                error = true;
                msg = "Se ha de añadir una fecha";
            }
        }

        if (error) {
            $('#divErrorTitle').html("<p style='font-weight: bold;'>Falta introducir los siguientes datos:</p>")
            $('#divErrorImage').html('<img src="' + route + '/Content/img/warning.png" style="width: 72px;height: 72px;">');
            $('#divErrorBody').html(msg)
            $("#dialogError").data("kendoDialog").open();
            stopLoadingIco();
        } else {
            if (sTypeCreate == "CL") {
                $("#sConcurso").val($("#concursoOH").data("kendoRadioGroup").value());
                $("#dConcurso").val($("#dConcursoInput").val());
            }
            $("#sListMedicamentos").val(JSON.stringify(itemsMedicamentos));
            $("#createOfertaHospitalForm").submit();
        }
    }

})

//AGREGADO POR JOSE DURAND - 28/02/2024 - INICIO

function onSelectCliente(e) {

    var dataItem = e.dataItem;
    var value;

    if ($('#viewGP').length) {
        value = dataItem.Value;
    } else {
        value = null;
    }

    $.ajax({
        type: 'POST',
        url: route + "/FuncionesGlobalesOfertaHospital/getDestinatarios/",
        data: {
            sCodSapCliente: value
        },
        success: function (data) {
            if (data == -1) {
                $('#divErrorTitle').html("<p style='font-weight: bold;'>Falta introducir los siguientes datos:</p>")
                $('#divErrorImage').html('<img src="' + route + '/Content/img/warning.png" style="width: 72px;height: 72px;">');
                $('#divErrorBody').html(msg)
                $("#dialogError").data("kendoDialog").open();
                stopLoadingIco();
            } else {
                //var comboBox = $("#destinatarioOfertaHospital").data("kendoDropDownList");

                //comboBox.dataSource.read();

                var dropdownlist = $("#destinatarioOfertaHospital").kendoDropDownList({
                    dataTextField: "Text",
                    dataValueField: "Value",
                    dataSource: data,
                    filter: "contains"
                }).data("kendoDropDownList");

                //comboBox.setDataSource(new kendo.data.DataSource({
                //    data: data
                //}));
            }
        }, complete: function () {
            //var comboBox = $("#destinatarioOfertaHospital").data("kendoDropDownList");

            //comboBox.dataSource.read();
        }
    });

    //var comboBox = $("#destinatarioOfertaHospital").data("kendoDropDownList");

    //comboBox.dataSource.read();
    //kendoConsole.log("event :: select (" + dataItem.text + " : " + dataItem.value + ")");

}

//AGREGADO POR JOSE DURAND - 28/02/2024 - INICIO

function onChangeConcurso(e) {
    var value = e.newValue;
    if (value == "C") {
        $(".dConcursoInput").show();
        $(".dConcursoInput").val("");
    } else {
        $(".dConcursoInput").hide();
        $(".dConcursoInput").val("");
    }
}

function manageItemIntoListMedicamentos(dataItem, sTypeAction) {

    if (itemsMedicamentos.filter(function (ele) { return ele.sCodNacional == dataItem.sCodNacional; }).length >= 1) {
        itemsMedicamentos = itemsMedicamentos.filter(function (ele) {
            return ele.sCodNacional != dataItem.sCodNacional;
        })
    }

    if (sTypeAction == "Add") {
        var medicamento = {};
        medicamento.sCodNacional = dataItem.sCodNacional;
        medicamento.dto = dataItem.dto;
        medicamento.rentabilidad = dataItem.rentabilidad;
        medicamento.precio = dataItem.precio;
        medicamento.precioUnitario = dataItem.precioUnitario;
        medicamento.precioOfertado = dataItem.precioOfertado;
        medicamento.precioUnitarioOfertado = dataItem.precioUnitarioOfertado;
        itemsMedicamentos.push(medicamento);
    }
}

function onDataBoundDetail(e) {
    var grid = this;
    var rows = grid.items();
    $(rows).each(function (e) {
        var row = this;
        grid.select(row);
    });

    if (grid.items().length > 0) {
        for (var i = 0; i <= grid.items().length - 1; i++) {
            dataItem = grid.dataItem(grid.items()[i]);
            if (dataItem.errorRenta == 0)
                $('.rentabilidad_' + dataItem.codAlemania).addClass("bg-green");
            else
                $('.rentabilidad_' + dataItem.codAlemania).addClass("bg-red");
        }
    }

}

function validateOfferDC(e) {
    showLoadingIco();

    //obtenemos la tabla de clientes mediante ajax 
    $.ajax({
        type: 'POST',
        url: route + "/Ofertas/ValidateOffer/",
        data: {
            iIdOffer: $('#iIdOffer').val(),
            iIdUsuario: $('#iIdUser').val()
        },
        success: function (data) {
            if (data == -1) {
                $('#divErrorTitle').html("<p style='font-weight: bold;'>Falta introducir los siguientes datos:</p>")
                $('#divErrorImage').html('<img src="' + route + '/Content/img/warning.png" style="width: 72px;height: 72px;">');
                $('#divErrorBody').html("Ha ocurrido un error.") //MENSAJE AGREGADO POR JOSE DURAND 13/04/2023
                $("#dialogError").data("kendoDialog").open();
                stopLoadingIco();
            } else {
                //nos redirige a la pagina home
                if ($("#typeView").val() == "Detail") {
                    location.replace(route + "/Ofertas/Index");
                } else if ($("#typeView").val() == "DetailsForValidate") {
                    location.replace(route + "/Ofertas/IndexForValidate");
                }
            }
        }, complete: function () {
        }
    })
}
//REVISAR JOSE DURAND - 01/04/2024
var accionEjecutada = false; //JOSE DURAND - 01/04/2024

function onDataBoundEdit(e) {

    var grid2 = $("#tableMedicamentos").data("kendoGrid");
    var gridData = grid2.dataSource.data();

    var grid = this;
    var rows = grid.items();
    if ($("#firtsUploadTableEditOH").val() == "true") {
        $(rows).each(function (e) {
            var row = this;
            if (grid.dataItem(row).dto != "" && grid.dataItem(row).dto != null && grid.dataItem(row).dto != undefined) {
                grid.select(row);
            }
        });
        //$("#firtsUploadTableEditOH").val("false"); -- COMENTADO POR JOSE DURAND - 12/12/2023
    }

    gridData.forEach(function (dataItem) {
        if (dataItem.dto != "" && dataItem.dto != null && dataItem.dto != undefined && !accionEjecutada) {
            calcValuesGridRow(dataItem.dto, dataItem);            
        }
    })

    accionEjecutada = true

    var listItems = $.map(this.select(), function (item) {
        return $(item)
    });

    var dataItem = null;

    if (listItems.length > 0) {
        for (var i = 0; i <= listItems.length - 1; i++) {
            dataItem = grid.dataItem(listItems[i]);
            manageItemIntoListMedicamentos(dataItem, "Add")
            if (dataItem.errorRenta == 0)
                $('.rentabilidad_' + dataItem.codAlemania).addClass("bg-green");
            else
                $('.rentabilidad_' + dataItem.codAlemania).addClass("bg-red");
        }
    }
}

$(document).on('click', ".onEditOH", function (ev) {
    showLoadingIco();

    var error = false;
    var msg = "Se ha de seleccionar mínimo 1 medicamento."
    var grid = $("#tableMedicamentos").data("kendoGrid");
    var selected = grid.selectedKeyNames().join(",");

    if (grid.dataSource._total > 0) {
        if (selected.length < 1 || selected == null) {
            error = true;
        }
    }

    if (error) {
        $('#divErrorTitle').html("<p style='font-weight: bold;'>Falta introducir los siguientes datos:</p>")
        $('#divErrorImage').html('<img src="' + route + '/Content/img/warning.png" style="width: 72px;height: 72px;">');
        $('#divErrorBody').html(msg)
        $("#dialogError").data("kendoDialog").open();
        stopLoadingIco();
    } else {

        if ($("#concursoOH").data("kendoRadioGroup").value() == null || $("#concursoOH").data("kendoRadioGroup").value() == undefined) {
            error = true;
            msg = "Se ha de seleccionar un valor del concurso";
        }
        else if ($("#concursoOH").data("kendoRadioGroup").value() == "C" && ($("#dConcursoInput").val() == "" || $("#dConcursoInput").val() == null || $("#dConcursoInput").val() == undefined)) {
            error = true;
            msg = "Se ha de añadir una fecha";
        }

        if (error) {
            $('#divErrorTitle').html("<p style='font-weight: bold;'>Falta introducir los siguientes datos:</p>")
            $('#divErrorImage').html('<img src="' + route + '/Content/img/warning.png" style="width: 72px;height: 72px;">');
            $('#divErrorBody').html(msg)
            $("#dialogError").data("kendoDialog").open();
            stopLoadingIco();
        } else {
            $("#sConcurso").val($("#concursoOH").data("kendoRadioGroup").value());
            $("#dConcurso").val($("#dConcursoInput").val());
            $("#sListMedicamentos").val(JSON.stringify(itemsMedicamentos));
            $("#editOfertaHospitalForm").submit();
        }
    }

})

$(document).on('click', ".onSaveGrupos", function (ev) {
    showLoadingIco();

    var error = false;
    var msg = "."
    //var grid = $("#tableClientes").data("kendoGrid");
    var grid = $("#tableClientes").getKendoGrid(); //--ES UNA FORMA DE TRAER LOS DATOS DE LA TABLA COMPLETA
    var selected = grid.selectedKeyNames().join(",");

    //AGREGADO POR JOSE DURAND - 26/02/2024 - INICIO

    var keyNames = grid.selectedKeyNames();
    var gridData = grid.dataSource.data();
    var selectedDestinatarios = [];
    var selectedClientes = [];
    let clientesDestinatarios = [];
    let clienteDestinatario = new Object();
    var index = 0;

    keyNames.forEach(function (number) {
        gridData.forEach(function (dataItem) {
            if (number == dataItem.RowNum) {
                //selectedClientes.push(dataItem.sSAPCliente) 
                //selectedDestinatarios.push(dataItem.sSAPDestinatario)
                clienteDestinatario = {
                    "codSapCliente": dataItem.sSAPCliente,
                    "codSapDestinatario": dataItem.sSAPDestinatario
                }
                clientesDestinatarios.push(clienteDestinatario);

                $("#createGruposForm").append('<input type="hidden" class="Formulario" name="GruposFormModel.clientesDestinatarios[' + index + '].codSapCliente" value="' + dataItem.sSAPCliente + '" />');
                $("#createGruposForm").append('<input type="hidden" class="Formulario" name="GruposFormModel.clientesDestinatarios[' + index + '].codSapDestinatario" value="' + dataItem.sSAPDestinatario + '" />');
                index++;
            }
        })
    });

    //AGREGADO POR JOSE DURAND - 26/02/2024 - FIN

    if (grid.dataSource._total > 0) {
        if (selected.length < 1 || selected == null) {
            error = true;
            msg = "Se ha de seleccionar mínimo 1 cliente."
        }
    }
    if ($("#sNameGrupo").val() == "" || $("#sNameGrupo").val() == null || $("#sNameGrupo").val() == undefined) {
        error = true;
        msg = msg + "<br> Se ha de añadir un nombre de grupo."
        var elementos = document.querySelectorAll('.Formulario');
        for (elemento of elementos) {
            elemento.remove();
        }
    }

    if (error) {
        $('#divErrorTitle').html("<p style='font-weight: bold;'>Falta introducir los siguientes datos:</p>")
        $('#divErrorImage').html('<img src="' + route + '/Content/img/warning.png" style="width: 72px;height: 72px;">');
        $('#divErrorBody').html(msg)
        $("#dialogError").data("kendoDialog").open();
        stopLoadingIco();
        var elementos = document.querySelectorAll('.Formulario');
        for (elemento of elementos) {
            elemento.remove();
        }
    } else {

        var sNameGrupo = $("#sNameGrupo").val();
        var iIdUsuario = $("#iIdUsuario").val();

        $("#createGruposForm").append('<input type="hidden" class="Formulario" name="GruposFormModel.sNameGrupo" value="' + sNameGrupo + '" />');
        $("#createGruposForm").append('<input type="hidden" class="Formulario" name="GruposFormModel.iIdUsuario" value="' + iIdUsuario + '" />');
        //$("#clientesDestinatarios").val(JSON.stringify(clientesDestinatarios)); //AGREGADO POR JOSE DURAND - 21/03/2024
        //$("#listDestinatarios").val(selectedDestinatarios.toString()); //AGREGADO POR JOSE DURAND - 26/02/2024 --COMENTADO POR JOSE DURAND - 21/03/2024
        //$("#listClientes").val(grid.selectedKeyNames().toString()); //COMENTADO POR JOSE DURAND - 26/02/2024
        //$("#listClientes").val(selectedClientes.toString()); //AGREGADO POR JOSE DURAND - 26/02/2024 --COMENTADO POR JOSE DURAND - 21/03/2024
        $("#createGruposForm").submit();

        var elementos = document.querySelectorAll('.Formulario');
        for (elemento of elementos) {
            elemento.remove();
        }
    }

})

function aceptarChangeStatusActivoGrupoDialog() {
    showLoadingIco();
    $.ajax({
        type: 'POST',
        url: route + "/Grupos/ChangeStatusGrupo/",
        data: {
            iIdGrupo: $("#hiddenIdGrupo").val(),
            typeEstado: $("#typeEstadoGrupo").val(),
        },
        success: function (data) {
            if (data == "-1") {
                $('#divErrorTitle').html("<p style='font-weight: bold;'>Error:</p>")
                $('#divErrorImage').html('<img src="' + route + '/Content/img/warning.png" style="width: 72px;height: 72px;">');
                $('#divErrorBody').html("Ha ocurrido algun error.")
                $("#dialogError").data("kendoDialog").open();
            } else {
                $('#messageDialogTitle').html("<p style='font-weight: bold;'>Mantenimiento Stada :</p>")
                $('#messageDialogImage').html('');
                $('#messageDialogBody').html("Cambio de estado correcto.")
                $("#dialogMessage").data("kendoDialog").open();
            }
            stopLoadingIco();
        }, complete: function () {
            $('#searchFiltrosGruposHospitales').click()
        }
    })
}

function onDataBoundEditGrupos(e) {
    var grid = this;
    var rows = grid.items();

    $(rows).each(function (e) {
        var row = this;
        if (grid.dataItem(row).bIsSelected == "true" || grid.dataItem(row).bIsSelected == true || grid.dataItem(row).bIsSelected == "True")
            grid.select(row);
    });
}

//REVISAR JOSE DURAND - 27/02/2024
$(document).on('click', ".onEditGrupo", function (ev) {
    showLoadingIco();

    var error = false;
    var msg = "."
    var grid = $("#tableClientes").data("kendoGrid");
    var selected = grid.selectedKeyNames().join(",");

    //AGREGADO POR JOSE DURAND - 26/02/2024 - INICIO

    var keyNames = grid.selectedKeyNames();
    var gridData = grid.dataSource.data();
    var selectedDestinatarios = [];
    var selectedClientes = [];
    let clientesDestinatarios = [];
    let clienteDestinatario = new Object();
    var index = 0;

    keyNames.forEach(function (number) {
        gridData.forEach(function (dataItem) {
            if (number == dataItem.RowNum) {
                //selectedClientes.push(dataItem.sSAPCliente)
                //selectedDestinatarios.push(dataItem.sSAPDestinatario)
                clienteDestinatario = {
                    "codSapCliente": dataItem.sSAPCliente,
                    "codSapDestinatario": dataItem.sSAPDestinatario
                }
                clientesDestinatarios.push(clienteDestinatario);

                $("#editGruposForm").append('<input type="hidden" class="Formulario" name="GruposFormModel.clientesDestinatarios[' + index + '].codSapCliente" value="' + dataItem.sSAPCliente + '" />');
                $("#editGruposForm").append('<input type="hidden" class="Formulario" name="GruposFormModel.clientesDestinatarios[' + index + '].codSapDestinatario" value="' + dataItem.sSAPDestinatario + '" />');
                index++;
            }
        })
    });

    //AGREGADO POR JOSE DURAND - 26/02/2024 - FIN

    if (grid.dataSource._total > 0) {
        if (selected.length < 1 || selected == null) {
            error = true;
            msg = "Se ha de seleccionar mínimo 1 cliente."
        }
    }
    if ($("#sNameGrupo").val() == "" || $("#sNameGrupo").val() == null || $("#sNameGrupo").val() == undefined) {
        error = true;
        msg = msg + "<br> Se ha de añadir un nombre de grupo."
        var elementos = document.querySelectorAll('.Formulario');
        for (elemento of elementos) {
            elemento.remove();
        }
    }

    if (error) {
        $('#divErrorTitle').html("<p style='font-weight: bold;'>Falta introducir los siguientes datos:</p>")
        $('#divErrorImage').html('<img src="' + route + '/Content/img/warning.png" style="width: 72px;height: 72px;">');
        $('#divErrorBody').html(msg)
        $("#dialogError").data("kendoDialog").open();
        stopLoadingIco();
        var elementos = document.querySelectorAll('.Formulario');
        for (elemento of elementos) {
            elemento.remove();
        }
    } else {
        var sNameGrupo = $("#sNameGrupo").val();
        var iIdUsuario = $("#iIdUsuario").val();
        var iIdGrupo = $("#iIdGrupo").val();

        $("#editGruposForm").append('<input type="hidden" class="Formulario" name="GruposFormModel.sNameGrupo" value="' + sNameGrupo + '" />');
        $("#editGruposForm").append('<input type="hidden" class="Formulario" name="GruposFormModel.iIdUsuario" value="' + iIdUsuario + '" />');
        $("#editGruposForm").append('<input type="hidden" class="Formulario" name="GruposFormModel.iIdGrupo" value="' + iIdGrupo + '" />');

        //$("#listDestinatarios").val(selectedDestinatarios.toString()); //AGREGADO POR JOSE DURAND - 26/02/2024 --COMENTADO POR JOSE DURAND - 22/03/2024
        //$("#listClientes").val(selectedClientes.toString()); //AGREGADO POR JOSE DURAND - 27/02/2024 --COMENTADO POR JOSE DURAND - 22/03/2024
        //$("#listClientes").val(grid.selectedKeyNames().toString()); //COMENTADO POR JOSE DURAND - 27/02/2024
        $("#editGruposForm").submit();
        var elementos = document.querySelectorAll('.Formulario');
        for (elemento of elementos) {
            elemento.remove();
        }
    }

})

function refusedOfferForValidated(e) {
    showLoadingIco();

    var textArea = $('#textAreaRefused').val();
    if (textArea != "") {
        //obtenemos la tabla de clientes mediante ajax 
        $.ajax({
            type: 'POST',
            url: route + "/Ofertas/RefusedOffer/",
            data: {
                iIdOffer: $('#iIdOffer').val(),
                iIdUsuario: $('#iIdUser').val(),
                motivoRefused: textArea
            },
            success: function (data) {
                if (data == -1) {
                    $('#messageTitle').html("<p style='font-weight: bold;'>Error:</p>")
                    $('#messageImage').html('<img src="~/../../Content/img/error.png" style="width: 72px;height: 72px;">');
                    $('#messageBody').html("Ha ocurrido un error.")
                    $('#buttonModalMessage').click();
                    stopLoadingIco();
                } else {
                    location.replace(route + "/Ofertas/IndexForValidate");
                }
            }, complete: function () {
            }
        })
    } else {
        $('#divErrorTitle').html("<p style='font-weight: bold;'>Falta introducir los siguientes datos:</p>")
        $('#divErrorImage').html('<img src="' + route + '/Content/img/warning.png" style="width: 72px;height: 72px;">');
        $('#divErrorBody').html("Se ha de escribir un motivo para el rechazo de la oferta.")
        $("#dialogError").data("kendoDialog").open();
        stopLoadingIco();
    }


}

$(document).on('click', ".onEditForValidatedOH", function (ev) {
    showLoadingIco();

    var error = false;
    var msg = "Se ha de seleccionar mínimo 1 medicamento."
    var grid = $("#tableMedicamentos").data("kendoGrid");
    var selected = grid.selectedKeyNames().join(",");

    if (grid.dataSource._total > 0) {
        if (selected.length < 1 || selected == null) {
            error = true;
        }
    }

    if (error) {
        $('#divErrorTitle').html("<p style='font-weight: bold;'>Falta introducir los siguientes datos:</p>")
        $('#divErrorImage').html('<img src="' + route + '/Content/img/warning.png" style="width: 72px;height: 72px;">');
        $('#divErrorBody').html(msg)
        $("#dialogError").data("kendoDialog").open();
        stopLoadingIco();
    } else {

        if ($("#concursoOH").data("kendoRadioGroup").value() == null || $("#concursoOH").data("kendoRadioGroup").value() == undefined) {
            error = true;
            msg = "Se ha de seleccionar un valor del concurso";
        }
        else if ($("#concursoOH").data("kendoRadioGroup").value() == "C" && ($("#dConcursoInput").val() == "" || $("#dConcursoInput").val() == null || $("#dConcursoInput").val() == undefined)) {
            error = true;
            msg = "Se ha de añadir una fecha";
        }

        if (error) {
            $('#divErrorTitle').html("<p style='font-weight: bold;'>Falta introducir los siguientes datos:</p>")
            $('#divErrorImage').html('<img src="' + route + '/Content/img/warning.png" style="width: 72px;height: 72px;">');
            $('#divErrorBody').html(msg)
            $("#dialogError").data("kendoDialog").open();
            stopLoadingIco();
        } else {
            $("#sConcurso").val($("#concursoOH").data("kendoRadioGroup").value());
            $("#dConcurso").val($("#dConcursoInput").val());
            $("#sListMedicamentos").val(JSON.stringify(itemsMedicamentos));
            $("#editOfertaHospitalForm").submit();
        }
    }

})

//AGREGADO POR JOSE DURAND - INICIO - 04/04/2024

function additionalDataViewGPValidate() {

    var value;

    if ($('#viewGP').length <= 0) {
        value = null;
    }

    return {
        sCodSapCliente: value,
    }
}

//AGREGADO POR JOSE DURAND - FIN - 04/04/2024