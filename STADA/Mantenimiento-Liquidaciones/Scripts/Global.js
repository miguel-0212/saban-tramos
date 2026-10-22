// La app se aloja en la raíz del sitio (no bajo un subdirectorio virtual), así que "route" es
// simplemente el origen. El cálculo anterior (origin + primer segmento de la ruta) añadía de más
// el nombre del controlador actual (p. ej. "/MSubClubs") a cada URL construida con "route + ...",
// rompiendo cualquier combo/lookup que dependiera de esta variable.
var route = location.origin;
var focusInputCan = false;


$(document).ready(function () {
    chargeFilters();
});

$(window).resize(function () {
    if ($('.tableGridKendo.activeTable')) {
        resizeTableKendo($('.tableGridKendo.activeTable').attr('data-idTable'));
    } else {
        if ($('.tableGridKendo').length >= 1) {
            resizeTableKendo($('.tableGridKendo').attr('data-idTable'));
        }
    }
});

function resizeTableKendo(idTable) {
    var grid, content;
    grid = $("#" + idTable);
    content = grid.find('.k-grid-content');

    grid.height($(window)[0].innerHeight - ($(window)[0].innerHeight * 0.45) - grid.parent().parent().find('.divFiltros').height());
    content.height($(window)[0].innerHeight - ($(window)[0].innerHeight * 0.45) - grid.parent().parent().find('.divFiltros').height());
}

function resizeTableKendoByFilters(eventTypeFilter, idTable) {
    var grid, content;

    grid = $("#" + idTable);
    content = grid.find('.k-grid-content');

    switch (eventTypeFilter) {
        case 'expanse':
            grid.height($(window)[0].innerHeight - ($(window)[0].innerHeight * 0.45) - maxHeightToolbar);
            content.height($(window)[0].innerHeight - ($(window)[0].innerHeight * 0.45) - maxHeightToolbar);
            break;
        case 'collapse':
            grid.height($(window)[0].innerHeight - ($(window)[0].innerHeight * 0.45) - minHeightToolbar);
            content.height($(window)[0].innerHeight - ($(window)[0].innerHeight * 0.45) - minHeightToolbar);
            break;
    }
}

function exportarExcel(gridName) {
    var grid = $("#" + gridName).data("kendoGrid");
    grid.saveAsExcel();
}

function createDatePicker(container, fieldName, value = null) {
    var datePicker = container.find("#" + fieldName);

    if (value) {
        datePicker.val(kendo.toString(new Date(value), "yyyy-MM-dd"));
    } else {
        datePicker.val(null);

    }

    datePicker.kendoDatePicker({
        format: "yyyy-MM-dd" 
    });
}

function initComboBoxIdClub(container, fieldName, selectedValue) {
    var comboBox = container.find("#" + fieldName);

    // Llama a tu API para obtener los datos de MClubs
    $.ajax({
        url: route + "/MClub/ReadMClub", // Cambia esta URL por la correcta de tu API
        type: "GET",
        dataType: "json",
        success: function (data) {
            comboBox.kendoComboBox({
                dataTextField: "sNombreClub", // Campo a mostrar en el ComboBox
                dataValueField: "sIdClub", // Valor del ComboBox
                dataSource: data.Data,
                suggest: true, // Sugerir valores mientras se escribe
                // Si deseas un valor por defecto
                value: selectedValue
            });
        },
        error: function (err) {
            console.error("Error al obtener datos de MClubs: ", err);
        }
    });
}

function initComboBoxIdTramo(container, fieldName, selectedValue) {
    var comboBox = container.find("#" + fieldName);

    // Llama a tu API para obtener los datos de MClubs
    $.ajax({
        url: route + "/SabanaTramos/ReadComboTramos", // Cambia esta URL por la correcta de tu API
        type: "GET",
        dataType: "json",
        success: function (data) {
            comboBox.kendoComboBox({
                dataTextField: "IdTramo", // Campo a mostrar en el ComboBox
                dataValueField: "IdTramo", // Valor del ComboBox
                dataSource: data.Data,
                suggest: true, // Sugerir valores mientras se escribe
                // Si deseas un valor por defecto
                value: selectedValue
            });
        },
        error: function (err) {
            console.error("Error al obtener datos de Tramos: ", err);
        }
    });
}

