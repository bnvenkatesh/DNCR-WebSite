$(document).ready(function () {
    $(window).resize(function() {
        if ($(window).width() <= 760) {
            var index = $("#table .header-row span").index($(".exterior"));
            if (index > 0) {
                $(".row").each(function() {
                    $(this).find("span.exterior").after($(this).find("span.vehicle"));
                });
            }
        } else {
            var index = $("#table .header-row span").index($(".vehicle"));
            if (index > 0) {
                $(".row").each(function () {
                    $(this).find("span.vehicle").after($(this).find("span.exterior"));
                });
            }
        }
    });

    if($(window).width() <= 760) {
        var index = $("#table .header-row span").index($(".exterior"));
        if (index > 0) {
            $(".row").each(function() {
                $(this).find("span.exterior").after($(this).find("span.vehicle"));
            });
        }
    }
});