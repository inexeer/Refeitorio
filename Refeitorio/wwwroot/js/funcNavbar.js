$(function () {
    $("#CriarProdutosBtn").click(function (e) {
        e.preventDefault();

        $.get("/Home/FuncCreateProduct", function (data) {
            $("#page-content").html(data);
        });
    });

    $(document).on("click", ".edit-product-btn", function (e) {
        e.preventDefault();
        var productId = $(this).data("id");

        $.get("/Home/FuncEditProduct?id=" + productId, function (data) {
            $("#page-content").html(data);
        });
    });
});