using EcommerceApp.Entities.DomainModels;
using EcommerceApp.Repositories.Contracts;
using EcommerceApp.Services.Contracts;
using EcommerceApp.Services.Implementations;
using Microsoft.CodeAnalysis.Elfie.Model.Structures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceAppTest.ServicesTests
{
    public class CategoryServiceTests
    {
        private readonly Mock<ICategoryRepository> categorytRepositoryMock;
        private readonly ICategoryService categoryService;
        private readonly Mock<ILogger<CategoryService>> logger;
        public CategoryServiceTests()
        {
            categorytRepositoryMock = new Mock<ICategoryRepository>();
            logger = new Mock<ILogger<CategoryService>>();
            categoryService = new CategoryService(categorytRepositoryMock.Object, logger.Object);
        }
        [Fact]
        public async Task Test_GetAllCategories_Returns_ProductList()
        {
            //Arrange
            var categories = new List<Category>()
            {
              new Category
              {
                Id = 1,
                Name = "Food",
              },
              new Category
              {
                Id = 2,
                Name = "Mobile",
              },
            };
            categorytRepositoryMock.Setup(p => p.GetAllCategories()).ReturnsAsync(categories);
            //Act
            var categoriesList = await categoryService.GetAllCategories();
            //Assert
            Assert.NotNull(categoriesList);
            Assert.IsType<List<Category>>(categoriesList);
            Assert.Equal(categories[0].Name, categoriesList[0].Name);
            Assert.Equal(categories[1].Name, categoriesList[1].Name);
            Assert.Equal(categories.Count, categoriesList.Count);

        }
        [Fact]
        public async Task Test_GetAllCategories_Returns_EmptyProductList()
        {
            //Arrange
            var categories = new List<Category>();
            categorytRepositoryMock.Setup(p => p.GetAllCategories()).ReturnsAsync(categories);
            //Act
            var categoriesData = await categoryService.GetAllCategories();
            //Assert
            Assert.IsType<List<Category>>(categoriesData);
            Assert.Equal(categories.Count, categoriesData.Count);

        }
        [Theory]
        [InlineData(4)]
        public async Task Test_GetCategoryById_Returns_Product(int id)
        {
            //Arrange
            var category = new Category
            {
                Id = 4,
                Name = "Mobile",
            };
            categorytRepositoryMock.Setup(p => p.GetCategoryById(id)).ReturnsAsync(category);
            //Act
            var categoryData = await categoryService.GetCategoryById(id);
            //Assert
            Assert.NotNull(categoryData);
            Assert.IsType<Category>(categoryData);
            Assert.Equal(category.Id, categoryData.Id);
            Assert.Equal(category.Name, categoryData.Name);
        }
        [Theory]
        [InlineData(-9)]
        public async Task Test_GetCategoryById_Returns_ArgumentException(int id)
        {
            //Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => categoryService.GetCategoryById(id));
            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
            Assert.Equal(nameof(id), exception.ParamName);
        }
        [Fact]
        public async Task Test_AddCategory_Returns_CreatedProduct()
        {
            //Arrange
            var category = new Category
            {
                Name = "Mobile",
            };
            categorytRepositoryMock.Setup(p => p.AddCategory(It.IsAny<Category>())).ReturnsAsync(category);
            //Act
            var categoryData = await categoryService.AddCategory(category);
            //Asserts
            Assert.NotNull(categoryData);
            Assert.IsType<Category>(categoryData);
            Assert.Equal(category.Name, categoryData.Name);

        }
        [Fact]
        public async Task Test_AddCategory_Returns_Exception()
        {
            //Arrange
            var category = new Category
            {
                Name = "Mobile",
            };
            categorytRepositoryMock.Setup(p => p.AddCategory(It.IsAny<Category>())).Throws(new DbUpdateException());
            //Act
            var exception = await Assert.ThrowsAsync<DbUpdateException>(() => categoryService.AddCategory(category));
            Assert.NotNull(exception);
            Assert.IsType<DbUpdateException>(exception);
        }
        [Theory]
        [InlineData(4)]
        public async Task Test_UpdateCategory_Returns_UpdatedCategoryId(int id)
        {
            //Arrange
            var category = new Category
            {
                Name = "Fashion",
            };
            categorytRepositoryMock.Setup(p => p.UpdateCategory(id, It.IsAny<Category>())).ReturnsAsync(id);
            //Act
            var categoryData = await categoryService.UpdateCategory(id, category);
            //Asserts
            Assert.IsType<int>(categoryData);
            Assert.Equal(id, categoryData);
        }
        [Theory]
        [InlineData(0)]
        public async Task Test_UpdateCategory_Returns_ArgumentException(int id)
        {
            var category = new Category
            {
                Name = "Fashion",
            };
            //Act
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => categoryService.UpdateCategory(id, category));
            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
            Assert.Equal(nameof(id), exception.ParamName);
        }
        [Theory]
        [InlineData(4)]
        public async Task Test_DeleteCategory_Returns_DeletedCategoryId(int id)
        {
            //Arrange
            categorytRepositoryMock.Setup(p => p.DeleteCategory(id)).ReturnsAsync(id);
            //Act
            var deletedcategoryId = await categoryService.DeleteCategory(id);
            //Asserts
            Assert.IsType<int>(deletedcategoryId);
            Assert.Equal(id, deletedcategoryId);
        }
        [Theory]
        [InlineData(0)]
        public async Task Test_DeleteCategory_Returns_ArgumentException(int id)
        {
            //Act
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => categoryService.DeleteCategory(id));
            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
            Assert.Equal(nameof(id), exception.ParamName);
        }
    }
}
