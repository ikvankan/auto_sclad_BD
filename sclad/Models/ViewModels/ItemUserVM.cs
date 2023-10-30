namespace sclad.Models.ViewModels
{
    public class ItemUserVM
    {
        public ApplicationUser ApplicationUser { get; set; }
        public IList<Item> ItemList { get; set; }
        public ItemUserVM()
        {
            ItemList = new List<Item>();
        }
    }
}