function initComboBoxPeriodicidad(container, fieldName, selectedValue) {
    var comboBox = container.find("#" + fieldName);

    // Llama a tu API para obtener los datos de MClubs
    $.ajax({
        url: route + "/MPeriodicidad/ReadMPeriodicidad", // Cambia esta URL por la correcta de tu API
        type: "GET",
        dataType: "json",
        success: function (data) {
            comboBox.kendoComboBox({
                dataTextField: "sPeriodicidad", // Campo a mostrar en el ComboBox
                dataValueField: "IdPeriodicidad", // Valor del ComboBox
                dataSource: data.Data,
                suggest: true, // Sugerir valores mientras se escribe
                // Si deseas un valor por defecto
                value: selectedValue
            });
        },
        error: function (err) {
            console.error("Error al obtener datos de Tramos: ", err);
        }
    });
}

function initComboBoxVW_MAT_LIQ(container, fieldName, selectedValue) {
    var comboBox = container.find("#" + fieldName);

    // Llama a tu API para obtener los datos de MClubs
    $.ajax({
        url: route + "/Transversal/ReadVW_MAT_LIQ", // Cambia esta URL por la correcta de tu API
        type: "GET",
        dataType: "json",
        success: function (data) {
            comboBox.kendoComboBox({
                dataTextField: "MATE_MATERIAL_TEXT", // Campo a mostrar en el ComboBox
                dataValueField: "MATE_MATERIAL_ID", // Valor del ComboBox
                dataSource: data.Data,
                suggest: true, // Sugerir valores mientras se escribe
                // Si deseas un valor por defecto
                value: selectedValue
            });
        },
        error: function (err) {
            console.error("Error al obtener datos de Tramos: ", err);
        }
    });
}

function initComboBoxVW_CLI_LIQ(container, fieldName, selectedValue) {
    var comboBox = container.find("#" + fieldName);

    // Llama a tu API para obtener los datos de MClubs
    $.ajax({
        url: route + "/Transversal/ReadVW_CLI_LIQ", // Cambia esta URL por la correcta de tu API
        type: "GET",
        dataType: "json",
        success: function (data) {
            comboBox.kendoComboBox({
                dataTextField: "NAME", // Campo a mostrar en el ComboBox
                dataValueField: "KUNNR", // Valor del ComboBox
                dataSource: data.Data,
                suggest: true, // Sugerir valores mientras se escribe
                // Si deseas un valor por defecto
                value: selectedValue
            });
        },
        error: function (err) {
            console.error("Error al obtener datos de Tramos: ", err);
        }
    });
}

function initComboBoxTipoVenta(container, fieldName, selectedValue) {
    var comboBox = container.find("#" + fieldName);

    // Llama a tu API para obtener los datos de MClubs
    $.ajax({
        url: route + "/MTipoVenta/ReadMTipoVenta", // Cambia esta URL por la correcta de tu API
        type: "GET",
        dataType: "json",
        success: function (data) {
            comboBox.kendoComboBox({
                dataTextField: "sTipoVenta", // Campo a mostrar en el ComboBox
                dataValueField: "IdTipoVenta", // Valor del ComboBox
                dataSource: data.Data,
                suggest: true, // Sugerir valores mientras se escribe
                // Si deseas un valor por defecto
                value: selectedValue
            });
        },
        error: function (err) {
            console.error("Error al obtener datos de Tramos: ", err);
        }
    });
}


function initComboBoxidClub(container, fieldName, selectedValue) {
    var comboBox = container.find("#" + fieldName);

    // Llama a tu API para obtener los datos de MClubs
    $.ajax({
        url: route + "/MClub/ReadMClubActivo", // Cambia esta URL por la correcta de tu API
        type: "GET",
        dataType: "json",
        success: function (data) {
            comboBox.kendoComboBox({
                dataTextField: "sNombreClub", // Campo a mostrar en el ComboBox
                dataValueField: "sIdClub", // Valor del ComboBox
                dataSource: data.Data,
                suggest: true, // Sugerir valores mientras se escribe
                // Si deseas un valor por defecto
                value: selectedValue
            });
        },
        error: function (err) {
            console.error("Error al obtener datos: ", err);
        }
    });
}



