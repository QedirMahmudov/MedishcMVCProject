using Microsoft.AspNetCore.Mvc;

namespace MedishcMVCProject.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
