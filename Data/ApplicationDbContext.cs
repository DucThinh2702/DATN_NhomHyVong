using DATN.Models;
using Microsoft.EntityFrameworkCore;
// File: Data/ApplicationDbContext.cs
// Project: DATN.Data
namespace DATN.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public virtual DbSet<User> Users { get; set; } // Define your DbSet properties here for each entity
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Add any additional model configurations here
        }
    }
}
