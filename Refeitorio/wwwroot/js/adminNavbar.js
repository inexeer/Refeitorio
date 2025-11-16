$(function () {
    $("#pedidosBtn").click(function (e) {
        e.preventDefault();

        $.get("/Home/AdminPendingUsers", function (data) {
            $("#page-content").html(data);
        });
    });

    $("#gerirMenusBtn").click(function (e) {
        e.preventDefault();

        $.get("/Home/AdminCreateMenus", function (data) {
            $("#page-content").html(data);

            if (typeof renderCalendar === "function") {
                renderCalendar();
            }
        });
    });

    $("#criarMenuBtn").click(function (e) {
        e.preventDefault();

        $.get("/Home/AdminCreateLunch", function (data) {
            $("#page-content").html(data);
        });
    });
});