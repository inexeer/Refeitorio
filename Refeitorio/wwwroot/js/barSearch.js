$(function () {
    // Only run if we're on the bar page
    if ($("#productSearch").length === 0) return;

    const $search = $("#productSearch");
    const $cards = $(".product-card");
    const $count = $("#searchCount");
    const $noRes = $("#noResultsSearch");
    const $clearBtn = $("#clearSearch");

    const $categoryFilter = $("#categoryFilter"); // NEW

    function doSearch() {
        const term = ($search.val() || "").toString().toLowerCase().trim();

        const selectedCat = $categoryFilter.val() || ""; // NEW

        let visible = 0;

        $cards.each(function () {
            const name = ($(this).data("name") || "").toString().toLowerCase();
            const category = ($(this).data("category") || "").toString().toLowerCase();

            const matchesSearch = !term || name.includes(term) || category.includes(term); // NEW
            const matchesCategory = !selectedCat || category === selectedCat; // NEW

            if (matchesSearch && matchesCategory) { // MODIFIED
                $(this).closest(".col").show();
                visible++;
            } else {
                $(this).closest(".col").hide();
            }
        });

        // Update counter
        const text = visible === 1 ? "1 produto encontrado" :
            visible === 0 && term ? "0 produtos" :
                visible + " produtos encontrados";
        $count.text(text);

        // Show/hide no results
        $noRes.toggleClass("d-none", visible > 0 || (!term && !selectedCat)); // MODIFIED
    }

    // Live search + browser clear button
    $search.off("input.search search.search").on("input.search search.search", doSearch);

    $categoryFilter.off("change.filter").on("change.filter", doSearch); // NEW

    // Custom clear button (X)
    $clearBtn.off("click.search").on("click.search", function () {
        $search.val("").focus();
        doSearch();
    });

    // Run once on load
    doSearch();
});

// RE-INITIALIZE SEARCH AFTER AJAX LOAD (VER BAR BUTTON)
$(document).on("click", "#VerBarBtn", function (e) {
    e.preventDefault();
    $("#page-content").load("/Home/StudentShowBar", function () {
        $(function () {
            if ($("#productSearch").length === 0) return;

            const $search = $("#productSearch");
            const $cards = $(".product-card");
            const $count = $("#searchCount");
            const $noRes = $("#noResults");
            const $clearBtn = $("#clearSearch");
            const $categoryFilter = $("#categoryFilter"); // NEW

            function doSearch() {
                const term = ($search.val() || "").toString().toLowerCase().trim();

                const selectedCat = $categoryFilter.val() || ""; // NEW

                let visible = 0;

                $cards.each(function () {
                    const name = ($(this).data("name") || "").toString().toLowerCase();
                    const category = ($(this).data("category") || "").toString().toLowerCase();

                    const matchesSearch = !term || name.includes(term) || category.includes(term); // NEW
                    const matchesCategory = !selectedCat || category === selectedCat; // NEW

                    if (matchesSearch && matchesCategory) {
                        $(this).closest(".col").show();
                        visible++;
                    } else {
                        $(this).closest(".col").hide();
                    }
                });

                const text = visible === 1 ? "1 produto encontrado" :
                    visible === 0 && term ? "0 produtos" :
                        visible + " produtos encontrados";
                $count.text(text);
                $noRes.toggleClass("d-none", visible > 0 || (!term && !selectedCat)); // MODIFIED
            }

            $search.off("input.search search.search").on("input.search search.search", doSearch);
            $categoryFilter.off("change.filter").on("change.filter", doSearch); // NEW
            $clearBtn.off("click.search").on("click.search", () => { $search.val("").focus(); doSearch(); });
            doSearch();
        });
    });
});