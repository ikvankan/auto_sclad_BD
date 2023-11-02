using Braintree;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using sclad.Data;
using sclad.Models;
using sclad.Models.ViewModels;
using sclad.Utility;
using sclad.Utility.BrainTree;
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
        private IBrainTreeGate _brain;
        [BindProperty]
        public ItemUserVM ItemUserVM { get; set; }
        public CartController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment, IEmailSender emailSender,IBrainTreeGate brain)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
            _emailSender = emailSender;
            _brain = brain;
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
            IEnumerable<Item> itemListTemp = _db.Item.Where(u=>itemInCart.Contains(u.Id));
            IList<Item> itemList = new List<Item>();
            
            foreach(var item in shoppingCartList)
            {
                Item itemTemp= itemListTemp.FirstOrDefault(u=> u.Id == item.ItemId);
                itemTemp.TempKol = item.Kol;
                itemList.Add(itemTemp);
            }
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
        public IActionResult IndexPost(IEnumerable<Item> ItemList)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            foreach (Item item in ItemList)
            {
                shoppingCartList.Add(new ShoppingCart { ItemId = item.Id, Kol = item.TempKol });
            }
            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);
            
            return RedirectToAction(nameof(Summary));
        }




        //GET-SUMMARY
        public IActionResult Summary()
        {

            var gateway = _brain.GetGateway();
            var clientToken = gateway.ClientToken.Generate();
            ViewBag.ClientToken = clientToken;





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
                ApplicationUser = _db.ApplicationUser.FirstOrDefault(u => u.Id == claim.Value)
            };

            foreach(var cartobj in shoppingCartList) 
            {
                Item ItemTemp = _db.Item.FirstOrDefault(u => u.Id == cartobj.ItemId);
                ItemTemp.TempKol = cartobj.Kol;
                ItemUserVM.ItemList.Add(ItemTemp);
            }

            return View(ItemUserVM);
        }




        //POST-SUMMARY
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost(/*ItemUserVM temUserVM*/IFormCollection collection)//так как есть атрибут [BindProperty] оно доступно и так
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);


            
            //чето
            decimal orderTotal = 0;
            foreach(Item item in ItemUserVM.ItemList)
            {
                orderTotal += item.Price * item.TempKol;
            }
            OrderHeader orderHeader = new OrderHeader()
            {
                CreatedByUserId = claim.Value,
                FinalOrderTotal = orderTotal,
                City=ItemUserVM.ApplicationUser.City,
                StreetAddress = ItemUserVM.ApplicationUser.StreetAdress,
                State = ItemUserVM.ApplicationUser.State,
                PostalCode = ItemUserVM.ApplicationUser.PostalCode,
                FullName = ItemUserVM.ApplicationUser.FullName,
                Email = ItemUserVM.ApplicationUser.Email,
                PhoneNumber = ItemUserVM.ApplicationUser.PhoneNumber,
                OrderDate = DateTime.Now,
                OrderStatus = WC.StatusPending
            };
            _db.OrderHeader.Add(orderHeader);
            _db.SaveChanges();

            foreach(var item in ItemUserVM.ItemList)
            {
                OrderDetail orderDetail = new OrderDetail()
                {
                    OrderHeaderId = orderHeader.Id,
                    PricePerKol = item.Price,
                    Kol = item.TempKol,
                    ItemId = item.Id
                };
                _db.OrderDetail.Add(orderDetail);
            }
            _db.SaveChanges();

            string nonceFromTheClient = collection["payment_method_nonce"];

            var request = new TransactionRequest
            {
                Amount = Convert.ToDecimal(orderHeader.FinalOrderTotal),
                PaymentMethodNonce = nonceFromTheClient,
                OrderId = orderHeader.Id.ToString(),
                Options = new TransactionOptionsRequest
                {
                    SubmitForSettlement = true
                }
            };
            var gateway = _brain.GetGateway();
            Result<Transaction> result = gateway.Transaction.Sale(request);

            if(result.Target.ProcessorResponseText == "Approved")
            {
                orderHeader.TransactionId = result.Target.Id;
                orderHeader.OrderStatus = WC.StatusApproved;
            }
            else
            {
                orderHeader.OrderStatus = WC.StatusCancelled;
            }
            _db.SaveChanges();
            return RedirectToAction(nameof(InquiryConfirmation), new {id = orderHeader.Id});



            //чето кончилось



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



        public IActionResult InquiryConfirmation(int id)
        {
            OrderHeader orderHeader = _db.OrderHeader.FirstOrDefault(u=>u.Id == id);
            HttpContext.Session.Clear();
            return View(orderHeader);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateCart(IEnumerable<Item> ItemList)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            foreach(Item item in ItemList)
            {
                shoppingCartList.Add(new ShoppingCart { ItemId = item.Id,Kol=item.TempKol });
            }
            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);
            return RedirectToAction(nameof(Index));
        }


        public IActionResult Clear()
        {
            
            HttpContext.Session.Clear();
            return RedirectToAction("Index","Home");
        }
    }
}
