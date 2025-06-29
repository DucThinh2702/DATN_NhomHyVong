using Microsoft.EntityFrameworkCore;
using DATN.Models; // Nếu namespace Models ở thư mục khác

namespace DATN.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        //public DbSet<Category> Categories { get; set; }
    }
}
