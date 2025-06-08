using Microsoft.AspNetCore.Mvc;

namespace MedishcMVCProject.Controllers.ClinicsController
{
    public class LaboratoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