function initComboBoxsIdClub(container, fieldName, selectedValue) {
    var comboBox = container.find("#" + fieldName);

    // Llama a tu API para obtener los datos de MClubs
    $.ajax({
        url: route + "/MSubClubs/ReadMSubClub", // Cambia esta URL por la correcta de tu API
        type: "GET",
        dataType: "json",
        success: function (data) {
            comboBox.kendoComboBox({
                dataTextField: "sIdClub", // Campo a mostrar en el ComboBox
                dataValueField: "sIdClub", // Valor del ComboBox
                dataSource: data.Data,
                suggest: true, // Sugerir valores mientras se escribe
                // Si deseas un valor por defecto
                value: selectedValue
            });
        },
        error: function (err) {
            console.error("Error al obtener datos: ", err);
        }
    });
}



function initComboBoxsIdSubClub(container, fieldName, selectedValue) {
    var comboBox = container.find("#" + fieldName);

    // Llama a tu API para obtener los datos de MClubs
    $.ajax({
        url: route + "/MSubClubs/ReadMSubClub", // Cambia esta URL por la correcta de tu API
        type: "GET",
        dataType: "json",
        success: function (data) {
            comboBox.kendoComboBox({
                dataTextField: "sIdSubClub", // Campo a mostrar en el ComboBox
                dataValueField: "sIdSubClub", // Valor del ComboBox
                dataSource: data.Data,
                suggest: true, // Sugerir valores mientras se escribe
                // Si deseas un valor por defecto
                value: selectedValue
            });
        },
        error: function (err) {
            console.error("Error al obtener datos: ", err);
        }
    });
}



function initComboBoxidSocio(container, fieldName, selectedValue) {
    var comboBox = container.find("#" + fieldName);

    // Llama a tu API para obtener los datos de MClubs
    $.ajax({
        url: route + "/MSocios/ReadMSocios", // Cambia esta URL por la correcta de tu API
        type: "GET",
        dataType: "json",
        success: function (data) {
            comboBox.kendoComboBox({
                dataTextField: "sNombreFcia", // Campo a mostrar en el ComboBox
                dataValueField: "sIdSocio", // Valor del ComboBox
                dataSource: data.Data,
                suggest: true, // Sugerir valores mientras se escribe
                // Si deseas un valor por defecto
                value: selectedValue
            });
        },
        error: function (err) {
            console.error("Error al obtener datos: ", err);
        }
    });
}

function initComboBoxFormasCobro(container, fieldName, selectedValue) {
    var comboBox = container.find("#" + fieldName);

    // Llama a tu API para obtener los datos de MClubs
    $.ajax({
        url: route + "/MFormasCobroes/ReadMFormasCobro", // Cambia esta URL por la correcta de tu API
        type: "GET",
        dataType: "json",
        success: function (data) {
            comboBox.kendoComboBox({
                dataTextField: "sFormaCobro", // Campo a mostrar en el ComboBox
                dataValueField: "IdFormaCobro", // Valor del ComboBox
                dataSource: data.Data,
                suggest: true, // Sugerir valores mientras se escribe
                // Si deseas un valor por defecto
                value: selectedValue
            });
        },
        error: function (err) {
            console.error("Error al obtener datos: ", err);
        }
    });
}

function initComboBoxidRed(container, fieldName, selectedValue) {
    var comboBox = container.find("#" + fieldName);

    // Llama a tu API para obtener los datos de MClubs
    $.ajax({
        url: route + "/MClub/ReadMClubActivo", // Cambia esta URL por la correcta de tu API
        type: "GET",
        dataType: "json",
        success: function (data) {
            comboBox.kendoComboBox({
                dataTextField: "sIdRed", // Campo a mostrar en el ComboBox
                dataValueField: "sIdRed", // Valor del ComboBox
                dataSource: data.Data,
                suggest: true, // Sugerir valores mientras se escribe
                // Si deseas un valor por defecto
                value: selectedValue
            });
        },
        error: function (err) {
            console.error("Error al obtener datos: ", err);
        }
    });
}

