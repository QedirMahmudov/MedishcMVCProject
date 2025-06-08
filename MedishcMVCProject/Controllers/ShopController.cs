using Microsoft.AspNetCore.Mvc;

namespace MedishcMVCProject.Controllers
{
    public class ShopController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ProductDetail()
        {
            return View();
        }


        public IActionResult ViewCart()
        {
            return View();
        }


        public IActionResult Checkout()
        {
            return View();
        }

    }
}
