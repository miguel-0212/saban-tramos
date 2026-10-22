var countShowLoading = 0;
function showLoadingIco() {
    $('#loadingICon').removeClass();
    $('#loader-primary').kendoLoader({ themeColor: 'primary' }).data("kendoLoader");
    $('#loadingICon').addClass("iconLoagingShow");
    $('#mainBody').addClass("disabledBody");
    countShowLoading++;
}

function stopLoadingIco() {
    countShowLoading--;
    if (countShowLoading <= 0) {
        $('#loadingICon').removeClass();
        $('#loader-primary').html("");
        $('#loadingICon').addClass("iconLoagingHide");
        $('#mainBody').removeClass("disabledBody");
    }
}