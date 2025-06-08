using Microsoft.AspNetCore.Mvc;

namespace MedishcMVCProject.Controllers.ClinicsController
{
    public class PediatricController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
