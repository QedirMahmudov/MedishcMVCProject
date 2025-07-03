using MedishcMVCProject.DAL;
using MedishcMVCProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedishcMVCProject.Areas.admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public HomeController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            AppUser user = await _userManager.GetUserAsync(User);

            var fullName = $"{user?.Name} {user?.Surname}".Trim();

            DashboardData data = new DashboardData
            {
                Patients = await _context.Patients.CountAsync(),
                Doctors = await _context.Doctors.CountAsync(),
                Appointments = await _context.Appointments.CountAsync(),
                Staff = await _context.Staffs.CountAsync(),
                FullName = string.IsNullOrWhiteSpace(fullName) ? "User" : fullName
            };

            return View(data);
        }
    }
}
