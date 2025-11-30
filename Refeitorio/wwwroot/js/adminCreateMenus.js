let currentDate = new Date(); // Tracks the visible month
let savedLunches = {};
$(document).ready(function () {
    $.get("/Home/GetSavedLunches", function (data) {
        savedLunches = data;
        renderCalendar();
    }).fail(function () {
        console.error("Erro ao ler o json");
        renderCalendar();
    });
});

function renderCalendar() {
    const year = currentDate.getFullYear();
    const month = currentDate.getMonth();

    const firstDay = new Date(year, month, 1);
    const lastDay = new Date(year, month + 1, 0);

    const monthName = currentDate.toLocaleString("default", { month: "long" });
    const weekdays = ["Dom", "Seg", "Ter", "Qua", "Qui", "Sex", "Sab"];

    const today = new Date();
    const todayMidnight = new Date(today.getFullYear(), today.getMonth(), today.getDate());

    $("#calendarTitle").text(`${monthName} ${year}`);

    let html = "";


    for (let i = 0; i < firstDay.getDay(); i++) {
        html += `<div class="col p-1"></div>`;
    }

    for (let day = 1; day <= lastDay.getDate(); day++) {
        const cellDate = new Date(year, month, day);
        cellDate.setHours(0, 0, 0, 0);
        const weekdayIndex = cellDate.getDay();
        const isWeekday = weekdayIndex >= 1 && weekdayIndex <= 5; 
        const isWeekend = !isWeekday;

        const isFutureOrToday = cellDate >= todayMidnight;


        const dateKey = `${year}-${String(month + 1).padStart(2, '0')}-${String(day).padStart(2, '0')}`;
        const menusForDay = savedLunches[dateKey] || {};


        const cellStyle = `
            background: ${cellDate.getTime() === todayMidnight.getTime() ? '#f3fbfc' : '#fff'};
            width:138px; min-width:120px; max-width:153px; min-height: 134px;
            border-radius: 18px; box-shadow: 0 2px 8px 0 rgba(60,60,90,0.07);
            border: 1.2px solid #eaedf3;
            margin-bottom: 7px; padding:14px 6px 12px 6px;
            display:flex; flex-direction:column; align-items:center; justify-content:flex-start;
            transition: box-shadow 0.13s;
        `;

        let content = `
            <div style="font-weight:700; font-size:1.18rem; margin-bottom:.22rem;">${day}</div>
            <div class="small text-muted mb-2" style="letter-spacing:.5px;">${weekdays[weekdayIndex]}</div>
        `;

        if (Object.keys(menusForDay).length > 0) {

            content += `<div class="d-flex flex-column align-items-center gap-1 mb-2">`;
            if (menusForDay.Normal)
                content += `<span class="badge bg-primary mt-1 px-3 py-1 rounded-pill" style="font-size:.97rem;">Normal</span>`;
            if (menusForDay.Vegetariano)
                content += `<span class="badge bg-success mb-1 px-3 py-1 rounded-pill" style="font-size:.97rem;">Vegetariano</span>`;
            content += `</div>`;

            if (!isWeekend && isFutureOrToday) {
                content += `
                    <button class="btn btn-warning btn-sm fw-semibold select-lunch px-3 py-1 rounded-pill mt-1" 
                            data-day="${day}" style="font-size:.99rem;">Alterar menu</button>
                `;
            }
        } else if (isWeekday && isFutureOrToday) {

            content += `
                <button class="btn btn-primary btn-sm fw-semibold select-lunch px-3 py-1 rounded-pill mt-2" 
                        data-day="${day}" style="font-size:.99rem;">Selecionar menu</button>
            `;
        } else if (isWeekday) {

            content += `<div class="mt-3 small text-danger fw-semibold" style="font-size:1rem;">Sem menu</div>`;
        }

        html += `
        <div class="col p-1" style="max-width:165px;">
            <div class="admin-calendar-card" style="${cellStyle}">
                ${content}
            </div>
        </div>`;
    }

    $("#calendarGrid").html(html);


    $(".select-lunch").off("click").on("click", function (e) {
        e.preventDefault();
        e.stopPropagation();
        const day = $(this).data("day");
        const year = currentDate.getFullYear();
        const month = currentDate.getMonth() + 1;
        const dateString = `${year}-${String(month).padStart(2, '0')}-${String(day).padStart(2, '0')}`;

        $.get("/Home/AdminSelectLunchByDate", { date: dateString }, function (data) {
            $("#page-content").html(data);
        });
    });
}

$(document).on("click", "#prevMonth", function () {
    currentDate.setMonth(currentDate.getMonth() - 1);
    renderCalendar();
});

$(document).on("click", "#nextMonth", function () {
    currentDate.setMonth(currentDate.getMonth() + 1);
    renderCalendar();
});