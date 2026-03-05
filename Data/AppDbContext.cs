using Microsoft.EntityFrameworkCore;
using Product_Management.Models;


namespace Product_Management.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Electronics" },
                new Category { Id = 2, Name = "Clothing" },
                new Category { Id = 3, Name = "Food & Beverages" },
                new Category { Id = 4, Name = "Books" },
                new Category { Id = 5, Name = "Sports & Outdoors" }
            );
        }
    }
}
