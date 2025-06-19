using MedishcMVCProject.DAL;
using Microsoft.AspNetCore.Mvc;

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
        //public IActionResult Create()
        //{
        //    var vm = new CreateAppointmentVM
        //    {
        //        Specialists = _context.Specialists
        //    .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Name }).ToList(),
        //        Doctors = _context.Doctors
        //    .Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Name + " " + d.Surname }).ToList(),
        //        BloodGroups = _context.BloodGroups
        //    .Select(b => new SelectListItem { Value = b.Id.ToString(), Text = b.Name }).ToList(),
        //        Diseases = _context.Diseases
        //    .Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Name }).ToList()
        //    };

        //    return View(vm);
        //}
        //[HttpPost]
        //public async Task<IActionResult> Create(CreateAppointmentVM vm)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        vm.Specialists = _context.Specialists
        //            .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Name }).ToList();
        //        vm.Doctors = _context.Doctors
        //            .Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Name + " " + d.Surname }).ToList();
        //        vm.BloodGroups = _context.BloodGroups
        //            .Select(b => new SelectListItem { Value = b.Id.ToString(), Text = b.Name }).ToList();
        //        vm.Diseases = _context.Diseases
        //            .Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Name }).ToList();

        //        return View(vm);
        //    }

        //    var appointment = new Appointment
        //    {
        //        DoctorId = vm.DoctorId,
        //        SpecialistId = vm.SpecialistId,
        //        Date = vm.Date,
        //        Time = vm.Time,
        //        PatientName = vm.PatientName,
        //        PatientSurname = vm.PatientSurname,
        //        Gender = vm.Gender,
        //        Age = vm.Age,
        //        DiseaseId = vm.DiseaseId,
        //        BloodGroupId = vm.BloodGroupId,
        //        Problem = vm.Problem
        //    };

        //    _context.Appointments.Add(appointment);
        //    await _context.SaveChangesAsync();

        //    return RedirectToAction("Index");
        //}





    }
}
