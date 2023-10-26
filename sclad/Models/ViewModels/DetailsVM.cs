namespace sclad.Models.ViewModels
{
    public class DetailsVM
    {
        public DetailsVM()
        {
            Item item = new Item();
        }
        public Item Item { get; set; }
        public bool ExistInCart { get; set; }
    }
}
