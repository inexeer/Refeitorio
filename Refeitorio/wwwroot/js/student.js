

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

    for (let i = 0; i < firstDay.getDay(); i++) {
        html += `<div class="col p-1"></div>`;
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

        const cellStyle = `
            background: ${isToday ? '#f7faff' : '#fff'};
            width:138px; min-width:120px; max-width:153px; min-height: 130px;
            border-radius: 18px; box-shadow: 0 2px 8px 0 rgba(60,60,90,0.07);
            border: 1.2px solid ${isToday ? '#50baff33' : '#edf0f2'};
            margin-bottom: 7px; padding:14px 6px 12px 6px;
            display:flex; flex-direction:column; align-items:center; justify-content:flex-start;
            transition: box-shadow 0.13s;
        `;

        let content = `
            <div style="font-weight:700; font-size:1.18rem; margin-bottom:.22rem;">${day}</div>
            <div class="small text-muted mb-2" style="letter-spacing:.5px;">${weekdays[weekdayIndex]}</div>
        `;

        if (menu && (menu.Normal || menu.Vegetariano)) {
            content += `<div class="d-flex flex-column align-items-center gap-1 mb-2">`;
            if (menu.Normal) content += `<span class="badge bg-primary mt-1 px-3 py-1 rounded-pill" style="font-size:.97rem;">Normal</span>`;
            if (menu.Vegetariano) content += `<span class="badge bg-success mb-1 px-3 py-1 rounded-pill" style="font-size:.97rem;">Vegetariano</span>`;
            content += `</div>`;

            content += `<div class="mt-1 text-center w-100" style="min-height:2rem;">`;
            if (alreadyBooked) {
                content += `<span class="badge rounded-pill bg-success-subtle text-success px-3 py-2" style="font-size:1.06rem; background:#e8fbe8 !important; color:#14ad3b !important; display:inline-block;">Marcado</span>`;
            } else if (canBook) {
                content += `<button class="btn btn-outline-success btn-sm fw-semibold marcar-btn px-3 py-1 mt-1 rounded-pill" style="font-size:1.02rem;min-width:75px;" data-date="${dateKey}">Marcar</button>`;
            }
            content += `</div>`;
        } else if (!isWeekend) {
            content += `<div class="mt-3 small text-danger fw-semibold" style="font-size:1rem;">Sem menu</div>`;
        }

        html += `
        <div class="col p-1" style="max-width:165px;">
            <div class="student-calendar-card" style="${cellStyle}">
                ${content}
            </div>
        </div>`;
    }

    $("#studentCalendarGrid").html(html);
}

