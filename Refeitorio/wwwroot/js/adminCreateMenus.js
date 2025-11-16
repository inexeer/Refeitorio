//let currentDate = new Date(); // today

//$(document).ready(function () {
//    renderCalendar();

//    $("#prevMonth").click(function () {
//        currentDate.setMonth(currentDate.getMonth() - 1);
//        renderCalendar();
//    });

//    $("#nextMonth").click(function () {
//        currentDate.setMonth(currentDate.getMonth() + 1);
//        renderCalendar();
//    });
//});

//function renderCalendar() {
//    const year = currentDate.getFullYear();
//    const month = currentDate.getMonth();

//    const firstDay = new Date(year, month, 1);
//    const lastDay = new Date(year, month + 1, 0);

//    const monthName = currentDate.toLocaleString('default', { month: 'long' });
//    $("#calendarTitle").text(`${monthName} ${year}`);

//    let html = "";

//    // Blank cells until first weekday
//    for (let i = 0; i < firstDay.getDay(); i++) {
//        html += `<div class="col-1 border p-2 text-muted"></div>`;
//    }

//    // Days of the month
//    for (let day = 1; day <= lastDay.getDate(); day++) {
//        html += `
//            <div class="col-1 border p-2 text-center dayCell" data-day="${day}">
//                ${day}
//            </div>
//        `;
//    }

//    $("#calendarGrid").html(html);

//    // add click event to each day cell
//    $(".dayCell").click(function () {
//        const selectedDay = $(this).data("day");
//        alert("Selected: " + selectedDay + "/" + (month + 1) + "/" + year);
//    });
//}

let currentDate = new Date(); // Global variable to track current date

// Function to render the calendar
function renderCalendar() {
    const year = currentDate.getFullYear();
    const month = currentDate.getMonth();
    const firstDay = new Date(year, month, 1);
    const lastDay = new Date(year, month + 1, 0);
    const monthName = currentDate.toLocaleString('default', { month: 'long' });

    const weekdays = ['Domingo', 'Segunda', 'Terça', 'Quarta', 'Quinta', 'Sexta', 'Sabado']

    const lunchOptions = ['Lunch A', 'Lunch B', 'Lunch C'];

    $("#calendarTitle").text(`${monthName} ${year}`);
    let html = "";

    // Blank cells until first weekday
    for (let i = 0; i < firstDay.getDay(); i++) {
        html += `<div class="col-1 border p-2 text-muted">${weekdays[i]}</div>`;
    }

    // Days of the month
    for (let day = 1; day <= lastDay.getDate(); day++) {
        const weekdayIndex = (firstDay.getDay() + day - 1) % 7;
        const isWeekday = weekdayIndex >= 1 && weekdayIndex <= 5;

        html += `
            <div class="col-1 border p-2 text-center dayCell d-flex flex-column align-items-center" data-day="${day}">
                <div>${day}</div>
                <div>${weekdays[weekdayIndex]}</div>
                ${isWeekday ? '<button class="btn btn-sm btn-primary mt-1 select-lunch" data-day="' + day + '">Select Lunch</button>' : ''}
            </div>
        `;
    }
    $("#calendarGrid").html(html);

    // Add click event to each day cell
    $(".dayCell").off("click").on("click", function () {
        const selectedDay = $(this).data("day");
        alert("Selected: " + selectedDay + "/" + (month + 1) + "/" + year);
    });

    // Add click event for lunch selection buttons
    $(".select-lunch").off("click").on("click", function (e) {
        e.stopPropagation(); // Prevent triggering dayCell click
        const day = $(this).data("day");
        // Placeholder action: show available lunches (replace with modal/form later)
        alert("Select lunch for " + day + "/" + (month + 1) + "/" + year + ": " + lunchOptions.join(", "));
    });
}

// Event delegation for navigation buttons
$(document).on("click", "#prevMonth", function () {
    currentDate.setMonth(currentDate.getMonth() - 1);
    renderCalendar();
});

$(document).on("click", "#nextMonth", function () {
    currentDate.setMonth(currentDate.getMonth() + 1);
    renderCalendar();
});
