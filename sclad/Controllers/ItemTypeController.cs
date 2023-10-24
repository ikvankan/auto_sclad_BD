using Microsoft.AspNetCore.Mvc;
using sclad.Data;
using sclad.Models;

namespace sclad.Controllers
{
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
    }
}
