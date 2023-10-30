using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sclad.Data;
using sclad.Models;
using System.Data;

namespace sclad.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class ItemTypeController : Controller
    {
        private readonly ApplicationDbContext _db;
        public ItemTypeController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            IEnumerable<ItemType> objList = _db.ItemType;
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
        public IActionResult Create(ItemType obj)
        {
            _db.ItemType.Add(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        //GET-EDIT
        public IActionResult Edit(int? Id)
        {
            if(Id == null) { return NotFound(); }
            var obj = _db.ItemType.Find(Id);
            if(obj == null) { return NotFound(); }
            return View(obj);
        }

        //POST-EDIT
        [HttpPost]//Action метод типа пост дададада
        [ValidateAntiForgeryToken]//Токен от взлома
        public IActionResult Edit(ItemType obj)
        {
            _db.ItemType.Update(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        //GET-DELETE
        public IActionResult Delete(int? Id)
        {
            if (Id == null) { return NotFound(); }
            var obj = _db.ItemType.Find(Id);
            if (obj == null) { return NotFound(); }
            return View(obj);
        }


        //POST-DELITE
        [HttpPost]//Action метод типа пост дададада
        [ValidateAntiForgeryToken]//Токен от взлома
        public IActionResult DeletePost(int? Id)
        {
            var obj = _db.ItemType.Find(Id);
            if (obj == null) { return NotFound(); }
            _db.ItemType.Remove(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}

