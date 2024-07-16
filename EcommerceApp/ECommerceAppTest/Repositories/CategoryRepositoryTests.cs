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
    public class CategoryRepositoryTests
    {
        [Fact]
        public async Task Test_GetAllCategories_Returns_ProductList()
        {
            //Arranage
            var options = new DbContextOptionsBuilder<AppDbContext>()
               .UseInMemoryDatabase(databaseName: "GetAllCategories_CategoryList")
               .Options;

            using (var _dbContext = new AppDbContext(options))
            {
                var categories = new List<Category>()
                 {
                   new Category
                   {
                     Id = 1,
                     Name = "TV",
                   },
                   new Category
                   {
                     Id = 2,
                     Name = "Mobile",
                   }
                 };
                await _dbContext.Categories.AddRangeAsync(categories);
                await _dbContext.SaveChangesAsync();
                var categoryRepository = new CategoryRepository(_dbContext);
                var categoryListData = await categoryRepository.GetAllCategories();

                //Asserts
                Assert.NotNull(categoryListData);
                Assert.IsType<List<Category>>(categoryListData);
                Assert.Equal(categories[0].Id, categoryListData[0].Id);
                Assert.Equal(categories[0].Name, categoryListData[0].Name);
                Assert.Equal(categories.Count, categoryListData.Count);
            }
        }
        [Fact]
        public async Task Test_GetAllCategories_Returns_EmptyResult()
        {
            //Arranage
            var options = new DbContextOptionsBuilder<AppDbContext>()
               .UseInMemoryDatabase(databaseName: "GetAllCategories_EmptyResult")
               .Options;

            using (var _dbContext = new AppDbContext(options))
            {
                var categories = new List<Category>();

                await _dbContext.Categories.AddRangeAsync(categories);
                await _dbContext.SaveChangesAsync();
                var categoryRepository = new CategoryRepository(_dbContext);
                var categoryListData = await categoryRepository.GetAllCategories();

                //Asserts
                Assert.NotNull(categoryListData);
                Assert.IsType<List<Category>>(categoryListData);
                Assert.Equal(categories.Count, categoryListData.Count);
            }
        }
        [Theory]
        [InlineData(5)]
        public async Task Test_GetCategoryById_Returns_Category(int id)
        {
            //Arranage
            var options = new DbContextOptionsBuilder<AppDbContext>()
               .UseInMemoryDatabase(databaseName: "GetCategoryById_Category")
               .Options;

            using (var _dbContext = new AppDbContext(options))
            {
                var category = new Category
                {
                    Id = 5,
                    Name = "TV",
                };
                await _dbContext.Categories.AddAsync(category);
                await _dbContext.SaveChangesAsync();
                var categoryRepository = new CategoryRepository(_dbContext);
                var categoryData = await categoryRepository.GetCategoryById(id);

                //Asserts
                Assert.NotNull(categoryData);
                Assert.IsType<Category>(categoryData);
                Assert.Equal(id, categoryData.Id);
                Assert.Equal(category.Name, categoryData.Name);
            }
        }
        [Theory]
        [InlineData(200)]
        public async Task Test_GetCategoryById_Returns_NullOrNoCategory(int id)
        {
            //Arranage
            var options = new DbContextOptionsBuilder<AppDbContext>()
               .UseInMemoryDatabase(databaseName: "GetCategoryById_NullCategory")
               .Options;

            using (var _dbContext = new AppDbContext(options))
            {
                var category = new Category
                {
                    Id = 5,
                    Name = "TV",
                };
                await _dbContext.Categories.AddAsync(category);
                await _dbContext.SaveChangesAsync();
                var categoryRepository = new CategoryRepository(_dbContext);
                var categoryData = await categoryRepository.GetCategoryById(id);

                //Asserts
                Assert.Null(categoryData);
                Assert.NotEqual(id, category.Id);

            }
        }
        [Fact]
        public async Task Test_AddCategory_Returns_CreatedCategory()
        {
            //Arranage
            var options = new DbContextOptionsBuilder<AppDbContext>()
               .UseInMemoryDatabase(databaseName: "AddCategory_CreatedCategory")
               .Options;

            using (var _dbContext = new AppDbContext(options))
            {
                var category = new Category
                {
                    Name = "TV",
                };
                var categoryRepository = new CategoryRepository(_dbContext);
                var categoryData = await categoryRepository.AddCategory(category);

                //Asserts
                Assert.NotNull(categoryData);
                Assert.IsType<Category>(categoryData);
                Assert.Equal(1, categoryData.Id);
                Assert.Equal(category.Name, categoryData.Name);
         
            }
        }
        [Fact]
        public async Task Test_AddCategory_Returns_Exception()
        {
            //Arranage
            var options = new DbContextOptionsBuilder<AppDbContext>()
               .UseInMemoryDatabase(databaseName: "AddCategory_ThrowsException")
               .Options;

            using (var _dbContext = new AppDbContext(options))
            {
                var category = new Category
                {
                    Name = "TV",
                };
                await _dbContext.Categories.AddAsync(category);
                await _dbContext.SaveChangesAsync();
            }
            using (var _dbContext = new AppDbContext(options))
            {
                var categoryToAdd = new Category
                {
                    Id=1,
                    Name = "Mobile",
                };
                var categoryRepository = new CategoryRepository(_dbContext);
                var exception = await Assert.ThrowsAsync<ArgumentException>(() => categoryRepository.AddCategory(categoryToAdd));
                //Asserts
                Assert.NotNull(exception);
                Assert.IsType<ArgumentException>(exception);

            }
        }
        [Fact]
        public async Task Test_UpdateCategory_Returns_CategoryId()
        {
            //Arranage
            var options = new DbContextOptionsBuilder<AppDbContext>()
               .UseInMemoryDatabase(databaseName: "UpdateCategory_CategoryId")
               .Options;

            using (var _dbContext = new AppDbContext(options))
            {

                var category = new Category
                {
                    Id=5,
                    Name = "TV",
                };
                var categoryToUpdate = new Category
                {
                    Name = "Mobile",
              
                };
                await _dbContext.Categories.AddAsync(category);
                await _dbContext.SaveChangesAsync();
                var categoryRepository = new CategoryRepository(_dbContext);
                var categoryId = await categoryRepository.UpdateCategory(categoryToUpdate);

                //Asserts
                Assert.IsType<int>(categoryId);
            }
        }
        [Fact]
        public async Task Test_DeleteCategory_Returns_CategoryId()
        {
            //Arranage
            var options = new DbContextOptionsBuilder<AppDbContext>()
               .UseInMemoryDatabase(databaseName: "DeleteCategory_CategoryId")
               .Options;

            using (var _dbContext = new AppDbContext(options))
            {
                var category = new Category
                {
                    Id = 5,
                    Name = "TV",
                };
                await _dbContext.Categories.AddAsync(category);
                await _dbContext.SaveChangesAsync();
                var categoryRepository = new CategoryRepository(_dbContext);
                var categoryId = await categoryRepository.DeleteCategory(category);

                //Asserts
                Assert.IsType<int>(categoryId);
                Assert.Equal(category.Id, categoryId);
            }
        }
        [Fact]
        public async Task Test_GetCategoriesWithProductDetails_Returns_CategoryWithProductList()
        {
            //Arranage
            var options = new DbContextOptionsBuilder<AppDbContext>()
               .UseInMemoryDatabase(databaseName: "GetCategoriesWithProductDetails_CategoryWithProductList")
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
                var categoryWithProductsRepo = new CategoryRepository(_dbContext);
                var categoryWithProductsData = await categoryWithProductsRepo.GetAllCategoriesWithProducts();

                //Asserts
                Assert.NotNull(categoryWithProductsData);
                Assert.IsType<List<Category>>(categoryWithProductsData);
                Assert.Equal(categories[0].Id, categoryWithProductsData[0].Id);
                Assert.Equal(categories[0].Name, categoryWithProductsData[0].Name);
                Assert.Equal(categories.Count, categoryWithProductsData.Count);
            }
        }
        [Fact]
        public async Task Test_GetCategoriesWithProductDetails_Returns_EmptyResult()
        {
            //Arranage
            var options = new DbContextOptionsBuilder<AppDbContext>()
               .UseInMemoryDatabase(databaseName: "GetCategoriesWithProductDetails_EmptyProductList")
               .Options;

            using (var _dbContext = new AppDbContext(options))
            {
                var categories = new List<Category>();
                await _dbContext.Categories.AddRangeAsync(categories);
                await _dbContext.SaveChangesAsync();
                var categoryWithProductsRepo = new CategoryRepository(_dbContext);
                var categoryWithProductsData = await categoryWithProductsRepo.GetAllCategoriesWithProducts();

                //Asserts
                Assert.NotNull(categoryWithProductsData);
                Assert.IsType<List<Category>>(categoryWithProductsData);
                Assert.Empty(categoryWithProductsData);
            }
        }
        [Fact]
        public async Task Test_GetCategoryWithProductDetails_Returns_CategoryWithProduct()
        {
            //Arranage
            var options = new DbContextOptionsBuilder<AppDbContext>()
               .UseInMemoryDatabase(databaseName: "GetCategoryWithProductDetails_CategoryWithProduct")
               .Options;

            using (var _dbContext = new AppDbContext(options))
            {
                var id = 1;
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
                var categoryWithProductsRepo = new CategoryRepository(_dbContext);
                var categoryWithProductsData = await categoryWithProductsRepo.GetCategoryWithProducts(id);

                //Asserts
                Assert.NotNull(categoryWithProductsData);
                Assert.IsType<List<Category>>(categoryWithProductsData);
                Assert.Equal(categories[0].Id, categoryWithProductsData[0].Id);
                Assert.Equal(categories[0].Name, categoryWithProductsData[0].Name);
                Assert.Single(categoryWithProductsData);
            }
        }
        [Fact]
        public async Task Test_GetCategoryWithProductDetails_Returns_EmptyResult()
        {
            //Arranage
            var options = new DbContextOptionsBuilder<AppDbContext>()
               .UseInMemoryDatabase(databaseName: "GetCategoryWithProductDetails_EmptyResult")
               .Options;

            using (var _dbContext = new AppDbContext(options))
            {
                var id = 400;
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
                var categoryWithProductsRepo = new CategoryRepository(_dbContext);
                var categoryWithProductsData = await categoryWithProductsRepo.GetCategoryWithProducts(id);
                //Asserts
                Assert.NotNull(categoryWithProductsData);
                Assert.IsType<List<Category>>(categoryWithProductsData);
                Assert.Empty(categoryWithProductsData);
            }
        }

    }
}
