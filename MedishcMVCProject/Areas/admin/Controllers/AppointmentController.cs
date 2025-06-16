using Microsoft.AspNetCore.Mvc;

namespace MedishcMVCProject.Areas.admin.Controllers
{
    [Area("Admin")]
    public class AppointmentController : Controller
    {
        public IActionResult Appointments()
        {
            return View();
        }
        public IActionResult List()
        {
            return View();
        }


        public IActionResult Create()
        {
            return View();
        }

    }
}
