$(function () {
    if (window.innerWidth > window.innerHeight) {
        $('#groupShower').collapse('show')
    }
});

function doSearch() {
    //var currentSearchStr = getQueryVariable('search');
    var currentSearchStr = encodeURIComponent($('#input_search').val());
    var currentGroup = getQueryVariable('group');
    if (currentGroup == false) currentGroup = '0';
    var currentPage = getQueryVariable('p');
    if (currentPage == false) currentPage = '0';

    window.location.href = "/main?p=" + currentPage + "&group=" + currentGroup + "&search=" + currentSearchStr;
}

$(function () {
    $('#input_search').on('keydown', function (e) {
        if (e.originalEvent.keyCode == 13) {
            doSearch();
        }
    });
});