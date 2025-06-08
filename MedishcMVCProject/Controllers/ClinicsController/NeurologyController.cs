using Microsoft.AspNetCore.Mvc;

namespace MedishcMVCProject.Controllers.ClinicsController
{
    public class NeurologyController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
