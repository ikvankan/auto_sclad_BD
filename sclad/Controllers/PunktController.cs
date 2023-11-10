using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sclad.Data;
using sclad.Models;
using System.Data;

namespace sclad.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
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
            TempData[WC.Success] = "Пункт создан успешно!";
            return RedirectToAction("Index");
        }

        //GET-EDIT
        public IActionResult Edit(int? Id)
        {
            if (Id == null) { return NotFound(); }
            var obj = _db.Punkt.Find(Id);
            if (obj == null) { return NotFound(); }
            return View(obj);
        }

        //POST-EDIT
        [HttpPost]//Action метод типа пост дададада
        [ValidateAntiForgeryToken]//Токен от взлома
        public IActionResult Edit(Punkt obj)
        {
            _db.Punkt.Update(obj);
            _db.SaveChanges();
            TempData[WC.Success] = "Пункт изменён успешно!";
            return RedirectToAction("Index");
        }

        //GET-DELETE
        public IActionResult Delete(int? Id)
        {
            if (Id == null) { return NotFound(); }
            var obj = _db.Punkt.Find(Id);
            if (obj == null) { return NotFound(); }
            return View(obj);
        }


        //POST-DELITE
        [HttpPost]//Action метод типа пост дададада
        [ValidateAntiForgeryToken]//Токен от взлома
        public IActionResult DeletePost(int? Id)
        {
            var obj = _db.Punkt.Find(Id);
            if (obj == null) { return NotFound(); }
            _db.Punkt.Remove(obj);
            _db.SaveChanges();
            TempData[WC.Success] = "Пункт удалён успешно!";
            return RedirectToAction("Index");
        }
    }
}

