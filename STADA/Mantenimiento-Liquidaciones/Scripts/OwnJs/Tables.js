//funcion para recargar las tablas con los datos de los filtros
$(document).on('click', ".filterDataTableWithFilters", function (ev) {
    showLoadingIco();

    var idTableData = $(this).attr("data-idTable");

    var idTable = "";

    if (idTableData != "" && idTableData != null) {
        idTable = idTableData
    } else {
        idTable = $(this).closest('.jumbotron').find('.tableKendoClass').attr("id")
    }

    $("#" + idTable).data("kendoGrid").dataSource.read();
    stopLoadingIco();
})

//funcion para eliminar todos los filtros y recargar la tabla
$(document).on('click', ".removeDataTableWithFilters", function (ev) {
    showLoadingIco();

    var idTableData = $(this).attr("data-idTable");

    var idTable = "";

    if (idTableData != "" && idTableData != null) {
        idTable = idTableData
    } else {
        idTable = $(this).closest('.jumbotron').find('.tableKendoClass').attr("id")
    }

    var listFilters = $('input.filterInputTable');

    for (var i = 0; i <= listFilters.length; i++) {
        if ($(listFilters[i]).hasClass('kendoClass')) {
            setDefaultValueKendoInputs($($('input.filterInputTable')[i]).attr('id'), $($('input.filterInputTable')[i]).attr('data-typeKendo'));
        } else {
            if ($(listFilters[i]).attr("type") == "checkbox") {
                $(listFilters[i]).prop('checked', false);
            } else {
                $(listFilters[i]).val("");
            }
        }
    }

    $("#" + idTable).data("kendoGrid").dataSource.read();
    stopLoadingIco();
})

function setDefaultValueKendoInputs(id, type) {
    switch (type) {
        case 'DropDownList':
            $('#' + id).data("kendoDropDownList").value(0);
            break;
    }
}