namespace sclad.Models.ViewModels
{
    public class HomeVM
    {
        public IEnumerable<Item> Items { get; set; }
        public IEnumerable<ItemType> ItemTypes { get; set; }
    }
}
