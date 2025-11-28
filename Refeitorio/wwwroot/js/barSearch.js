//$(function () {
//    // Only run if we're on the bar page
//    if ($("#productSearch").length === 0) return;

//    const $search = $("#productSearch");
//    const $cards = $(".product-card");
//    const $count = $("#searchCount");
//    const $noRes = $("#noResultsSearch");
//    const $clearBtn = $("#clearSearch");

//    const $categoryFilter = $("#categoryFilter"); // NEW

//    function doSearch() {
//        const term = ($search.val() || "").toString().toLowerCase().trim();

//        const selectedCat = $categoryFilter.val() || ""; // NEW

//        let visible = 0;

//        $cards.each(function () {
//            const name = ($(this).data("name") || "").toString().toLowerCase();
//            const category = ($(this).data("category") || "").toString().toLowerCase();

//            const matchesSearch = !term || name.includes(term) || category.includes(term); // NEW
//            const matchesCategory = !selectedCat || category === selectedCat; // NEW

//            if (matchesSearch && matchesCategory) { // MODIFIED
//                $(this).closest(".col").show();
//                visible++;
//            } else {
//                $(this).closest(".col").hide();
//            }
//        });

//        // Update counter
//        const text = visible === 1 ? "1 produto encontrado" :
//            visible === 0 && term ? "0 produtos" :
//                visible + " produtos encontrados";
//        $count.text(text);

//        // Show/hide no results
//        $noRes.toggleClass("d-none", visible > 0 || (!term && !selectedCat)); // MODIFIED
//    }

//    // Live search + browser clear button
//    $search.off("input.search search.search").on("input.search search.search", doSearch);

//    $categoryFilter.off("change.filter").on("change.filter", doSearch); // NEW

//    // Custom clear button (X)
//    $clearBtn.off("click.search").on("click.search", function () {
//        $search.val("").focus();
//        doSearch();
//    });

//    // Run once on load
//    doSearch();
//});

//// RE-INITIALIZE SEARCH AFTER AJAX LOAD (VER BAR BUTTON)
//$(document).on("click", "#VerBarBtn", function (e) {
//    e.preventDefault();
//    $("#page-content").load("/Home/StudentShowBar", function () {
//        $(function () {
//            if ($("#productSearch").length === 0) return;

//            const $search = $("#productSearch");
//            const $cards = $(".product-card");
//            const $count = $("#searchCount");
//            const $noRes = $("#noResults");
//            const $clearBtn = $("#clearSearch");
//            const $categoryFilter = $("#categoryFilter");

//            function doSearch() {
//                const term = ($search.val() || "").toString().toLowerCase().trim();

//                const selectedCat = $categoryFilter.val() || "";

//                let visible = 0;

//                $cards.each(function () {
//                    const name = ($(this).data("name") || "").toString().toLowerCase();
//                    const category = ($(this).data("category") || "").toString().toLowerCase();

//                    const priceText = $(this).find("h4.text-primary").text().replace(" €", "").trim();
//                    const price = parseFloat(priceText) || 0;

//                    const matchesSearch = !term || name.includes(term) || category.includes(term);
//                    const matchesCategory = !selectedCat || category === selectedCat;

//                    if (matchesSearch && matchesCategory) {
//                        $(this).closest(".col").show();
//                        visible++;
//                    } else {
//                        $(this).closest(".col").hide();
//                    }
//                });

//                const text = visible === 1 ? "1 produto encontrado" :
//                    visible === 0 && term ? "0 produtos" :
//                        visible + " produtos encontrados";
//                $count.text(text);
//                $noRes.toggleClass("d-none", visible > 0 || (!term && !selectedCat));
//            }

//            $search.off("input.search search.search").on("input.search search.search", doSearch);
//            $categoryFilter.off("change.filter").on("change.filter", doSearch);
//            $clearBtn.off("click.search").on("click.search", () => { $search.val("").focus(); doSearch(); });
//            doSearch();
//        });
//    });
//});

// wwwroot/js/student-bar.js
// FULLY WORKING: Search + Category Filter + Price Sort + AJAX Re-init

let initStudentBar = function () {
    // Prevent double initialization
    if ($("#productSearch").length === 0) return;

    const $search = $("#productSearch");
    const $categoryFilter = $("#categoryFilter");
    const $priceSort = $("#priceSort");
    const $clearBtn = $("#clearSearch");
    const $count = $("#searchCount");
    const $noRes = $("#noResultsSearch");
    const $container = $("#productsContainer");

    // ─────────────────────────────────────────────────────
    // 1. FILTER: Search + Category (your original perfect logic)
    // ─────────────────────────────────────────────────────
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

    // ─────────────────────────────────────────────────────
    // 2. SORT: Only price (clean and separate)
    // ─────────────────────────────────────────────────────
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

    // ─────────────────────────────────────────────────────
    // 3. MAIN: Apply filter → sort → update UI
    // ─────────────────────────────────────────────────────
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

    // ─────────────────────────────────────────────────────
    // EVENTS (with proper off() to prevent duplicates)
    // ─────────────────────────────────────────────────────
    $search.off("input.bar").on("input.bar", applyAll);
    $categoryFilter.off("change.bar").on("change.bar", applyAll);
    $priceSort.off("change.bar").on("change.bar", applyAll);
    $clearBtn.off("click.bar").on("click.bar", function () {
        $search.val("").focus();
        applyAll();
    });

    // Run once on init
    applyAll();
};

// ─────────────────────────────────────────────────────
// INITIALIZE ON PAGE LOAD
// ─────────────────────────────────────────────────────
$(document).ready(function () {
    initStudentBar();
});

// ─────────────────────────────────────────────────────
// RE-INITIALIZE AFTER AJAX LOAD (Ver Bar button)
// ─────────────────────────────────────────────────────
$(document).on("click", "#VerBarBtn", function (e) {
    e.preventDefault();
    $("#page-content").load("/Home/StudentShowBar", function () {
        // After AJAX content is loaded → re-initialize everything
        initStudentBar();
    });
});