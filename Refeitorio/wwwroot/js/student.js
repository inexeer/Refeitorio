let studentCurrentDate = new Date();
let studentSavedLunches = {};
let myBookings = {};
$(document).ready(function () {
    $.get("/Home/GetSavedLunches", function (data) {
        studentSavedLunches = data;
        renderStudentCalendar();
    }).fail(function () {
        console.error("Erro ao ler o json");
        renderStudentCalendar();
    });

    $.get("/Home/GetMyBookings", function (data) {
        myBookings = data || {};
    }).fail(function () {
        console.error("Erro ao carregar reservas do utilizador");
    });
});

function renderStudentCalendar() {
    const year = studentCurrentDate.getFullYear();
    const month = studentCurrentDate.getMonth();
    const firstDay = new Date(year, month, 1);
    const lastDay = new Date(year, month + 1, 0);
    const monthName = studentCurrentDate.toLocaleString("default", { month: "long" });
    const weekdays = ["Dom", "Seg", "Ter", "Qua", "Qui", "Sex", "Sab"];

    $("#calendarTitle").text(`${monthName} ${year}`);

    let html = "";

    // Empty cells before first day
    for (let i = 0; i < firstDay.getDay(); i++) {
        html += `<div class="col-1 p-2"></div>`;
    }

    for (let day = 1; day <= lastDay.getDate(); day++) {
        const cellDate = new Date(year, month, day);
        const dateKey = `${year}-${String(month + 1).padStart(2, '0')}-${String(day).padStart(2, '0')}`;
        const menu = studentSavedLunches[dateKey];
        const weekdayIndex = cellDate.getDay();
        const isWeekend = weekdayIndex === 0 || weekdayIndex === 6;

        const todayOnly = new Date();
        todayOnly.setHours(0, 0, 0, 0);
        const cellOnly = new Date(year, month, day);
        const isToday = cellOnly.getTime() === todayOnly.getTime();
        const isFuture = cellOnly > todayOnly;
        const before10am = new Date().getHours() < 10;

        const canBook = !isWeekend && ((isToday && before10am) || isFuture);


        const alreadyBooked = myBookings[dateKey] !== undefined;
        let content = `<div class="fw-bold">${day}</div>`;
        content += `<small class="text-muted">${weekdays[weekdayIndex]}</small>`;

        if (menu) {
            const type = menu.option === "Vegetariano" ? "Veg" : "Normal";
            const badgeClass = menu.option === "Vegetariano" ? "bg-success" : "bg-primary";
            content += `
            <div class="mt-2 text-center">
                <span class="badge ${badgeClass} mb-1">${type}</span>
                <div class="small fw-bold">${menu.mainDish || "Sem prato"}</div>
                <div class="text-muted small">${menu.soup || ""}</div>
                <div class="text-muted small">${menu.dessert || ""}</div>
            </div>`;

            if (alreadyBooked) {
                content += `<div class="mt-2 text-success fw-bold">Marcado</div>`;
            } else if (canBook) {
                content += `
                    <button class="btn btn-sm btn-outline-success mt-2 marcar-btn"
                            data-date="${dateKey}">
                        Marcar
                    </button>`;
            } else {
                content += `<small class="text-muted mt-1 d-block">Indisponível</small>`;
            }
        } else if (!isWeekend) {
            if (cellOnly < todayOnly) {
                content += `<small class="text-muted mt-1 d-block">Indisponível</small>`;
            } else {
                content += `<small class="text-danger mt-2 d-block">Sem menu</small>`;
            }
        }

        const dayClass = isToday ? "bg-light border-primary border-2" : "";

        html += `
        <div class="col-1 border p-2 text-center dayCell ${dayClass}">
            ${content}
        </div>`;
    }

    $("#studentCalendarGrid").html(html);
}

$(document).on("click", ".marcar-btn", function () {
    const date = $(this).data("date");
    const menu = studentSavedLunches[date];

    if (!menu) return;

    if (confirm(`Marcar refeição para ${date}?\n${menu.mainDish} (${menu.option === "Vegetariano" ? "Veg" : "Normal"})`)) {
        $.post("/Home/BookLunch", { date: date }, function (res) {
            if (res.success) {
                myBookings[date] = menu.option;     // Update local cache
                renderStudentCalendar();            // Refresh view → shows "Marcado"
                alert("Refeição marcada com sucesso!");
            } else {
                alert("Erro: " + (res.message || "Não foi possível marcar"));
            }
        }).fail(function () {
            alert("Erro de comunicação com o servidor");
        });
    }
});

$(document).on("click", "#prevMonth", function () {
    studentCurrentDate.setMonth(studentCurrentDate.getMonth() - 1);
    renderStudentCalendar();
});

$(document).on("click", "#nextMonth", function () {
    studentCurrentDate.setMonth(studentCurrentDate.getMonth() + 1);
    renderStudentCalendar();
});



$(document).on("click", "#VerMenuBtn", function (e) {
    e.preventDefault();

    // First: load the partial
    $.get("/Home/StudentMenuCalendar", function (data) {
        $("#page-content").html(data);

        // THEN: load lunches and render — with proper chaining
        $.get("/Home/GetSavedLunches")
            .done(function (lunches) {
                studentSavedLunches = lunches;
                renderStudentCalendar();  // ← This will now have real data
            })
            .fail(function () {
                studentSavedLunches = {};
                renderStudentCalendar();
            });
    });
});

