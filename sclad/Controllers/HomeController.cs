using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sclad.Data;
using sclad.Models;
using sclad.Models.ViewModels;
using sclad.Utility;
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
        //GET-DETAILS
        public IActionResult Details(int Id)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart) != null && HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart).Count > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }



            DetailsVM DetailsVM = new DetailsVM()
            { 
                Item = _db.Item.Include(u => u.ItemType).Include(u => u.Punkt).Where(u => u.Id == Id).FirstOrDefault(),
                ExistInCart = false

            };

            foreach(var item in shoppingCartList)
            {
                if(item.ItemId == Id)
                {
                    DetailsVM.ExistInCart = true;
                }
            }
            return View(DetailsVM);
        }


        //POST-DETAILS
        [HttpPost,ActionName("Details")]
        public IActionResult DetailsPost(int Id)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart) != null&& HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart).Count>0 )
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }
            shoppingCartList.Add(new ShoppingCart { ItemId = Id });
            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);
            return RedirectToAction(nameof(Index));
        }



        public IActionResult RemoveFromCart(int Id)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart) != null && HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart).Count > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }
            var ItemToRemove=shoppingCartList.SingleOrDefault(r=>r.ItemId==Id);
            if (ItemToRemove != null)
            {
                shoppingCartList.Remove(ItemToRemove);
            }
            
            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);
            return RedirectToAction(nameof(Index));
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