function initComboBoxidSubClub(container, fieldName, selectedValue) {
    var comboBox = container.find("#" + fieldName);

    // Llama a tu API para obtener los datos de MClubs
    $.ajax({
        url: route + "/MsubClubs/ReadMSubClubActivo", // Cambia esta URL por la correcta de tu API
        type: "GET",
        dataType: "json",
        success: function (data) {
            comboBox.kendoComboBox({
                dataTextField: "sNombreSubClub", // Campo a mostrar en el ComboBox
                dataValueField: "sIdSubClub", // Valor del ComboBox
                dataSource: data.Data,
                suggest: true, // Sugerir valores mientras se escribe
                // Si deseas un valor por defecto
                value: selectedValue
            });
        },
        error: function (err) {
            console.error("Error al obtener datos de SubClubs: ", err);
        }
    });
}

function initComboBoxIdTipoDescuento(container, fieldName, selectedValue) {
    var comboBox = container.find("#" + fieldName);

    // Llama a tu API para obtener los datos de MClubs
    $.ajax({
        url: route + "/MTipoDescuento/ReadMTipoDescuento", // Cambia esta URL por la correcta de tu API
        type: "GET",
        dataType: "json",
        success: function (data) {
            comboBox.kendoComboBox({
                dataTextField: "sTipoDescuento", // Campo a mostrar en el ComboBox
                dataValueField: "IdTipoDescuento", // Valor del ComboBox
                dataSource: data.Data,
                suggest: true, // Sugerir valores mientras se escribe
                // Si deseas un valor por defecto
                value: selectedValue
            });
        },
        error: function (err) {
            console.error("Error al obtener datos: ", err);
        }
    });
}


function initComboBoxMMayorista(container, fieldName, selectedValue) {
    var comboBox = container.find("#" + fieldName);

    // Llama a tu API para obtener los datos de MClubs
    $.ajax({
        url: route + "/MMayoristas/ReadMMayorista", // Cambia esta URL por la correcta de tu API
        type: "GET",
        dataType: "json",
        success: function (data) {
            comboBox.kendoComboBox({
                dataTextField: "sMayoristaAgrupador", // Campo a mostrar en el ComboBox
                dataValueField: "sIdMayoristaAgrupador", // Valor del ComboBox
                dataSource: data.Data,
                suggest: true, // Sugerir valores mientras se escribe
                // Si deseas un valor por defecto
                value: selectedValue
            });
        },
        error: function (err) {
            console.error("Error al obtener datos de MClubs: ", err);
        }
    });
}


function initComboBoxTipoFarmacia(container, fieldName, selectedValue) {
    var comboBox = container.find("#" + fieldName);

    // Llama a tu API para obtener los datos de MClubs
    $.ajax({
        url: route + "/MTipoFarmacia/ReadMTipoFarmacia", // Cambia esta URL por la correcta de tu API
        type: "GET",
        dataType: "json",
        success: function (data) {
            comboBox.kendoComboBox({
                dataTextField: "sTipoFarmacia", // Campo a mostrar en el ComboBox
                dataValueField: "sTipoFarmacia", // Valor del ComboBox
                dataSource: data.Data,
                suggest: true, // Sugerir valores mientras se escribe
                // Si deseas un valor por defecto
                value: selectedValue
            });
        },
        error: function (err) {
            console.error("Error al obtener datos de MClubs: ", err);
        }
    });
}


