using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using sclad.Data;
using sclad.Models;
using sclad.Models.ViewModels;

namespace sclad.Controllers
{
    [Authorize(Roles =WC.AdminRole)]
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
                obj.Punkt = _db.Punkt.FirstOrDefault(u => u.Id == obj.PunktId);
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
                }),
                PunktSelectLIst = _db.Punkt.Select(i => new SelectListItem
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
                var objFromDb = _db.Item.AsNoTracking().FirstOrDefault(u=>u.Id== itemVM.Item.Id);
                if (files.Count > 0)
                {
                    string upload = webRootPath + WC.ImagePath;
                    string fileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(files[0].FileName);

                    var oldFile= Path.Combine(upload, objFromDb.Img);

                    if (System.IO.File.Exists(oldFile))
                    {
                        System.IO.File.Delete(oldFile);
                    }

                    using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }
                    itemVM.Item.Img = fileName+extension;
                }
                else
                {
                    itemVM.Item.Img = objFromDb.Img;
                }
                _db.Item.Update(itemVM.Item);
            }


            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        //GET-DELETE
        public IActionResult Delete(int? Id)
        {
            if (Id == null) { return NotFound(); }
            Item item = _db.Item.Include(u => u.ItemType).Include(u => u.Punkt).FirstOrDefault(u=>u.Id==Id);
            //item.ItemType = _db.ItemType.Find(Id);
            var obj = _db.ItemType.Find(Id);
            if (item == null) { return NotFound(); }
            return View(item);
        }


        //POST-DELITE
        
        [HttpPost, ActionName("Delete")]//Action метод типа пост дададада,ну и имя для метода чтобы асп понимал как к нему лучше обращатся
        [ValidateAntiForgeryToken]//Токен от взлома
        public IActionResult DeletePost(int? Id)
        {
            var obj = _db.Item.Find(Id);
            if (obj == null) { return NotFound(); }

            string upload = _webHostEnvironment.WebRootPath + WC.ImagePath;
            var oldFile = Path.Combine(upload, obj.Img);

            if (System.IO.File.Exists(oldFile))
            {
                System.IO.File.Delete(oldFile);
            }

            _db.Item.Remove(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}

