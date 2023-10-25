using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sclad.Models
{
    public class Item
    {
        [Key]
        public int Id { get; set; }
        [DisplayName("Наименование")]
        [Required]
        public string  Name { get; set; }

        [Display(Name="Тип предмета")]
        public int ItemTypeId { get; set; }

        public int Weight { get; set; }
        public decimal? TotalCost { get; set; }
        public string? Discription { get; set; }
        public string? ItemByUser { get; set; }

        [Display(Name = "Пункт предмета")]
        public int PunktId { get; set; }

        public string? Img { get; set; }

        [ForeignKey("ItemTypeId")]
        public virtual ItemType ItemType { get; set; }
        [ForeignKey("PunktId")]
        public virtual Punkt Punkt { get; set; }

    }
}
