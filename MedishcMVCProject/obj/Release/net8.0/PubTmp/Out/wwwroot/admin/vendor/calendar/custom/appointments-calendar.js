document.addEventListener("DOMContentLoaded", function () {
    var calendarEl = document.getElementById("appointmentsCal");

    var calendar = new FullCalendar.Calendar(calendarEl, {
        headerToolbar: {
            left: "prevYear,prev,next,nextYear today",
            center: "title",
            right: "dayGridMonth,dayGridWeek,dayGridDay",
        },
        initialDate: new Date().toISOString().split('T')[0],
        initialView: "dayGridMonth",
        navLinks: true,
        editable: false,
        dayMaxEvents: true,
        displayEventTime: false,
        eventDisplay: 'block',

        events: function (fetchInfo, successCallback, failureCallback) {
            fetch('/Appointments/GetCalendarEvents')
                .then(response => response.json())
                .then(data => {
                    const styledEvents = data.map(e => ({
                        title: e.title,
                        start: e.start,
                        url: e.url,
                        textColor: "#116aef",
                        color: "#ffffff",
                        borderColor: "#469ED8"
                    }));
                    successCallback(styledEvents);
                })
                .catch(error => failureCallback(error));
        },

        eventClick: function (info) {
            info.jsEvent.preventDefault();
            if (info.event.url) {
                window.location.href = info.event.url;
            }
        },

        eventDidMount: function (info) {
            info.el.innerHTML = info.event.title.replace(/\n/g, "<br>");
        }
    });

    calendar.today();
});










