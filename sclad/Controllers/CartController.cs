using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sclad.Data;
using sclad.Models;
using sclad.Utility;

namespace sclad.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CartController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if(HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0 &&
                HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count()!=null)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }
            List<int> itemInCart = shoppingCartList.Select(i=>i.ItemId).ToList();
            IEnumerable<Item> itemList = _db.Item.Where(u=>itemInCart.Contains(u.Id));
            foreach (var obj in itemList)
            {
                obj.ItemType = _db.ItemType.FirstOrDefault(u => u.Id == obj.ItemTypeId);
                obj.Punkt = _db.Punkt.FirstOrDefault(u => u.Id == obj.PunktId);
            }
            return View(itemList);
        }


        public IActionResult Remove(int Id)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0 &&
                HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() != null)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }

            shoppingCartList.Remove(shoppingCartList.Where(u=>u.ItemId == Id).FirstOrDefault());
            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);
            return RedirectToAction(nameof(Index));

            List<int> itemInCart = shoppingCartList.Select(i => i.ItemId).ToList();
            IEnumerable<Item> itemList = _db.Item.Where(u => itemInCart.Contains(u.Id));
            foreach (var obj in itemList)
            {
                obj.ItemType = _db.ItemType.FirstOrDefault(u => u.Id == obj.ItemTypeId);
                obj.Punkt = _db.Punkt.FirstOrDefault(u => u.Id == obj.PunktId);
            }
            return View(itemList);
        }
    }
}
