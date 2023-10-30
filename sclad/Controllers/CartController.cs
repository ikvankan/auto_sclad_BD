using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using sclad.Data;
using sclad.Models;
using sclad.Models.ViewModels;
using sclad.Utility;
using System.Security.Claims;
using System.Text;

namespace sclad.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private IEmailSender _emailSender;
        [BindProperty]
        public ItemUserVM ItemUserVM { get; set; }
        public CartController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment, IEmailSender emailSender)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
            _emailSender = emailSender;
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




        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost()
        {
            return RedirectToAction(nameof(Summary));
        }




        //GET-SUMMARY
        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);



            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0 &&
                HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() != null)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }
            List<int> itemInCart = shoppingCartList.Select(i => i.ItemId).ToList();
            IEnumerable<Item> itemList = _db.Item.Where(u => itemInCart.Contains(u.Id));
            foreach (var obj in itemList)
            {
                obj.ItemType = _db.ItemType.FirstOrDefault(u => u.Id == obj.ItemTypeId);
                obj.Punkt = _db.Punkt.FirstOrDefault(u => u.Id == obj.PunktId);
            }

            ItemUserVM = new ItemUserVM()
            {
                ApplicationUser = _db.ApplicationUser.FirstOrDefault(u => u.Id == claim.Value),
                ItemList = itemList.ToList()
            };

            return View(ItemUserVM);
        }




        //POST-SUMMARY
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost(/*ItemUserVM temUserVM*/)//так как есть атрибут [BindProperty] оно доступно и так
        {
            var PathToTemplate = _webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString()
                + "templates" + Path.DirectorySeparatorChar.ToString() +
                "Inquiry.html";

            var subject = "new Inquiry";
            string HtmlBody = "";
            using(StreamReader sr  = System.IO.File.OpenText(PathToTemplate))
            {
                HtmlBody= sr.ReadToEnd();
            }
            /* 
            name {0}
            Email{1}
            Phone{2}
            Items{3}
            */

            StringBuilder ItemListSB = new StringBuilder();
            foreach(var item in ItemUserVM.ItemList)
            {
                ItemListSB.Append($" - Name: {item.Name} <span style='font-size:14px;'> (ID: {item.Id})</span><br />");
            }
            string messageBody = string.Format(HtmlBody,
                ItemUserVM.ApplicationUser.FullName,
                ItemUserVM.ApplicationUser.Email,
                ItemUserVM.ApplicationUser.PhoneNumber,
                ItemListSB.ToString());


            await _emailSender.SendEmailAsync(WC.EmailAdmin,subject,messageBody);


            return RedirectToAction(nameof(InquiryConfirmation));
        }



        public IActionResult InquiryConfirmation()
        {
            HttpContext.Session.Clear();
            return View();
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
        }
    }
}
