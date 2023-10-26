using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sclad.Data;
using sclad.Models;
using sclad.Models.ViewModels;
using System.Diagnostics;

namespace sclad.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            HomeVM homeVM = new HomeVM()
            {
                Items = _db.Item.Include(u => u.ItemType).Include(u => u.Punkt),
                ItemTypes = _db.ItemType

            };
            return View(homeVM);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}