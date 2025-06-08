using MedishcMVCProject.DAL;
using MedishcMVCProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedishcMVCProject.Controllers
{
    public class AboutUsController : Controller
    {
        private readonly AppDbContext _context;

        public AboutUsController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> AboutUs()
        {
            HomeVM vm = new HomeVM()
            {
                Doctors = await _context.Doctors.Where(d => d.IsDeleted == false).Take(6).ToListAsync(),
                Specialists = await _context.Specialists.ToListAsync(),
                Clinics = await _context.Clinics.Where(c => c.IsDeleted == false).Take(3).ToListAsync(),
                ClinicServices = await _context.ClinicServices.Where(cs => cs.IsDeleted == false).ToListAsync(),
                Blogs = await _context.Blogs
                        .Include(b => b.BlogCategories)
                        .ThenInclude(bc => bc.Category)
                        .Where(b => b.IsDeleted == false)
                        .Take(6)
                        .ToListAsync(),
                Authors = await _context.Authors.Where(a => a.IsDeleted == false).ToListAsync(),
                Categories = await _context.Categories.Where(c => c.IsDeleted == false).ToListAsync()
            };
            return View(vm);
        }
        public async Task<IActionResult> MedicalServices()
        {
            AboutVM aboutVM = new AboutVM()
            {
                MedicalServices = await _context.MedicalServices.Take(6).ToListAsync(),
                Doctors = await _context.Doctors.Include(d => d.Specialist).Take(3).ToListAsync()
            };

            return View(aboutVM);
        }

        public IActionResult AboutService()
        {
            return View();
        }

        public async Task<IActionResult> Appointments()
        {
            AboutVM aboutVM = new AboutVM()
            {
                Doctors = await _context.Doctors.Include(d => d.Specialist).Take(3).ToListAsync()
            };

            return View(aboutVM);
        }

        public IActionResult PricingAndPlans()
        {
            return View();
        }
        public IActionResult FAQS()
        {
            return View();
        }
        public IActionResult Gallery()
        {
            return View();
        }
    }
}
