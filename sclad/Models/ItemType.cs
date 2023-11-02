using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace sclad.Models
{
    public class ItemType
    {
        [Key]
        public int Id { get; set; }
        [DisplayName("Наименование")]
        [Required(ErrorMessage ="Это обязательное поле")]
        public string Name  { get; set; }
        [Required]
        [DisplayName("еденица измерения")]
        public string Unit { get; set; }
        
    }
}
