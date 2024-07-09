using EcommerceApp.Data;
using EcommerceApp.Entities.DomainModels;
using EcommerceApp.Repositories.Contracts;
using EcommerceApp.Repositories.Implementation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceAppTest.Repositories
{
    public class CategoryWithProductsRepository
    {
        [Fact]
        public async Task Test_GetCategoryWithProductDetails_Returns_CategoryWithProductList()
        {
            //Arranage
            var options = new DbContextOptionsBuilder<AppDbContext>()
               .UseInMemoryDatabase(databaseName: "GetCategoryWithProductDetails_CategoryWithProductList")
               .Options;

            using (var _dbContext = new AppDbContext(options))
            {
                var categories = new List<Category>()
                {
                  new Category
                   {
                     Id = 1,
                     Name = "TV",
                     Products = new List<Product>()
                     {
                       new Product
                       {
                         Id = 1,
                         Name = "Nike sneaker",
                         Price=15000,
                         Rating=1,
                         CategoryId=1,
                       }
                     }
                   },
                  new Category
                   {
                     Id = 2,
                     Name = "Mobile",
                     Products = new List<Product>()
                       {
                          new Product
                          {
                            Id = 2,
                            Name = "Adidas sports",
                            Price=2000,
                            Rating=2,
                            CategoryId=2,
                          }
                       }
                   }
                };
                await _dbContext.Categories.AddRangeAsync(categories);
                await _dbContext.SaveChangesAsync();
                var categoryWithProductsRepo = new CategoryWithProductsRepo(_dbContext);
                var categoryWithProductsData = await categoryWithProductsRepo.GetCategoryWithProductDetails(null);

                //Asserts
                Assert.NotNull(categoryWithProductsData);
                Assert.IsType<List<Category>>(categoryWithProductsData);
                Assert.Equal(categories[0].Id, categoryWithProductsData[0].Id);
                Assert.Equal(categories[0].Name, categoryWithProductsData[0].Name);
                Assert.Equal(categories.Count, categoryWithProductsData.Count);
            }
        }
        [Theory]
        [InlineData(1)]
        public async Task Test_GetCategoryWithProductDetails_Returns_CategoryWithProduct(int id)
        {
            //Arranage
            var options = new DbContextOptionsBuilder<AppDbContext>()
               .UseInMemoryDatabase(databaseName: "GetCategoryWithProductDetails_CategoryWithProduct")
               .Options;

            using (var _dbContext = new AppDbContext(options))
            {
                var categories = new List<Category>()
                {
                  new Category
                  {
                    Id = 1,
                    Name = "TV",
                    Products = new List<Product>()
                    {
                      new Product
                      {
                        Id = 1,
                        Name = "Nike sneaker",
                        Price=15000,
                        Rating=1,
                        CategoryId=1,
                      }
                    }
                  }
                };
                await _dbContext.Categories.AddRangeAsync(categories);
                await _dbContext.SaveChangesAsync();
                var categoryWithProductsRepo = new CategoryWithProductsRepo(_dbContext);
                var categoryWithProductsData = await categoryWithProductsRepo.GetCategoryWithProductDetails(id);

                //Asserts
                Assert.NotNull(categoryWithProductsData);
                Assert.IsType<List<Category>>(categoryWithProductsData);
                Assert.Equal(categories[0].Id, categoryWithProductsData[0].Id);
                Assert.Equal(categories[0].Name, categoryWithProductsData[0].Name);
                Assert.Equal(categories.Count, categoryWithProductsData.Count);
            }
        }
        [Fact]
        public async Task Test_GetCategoryWithProductDetails_Returns_NullOrEmptyResult()
        {
            //Arranage
            var options = new DbContextOptionsBuilder<AppDbContext>()
               .UseInMemoryDatabase(databaseName: "GetCategoryWithProductDetails_EmptyResult")
               .Options;

            using (var _dbContext = new AppDbContext(options))
            {
                var categories = new List<Category>();

                await _dbContext.Categories.AddRangeAsync(categories);
                await _dbContext.SaveChangesAsync();
                var categoryWithProductsRepo = new CategoryWithProductsRepo(_dbContext);
                var categoryWithProductsData = await categoryWithProductsRepo.GetCategoryWithProductDetails(null);
                //Asserts
                Assert.Null(categoryWithProductsData);
            }
        }
    }
}
