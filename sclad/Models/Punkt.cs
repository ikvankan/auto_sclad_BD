using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace sclad.Models
{
    public class Punkt
    {
        [Key]
        public int Id { get; set; }
        [DisplayName("Наименование Пункта")]
        public string Name  { get; set; }
        [DisplayName("Адресс Пункта")]
        public string Adress { get; set; }
        [DisplayName("Контакты Пункта")]
        public string Kontakti { get; set; }
    }
}
