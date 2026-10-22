
$(document).on('click', ".sidebar-dropdown > a", function (ev) {
    var isGlobalAction = $(this).parent().hasClass("globalAction");

    if (isGlobalAction) {
        $(".sidebar-submenu").slideUp(200);
    } else {
        $(".sidebar-submenu.subAction").slideUp(200);
    }

    if ($(this).parent().hasClass("active")) {
        $(".sidebar-dropdown").removeClass("active");
        $(this).parent().removeClass("active");
    } else {
        $(".sidebar-dropdown").removeClass("active");
        $(this).next(".sidebar-submenu").slideDown(200);
        $(this).parent().addClass("active");
    }
});

$(document).on('click', "#close-sidebar", function (ev) {
    $(".page-wrapper").removeClass("toggled");
});

$(document).on('click', "#show-sidebar", function (ev) {
    $(".page-wrapper").addClass("toggled");
});

//funcion que se ejecuta cuando se hace click a cualquier submenu del layout
$(document).on('click', ".buttonLayoutURL", function (ev) {
    showLoadingIco();
    sessionStorage.setItem("textMenu", $(this).text());
    var url = $(this).attr('data-url');
    location.replace(route + "/" + url);
})