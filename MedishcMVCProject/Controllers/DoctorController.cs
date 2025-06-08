using MedishcMVCProject.DAL;
using MedishcMVCProject.Models;
using MedishcMVCProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedishcMVCProject.Controllers
{
    public class DoctorController : Controller
    {
        private readonly AppDbContext _context;

        public DoctorController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {

            List<Doctor>? doctors = _context.Doctors
                                        .Include(d => d.Specialist)
                                        .Include(d => d.Degree)
                                        .Include(d => d.University)
                                        .ToList();

            DoctorIndexVM vm = new DoctorIndexVM
            {
                Doctors = doctors
            };

            return View(vm);


        }
        public async Task<IActionResult> DoctorDetail(int? id)
        {
            if (id is null || id <= 0) return BadRequest();

            Doctor? doctor = await _context.Doctors
                .Include(d => d.Specialist)
                .Include(d => d.Degree)
                .Include(d => d.University)
                .Include(d => d.PriceLists)
                .Include(d => d.OpeningHours)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (doctor is null) return NotFound();

            DoctorDetailVM vm = new DoctorDetailVM()
            {
                Doctor = doctor,
                Degree = doctor.Degree,
                Specialist = doctor.Specialist,
                University = doctor.University,
                OpeningHours = doctor.OpeningHours.ToList(),
                PriceLists = doctor.PriceLists.ToList()
            };


            return View(vm);
        }
    }
}
