using Microsoft.AspNetCore.Mvc;

namespace MedishcMVCProject.Controllers.ClinicsController
{
    public class CardiacController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