function initComboBoxIdTipoFarmacia(container, fieldName, selectedValue) {
    var comboBox = container.find("#" + fieldName);

    // Llama a tu API para obtener los datos de MClubs
    $.ajax({
        url: route + "/MTipoFarmacia/ReadMTipoFarmacia", // Cambia esta URL por la correcta de tu API
        type: "GET",
        dataType: "json",
        success: function (data) {
            comboBox.kendoComboBox({
                dataTextField: "IdTipoFarmacia", // Campo a mostrar en el ComboBox
                dataValueField: "IdTipoFarmacia", // Valor del ComboBox
                dataSource: data.Data,
                suggest: true, // Sugerir valores mientras se escribe
                // Si deseas un valor por defecto
                value: selectedValue
            });
        },
        error: function (err) {
            console.error("Error al obtener datos de MClubs: ", err);
        }
    });
}

function initComboBoxIdTipoSubVenta(container, fieldName, selectedValue) {
    var comboBox = container.find("#" + fieldName);

    // Llama a tu API para obtener los datos de MClubs
    $.ajax({
        url: route + "/MTipoVenta/ReadMTipoVenta", // Cambia esta URL por la correcta de tu API
        type: "GET",
        dataType: "json",
        success: function (data) {
            comboBox.kendoComboBox({
                dataTextField: "IdTipoSubVenta", // Campo a mostrar en el ComboBox
                dataValueField: "IdTipoSubVenta", // Valor del ComboBox
                dataSource: data.Data,
                suggest: true, // Sugerir valores mientras se escribe
                // Si deseas un valor por defecto
                value: selectedValue
            });
        },
        error: function (err) {
            console.error("Error al obtener datos de MClubs: ", err);
        }
    });
}


function initComboBoxsTipoVenta(container, fieldName, selectedValue) {
    var comboBox = container.find("#" + fieldName);

    // Llama a tu API para obtener los datos de MClubs
    $.ajax({
        url: route + "/MTipoVenta/ReadMTipoVenta", // Cambia esta URL por la correcta de tu API
        type: "GET",
        dataType: "json",
        success: function (data) {
            comboBox.kendoComboBox({
                dataTextField: "sTipoVenta", // Campo a mostrar en el ComboBox
                dataValueField: "sTipoVenta", // Valor del ComboBox
                dataSource: data.Data,
                suggest: true, // Sugerir valores mientras se escribe
                // Si deseas un valor por defecto
                value: selectedValue
            });
        },
        error: function (err) {
            console.error("Error al obtener datos de MClubs: ", err);
        }
    });
}


function recargarTabla(gridName) {
    debugger;
    var grid = $("#" + gridName).data("kendoGrid"); // Aseg�rate de agregar el #
    if (grid) {
        grid.dataSource.read(); // Recarga la data del grid
    } else {
        console.error("No se pudo encontrar el grid: " + gridName);
    }
}

function recargarTablaEvento(gridName) {
    debugger;
    var grid = $("#" + gridName).data("kendoGrid"); // Aseg�rate de agregar el #
    if (grid) {
        grid.dataSource.read(); // Recarga la data del grid
    } else {
        console.error("No se pudo encontrar el grid: " + gridName);
    }
}

