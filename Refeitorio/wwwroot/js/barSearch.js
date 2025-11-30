let initStudentBar = function () {

    if ($("#productSearch").length === 0) return;

    const $search = $("#productSearch");
    const $categoryFilter = $("#categoryFilter");
    const $priceSort = $("#priceSort");
    const $clearBtn = $("#clearSearch");
    const $count = $("#searchCount");
    const $noRes = $("#noResultsSearch");
    const $container = $("#productsContainer");

    function filterProducts() {
        const term = $search.val().toString().toLowerCase().trim();
        const selectedCat = $categoryFilter.val() || "";

        let visible = 0;

        $(".product-card").each(function () {
            const name = ($(this).data("name") || "").toString().toLowerCase();
            const category = ($(this).data("category") || "").toString().toLowerCase();

            const matchesSearch = !term || name.includes(term) || category.includes(term);
            const matchesCategory = !selectedCat || category === selectedCat;

            if (matchesSearch && matchesCategory) {
                $(this).closest(".col").show();
                visible++;
            } else {
                $(this).closest(".col").hide();
            }
        });

        return visible;
    }


    function sortProducts() {
        const sortValue = $priceSort.val() || "";
        if (!sortValue) return;

        const $visibleCols = $container.children(".col:visible");

        const sortedCols = $visibleCols.get().sort(function (a, b) {
            const priceA = parseFloat($(a).find("h4.text-primary").text().replace(" €", "").trim()) || 0;
            const priceB = parseFloat($(b).find("h4.text-primary").text().replace(" €", "").trim()) || 0;
            return sortValue === "low-high" ? priceA - priceB : priceB - priceA;
        });

        $container.append(sortedCols);
    }


    function applyAll() {
        const visible = filterProducts();
        sortProducts();

        const term = $search.val().toString().trim();
        const selectedCat = $categoryFilter.val() || "";
        const sortValue = $priceSort.val() || "";

        let text = "";
        if (visible === $(".product-card").length && !term && !selectedCat && !sortValue) {
            text = `${visible} produtos`;
        } else if (visible === 1) {
            text = "1 produto encontrado";
        } else if (visible === 0) {
            text = "0 produtos";
        } else {
            text = `${visible} produtos encontrados`;
        }

        $count.text(text);
        $noRes.toggleClass("d-none", visible > 0 || (!term && !selectedCat));
    }

    $search.off("input.bar").on("input.bar", applyAll);
    $categoryFilter.off("change.bar").on("change.bar", applyAll);
    $priceSort.off("change.bar").on("change.bar", applyAll);
    $clearBtn.off("click.bar").on("click.bar", function () {
        $search.val("").focus();
        applyAll();
    });


    applyAll();
};


$(document).ready(function () {
    initStudentBar();
});


$(document).on("click", "#VerBarBtn", function (e) {
    e.preventDefault();
    $("#page-content").load("/Home/StudentShowBar", function () {

        initStudentBar();
    });
});