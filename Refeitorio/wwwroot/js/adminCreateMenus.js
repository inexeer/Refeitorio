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

    // Add blank cells until the first weekday
    for (let i = 0; i < firstDay.getDay(); i++) {
        html += `<div class="col-1 p-2"></div>`;
    }

    // Generate calendar days
    for (let day = 1; day <= lastDay.getDate(); day++) {
        const cellDate = new Date(year, month, day);
        const cellMidnight = new Date(year, month, day);  // Clean copy at midnight
        cellMidnight.setHours(0, 0, 0, 0);

        const weekdayIndex = cellDate.getDay();
        const isWeekday = weekdayIndex >= 1 && weekdayIndex <= 5; // Monday–Friday

        // Conditions to allow booking
        const isFuture = cellMidnight >= todayMidnight;
        const isToday = cellMidnight.getTime() === todayMidnight.getTime();
        const isBefore10 = today.getHours() < 10;

        const canBook =
            isWeekday &&
            (
                (isToday && isBefore10) ||      // Today and before 10:00
                (isFuture && !isToday)          // Any future weekday
            );

        let lunchInfo = "";
        const dateKey = `${year}-${String(month + 1).padStart(2, '0')}-${String(day).padStart(2, '0')}`;

        if (savedLunches[dateKey]) {
            const menu = savedLunches[dateKey];
            const type = menu.option === "Vegetariano" ? "Veg" : "Normal";
            lunchInfo = `
                <div class="text-center small">
                    <div class="fw-bold text-success">${type}: ${menu.mainDish}</div>
                    <small class="text-muted">${menu.soup}</small>
                </div>`;
        }

        html += `
            <div class="col-1 border p-2 text-center dayCell d-flex flex-column align-items-center" data-day="${day}">
                <div class="fw-bold">${day}</div>
                <div class="small text-muted">${weekdays[weekdayIndex]}</div>
        
                ${savedLunches[dateKey] ? lunchInfo : (
                        canBook
                            ? `<button class="btn btn-sm btn-primary mt-2 select-lunch" data-day="${day}">Selecinar menu</button>`
                            : (isWeekday ? `<small class="text-danger mt-2">Indisponível</small>` : "")
                    )}
            </div>
        `;
    }

    $("#calendarGrid").html(html);

    // Click select lunch button
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