function onRequestEnd(e, gridName) {

    if (e.type === "create" && e.response) {
        recargarTabla(gridName);
    }
    if (e.type === "destroy" && e.response) {
        if (e.response.success) {
            // Mostrar mensaje de �xito en dialogMessage
            $("#messageDialogTitle").text("Eliminaci�n exitosa");
            $("#messageDialogBody").text(e.response.message);
            $("#dialogMessage").data("kendoDialog").open();
            recargarTabla(gridName);
        } else {
            // Mostrar mensaje de error en dialogError
            $("#divErrorTitle").text("Error al eliminar");
            $("#messageDialogBody").text(e.response.message);
            $("#dialogError").data("kendoDialog").open();
        }
    }
}


    function onEdit(e) {

        var grid = e.sender; 

        var esCreacion = e.model.isNew();
        var window = e.container.data("kendoWindow");
        var updateButton = e.container.find(".k-grid-update");
        updateButton.text("Guardar Cambios");
        e.container.find("label[for='sIdClub']").text("Id Club");

        //M Materiales
        e.container.find("label[for='sIdClub']").text("Id Club");
        e.container.find("label[for='sCodSAPMaterial']").text("Cod Material SAP");
        e.container.find("label[for='dFechaIni']").text("Fecha Ini");
        e.container.find("label[for='dFechaFin']").text("Fecha Fin");
        e.container.find("label[for='sCodNacional']").text("Codigo Nacional");
        e.container.find("label[for='fPrecio']").text("PVL");
        e.container.find("label[for='iIdTipo']").text("Clasificacion Material");
        e.container.find("label[for='iEstado']").text("Estado");
        e.container.find("label[for='sUsuarioActualizacion']").text("Usuario Actualizacion");
        e.container.find("label[for='DescuentoAdelantadoClub']").text("Descuento Adelantado Club");

        e.container.find("label[for='sDescripcionMaterial']").text("Descripcion Material");


        //M AbonoIve
        e.container.find("label[for='sIdAbonoIVA']").text("Id Abono");
        e.container.find("label[for='sAbonoIVA']").text("Descripcion Abono");
        e.container.find("label[for='sIdDivision']").text("Division");
        e.container.find("label[for='sIdRed']").text("Red");
        e.container.find("label[for='fIVA']").text("IVA");
        var fIvaInput = e.container.find("#fIVA").data("kendoNumericTextBox");
        if (!fIvaInput) {
            e.container.find("#fIVA").kendoNumericTextBox({
                format: "#", // Formato para n�meros enteros
                decimals: 0, // No permitir decimales
                spinners: false // Ocultar los botones de incremento/decremento (opcional)
            });
        }
        //M CLub
        e.container.find("label[for='sIdSubClub']").text("Id Sub Club");
        e.container.find("label[for='sIdRed']").text("Red");
        e.container.find("label[for='sNombreSubClub']").text("Nombre Sub Club");
        e.container.find("label[for='iEstado']").text("Estado");
        e.container.find("label[for='sUsuarioActualizacion']").text("Usuario Actualizacion");
        //MTipoDecuento
        e.container.find("label[for='sTipoDescuento']").text("Tipo Descuento");
        e.container.find("label[for='IdTipoDescuento']").text("Id Tipo Descuento");
        e.container.find("label[for='sUsuarioActualizacion']").text("Usuario Actualizacion");
        //MTipoFarmacio
        e.container.find("label[for='IdTipoFarmacia']").text("Id Tipo Farmacia");
        e.container.find("label[for='sTipoFarmacia']").text("Tipo Farmacia");
        //MTipoMateriales
        createDatePicker(e.container, "dFechaIni");
        createDatePicker(e.container, "dFechaFin");
        e.container.find("label[for='bCuentaBaseTramo']").text("Cuenta Base Tramo");
        e.container.find("label[for='bCuentaBaseCalculo']").text("Cuenta Base Calculo");
        e.container.find("label[for='IdTipo']").text("Id Clasificacion Material");
        e.container.find("label[for='sTipo']").text("Clasificacion Material");
        e.container.find("label[for='dFechaIni']").text("Fecha Ini");
        e.container.find("label[for='dFechaFin']").text("Fecha Fin");
        //MTipoTramo
        e.container.find("label[for='IdTipoTramo']").text("Id Tipo Tramo");
        e.container.find("label[for='sTipoTramo']").text("Tipo Tramo");
        //Sabana
        e.container.find("label[for='sIdMayoristaAgrupador']").text("Id Mayorista Agrupador");
        e.container.find("label[for='sMayoristaAgrupador']").text("Mayorista Agrupador");
        e.container.find("label[for='sRed']").text("Red");
        e.container.find("label[for='IdPeriodicidad']").text("Periodicidad");
        e.container.find("label[for='IdTipoVenta']").text("Tipo Venta");
        e.container.find("label[for='idClub']").text("Club");
        e.container.find("label[for='idSubClub']").text("Sub club");
        e.container.find("label[for='IdTramo']").text("Tramo");
        e.container.find("label[for='IdTipoDescuento2']").text("Tipo Descuento");
        //AnexoCambioMateriales
        createDatePicker(e.container, "dFechaInicio");
        e.container.find("label[for='CN_old']").text("Codigo Nacional Old");
        e.container.find("label[for='CN_NEW']").text("Codigo Nacional New");
        e.container.find("label[for='SAP_old']").text("Cod Material SAP Old");
        e.container.find("label[for='SAP_new']").text("Cod Material SAP New");
        e.container.find("label[for='dFechaInicio']").text("Fecha Ini");
        //AnexoClubSocios
        e.container.find("label[for='sIdSocio']").text("Id Socio");
        e.container.find("label[for='iFormaCobro']").text("Forma Cobro");
        //AnexoClubSubClubs
        e.container.find("label[for='sIdMayorista']").text("Id Mayorista");
        e.container.find("label[for='sTipoVenta']").text("Tipo Venta");
        e.container.find("label[for='IdTipoSubVenta']").text("Id Tipo Sub Venta");
        e.container.find("label[for='sZona']").text("Zona");
        e.container.find("label[for='sCooperativa']").text("Cooperativa");
        e.container.find("label[for='sClub']").text("Club");
        e.container.find("label[for='sSubClub']").text("Sub club");
        e.container.find("label[for='sComentarios']").text("Comentarios");
        //AnexoExpositores
        e.container.find("label[for='IdMayorista']").text("Id Mayorista");
        e.container.find("label[for='sCodigoExpositor']").text("Codigo Expositor");
        e.container.find("label[for='sCNOk']").text("Codigo Nacional");
        e.container.find("label[for='sDescripcion']").text("Descripcion");
        e.container.find("label[for='sMayorista']").text("Mayorista");
        //AnexoTipoFarmacias
        e.container.find("label[for='sNIF']").text("NIF");
        e.container.find("label[for='sCliente']").text("Cliente");
        //AnexoTipoVentas
        e.container.find("label[for='sCodigo']").text("Codigo");
        e.container.find("label[for='sNotas']").text("Notas");

        e.container.find("input[type='text'], textarea, select").each(function () {
            var fieldName = $(this).attr("name");
            if (fieldName !== "dFechaActualizacion" && fieldName !== "sUsuarioActualizacion" && fieldName !== "sIdAbonoIVA") {
                $(this).attr("required", "required"); // A�adir el atributo required
                $(this).attr("data-val-required", "Este campo es obligatorio."); // Mensaje de validaci�n personalizado
            }
        });



        var estadoInput = e.container.find("#iEstado").kendoDropDownList({
            dataSource: [
                { valor: 1, texto: "Inactivo" },
                { valor: 0, texto: "Activo" }
            ],
            dataTextField: "texto",
            dataValueField: "valor",
            optionLabel: "Seleccione un valor",
            valuePrimitive: true,
        }).data("kendoDropDownList");
        if (esCreacion) {
            initComboBoxIdClub(e.container, "sIdClub");
            //Combo Sabanas
            initComboBoxIdTramo(e.container, "IdTramo");
            initComboBoxPeriodicidad(e.container, "IdPeriodicidad");
            initComboBoxTipoVenta(e.container, "IdTipoVenta");
            initComboBoxidClub(e.container, "idClub");
            initComboBoxidRed(e.container, "sIdRed");
            initComboBoxidSocio(e.container, "sIdSocio");
            //initComboBoxFormasCobro(e.container, "iFormaCobro");
            initComboBoxidSubClub(e.container, "idSubClub");
            initComboBoxIdTipoDescuento(e.container, "IdTipoDescuento");
            initComboBoxIdTipoDescuento(e.container, "IdTipoDescuento2");
            initComboBoxVW_MAT_LIQ(e.container, "sCodSAPMaterial");
            //AnexoCLubSubClubs
            initComboBoxMMayorista(e.container, "sIdMayorista");
            initComboBoxIdTipoFarmacia(e.container, "IdTipoFarmacia");
            initComboBoxTipoFarmacia(e.container, "sTipoFarmacia");
            initComboBoxidClub(e.container, "sidClub");
            initComboBoxIdTipoSubVenta(e.container, "IdTipoSubVenta");
            initComboBoxsTipoVenta(e.container, "sTipoVenta");
            initComboBoxsIdClub(e.container, "sClub");
            initComboBoxsIdSubClub(e.container, "sSubClub");

            e.container.find("label[for='IdTipoFarmacia']").text("Id Tipo Farmacia");
            e.container.find("label[for='sTipoFarmacia']").text("Tipo Farmacia");

            e.container.find("input[type='text'], textarea, select").val("");
            window.title("Crear Nuevo Registro");
            updateButton.text("Guardar Cambios");
            e.container.find("#dFechaActualizacion").parents(".k-edit-field").hide();
            e.container.find("label[for='dFechaActualizacion']").parents(".k-edit-label").hide();
            var fechaActualizacionInput = e.container.find("#dFechaActualizacion");
            fechaActualizacionInput.val(kendo.toString(new Date(), "yyyy-MM-dd HH:mm:ss.fff")); // Formato de fecha compatible con SQL Server

            var UsuarioActualizacionInput = e.container.find("#sUsuarioActualizacion");
            var nameUser = $("#nameUserHiddenLayout").val();
            UsuarioActualizacionInput.val(nameUser);
            UsuarioActualizacionInput.attr("readonly", true);
            UsuarioActualizacionInput.addClass("k-state-disabled");

        } else {
            initComboBoxIdTramo(e.container, "IdTramo", e.model.IdTramo);
            initComboBoxPeriodicidad(e.container, "IdPeriodicidad", e.model.IdPeriodicidad);
            initComboBoxTipoVenta(e.container, "IdTipoVenta", e.model.IdTipoVenta);
            initComboBoxidClub(e.container, "idClub", e.model.idClub);
            initComboBoxidRed(e.container, "sIdRed", e.model.sIdRed);
            initComboBoxidSubClub(e.container, "idSubClub", e.model.idSubClub);
            initComboBoxIdTipoDescuento(e.container, "IdTipoDescuento", e.model.IdTipoDescuento);
            initComboBoxIdTipoDescuento(e.container, "IdTipoDescuento2", e.model.IdTipoDescuento2);
            initComboBoxVW_MAT_LIQ(e.container, "sCodSAPMaterial", e.model.sCodSAPMaterial);
            //initComboBoxFormasCobro(e.container, "iFormaCobro", e.model.iFormaCobro);

            //anexo club sub clubs
            initComboBoxMMayorista(e.container, "sIdMayorista", e.model.sIdMayorista);
            initComboBoxIdTipoFarmacia(e.container, "IdTipoFarmacia", e.model.IdTipoFarmacia);
            initComboBoxTipoFarmacia(e.container, "sTipoFarmacia", e.model.sTipoFarmacia);
            initComboBoxidClub(e.container, "sidClub", e.model.sidClub);
            initComboBoxIdTipoSubVenta(e.container, "IdTipoSubVenta", e.model.IdTipoSubVenta);
            initComboBoxsTipoVenta(e.container, "sTipoVenta", e.model.sTipoVenta);
            initComboBoxsIdClub(e.container, "sClub", e.model.sClub);
            initComboBoxsIdSubClub(e.container, "sSubClub", e.model.sSubClub);


            // Cambiar el t�tulo de la ventana
            window.title("Editar Registro");
            e.container.find("input:first").parents(".k-edit-field").hide();
            e.container.find("label:first").parents(".k-edit-label").hide();
            e.container.find("#dFechaActualizacion").parents(".k-edit-field").hide();
            e.container.find("label[for='dFechaActualizacion']").parents(".k-edit-label").hide();
            var fechaActualizacionInput = e.container.find("#dFechaActualizacion");
            fechaActualizacionInput.val(kendo.toString(new Date(), "yyyy-MM-dd HH:mm:ss.fff")); // Formato de fecha compatible con SQL Server
            e.container.find("#sUsuarioActualizacion").parents(".k-edit-field").hide();
            e.container.find("label[for='sUsuarioActualizacion']").parents(".k-edit-label").hide();
            var UsuarioActualizacionInput = e.container.find("#sUsuarioActualizacion");
            var nameUser = $("#nameUserHiddenLayout").val();
            UsuarioActualizacionInput.val(nameUser);
        }

    }