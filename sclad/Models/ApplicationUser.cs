using Microsoft.AspNetCore.Identity;
using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations.Schema;

namespace sclad.Models
{
    public class ApplicationUser: IdentityUser
    {
        [Required]
        public string FullName { get; set; }
        [NotMapped]
        [Required]
        public string StreetAdress { get; set; }
        [NotMapped]
        [Required]
        public string City { get; set; }
        [NotMapped]
        [Required]
        public string PostalCode { get; set; }
        [NotMapped]
        [Required]
        public string State { get; set; }

    }
}
