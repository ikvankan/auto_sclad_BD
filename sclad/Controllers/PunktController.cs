using Microsoft.AspNetCore.Mvc;
using sclad.Data;
using sclad.Models;

namespace sclad.Controllers
{
    public class PunktController : Controller
    {
        private readonly ApplicationDbContext _db;
        public PunktController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            IEnumerable<Punkt> objList = _db.Punkt;
            return View(objList);
        }
        //GET-CREATE
        public IActionResult Create()
        {
            
            return View();
        }
        //POST-CREATE
        [HttpPost]//Action метод типа пост дададада
        [ValidateAntiForgeryToken]//Токен от взлома
        public IActionResult Create(Punkt obj)
        {
            _db.Punkt.Add(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}

