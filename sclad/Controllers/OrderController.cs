using Microsoft.AspNetCore.Mvc;

namespace sclad.Controllers
{
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