$(document).on("click", ".marcar-btn", function () {
    const date = $(this).data("date");
    const menu = studentSavedLunches[date];

    $("#bookingModal").remove();

    function buildCardHtml(type) {
        const menuObj = menu && menu[type];
        const imageSrc = "/images/menu-placeholder.png";
        if (menuObj) {
            return `
                <div class="booking-card card h-100 shadow-sm border-0">
                    <div class="booking-card-img">
                        <img src="${escapeHtml(imageSrc)}" alt="${escapeHtml(menuObj.mainDish || type)}" onerror="this.onerror=null;this.parentElement.classList.add('no-img');this.style.display='none'">
                        <div class="no-img-label d-none">Sem imagem</div>
                    </div>
                    <div class="card-body d-flex flex-column">
                        <h5 class="card-title mb-1">${type}</h5>
                        <ul class="list-unstyled small mb-3">
                            <li><strong>Prato:</strong> ${escapeHtml(menuObj.mainDish || "")}</li>
                            <li><strong>Sopa:</strong> ${escapeHtml(menuObj.soup || "")}</li>
                            <li><strong>Sobremesa:</strong> ${escapeHtml(menuObj.dessert || "")}</li>
                        </ul>
                        <div class="mt-auto d-flex gap-2">
                            <button class="btn btn-success btn-sm flex-fill confirm-book" data-date="${date}" data-type="${type}">Marcar ${type}</button>
                        </div>
                    </div>
                </div>
            `;
        } else {
            return `
                <div class="booking-card card h-100 border-0 disabled-card">
                    <div class="booking-card-img no-img">
                        <div class="no-img-placeholder">Sem imagem</div>
                    </div>
                    <div class="card-body d-flex flex-column text-center">
                        <h5 class="card-title text-muted mb-2">${type}</h5>
                        <p class="text-muted small mb-3">Sem menu disponível para este dia</p>
                        <div class="mt-auto">
                            <button class="btn btn-outline-secondary btn-sm" disabled>Indisponível</button>
                        </div>
                    </div>
                </div>
            `;
        }
    }

    const modalHtml = `
    <div id="bookingModal" class="booking-modal">
        <div id="bookingModalBackdrop" class="booking-modal-backdrop"></div>
        <div class="booking-modal-dialog">
            <div class="card shadow-lg">
                <div class="card-header d-flex align-items-center justify-content-between">
                    <div>
                        <h5 class="mb-0">Escolhe a opção — <small class="text-muted">${date}</small></h5>
                    </div>
                    <div>
                        <button class="btn btn-sm btn-light" id="bookingModalClose" aria-label="Fechar">✕</button>
                    </div>
                </div>
                <div class="card-body">
                    <div class="row g-3">
                        <div class="col-md-6">${buildCardHtml("Normal")}</div>
                        <div class="col-md-6">${buildCardHtml("Vegetariano")}</div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    `;

    $("body").append(modalHtml);

    $("#bookingModal img").each(function () {
        $(this).on("load", function () {
        }).on("error", function () {
            $(this).hide();
        });
    });

    $("#bookingModalClose, #bookingModalBackdrop").on("click", function () {
        $("#bookingModal").remove();
    });

    $(document).on("keydown.bookingModal", function (e) {
        if (e.key === "Escape") {
            $("#bookingModal").remove();
            $(document).off("keydown.bookingModal");
        }
    });
});

function escapeHtml(text) {
    if (!text) return "";
    return text.toString().replace(/[&<>"']/g, function (m) {
        return ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' })[m];
    });
}

$(document).on("click", ".confirm-book", function () {
    const date = $(this).data("date");
    const type = $(this).data("type");

    $(".confirm-book, .cancel-book").prop("disabled", true);

    $.post("/Home/BookLunch", { date: date, type: type })
        .done(function (res) {
            if (res.success) {
                if (studentSavedLunches[date] && studentSavedLunches[date][type]) {
                    myBookings[date] = type;
                }
                renderStudentCalendar();
                $("#bookingModal").remove();
                alert("Refeição marcada com sucesso!");
            } else {
                alert("Erro: " + (res.message || "Não foi possível marcar"));
                $(".confirm-book, .cancel-book").prop("disabled", false);
            }
        })
        .fail(function () {
            alert("Erro de comunicação com o servidor");
            $(".confirm-book, .cancel-book").prop("disabled", false);
        });
});

$(document).on("click", ".cancel-book", function () {
    $("#bookingModal").remove();
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

    $.get("/Home/StudentMenuCalendar", function (data) {
        $("#page-content").html(data);

        $.get("/Home/GetSavedLunches")
            .done(function (lunches) {
                studentSavedLunches = lunches;
                renderStudentCalendar();
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

    if (target.classList.contains('qty-minus') || target.closest('.qty-minus')) {
        const controls = target.closest('.qty-controls');
        if (!controls) return;
        const input = controls.querySelector('.qty-input');
        let val = parseInt(input.value) || 1;
        if (val > 1) {
            input.value = val - 1;
        }
    }

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

    btn.prop("disabled", true).html('<span class="spinner-border spinner-border-sm"></span>');

    $.post("/Home/BuyProduct", { productId: productId, quantity: quantity })
        .done(function (res) {
            if (res.success) {
                alert(res.message);

                $("#currentSaldo, #navbarSaldo").text(res.newSaldo.toFixed(2) + " €");

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

