using Braintree;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sclad.Data;
using sclad.Models;
using sclad.Models.ViewModels;
using sclad.Utility.BrainTree;

namespace sclad.Controllers
{
    [Authorize(Roles =WC.AdminRole)]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _db;
        private IBrainTreeGate _brain;
        [BindProperty]
        public OrderVM OrderVM { get; set; }
        public OrderController(ApplicationDbContext db, IBrainTreeGate brain)
        {
            _db = db;
            _brain = brain;
        }
        public IActionResult Index(string searchName = null, string searchEmail = null, string searchPhone = null, string Status = null)
        {
            OrderListVM orderListVM = new OrderListVM()
            {
                OrderHList = _db.OrderHeader,
                StatusList = WC.ListStatus.ToList().Select(i => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Text = i,
                    Value = i
                })
            };
            if (!string.IsNullOrEmpty(searchName))
            {
                orderListVM.OrderHList = orderListVM.OrderHList.Where(u => u.FullName.ToLower().Contains(searchName.ToLower()));
            }
            if (!string.IsNullOrEmpty(searchEmail))
            {
                orderListVM.OrderHList = orderListVM.OrderHList.Where(u=>u.Email.ToLower().Contains(searchEmail.ToLower()));
            }
            if (!string.IsNullOrEmpty(searchPhone))
            {
                orderListVM.OrderHList = orderListVM.OrderHList.Where(u => u.PhoneNumber.ToLower().Contains(searchPhone.ToLower()));
            }
            if (!string.IsNullOrEmpty(Status)&& Status!= "--Order Status--")
            {
                orderListVM.OrderHList = orderListVM.OrderHList.Where(u => u.OrderStatus.ToLower().Contains(Status.ToLower()));
            }
            return View(orderListVM);
        }

        public IActionResult Details(int Id)
        {
            OrderVM = new OrderVM()
            {
                OrderHeader = _db.OrderHeader.FirstOrDefault(u=>u.Id==Id),
                OrderDetail= _db.OrderDetail.Include(o => o.Item).Where(o => o.OrderHeaderId == Id).ToList()
            };
            return View(OrderVM);
        }



        [HttpPost]
        public IActionResult CancelOrder()
        {
            OrderHeader orderHeader = _db.OrderHeader.FirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id);

            var gateway = _brain.GetGateway();
            Transaction transaction = gateway.Transaction.Find(orderHeader.TransactionId);

            if (transaction.Status == TransactionStatus.AUTHORIZED || transaction.Status == TransactionStatus.SUBMITTED_FOR_SETTLEMENT)
            {
                //no refund
                Result<Transaction> resultvoid = gateway.Transaction.Void(orderHeader.TransactionId);
            }
            else
            {
                //refund
                Result<Transaction> resultRefund = gateway.Transaction.Refund(orderHeader.TransactionId);
            }
            orderHeader.OrderStatus = WC.StatusRefunded;
            _db.SaveChanges();
            TempData[WC.Success] = "Заказ успешно отменен!";
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public IActionResult StartProcessing()
        {
            OrderHeader orderHeader = _db.OrderHeader.FirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id);
            orderHeader.OrderStatus = WC.StatusInProcess;
            _db.SaveChanges();
            TempData[WC.Success] = "Заказ перешёл в процесс оформления!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult ShipOrder()
        {
            OrderHeader orderHeader = _db.OrderHeader.FirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id);
            orderHeader.OrderStatus = WC.StatusShipped;
            orderHeader.ShippingDate = DateTime.Now;
            _db.SaveChanges();
            TempData[WC.Success] = "Заказ успешно отправлен!";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult UpdateOrderDetails()
        {
            OrderHeader orderHeaderFromDb = _db.OrderHeader.FirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id);
            orderHeaderFromDb.FullName = OrderVM.OrderHeader.FullName;
            orderHeaderFromDb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
            orderHeaderFromDb.StreetAddress = OrderVM.OrderHeader.StreetAddress;
            orderHeaderFromDb.City = OrderVM.OrderHeader.City;
            orderHeaderFromDb.State = OrderVM.OrderHeader.State;
            orderHeaderFromDb.PostalCode = OrderVM.OrderHeader.PostalCode;
            orderHeaderFromDb.Email = OrderVM.OrderHeader.Email;

            _db.SaveChanges();
            TempData[WC.Success] = "Детали заказа успешно обновлены!";

            return RedirectToAction("Details", "Order", new { id = orderHeaderFromDb.Id });
        }

    }
}
