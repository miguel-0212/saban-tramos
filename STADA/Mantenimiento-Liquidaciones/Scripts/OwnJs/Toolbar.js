//funcion que se ejecuta cuando se pulsa al boton de recargar una tabla
$(document).on('click', ".refreshBtnContainer", function (ev) {
    var grid = $("#" + $('.tableGridKendo').attr("data-idTable"));
    grid.find(".k-grid-toolbar").on("click", ".k-pager-refresh", function (e) {
        e.preventDefault();
        grid.data("kendoGrid").dataSource.read();
    });
})

//funcion que se ejecuta cuando se pulsa al boton de ocultar/mostrar paginación
$(document).on('click', ".enableDisabledPagination", function (ev) {
    showLoadingIco();
    var grid = $("#" + $('.tableGridKendo').attr("data-idTable")).data("kendoGrid");

    var type = $(this).find('i').attr("data-type");

    if (type == "pagination") {
        $(this).find('i').attr("data-type", "withoutPagination");
        $(this).find('i').attr("class", $(this).find('i').attr("data-uprow"))
        grid.dataSource.pageSize(grid.dataSource.total());
    } else {
        $(this).find('i').attr("data-type", "pagination");
        $(this).find('i').attr("class", $(this).find('i').attr("data-downrow"))
        grid.dataSource.pageSize(10);
    }
    resizeTableKendo();
    stopLoadingIco();
})
