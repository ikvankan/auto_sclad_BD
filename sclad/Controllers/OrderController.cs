using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using sclad.Data;
using sclad.Models.ViewModels;
using sclad.Utility.BrainTree;

namespace sclad.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _db;
        private IBrainTreeGate _brain;
        public OrderController(ApplicationDbContext db, IBrainTreeGate brain)
        {
            _db = db;
            _brain = brain;
        }
        public IActionResult Index()
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
            return View(orderListVM);
        }
    }
}
