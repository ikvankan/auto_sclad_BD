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
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ItemController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
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
        public IActionResult Upsert(ItemVM itemVM)
        {
            var files = HttpContext.Request.Form.Files;
            string webRootPath = _webHostEnvironment.WebRootPath;

            if(itemVM.Item.Id == 0)
            {
                //создаем
                string upload = webRootPath + WC.ImagePath;
                string fileName = Guid.NewGuid().ToString();
                string extension = Path.GetExtension(files[0].FileName);


                using(var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                {
                    files[0].CopyTo(fileStream);
                }

                itemVM.Item.Img = fileName + extension;

                _db.Item.Add(itemVM.Item);
            }
            else
            {
                //обновляем
            }
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

