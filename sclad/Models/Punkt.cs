using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace sclad.Models
{
    public class Punkt
    {
        [Key]
        public int Id { get; set; }
        [DisplayName("Наименование Пункта")]
        [Required(ErrorMessage = "Это обязательное поле")]
        public string Name  { get; set; }
        [DisplayName("Адресс Пункта")]
        [Required(ErrorMessage = "Это обязательное поле")]
        public string Adress { get; set; }
        [DisplayName("Контакты Пункта")]
        [Required(ErrorMessage = "Это обязательное поле")]
        public string Kontakti { get; set; }
    }
}
