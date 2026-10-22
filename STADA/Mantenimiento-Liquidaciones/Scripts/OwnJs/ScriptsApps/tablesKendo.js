function additionalDataIndexMAbono_IVA() {
    return {
        sAbonoIVA: $("#sAbonoIVA").val(),
        sIdDivision: $("#sIdDivision").val(),
        sIdRed: $("#sIdRed").val(),
    }
}

function additionalDataIndexMTipoVenta() {
    // Obtén los valores de los filtros desde los campos de entrada
    var sTipoSubVentaFilter = $("#sTipoSubVenta").val();
    var sTipoVentaFilter = $("#sTipoVenta").val();

    // Construye el objeto de datos adicionales
    var additionalData = {
        sTipoSubVenta: sTipoSubVentaFilter,
        sTipoVenta: sTipoVentaFilter
    };

    return additionalData;
}

function additionalDataIndexAnexoTipoVenta() {
    
    return {
        idMayorista: $("#idMayorista").val(),
        idTipoSubVenta: $("#idTipoSubVenta").val(),
        sNotas: $("#sNotas").val(),
    }
}

function additionalDataIndexMSocios() {

    return {
        sIdSocio: $("#sIdSocio").val(),
        sCodSAP: $("#sCodSAP").val(),
        sNIF: $("#sNIF").val(),
        sNombreFcia: $("#sNombreFcia").val(),
        sDireccion: $("#sDireccion").val(),
        sCP: $("#sCP").val(),
        sPoblacion: $("#sPoblacion").val(),
        dFechaActualizacion: $("#dFechaActualizacion").val(),

    }
}

function additionalDataIndexMMateriales() {

    return {
        sIdClub: $("#sIdClub").val(),
        sCodNacional: $("#sCodNacional").val(),
        sCodSAPMaterial: $("#sCodSAPMaterial").val(),
        dFechaIni: $("#dFechaIni").val(),
        dFechaFin: $("#dFechaFin").val(),
    }
}

function additionalDataIndexAnexoTipoFarmacia() {

    return {
        sIdMayorista: $("#sIdMayorista").val(),
        sNIF: $("#sNIF").val(),
        IdTipoFarmacia: $("#IdTipoFarmacia").val(),
    }
}

function additionalDataIndexAnexoExpositores() {

    return {
        sCodigoExpositor: $("#sCodigoExpositor").val(),
        sCNOk: $("#sCNOk").val(),
        IdMayorista: $("#IdMayorista").val(),
    }
}

function additionalDataIndexAnexoClubSubClub() {

    return {
        sIdMayorista: $("#sIdMayorista").val(),
        sRed: $("#sRed").val(),
        IdTipoFarmacia: $("#IdTipoFarmacia").val(),
        sTipoVenta: $("#sTipoVenta").val(),
        sZona: $("#sZona").val(),
        IdTipoSubVenta: $("#IdTipoSubVenta").val(),
        dFechaIni: $("#dFechaIni").val(),
        dFechaFin: $("#dFechaFin").val(),
    }
}

function additionalDataIndexMClub() {

    return {
        sIdRed: $("#sIdRed").val(),
        sIdClub: $("#sIdClub").val(),
        sNombreClub: $("#sNombreClub").val(),
    }
}

function additionalDataIndexMFormasCobro() {

    return {
        sFormaCobro: $("#sFormaCobro").val(),

    }
}

function additionalDataIndexMMayorista() {

    return {
        IdTipoFarmaciaPorDefecto: $("#IdTipoFarmaciaPorDefecto").val(),
        sMayoristaAgrupador: $("#sMayoristaAgrupador").val(),
    }
}

function additionalDataIndexMPeriodicidad() {

    return {
        sPeriodicidad: $("#sPeriodicidad").val(),

    }
}

function additionalDataIndexMSubClub() {

    return {
        sNombreSubClub: $("#sNombreSubClub").val(),

    }
}



function additionalDataIndexMTipoDescuento() {

    return {
        sTipoDescuento: $("#sTipoDescuento").val(),

    }
}

function additionalDataIndexMTipoFarmacia() {

    return {
        IdTipoFarmacia: $("#IdTipoFarmacia").val(),
        sTipoFarmacia: $("#sTipoFarmacia").val(),
    }
}
function additionalDataIndexMTipoMaterial() {

    return {
        IdTipo: $("#IdTipo").val(),
        sTipo: $("#sTipo").val(),
        sIdRed: $("#sIdRed").val(),
    }
}

function additionalDataIndexMTipoTramo() {

    return {
        sTipoTramo: $("#sTipoTramo").val(),

    }
}

function additionalDataIndexAnexoCambioMaterial() {

    return {
        CN_old: $("#CN_old").val(),
        CN_NEW: $("#CN_NEW").val(),
        SAP_old: $("#SAP_old").val(),
        SAP_new: $("#SAP_new").val(),
    }
}

function additionalDataIndexAnexoClubSocios() {

    return {
        sIdClub: $("#sIdClub").val(),
        sIdRed: $("#sIdRed").val(),
        iFormaCobro: $("#iFormaCobro").val(),
    }
}

function additionalDataIndexSabana() {

    return {
        sMayoristaAgrupador: $("#sMayoristaAgrupador").val(),
        sRed: $("#sRed").val(),
        IdTipoVenta: $("#IdTipoVenta").val(),
        idClub: $("#idClub").val(),
        idSubClub: $("#idSubClub").val(),

    }
}

