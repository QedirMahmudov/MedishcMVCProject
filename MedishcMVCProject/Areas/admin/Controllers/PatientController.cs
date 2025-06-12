using Microsoft.AspNetCore.Mvc;

namespace MedishcMVCProject.Areas.admin.Controllers
{
    [Area("Admin")]
    public class PatientController : Controller
    {
        public IActionResult List()
        {
            return View();
        }
    }
}