$(document).on("click", "#SaldoBtn", function (e) {
    e.preventDefault();
    $.get("/Home/StudentSaldo", function (data) {
        $("#page-content").html(data);
    });
});

$(document).on("click", "#VerHistoricoBtn", function (e) {
    e.preventDefault();

    $.get("/Home/StudentBookingHistory", function (data) {
        $("#page-content").html(data);
    });
});

$(document).on("click", "#VerBarBtn", function (e) {
    e.preventDefault();

    $.get("/Home/StudentShowBar", function (data) {
        $("#page-content").html(data);
    });
});

$(document).on("click", "#VerHistoricoBarBtn", function (e) {
    e.preventDefault();

    $.get("/Home/StudentBarHistory", function (data) {
        $("#page-content").html(data);
    });
});

document.addEventListener('click', function (e) {
    const target = e.target;

    // Botão MENOS
    if (target.classList.contains('qty-minus') || target.closest('.qty-minus')) {
        const controls = target.closest('.qty-controls');
        if (!controls) return;
        const input = controls.querySelector('.qty-input');
        let val = parseInt(input.value) || 1;
        if (val > 1) {
            input.value = val - 1;
        }
    }

    // Botão MAIS
    if (target.classList.contains('qty-plus') || target.closest('.qty-plus')) {
        const controls = target.closest('.qty-controls');
        if (!controls) return;
        const input = controls.querySelector('.qty-input');
        let val = parseInt(input.value) || 1;
        const max = parseInt(input.dataset.max) || 999;
        if (val < max) {
            input.value = val + 1;
        }
    }
});

$(document).on("click", ".ver-detalhes-btn", function () {
    const productId = $(this).data("id");
    $("#page-content").load("/Home/GetProductDetails?id=" + productId);
});

$(document).on("click", ".comprar-btn", function () {
    const btn = $(this);
    if (btn.prop("disabled")) return;

    const productId = btn.data("id");
    const qtyInput = btn.closest(".card-body").find(".qty-input");
    const quantity = qtyInput.length > 0 ? parseInt(qtyInput.val()) || 1 : 1;

    // Optional: disable button during request
    btn.prop("disabled", true).html('<span class="spinner-border spinner-border-sm"></span>');

    $.post("/Home/BuyProduct", { productId: productId, quantity: quantity })
        .done(function (res) {
            if (res.success) {
                // ONLY UPDATE UI IF PURCHASE WAS SUCCESSFUL
                alert(res.message);

                // Update saldo
                $("#currentSaldo, #navbarSaldo").text(res.newSaldo.toFixed(2) + " €");

                // ONLY UPDATE STOCK IF SUCCESS
                if (res.newStock !== undefined) {
                    const cardBody = btn.closest(".card-body");
                    const stockSpan = cardBody.find("span").filter(function () {
                        return $(this).text().includes("unid.") || $(this).text().includes("esgotado");
                    });

                    if (res.newStock > 0) {
                        stockSpan.text(res.newStock + " unid.")
                            .removeClass("text-danger")
                            .addClass("text-success");
                    } else {
                        stockSpan.text("Produto esgotado")
                            .removeClass("text-success")
                            .addClass("text-danger fw-bold");

                        cardBody.find(".qty-controls").remove();
                        btn.prop("disabled", true).text("Esgotado");

                        if (cardBody.find(".text-danger.text-center.small").length === 0) {
                            btn.closest(".d-flex").after(
                                '<small class="text-danger text-center mt-2 d-block">Produto esgotado</small>'
                            );
                        }
                    }
                }
            } else {
                alert("Erro: " + res.message);
            }
        })
        .fail(function () {
            alert("Compra falhou. Tenta novamente.");
        })
        .always(function () {
            // Re-enable button
            btn.prop("disabled", false).html("Comprar");
        });
});

$(document).on("submit", "#topupForm", function (e) {
    e.preventDefault();

    const amount = parseFloat($("#topupAmount").val()) || 0;
    if (amount < 1) {
        $("#topupMessage").html('<div class="alert alert-danger">Valor mínimo: 1.00 €</div>');
        return;
    }

    const $btn = $(this).find("button[type=submit]");
    $btn.prop("disabled", true).html('<span class="spinner-border spinner-border-sm"></span> A carregar...');

    $.post("/Home/TopUpSaldo", { amount: amount })
        .done(function (res) {
            if (res.success) {
                $("#currentSaldo").text(res.newSaldo.toFixed(2) + " €");
                $("#navbarSaldo").text(res.newSaldo.toFixed(2) + " €");
                $("#topupMessage").html(
                    `<div class="alert alert-success">
                        <strong>Sucesso!</strong> +${amount.toFixed(2)} € adicionados!
                     </div>`
                );
                setTimeout(() => $("#topupMessage").empty(), 4000);
            } else {
                $("#topupMessage").html(`<div class="alert alert-danger">${res.message}</div>`);
            }
        })
        .fail(function () {
            $("#topupMessage").html('<div class="alert alert-danger">Erro de ligação.</div>');
        })
        .always(function () {
            $btn.prop("disabled", false).html('Carregar');
        });
});

