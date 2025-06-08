using Microsoft.AspNetCore.Mvc;

namespace MedishcMVCProject.Controllers.ClinicsController
{
    public class PathologyController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
