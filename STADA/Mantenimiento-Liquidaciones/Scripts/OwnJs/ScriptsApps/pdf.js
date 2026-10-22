//funcion para descargar pdf
$(document).on('click', "#downloadPDFOfertaGP", function (ev) {
    var listOH = $("#tablaOfertasHospitales").data("kendoGrid").dataItem("tbody tr:eq(0)")


    if (listOH != null) {
        var idOffer = listOH.idOffer;

        showLoadingIco();
        var link = document.createElement("a");
        link.href = route + "/PDF/PDF?iIdOffer=" + idOffer;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        delete link;
        stopLoadingIco();

    } else {

        $('#messageDialogTitle').html("<p style='font-weight: bold;'>Oferta Hospital :</p>")
        $('#messageDialogImage').html('<img src="' + route + '/Content/img/warning.png" style="width: 72px;height: 72px;">');
        $('#messageDialogBody').html("No se ha podido generar el PDF, debido a que no hay ningun elemento en la tabla.")
        $("#dialogMessage").data("kendoDialog").open();

    }
})