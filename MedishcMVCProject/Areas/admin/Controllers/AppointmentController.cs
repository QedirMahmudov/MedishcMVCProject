using MedishcMVCProject.DAL;
using MedishcMVCProject.Models;
using MedishcMVCProject.Utilities;
using MedishcMVCProject.Utilities.Enums;
using MedishcMVCProject.Utilities.Helpers;
using MedishcMVCProject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MedishcMVCProject.Areas.admin.Controllers
{
    [Area("Admin")]
    public class AppointmentController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public AppointmentController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Appointments()
        {
            AppUser? user = await _userManager.GetUserAsync(User);
            if (user is null)
                return RedirectToAction("Login", "Account");

            List<Appointment> appointments;

            if (User.IsInRole(nameof(UserRole.Admin)))
            {
                appointments = await _context.Appointments
                    .Include(a => a.Patient)
                    .Include(a => a.Doctor)
                    .ToListAsync();
            }
            else if (User.IsInRole("Doctor") || User.IsInRole("HeadDoctor"))
            {
                Doctor? doctor = await _context.Doctors
                    .FirstOrDefaultAsync(d => d.AppUserId == user.Id);

                if (doctor == null) return NotFound();

                appointments = await _context.Appointments
                    .Where(a => a.DoctorId == doctor.Id)
                    .Include(a => a.Patient)
                    .Include(a => a.Doctor)
                    .ToListAsync();
            }
            else
            {
                return Forbid();
            }

            return View(appointments);
        }
        public async Task<IActionResult> Details(int id)
        {
            Appointment? appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment is null) return NotFound();


            return RedirectToAction("Profile", "Patient", new { area = "Admin", id = appointment.PatientId });
        }
        public async Task<IActionResult> List(string patientName = null, string doctorName = null, string department = null, string time = null)
        {
            DateTime now = DateTime.Now;
            DateTime today = now.Date;
            TimeSpan currentTime = now.TimeOfDay;

            List<Appointment>? expiredAppointments = await _context.Appointments
                .Where(a => !a.IsDeleted &&
                           (a.Date < today || (a.Date == today && a.Time < currentTime)))
                .ToListAsync();

            foreach (var item in expiredAppointments)
                item.IsDeleted = true;

            if (expiredAppointments.Any())
                await _context.SaveChangesAsync();

            IQueryable<Appointment> query = _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.Specialist)
                .Where(a => !a.IsDeleted);

            if (User.IsInRole("Doctor") || User.IsInRole("HeadDoctor"))
            {
                var user = await _userManager.GetUserAsync(User);
                var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.AppUserId == user.Id);

                if (doctor == null)
                    return NotFound("Doctor profile not found.");

                query = query.Where(a => a.DoctorId == doctor.Id);
            }

            var appointments = await query
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

            appointments = Helpers.FilterByText(appointments, a => a.PatientName, patientName);
            appointments = Helpers.FilterByText(appointments, a => a.DoctorName, doctorName);
            appointments = Helpers.FilterByText(appointments, a => a.Department, department);
            appointments = Helpers.FilterByText(appointments, a => a.Time, time);

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
        public async Task<IActionResult> GetAvailableTimes(int? doctorId, string? doctorEmail, DateTime date)
        {
            Doctor? doctor = null;

            if (doctorId.HasValue)
            {
                doctor = await _context.Doctors.FindAsync(doctorId.Value);
            }
            else if (!string.IsNullOrWhiteSpace(doctorEmail))
            {
                AppUser? user = await _userManager.FindByEmailAsync(doctorEmail.Trim());
                if (user != null)
                {
                    doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.AppUserId == user.Id);
                }
            }

            if (doctor == null)
                return Json(new List<string>());

            DayOfWeekEnum dayOfWeek = (DayOfWeekEnum)date.DayOfWeek;

            var workingHour = await _context.WorkingHours
                .FirstOrDefaultAsync(x => x.DoctorId == doctor.Id && x.DayOfWeek == dayOfWeek);

            if (workingHour == null || !workingHour.OpenTime.HasValue || !workingHour.CloseTime.HasValue)
            {
                return Json(new List<string>());
            }

            var timeSlots = new List<string>();
            var time = workingHour.OpenTime.Value;

            while (time < workingHour.CloseTime.Value)
            {
                timeSlots.Add(time.ToString(@"hh\:mm"));
                time = time.Add(TimeSpan.FromMinutes(30));
            }

            var bookedTimes = await _context.Appointments
                .Where(a => a.DoctorId == doctor.Id && a.Date.Date == date.Date)
                .Select(a => a.Time.ToString(@"hh\:mm"))
                .ToListAsync();

            var availableSlots = timeSlots.Except(bookedTimes).ToList();

            return Json(availableSlots);
        }

        public async Task<IActionResult> GetCalendarEvents()
        {
            AppUser? user = await _userManager.GetUserAsync(User);
            if (user is null) return Unauthorized();

            DateTime today = DateTime.Today;

            //Sorgu tolist olana qeder hazirlanir...
            IQueryable<Appointment> query = _context.Appointments
                .Include(x => x.Patient)
                .Include(x => x.Doctor);

            if (User.IsInRole("Doctor") || User.IsInRole("HeadDoctor"))
            {
                Doctor? doctor = await _context.Doctors
                    .FirstOrDefaultAsync(d => d.AppUserId == user.Id);

                if (doctor is null) return NotFound();

                query = query.Where(x => x.DoctorId == doctor.Id);
            }

            List<Appointment>? appointments = await query.ToListAsync();

            var events = appointments.Select(x => new
            {
                id = x.Id,
                title = $"{x.Time:hh\\:mm} - {x.Patient.Name} {x.Patient.Surname}",
                start = $"{x.Date:yyyy-MM-dd}T{x.Time}",
                url = $"/Admin/Appointment/Details/{x.Id}"
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

            if (!DateTime.TryParse(model.NewDate, out DateTime newDate))
                return Json(new { success = false, message = "Tarix düzgün formatda deyil." });

            if (newDate.Date < DateTime.Today)
                return Json(new { success = false, message = "Keçmiş tarixə təyin etmək olmaz." });

            TimeSpan parsedTime = appointment.Time;
            if (!string.IsNullOrWhiteSpace(model.NewTime))
            {
                if (!TimeSpan.TryParse(model.NewTime, out parsedTime))
                    return Json(new { success = false, message = "Saat formatı yanlışdır." });
            }

            DayOfWeekEnum selectedDay = (DayOfWeekEnum)((int)newDate.DayOfWeek == 0 ? 7 : (int)newDate.DayOfWeek);

            WorkingHours? doctorWorkingHour = _context.WorkingHours
                .FirstOrDefault(h => h.DoctorId == appointment.DoctorId && h.DayOfWeek == selectedDay);

            if (doctorWorkingHour == null
                || !doctorWorkingHour.OpenTime.HasValue
                || !doctorWorkingHour.CloseTime.HasValue
                || doctorWorkingHour.OpenTime.Value == TimeSpan.Zero
                || doctorWorkingHour.CloseTime.Value == TimeSpan.Zero)
            {
                return Json(new { success = false, message = "Həkim bu gün işləmir." });
            }

            TimeSpan start = TimeSpan.FromMinutes(doctorWorkingHour.OpenTime.Value.TotalMinutes);
            TimeSpan end = TimeSpan.FromMinutes(doctorWorkingHour.CloseTime.Value.TotalMinutes);
            TimeSpan current = TimeSpan.FromMinutes(parsedTime.TotalMinutes);

            if (current < start || current > end)
            {
                return Json(new
                {
                    success = false,
                    message = $"Həkim yalnız saat {start:hh\\:mm} - {end:hh\\:mm} arasında işləyir. Sizin saat: {parsedTime:hh\\:mm}"
                });
            }

            bool isConflict = _context.Appointments.Any(a =>
                a.Id != model.AppointmentId &&
                a.DoctorId == appointment.DoctorId &&
                a.Date.Date == newDate.Date &&
                a.Time == parsedTime);

            if (isConflict)
            {
                return Json(new { success = false, message = "Bu tarix və saatda artıq görüş mövcuddur." });
            }

            appointment.Date = newDate;
            appointment.Time = parsedTime;

            _context.SaveChanges();

            return Json(new { success = true });
        }




        public async Task<IActionResult> Create()
        {
            CreateAppointmentVM appointmentVM = new CreateAppointmentVM();

            if (User.IsInRole("Admin"))
            {
                appointmentVM.Specialists = await _context.Specialists
                    .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Name })
                    .ToListAsync();
                appointmentVM.Doctors = new List<SelectListItem>();
            }

            return View(appointmentVM);
        }


        [HttpPost]
        public async Task<IActionResult> Create(CreateAppointmentVM vm)
        {
            Doctor? doctor = null;

            if (User.IsInRole("Doctor"))
            {
                ModelState.Remove(nameof(vm.SpecialistId));
                ModelState.Remove(nameof(vm.DoctorId));

                AppUser user = await _userManager.GetUserAsync(User);
                doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.AppUserId == user.Id);

                if (doctor == null)
                    ModelState.AddModelError("", "Your doctor profile was not found.");
            }
            else
            {
                if (vm.DoctorId == null)
                    ModelState.AddModelError(nameof(vm.DoctorId), "Please select a doctor.");
                else
                    doctor = await _context.Doctors.FindAsync(vm.DoctorId.Value);

                if (doctor == null)
                    ModelState.AddModelError(nameof(vm.DoctorId), "Doctor not found.");
            }

            ContactInfo? contact = await _context.ContactInfos
                .FirstOrDefaultAsync(x => x.ContactType == ContactType.Email &&
                                          x.Value == vm.Email &&
                                          x.OwnerType == OwnerType.Patient);

            if (contact == null)
                ModelState.AddModelError(nameof(vm.Email), "No patient found with the provided email.");

            Patient? patient = contact != null ? await _context.Patients.FindAsync(contact.OwnerId) : null;

            if (contact != null && patient == null)
                ModelState.AddModelError(nameof(vm.Email), "Patient not found.");

            if (vm.Date.Date < DateTime.Today)
            {
                ModelState.AddModelError(nameof(vm.Date), "Appointment date cannot be in the past.");
            }

            if (!TimeSpan.TryParse(vm.Time, out var selectedTime))
            {
                ModelState.AddModelError(nameof(vm.Time), "Invalid time format.");
            }
            else if (doctor != null)
            {
                DayOfWeekEnum selectedDay = (DayOfWeekEnum)vm.Date.DayOfWeek;

                WorkingHours? workingHour = await _context.WorkingHours
                    .FirstOrDefaultAsync(h => h.DoctorId == doctor.Id && h.DayOfWeek == selectedDay);

                if (workingHour == null)
                {
                    ModelState.AddModelError(nameof(vm.Time), "The doctor does not work on the selected day.");
                }
                else if (selectedTime < workingHour.OpenTime || selectedTime >= workingHour.CloseTime)
                {
                    string start = workingHour.OpenTime.Value.ToString(@"hh\:mm");
                    string end = workingHour.CloseTime.Value.ToString(@"hh\:mm");
                    ModelState.AddModelError(nameof(vm.Time), $"The doctor is available only between {start} and {end}.");
                }
                else
                {
                    bool isBooked = await _context.Appointments.AnyAsync(a =>
                        a.DoctorId == doctor.Id &&
                        a.Date == vm.Date &&
                        a.Time == selectedTime);

                    if (isBooked)
                    {
                        ModelState.AddModelError(nameof(vm.Time), "The selected time is already booked.");
                    }
                }
            }

            if (ModelState.IsValid)
            {
                var appointment = new Appointment
                {
                    DoctorId = doctor!.Id,
                    PatientId = patient!.Id,
                    Date = vm.Date,
                    Time = selectedTime,
                    Description = vm.Description
                };

                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(List));
            }

            if (User.IsInRole("Admin"))
            {
                vm.Specialists = await _context.Specialists
                    .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Name })
                    .ToListAsync();

                vm.Doctors = await _context.Doctors
                    .Where(d => d.SpecialistId == vm.SpecialistId)
                    .Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Name + " " + d.Surname })
                    .ToListAsync();
            }

            return View(vm);
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id is null || id <= 0) return BadRequest();

            Appointment? appointment = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .ThenInclude(d => d.Specialist)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null) return NotFound();

            int specialistId = appointment.Doctor.SpecialistId;

            UpdateAppointmentVM vm = new UpdateAppointmentVM
            {
                AppointmentId = appointment.Id,
                Description = appointment.Description,
                Date = appointment.Date,
                DoctorId = appointment.DoctorId,
                SpecialistId = specialistId,
                Time = appointment.Time.ToString(@"hh\:mm"),
                Email = await _context.ContactInfos
                    .Where(c => c.OwnerType == OwnerType.Patient && c.OwnerId == appointment.PatientId && c.ContactType == ContactType.Email)
                    .Select(c => c.Value)
                    .FirstOrDefaultAsync() ?? "",

                Specialists = await _context.Specialists
                    .Select(s => new SelectListItem
                    {
                        Value = s.Id.ToString(),
                        Text = s.Name
                    }).ToListAsync(),

                Doctors = await _context.Doctors
                    .Where(d => d.SpecialistId == specialistId)
                    .Select(d => new SelectListItem
                    {
                        Value = d.Id.ToString(),
                        Text = d.Name + " " + d.Surname
                    }).ToListAsync()
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Update(UpdateAppointmentVM vm)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdowns(vm);
                return View(vm);
            }

            Appointment? appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id == vm.AppointmentId);

            if (appointment == null) return NotFound();

            if (!TimeSpan.TryParse(vm.Time, out TimeSpan parsedTime))
            {
                ModelState.AddModelError("Time", "Saat formatı yanlışdır.");
                await LoadDropdowns(vm);
                return View(vm);
            }

            bool isConflict = await _context.Appointments.AnyAsync(a =>
                a.Id != vm.AppointmentId &&
                a.DoctorId == vm.DoctorId &&
                a.Date.Date == vm.Date.Date &&
                a.Time == parsedTime);

            if (isConflict)
            {
                ModelState.AddModelError(string.Empty, "Bu tarix və saatda artıq təyinat var.");
                await LoadDropdowns(vm);
                return View(vm);
            }

            appointment.Date = vm.Date;
            appointment.Time = parsedTime;
            appointment.DoctorId = vm.DoctorId.Value;
            appointment.Description = vm.Description;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(List));
        }

        private async Task LoadDropdowns(UpdateAppointmentVM vm)
        {
            vm.Specialists = await _context.Specialists
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                }).ToListAsync();

            vm.Doctors = await _context.Doctors
                .Where(d => d.SpecialistId == vm.SpecialistId)
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Name + " " + d.Surname
                }).ToListAsync();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSelected(List<int> selectedIds)
        {
            if (selectedIds == null || !selectedIds.Any())
            {
                TempData["Warning"] = "Please select at least one appointment to delete.";
                return RedirectToAction(nameof(List));
            }

            List<Appointment> appointments = await _context.Appointments
                .Where(a => selectedIds.Contains(a.Id))
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .ToListAsync();



            _context.Appointments.RemoveRange(appointments);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Selected appointments deleted successfully.";
            return RedirectToAction(nameof(List));
        }
    }
}
