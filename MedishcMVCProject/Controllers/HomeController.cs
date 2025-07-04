using MedishcMVCProject.DAL;
using MedishcMVCProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedishcMVCProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
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
        public async Task<IActionResult> PharmacyIndex()
        {
            PharmacyHomeVM? vm = new PharmacyHomeVM
            {
                Products = await _context.Products.Take(6).ToListAsync(),
                Blogs = await _context.Blogs.Include(b => b.Author).Take(6).ToListAsync()
            };

            return View(vm);
        }
    }
}
