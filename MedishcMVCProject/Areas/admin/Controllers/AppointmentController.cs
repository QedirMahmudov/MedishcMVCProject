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
        public IActionResult List()
        {
            return View();
        }




        [HttpGet]
        public IActionResult GetDoctorsBySpecialist(int specialistId)
        {
            var doctors = _context.Doctors
                .Where(d => d.SpecialistId == specialistId)
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Name + " " + d.Surname
                }).ToList();

            return Json(doctors);
        }

        public IActionResult Create()
        {
            var vm = new CreateAppointmentVM
            {
                Specialists = _context.Specialists
                    .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Name }).ToList(),
                Doctors = new List<SelectListItem>()
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAppointmentVM vm)
        {
            if (vm.Date.Date < DateTime.Today)
            {
                ModelState.AddModelError(nameof(vm.Date), "Keçmiş tarixə görüş təyin edilə bilməz.");
            }

            // Əgər model düzgün deyilsə, dropdown-ları yenidən yüklə və view-ə qaytar
            if (!ModelState.IsValid)
            {
                vm.Specialists = _context.Specialists
                    .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Name })
                    .ToList();

                vm.Doctors = _context.Doctors
                    .Where(d => d.SpecialistId == vm.SpecialistId)
                    .Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Name + " " + d.Surname })
                    .ToList();

                return View(vm);
            }

            var contact = await _context.ContactInfos
                .FirstOrDefaultAsync(x => x.ContactType == ContactType.Email &&
                                          x.Value == vm.Email &&
                                          x.OwnerType == OwnerType.Patient);

            if (contact == null)
            {
                TempData["PatientNotFound"] = "Bu email ilə heç bir patient tapılmadı. Zəhmət olmasa əvvəlcə patient yaradın.";
                return RedirectToAction("Create", "Patients");
            }

            var patient = await _context.Patients.FindAsync(contact.OwnerId);
            if (patient == null)
            {
                TempData["PatientNotFound"] = "Patient məlumatları tam tapılmadı.";
                return RedirectToAction("Create", "Patients");
            }

            var appointment = new Appointment
            {
                DoctorId = vm.DoctorId,
                PatientId = patient.Id,
                Date = vm.Date,
                Time = vm.Time,
                Description = vm.Description
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(List));
        }


    }
}
