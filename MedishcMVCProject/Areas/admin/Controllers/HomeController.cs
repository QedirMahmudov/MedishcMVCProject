using Microsoft.AspNetCore.Mvc;

namespace MedishcMVCProject.Areas.admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