//document.addEventListener("DOMContentLoaded", function () {
//  var calendarEl = document.getElementById("appointmentsCal");
//  var calendar = new FullCalendar.Calendar(calendarEl, {
//    headerToolbar: {
//      left: "prevYear,prev,next,nextYear today",
//      center: "title",
//      right: "dayGridMonth,dayGridWeek,dayGridDay",
//    },
//    initialDate: "2024-05-10",
//    navLinks: true, // can click day/week names to navigate views
//    editable: true,
//    dayMaxEvents: true, // allow "more" link when too many events
//    events: [
//      {
//        title: "5 Appointments",
//        url: "appointments-list.html",
//        start: "2024-05-01",
//        textColor: "#116aef",
//        color: "#ffffff",
//        borderColor: "#469ED8",
//      },
//      {
//        title: "9 Appointments",
//        url: "appointments-list.html",
//        start: "2024-05-02",
//        textColor: "#116aef",
//        color: "#ffffff",
//        borderColor: "#469ED8",
//      },
//      {
//        title: "12 Appointments",
//        url: "appointments-list.html",
//        start: "2024-05-03",
//        textColor: "#116aef",
//        color: "#ffffff",
//        borderColor: "#469ED8",
//      },
//      {
//        title: "9 Appointments",
//        url: "appointments-list.html",
//        start: "2024-05-04",
//        textColor: "#116aef",
//        color: "#ffffff",
//        borderColor: "#469ED8",
//      },
//      {
//        title: "7 Appointments",
//        url: "appointments-list.html",
//        start: "2024-05-05",
//        textColor: "#116aef",
//        color: "#ffffff",
//        borderColor: "#469ED8",
//      },
//      {
//        title: "16 Appointments",
//        url: "appointments-list.html",
//        start: "2024-05-06",
//        textColor: "#116aef",
//        color: "#ffffff",
//        borderColor: "#469ED8",
//      },
//      {
//        title: "9 Appointments",
//        url: "appointments-list.html",
//        start: "2024-05-07",
//        textColor: "#116aef",
//        color: "#ffffff",
//        borderColor: "#469ED8",
//      },
//      {
//        title: "13 Appointments",
//        url: "appointments-list.html",
//        start: "2024-05-08",
//        textColor: "#116aef",
//        color: "#ffffff",
//        borderColor: "#469ED8",
//      },
//      {
//        title: "20 Appointments",
//        url: "appointments-list.html",
//        start: "2024-05-09",
//        textColor: "#116aef",
//        color: "#ffffff",
//        borderColor: "#469ED8",
//      },
//      {
//        title: "11 Appointments",
//        url: "appointments-list.html",
//        start: "2024-05-10",
//        textColor: "#116aef",
//        color: "#ffffff",
//        borderColor: "#469ED8",
//      },
//      {
//        title: "3 Appointments",
//        url: "appointments-list.html",
//        start: "2024-05-11",
//        textColor: "#116aef",
//        color: "#ffffff",
//        borderColor: "#469ED8"
//      },
//      {
//        title: "6 Appointments",
//        url: "appointments-list.html",
//        start: "2024-05-12",
//        textColor: "#116aef",
//        color: "#ffffff",
//        borderColor: "#469ED8",
//      },
//      {
//        title: "18 Appointments",
//        url: "appointments-list.html",
//        start: "2024-05-13",
//        textColor: "#116aef",
//        color: "#ffffff",
//        borderColor: "#469ED8",
//      },
//      {
//        title: "4 Appointments",
//        url: "appointments-list.html",
//        start: "2024-05-14",
//        textColor: "#116aef",
//        color: "#ffffff",
//        borderColor: "#469ED8",
//      },
//      {
//        title: "5 Appointments",
//        url: "appointments-list.html",
//        start: "2024-05-15",
//        textColor: "#116aef",
//        color: "#ffffff",
//        borderColor: "#469ED8",
//      },
//      {
//        title: "5 Appointments",
//        url: "appointments-list.html",
//        start: "2024-05-16",
//        textColor: "#116aef",
//        color: "#ffffff",
//        borderColor: "#469ED8",
//      },
//      {
//        title: "9 Appointments",
//        url: "appointments-list.html",
//        start: "2024-05-17",
//        textColor: "#116aef",
//        color: "#ffffff",
//        borderColor: "#469ED8",
//      },
//      {
//        title: "12 Appointments",
//        url: "appointments-list.html",
//        start: "2024-05-18",
//        textColor: "#116aef",
//        color: "#ffffff",
//        borderColor: "#469ED8",
//      },
//      {
//        title: "9 Appointments",
//        url: "appointments-list.html",
//        start: "2024-05-19",
//        textColor: "#116aef",
//        color: "#ffffff",
//        borderColor: "#469ED8",
//      },
//      {
//        title: "7 Appointments",
//        url: "appointments-list.html",
//        start: "2024-05-20",
//        textColor: "#116aef",
//        color: "#ffffff",
//        borderColor: "#469ED8",
//      },
//      {
//        title: "16 Appointments",
//        url: "appointments-list.html",
//        start: "2024-05-21",
//        textColor: "#116aef",
//        color: "#ffffff",
//        borderColor: "#469ED8",
//      },
//      {
//        title: "9 Appointments",
//        url: "appointments-list.html",
//        start: "2024-05-22",
//        textColor: "#116aef",
//        color: "#ffffff",
//        borderColor: "#469ED8",
//      },
//      {
//        title: "13 Appointments",
//        url: "appointments-list.html",
//        start: "2024-05-23",
//        textColor: "#116aef",
//        color: "#ffffff",
//        borderColor: "#469ED8",
//      },
//      {
//        title: "20 Appointments",
//        url: "appointments-list.html",
//        start: "2024-05-24",
//        textColor: "#116aef",
//        color: "#ffffff",
//        borderColor: "#469ED8",
//      },
//      {
//        title: "11 Appointments",
//        url: "appointments-list.html",
//        start: "2024-05-25",
//        textColor: "#116aef",
//        color: "#ffffff",
//        borderColor: "#469ED8",
//      },
//      {
//        title: "3 Appointments",
//        url: "appointments-list.html",
//        start: "2024-05-26",
//        textColor: "#116aef",
//        color: "#ffffff",
//        borderColor: "#469ED8",
//      },
//      {
//        title: "6 Appointments",
//        url: "appointments-list.html",
//        start: "2024-05-27",
//        textColor: "#116aef",
//        color: "#ffffff",
//        borderColor: "#469ED8",
//      },
//      {
//        title: "18 Appointments",
//        url: "appointments-list.html",
//        start: "2024-05-28",
//        textColor: "#116aef",
//        color: "#ffffff",
//        borderColor: "#469ED8",
//      },
//      {
//        title: "4 Appointments",
//        url: "appointments-list.html",
//        start: "2024-05-29",
//        textColor: "#116aef",
//        color: "#ffffff",
//        borderColor: "#469ED8",
//      },
//      {
//        title: "5 Appointments",
//        url: "appointments-list.html",
//        start: "2024-05-30",
//        textColor: "#116aef",
//        color: "#ffffff",
//        borderColor: "#469ED8",
//      },
//      {
//        title: "8 Appointments",
//        url: "appointments-list.html",
//        start: "2024-05-31",
//        textColor: "#116aef",
//        color: "#ffffff",
//        borderColor: "#469ED8",
//      },
//    ],
//  });

//  calendar.render();
//});







