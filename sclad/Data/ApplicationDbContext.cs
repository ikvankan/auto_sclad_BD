using Microsoft.EntityFrameworkCore;
using sclad.Models;

namespace sclad.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {
            
        }
        public DbSet<ItemType> ItemType { get; set; }
    }
}
