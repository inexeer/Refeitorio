//let currentDate = new Date(); // Global variable to track current date

//// Function to render the calendar
//function renderCalendar() {
//    const year = currentDate.getFullYear();
//    const month = currentDate.getMonth();
//    const firstDay = new Date(year, month, 1);
//    const lastDay = new Date(year, month + 1, 0);
//    const monthName = currentDate.toLocaleString('default', { month: 'long' });

//    const weekdays = ['Domingo', 'Segunda', 'Terça', 'Quarta', 'Quinta', 'Sexta', 'Sabado']

//    const lunchOptions = ['Lunch A', 'Lunch B', 'Lunch C'];

//    $("#calendarTitle").text(`${monthName} ${year}`);
//    let html = "";

//    // Blank cells until first weekday
//    for (let i = 0; i < firstDay.getDay(); i++) {
//        html += `<div class="col-1 border p-2 text-muted">${weekdays[i]}</div>`;
//    }

//    // Days of the month
//    const now = new Date();
//    const todayMidnight = new Date(now.getFullYear(), now.getMonth(), now.getDate());

//    // Loop through all days of the month
//    for (let day = 1; day <= lastDay.getDate(); day++) {
//        const weekdayIndex = (firstDay.getDay() + day - 1) % 7;
//        const isWeekday = weekdayIndex >= 1 && weekdayIndex <= 5; // Mon–Fri

//        // Date of this cell
//        const cellDate = new Date(year, month, day);
//        const cellMidnight = new Date(year, month, day);
//        cellMidnight.setHours(0, 0, 0, 0);

//        // NEW: Conditions to show the button
//        const isPast = cellMidnight < todayMidnight;                                    // Already passed
//        const isToday = cellMidnight.getTime() === todayMidnight.getTime();
//        const isBefore10AM = isToday && now.getHours() < 10;

//        const canBook = isWeekday && !isPast && (isToday ? isBefore10AM : true);
//        // → Button only appears on weekdays that are today (before 10:00) or in the future

//        html += `
//            <div class="col-1 border p-2 text-center dayCell d-flex flex-column align-items-center position-relative" data-day="${day}">
//                <div class="fw-bold">${day}</div>
//                <div class="small text-muted">${weekdays[weekdayIndex]}</div>
//                ${canBook
//                ? `<button class="btn btn-sm btn-primary mt-2 select-lunch" data-day="${day}">Marcar Almoço</button>`
//                : (isWeekday
//                    ? `<small class="text-danger mt-2">Indisponível</small>`
//                    : '')
//            }
//            </div>
//        `;
//    }
//    $("#calendarGrid").html(html);

//    // Add click event to each day cell
//    $(".dayCell").off("click").on("click", function () {
//        const selectedDay = $(this).data("day");
//        alert("Selected: " + selectedDay + "/" + (month + 1) + "/" + year);
//    });

//    // Add click event for lunch selection buttons
//    $(".select-lunch").off("click").on("click", function (e) {
//        e.stopPropagation(); // Prevent triggering dayCell click
//        const day = $(this).data("day");
//        // Placeholder action: show available lunches (replace with modal/form later)
//        alert("Select lunch for " + day + "/" + (month + 1) + "/" + year + ": " + lunchOptions.join(", "));
//    });
//}

//// Event delegation for navigation buttons
//$(document).on("click", "#prevMonth", function () {
//    currentDate.setMonth(currentDate.getMonth() - 1);
//    renderCalendar();
//});

//$(document).on("click", "#nextMonth", function () {
//    currentDate.setMonth(currentDate.getMonth() + 1);
//    renderCalendar();
//});

let currentDate = new Date(); // Tracks the visible month

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

        html += `
            <div class="col-1 border p-2 text-center dayCell d-flex flex-column align-items-center"
                 data-day="${day}">
                 
                <div class="fw-bold">${day}</div>
                <div class="small text-muted">${weekdays[weekdayIndex]}</div>

                ${canBook
                ? `<button class="btn btn-sm btn-primary mt-2 select-lunch" data-day="${day}">
                        Marcar Almoço
                       </button>`
                : (isWeekday ? `<small class="text-danger mt-2">Indisponível</small>` : "")
            }
            </div>
        `;
    }

    $("#calendarGrid").html(html);

    // Click select lunch button
    $(".select-lunch").off("click").on("click", function (e) {
        e.stopPropagation();
        const day = $(this).data("day");
        alert(`Select lunch for ${day}/${month + 1}/${year}`);
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

