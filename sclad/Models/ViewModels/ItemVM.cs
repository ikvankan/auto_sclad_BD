using Microsoft.AspNetCore.Mvc.Rendering;

namespace sclad.Models.ViewModels
{
    public class ItemVM
    {
        public Item Item { get; set; }
        public IEnumerable<SelectListItem> ItemTypeSelectLIst { get; set; }
        public IEnumerable<SelectListItem> PunktSelectLIst { get; set; }
    }
}
