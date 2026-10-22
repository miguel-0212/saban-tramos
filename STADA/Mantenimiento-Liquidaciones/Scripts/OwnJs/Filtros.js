var maxHeightToolbar = 0;
var minHeightToolbar = 0;


//funcion que carga los filtros cuando se termina de cargar la pantalla de index
function chargeFilters(){
    var item;
    if ($("#chargeFilters").val() == "true") {

        $("input.filterClassSaveData").each(function (index) {
            item = $($("input.filterClassSaveData")[index])
            if (sessionStorage.getItem(item.attr("id")) != null) {
                if (item.attr("type") == "checkbox") {
                    item.attr("checked", sessionStorage.getItem(item.attr("id")));
                } else {
                    item.val(sessionStorage.getItem(item.attr("id")));
                }
            }
        });

        $(".filterClassSaveData").not('input').not('span').each(function (index) {
            item = $($(".filterClassSaveData").not('input').not('span')[index])
            if (sessionStorage.getItem(item.attr("id")) != null) {
                putvalueItemKendo(item.attr("id"), item.attr("data-typeKendo"), sessionStorage.getItem(item.attr("id")));
            }
        });

        var idTable = $("#chargeFilters").attr("data-idTable")
        var buttonRefresh = $("#chargeFilters").attr("data-buttonRefresh")
        var checkExist = setInterval(function () {
            if ($('#' + idTable).parent('.fixed-table-body').length = 1) {
                $('#' + buttonRefresh).click();
                isLoadPageIndex = true;
                clearInterval(checkExist);
            }
        }, 100); // check every 100ms
    }
}

$(document).on('click', ".saveSessionStorageFilters", function (ev) {

    var id, value;
    $("input.filterClassSaveData").each(function (index) {
        item = $($("input.filterClassSaveData")[index])

        value = getValueOfInput(item);
        id = item.attr("id");

        if (item.attr("type") == "checkbox") {
            sessionStorage.setItem(id, item.is(":checked"));
        } else {
            sessionStorage.setItem(id, item.val());
        }
    });


    $(".filterClassSaveData").not('input').not('span').each(function (index) {
        item = $($(".filterClassSaveData").not('input').not('span')[index])

        value = getValueOfInputKendo(item.attr("id"), item.attr("data-typeKendo"));
        id = item.attr("id");

        sessionStorage.setItem(id, value);
    });
})

$(document).on('click', ".removeSessionStorageFilters", function (ev) {
    $(".filterClassSaveData").each(function (index) {
        item = $($(".filterClassSaveData")[index])
        if (sessionStorage.getItem(item.attr("id")) != null) {
            sessionStorage.removeItem(item.attr("id"));
        }
    });
})

function onCollapseFilters(e) {
    if (maxHeightToolbar == 0) {
        maxHeightToolbar = $('.divFiltros').height();
    }
    resizeTableKendoByFilters('collapse', $(e.item.parentElement).attr("data-idTable"));
}

function onExpanseFilters(e) {
    if (minHeightToolbar == 0) {
        minHeightToolbar = $('.divFiltros').height();
    }
    resizeTableKendoByFilters('expanse', $(e.item.parentElement).attr("data-idTable"));
}

function putvalueItemKendo(id, typeKendo, valItem) {
    switch (typeKendo) {
        case 'DropDownList':
            $('#' + id).data("kendoDropDownList").value(valItem);
            break;
        case 'RadioGroup':
            $('#' + id).data("kendoRadioGroup").value(valItem);
            break;
    }
}

function getValueOfInput(item) {
    return item.val();
}

function getValueOfInputKendo(id, typeKendo) {
    var value;
    switch (typeKendo) {
        case 'DropDownList':
            value=$('#' + id).data("kendoDropDownList").value();
            break;
        case 'RadioGroup':
            value=$('#' + id).data("kendoRadioGroup").value();
            break;
    }
    return value;
}