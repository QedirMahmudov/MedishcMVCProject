using MedishcMVCProject.DAL;
using MedishcMVCProject.Models;
using MedishcMVCProject.Utilities;
using MedishcMVCProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MedishcMVCProject.Areas.admin.Controllers
{
    [Area("Admin")]
    public class AppointmentController : Controller
    {
        private readonly AppDbContext _context;

        public AppointmentController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Appointments()
        {
            return View();
        }
        public async Task<IActionResult> List()
        {
            DateTime now = DateTime.Now;
            DateTime today = now.Date;
            TimeSpan currentTime = now.TimeOfDay;

            List<Appointment> expiredAppointments = await _context.Appointments
                 .Where(a => !a.IsDeleted &&
                            (a.Date.Date < today ||
                            (a.Date.Date == today && a.Time < currentTime)))
                 .ToListAsync();

            foreach (var item in expiredAppointments)
            {
                item.IsDeleted = true;
            }

            if (expiredAppointments.Any())
                await _context.SaveChangesAsync();

            List<GetAppointmentVM> appointments = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.Specialist)
                .Where(a => !a.IsDeleted)
                .OrderBy(a => a.Date)
                .ThenBy(a => a.Time)
                .Select(a => new GetAppointmentVM
                {
                    Id = a.Id,
                    PatientId = a.PatientId,
                    PatientName = a.Patient.Name + " " + a.Patient.Surname,
                    DoctorId = a.DoctorId,
                    DoctorName = a.Doctor.Name + " " + a.Doctor.Surname,
                    Department = a.Doctor.Specialist.Name,
                    Date = a.Date,
                    Time = a.Time.ToString(@"hh\:mm"),
                    Disease = a.Description,
                    DoctorImage = a.Doctor.Image,
                    PatientImage = a.Patient.Image
                })
                .ToListAsync();

            return View(appointments);
        }

        [HttpGet]
        public IActionResult GetDoctorsBySpecialist(int specialistId)
        {
            List<SelectListItem>? doctors = _context.Doctors
                .Where(d => d.SpecialistId == specialistId)
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Name + " " + d.Surname
                }).ToList();

            return Json(doctors);
        }


        [HttpGet]
        public async Task<IActionResult> GetAvailableTimes(int doctorId, DateTime date)
        {
            DayOfWeekEnum dayOfWeek = (DayOfWeekEnum)date.DayOfWeek;

            WorkingHours? workingHour = await _context.WorkingHours
                .FirstOrDefaultAsync(x => x.DoctorId == doctorId && x.DayOfWeek == dayOfWeek);

            if (workingHour == null || !workingHour.OpenTime.HasValue || !workingHour.CloseTime.HasValue)
            {
                return Json(new List<string>());
            }

            var timeSlots = new List<string>();
            var time = workingHour.OpenTime.Value;

            while (time <= workingHour.CloseTime.Value)
            {
                timeSlots.Add(time.ToString(@"hh\:mm"));
                time = time.Add(TimeSpan.FromMinutes(30));
            }

            var bookedTimes = await _context.Appointments
                .Where(a => a.DoctorId == doctorId && a.Date.Date == date.Date)
                .Select(a => a.Time.ToString(@"hh\:mm"))
                .ToListAsync();

            var availableSlots = timeSlots.Except(bookedTimes).ToList();

            return Json(availableSlots);
        }


        public IActionResult GetCalendarEvents()
        {
            DateTime today = DateTime.Today;

            List<Appointment> appointments = _context.Appointments
                .Include(x => x.Patient)
                .Where(x => x.Date.Date >= today)
                .ToList();

            var events = appointments.Select(x => new
            {
                id = x.Id,
                title = $"{x.Time:hh\\:mm} - {x.Patient.Name} {x.Patient.Surname}",
                start = x.Date.ToString("yyyy-MM-dd"),
                url = $"/Appointments/Details/{x.Id}"
            });

            return Json(events);
        }

        [HttpPost]
        public IActionResult UpdateDate([FromBody] AppointmentUpdateDTO model)
        {
            Appointment? appointment = _context.Appointments
                .Include(a => a.Patient)
                .FirstOrDefault(a => a.Id == model.AppointmentId);

            if (appointment == null)
                return Json(new { success = false, message = "Appointment tapılmadı." });

            bool isConflict = _context.Appointments.Any(a =>
                a.Id != model.AppointmentId &&
                a.DoctorId == appointment.DoctorId &&
                a.Date.Date == DateTime.Parse(model.NewDate).Date &&
                a.Time == appointment.Time);

            if (isConflict)
                return Json(new { success = false, message = "There is already an appointment for this time on this date." });

            appointment.Date = DateTime.Parse(model.NewDate);
            _context.SaveChanges();

            return Json(new { success = true });
        }


        public IActionResult Create()
        {
            CreateAppointmentVM appointmentVM = new CreateAppointmentVM
            {
                Specialists = _context.Specialists
                    .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Name }).ToList(),
                Doctors = new List<SelectListItem>()
            };

            return View(appointmentVM);
        }


        [HttpPost]
        public async Task<IActionResult> Create(CreateAppointmentVM appointmentVM)
        {
            if (appointmentVM.Date.Date < DateTime.Today)
            {
                ModelState.AddModelError(nameof(appointmentVM.Date), "Appointment date cannot be in the past.");
            }
            else
            {
                DayOfWeekEnum selectedDay = (DayOfWeekEnum)appointmentVM.Date.DayOfWeek;

                var doctorWorkingHour = await _context.WorkingHours
                    .FirstOrDefaultAsync(h => h.DoctorId == appointmentVM.DoctorId && h.DayOfWeek == selectedDay);

                if (doctorWorkingHour == null)
                {
                    ModelState.AddModelError("", "The selected doctor does not work on this day.");
                }
                else if (!TimeSpan.TryParse(appointmentVM.Time, out var selectedTime))
                {
                    ModelState.AddModelError(nameof(appointmentVM.Time), "Invalid time format.");
                }
                else if (selectedTime < doctorWorkingHour.OpenTime || selectedTime > doctorWorkingHour.CloseTime)
                {
                    string start = doctorWorkingHour.OpenTime.Value.ToString(@"hh\:mm");
                    string end = doctorWorkingHour.CloseTime.Value.ToString(@"hh\:mm");
                    ModelState.AddModelError(nameof(appointmentVM.Time), $"The doctor is available only between {start} and {end}.");
                }
                else
                {
                    bool isBooked = await _context.Appointments.AnyAsync(a =>
                        a.DoctorId == appointmentVM.DoctorId &&
                        a.Date == appointmentVM.Date &&
                        a.Time == selectedTime);

                    if (isBooked)
                    {
                        ModelState.AddModelError(nameof(appointmentVM.Time), "The selected time is already booked.");
                    }
                }
            }

            ContactInfo? contact = await _context.ContactInfos
                .FirstOrDefaultAsync(x => x.ContactType == ContactType.Email &&
                                          x.Value == appointmentVM.Email &&
                                          x.OwnerType == OwnerType.Patient);

            if (contact == null)
            {
                ModelState.AddModelError(nameof(appointmentVM.Email), "No patient found with the provided email.");
            }
            else
            {
                Patient? patient = await _context.Patients.FindAsync(contact.OwnerId);
                if (patient == null)
                {
                    ModelState.AddModelError(nameof(appointmentVM.Email), "Patient not found!");
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        var appointment = new Appointment
                        {
                            DoctorId = appointmentVM.DoctorId,
                            PatientId = patient.Id,
                            Date = appointmentVM.Date,
                            Time = TimeSpan.Parse(appointmentVM.Time),
                            Description = appointmentVM.Description
                        };

                        _context.Appointments.Add(appointment);
                        await _context.SaveChangesAsync();

                        return RedirectToAction(nameof(List));
                    }
                }
            }

            appointmentVM.Specialists = _context.Specialists
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Name })
                .ToList();

            appointmentVM.Doctors = _context.Doctors
                .Where(d => d.SpecialistId == appointmentVM.SpecialistId)
                .Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Name + " " + d.Surname })
                .ToList();

            return View(appointmentVM);
        }
    }
}
