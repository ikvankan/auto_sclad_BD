using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using sclad.Models;

namespace sclad.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<ItemType> ItemType { get; set; }
        public DbSet<Punkt> Punkt { get; set; }
        public DbSet<Item> Item { get; set; }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
    }
}
