using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using sclad.Data;
using sclad.Models;
using sclad.Models.ViewModels;

namespace sclad.Controllers
{
    public class ItemController : Controller
    {
        private readonly ApplicationDbContext _db;
        public ItemController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            IEnumerable<Item> objList = _db.Item;
            foreach(var obj in objList)
            {
                obj.ItemType = _db.ItemType.FirstOrDefault(u => u.Id == obj.ItemTypeId);
            }
            return View(objList);
        }
        //GET-UPSERT
        public IActionResult Upsert(int? Id)
        {
            //IEnumerable<SelectListItem> ItemTypeDropDown = _db.ItemType.Select(i => new SelectListItem
            //{
            //    Text = i.Name,
            //    Value = i.Id.ToString()
            //});
            //ViewBag.ItemTypeDropDown = ItemTypeDropDown;
            //Item item = new Item();


            ItemVM itemVM = new ItemVM()
            {
                Item = new Item(),
                ItemTypeSelectLIst = _db.ItemType.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };


            if (Id == null)
            {
                //Создаем новый
                return View(itemVM);
            }
            else
            {
                itemVM.Item = _db.Item.Find(Id);
                if(itemVM.Item == null)
                {
                    return NotFound();
                }
                return View(itemVM);
            }
            
        }
        //POST-UPSERT
        [HttpPost]//Action метод типа пост дададада
        [ValidateAntiForgeryToken]//Токен от взлома
        public IActionResult Upsert(ItemType obj)
        {
            _db.ItemType.Add(obj);
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

