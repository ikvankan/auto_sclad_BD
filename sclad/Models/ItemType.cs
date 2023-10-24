using System.ComponentModel.DataAnnotations;

namespace sclad.Models
{
    public class ItemType
    {
        [Key]
        public int Id { get; set; }
        public string Name  { get; set; }
        public double price { get; set; }
    }
}
