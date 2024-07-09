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
        public async Task Test_AddCategory_Returns_NullOrNoCategory()
        {
            //Arranage
            var options = new DbContextOptionsBuilder<AppDbContext>()
               .UseInMemoryDatabase(databaseName: "AddCategory_NullCategory")
               .Options;

            using (var _dbContext = new AppDbContext(options))
            {
                var category = new Category
                {
                    Name = "TV",
                };
                var categoryToAdd = new Category
                {
                    Name = "TV",
                };
                await _dbContext.Categories.AddAsync(category);
                await _dbContext.SaveChangesAsync();
                var categoryRepository = new CategoryRepository(_dbContext);
                var categoryData = await categoryRepository.AddCategory(categoryToAdd);

                //Asserts
                Assert.Null(categoryData);
                Assert.Equal(category.Name, categoryToAdd.Name);
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
        [Theory]
        [InlineData(5)]
        public async Task Test_UpdateCategory_Returns_CategoryId(int id)
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
                var categoryId = await categoryRepository.UpdateCategory(id, categoryToUpdate);

                //Asserts
                Assert.IsType<int>(categoryId);
                Assert.Equal(id, categoryId);
                Assert.Equal(category.Name, categoryToUpdate.Name);
            }
        }
        [Theory]
        [InlineData(200)]
        public async Task Test_UpdateCategory_Returns_NullOrNoCategoryId(int id)
        {
            //Arranage
            var options = new DbContextOptionsBuilder<AppDbContext>()
               .UseInMemoryDatabase(databaseName: "UpdateCategory_NullorNoCategory")
               .Options;

            using (var _dbContext = new AppDbContext(options))
            {
                var category = new Category
                {
                    Id = 5,
                    Name = "TV",
                };
                var categoryToUpdate = new Category
                {
                    Name = "Mobile",

                };
                await _dbContext.Categories.AddAsync(category);
                await _dbContext.SaveChangesAsync();
                var categoryRepository = new CategoryRepository(_dbContext);
                var categoryId = await categoryRepository.UpdateCategory(id, categoryToUpdate);

                //Asserts
                Assert.IsType<int>(categoryId);
                Assert.Equal(-1, categoryId);
                Assert.NotEqual(category.Name, categoryToUpdate.Name);

            }
        }
        [Theory]
        [InlineData(5)]
        public async Task Test_DeleteCategory_Returns_CategoryId(int id)
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
                var categoryId = await categoryRepository.DeleteCategory(id);

                //Asserts
                Assert.IsType<int>(categoryId);
                Assert.Equal(id, categoryId);
            }
        }
        [Theory]
        [InlineData(200)]
        public async Task Test_DeleteCategory_Returns_NullOrNoCategoryId(int id)
        {
            //Arranage
            var options = new DbContextOptionsBuilder<AppDbContext>()
               .UseInMemoryDatabase(databaseName: "DeleteCategory_NullorNoCategory")
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
                var categoryId = await categoryRepository.DeleteCategory(id);

                //Asserts
                Assert.IsType<int>(categoryId);
                Assert.Equal(-1, categoryId);
            }
        }
    }
}
