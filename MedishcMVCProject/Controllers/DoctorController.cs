using MedishcMVCProject.DAL;
using MedishcMVCProject.Models;
using MedishcMVCProject.ViewModels;
using MedishcMVCProject.ViewModels.DoctorVM;
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
        public IActionResult Index(int page = 1)
        {
            int pageSize = 6;

            var totalDoctors = _context.Doctors.Count();
            var totalPages = (int)Math.Ceiling((double)totalDoctors / pageSize);

            var doctors = _context.Doctors
                .Include(d => d.Specialist)
                .Include(d => d.Degree)
                .Include(d => d.University)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            DoctorListVM vm = new DoctorListVM
            {
                Doctors = doctors,
                CurrentPage = page,
                TotalPages = totalPages
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
                .Include(d => d.WorkingHours)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (doctor is null) return NotFound();

            DoctorDetailVM vm = new DoctorDetailVM()
            {
                Doctor = doctor,
                Degree = doctor.Degree ?? new Degree { Name = "N/A" },
                Specialist = doctor.Specialist ?? new Specialist { Name = "N/A" },
                University = doctor.University ?? new University { Name = "N/A" },
                WorkingHours = doctor.WorkingHours?.ToList() ?? new List<WorkingHours>(),
                PriceLists = doctor.PriceLists?.ToList() ?? new List<PriceList>()
            };


            return View(vm);
        }
    }
}
