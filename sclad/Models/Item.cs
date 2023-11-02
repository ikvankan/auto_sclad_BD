using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sclad.Models
{
    public class Item
    {
        public Item()
        {
            TempKol = 1;
        }
        [Key]
        public int Id { get; set; }
        [DisplayName("Наименование")]
        [Required]
        public string  Name { get; set; }

        [Display(Name="Тип предмета")]
        public int ItemTypeId { get; set; }

        public int Weight { get; set; }
        public string? Discription { get; set; }
        public string? ShortDesc { get; set; }
        [Display(Name = "Пункт предмета")]
        public int PunktId { get; set; }
        public string? Img { get; set; }
        [ForeignKey("ItemTypeId")]
        public virtual ItemType ItemType { get; set; }
        [ForeignKey("PunktId")]
        public virtual Punkt Punkt { get; set; }

        [NotMapped]
        [Range(1,10000)]
        public int TempKol { get; set; }
        [DisplayName("Цена")]
        [Range(0, 9999999999999999.99, ErrorMessage = "Значение либо меньше нули либо слишком большое")]
        [Required(ErrorMessage = "Это обязательное поле")]
        public decimal Price { get; set; }
    }
}
