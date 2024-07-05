using EcommerceApp.Entities.DomainModels;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApp.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> dbContextOptions):base(dbContextOptions)
        {
                
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }

       protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .Property(x => x.Price)
                .HasPrecision(10, 2);

            //products data seed
            var products = new List<Product>()
            {
              new Product
              {
                Id = 1,
                Name = "Nike sneaker",
                Price=15000,
                Rating=1,
                CategoryId=5,
              },
              new Product
              {
                Id = 2,
                Name = "Adidas sports",
                Price=2000,
                Rating=2,
                CategoryId=2,
              },
              new Product
              {
                Id = 3,
                Name = "Samsung 40",
                Price=5000,
                Rating=3.5,
                CategoryId=3,
              },
              new Product
              {
                Id = 4,
                Name = "LG",
                Price=10000,
                Rating=4,
                CategoryId=4,
              },
              new Product
              {
                Id = 5,
                Name = "Iphone 14",
                Price=100000,
                Rating=5,
                CategoryId=1,
              },
              new Product
              {
                Id = 6,
                Name = "One Plus",
                Price=10000,
                Rating=2,
                CategoryId=1,
              },
              new Product
              {
                Id = 7,
                Name = "Vivo 10",
                Price=10000,
                Rating=3,
                CategoryId=1,
              },
              new Product
              {
                Id = 8,
                Name = "Samsung Galaxy",
                Price=10000,
                Rating=4,
                CategoryId=1,
              }
            };
            modelBuilder.Entity<Product>().
                HasData(products);

            //Category data seed
            var categories = new List<Category>()
            {
              new Category
              {
                Id = 1,
                Name = "Mobile",
              },
              new Category
              {
                Id = 2,
                Name = "Microwave",
              },
              new Category
              {
                Id = 3,
                Name = "TV",
              },
              new Category
              {
                Id = 4,
                Name = "Refrigerator",
              },
              new Category
              {
                Id = 5,
                Name = "Shoes",
              }
            };
            modelBuilder.Entity<Category>().
                HasData(categories);

        }
    }
}
