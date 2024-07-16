using EcommerceApp.Entities.DomainModels;
using EcommerceApp.Repositories.Contracts;
using EcommerceApp.Services.Contracts;
using EcommerceApp.Services.Implementations;
using ECommerceAppTest.DataSetUp;
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
        private readonly InitialCategoryDataSetUp categoryDataSetUp;
        private readonly List<Category> categories;
        private readonly List<Category> categoriesWithProducts;
        public CategoryServiceTests()
        {
            categorytRepositoryMock = new Mock<ICategoryRepository>();
            categoryService = new CategoryService(categorytRepositoryMock.Object);
            categoryDataSetUp = new InitialCategoryDataSetUp();
            categories = categoryDataSetUp.CategoryList;
            categoriesWithProducts = categoryDataSetUp.CategoriesWithProductsList;
        }
        private Category? GetCategoryById(int id)
        {
            return categories?.Find(x => x.Id == id);
        }
        private List<Category> GetCategoryWithProductsById(int id)
        {
            return categoriesWithProducts.Where(x => x.Id == id).ToList();
        }
        [Fact]
        public async Task Test_GetAllCategories_Returns_ProductList()
        {
            //Arrange
            categorytRepositoryMock.Setup(p => p.GetAllCategories()).ReturnsAsync(categories);
            //Act
            var categoriesList = await categoryService.GetAllCategories();
            //Assert
            Assert.NotNull(categoriesList);
            Assert.IsType<List<Category>>(categoriesList);
            Assert.Equal(categories[0].Name, categoriesList[0].Name);
            Assert.Equal(categories.Count, categoriesList.Count);
        }
        [Fact]
        public async Task Test_GetAllCategories_Returns_EmptyProductList()
        {
            //Arrange
            var categorieList = new List<Category>();
            categorytRepositoryMock.Setup(p => p.GetAllCategories()).ReturnsAsync(categorieList);
            //Act
            var categoriesData = await categoryService.GetAllCategories();
            //Assert
            Assert.IsType<List<Category>>(categoriesData);
            Assert.Equal(categorieList.Count, categoriesData.Count);

        }
        [Fact]
        public async Task Test_GetCategoryById_Returns_CategoryData()
        {
            //Arrange
            var id = 4;
            var category = GetCategoryById(id);
            categorytRepositoryMock.Setup(p => p.GetCategoryById(id)).ReturnsAsync(category);
            //Act
            var categoryData = await categoryService.GetCategoryById(id);
            //Assert
            Assert.NotNull(categoryData);
            Assert.IsType<Category>(categoryData);
            Assert.Equal(category?.Id, categoryData.Id);
            Assert.Equal(category?.Name, categoryData.Name);
        }
        [Fact]
        public async Task Test_GetCategoryById_Returns_NullOrNoData()
        {
            //Arrange
            var id = 256;
            var category = GetCategoryById(id);
            categorytRepositoryMock.Setup(p => p.GetCategoryById(id)).ReturnsAsync(category);
            //Act
            var categoryData = await categoryService.GetCategoryById(id);
            //Assert
            Assert.Null(categoryData);
        }
        [Fact]
        public async Task Test_AddCategory_Returns_CreatedProduct()
        {
            //Arrange
            var category = new Category
            {
                Name = "Fashion",
            };
            categorytRepositoryMock.Setup(p => p.GetAllCategories()).ReturnsAsync(categories);
            categorytRepositoryMock.Setup(p => p.AddCategory(It.IsAny<Category>())).ReturnsAsync(category);
            //Act
            var categoryData = await categoryService.AddCategory(category);
            //Asserts
            Assert.NotNull(categoryData);
            Assert.IsType<Category>(categoryData);
            Assert.Equal(category.Name, categoryData.Name);

        }
        [Fact]
        public async Task Test_AddCategory_Returns_NullOrNoCategory()
        {
            //Arrange
            var category = new Category
            {
                Name = "Mobile",
            };
            categorytRepositoryMock.Setup(p => p.GetAllCategories()).ReturnsAsync(categories);
            //Act
            var categoryData = await categoryService.AddCategory(category);
            Assert.Null(categoryData);
        }
        [Fact]
        public async Task Test_UpdateCategory_Returns_UpdatedCategoryId()
        {
            //Arrange
            var categoryToUpdate = new Category
            {
                Name = "Fashion",
            };
            var id = 4;
            var categoryDetails = GetCategoryById(id);
            categorytRepositoryMock.Setup(p => p.GetCategoryById(id)).ReturnsAsync(categoryDetails);
            categorytRepositoryMock.Setup(p => p.UpdateCategory(It.IsAny<Category>())).ReturnsAsync(id);
            //Act
            var categoryData = await categoryService.UpdateCategory(id, categoryToUpdate);
            //Asserts
            Assert.IsType<int>(categoryData);
            Assert.Equal(id, categoryData);
        }
        [Fact]
        public async Task Test_UpdateCategory_Returns_WhenIdIsNotFound()
        {
            var categoryToUpdate = new Category
            {
                Name = "Fashion",
            };
            var id = 90;
            var categoryDetails = GetCategoryById(id);
            categorytRepositoryMock.Setup(p => p.GetCategoryById(id)).ReturnsAsync(categoryDetails);
            //Act
            var categoryData = await categoryService.UpdateCategory(id, categoryToUpdate);
            Assert.Equal(-1,categoryData);
        }
        [Fact]
        public async Task Test_DeleteCategory_Returns_DeletedCategoryId()
        {
            //Arrange
            var id = 4;
            var categoryDetails = GetCategoryById(id);
            categorytRepositoryMock.Setup(p => p.GetCategoryById(id)).ReturnsAsync(categoryDetails);
            categorytRepositoryMock.Setup(p => p.DeleteCategory(It.IsAny<Category>())).ReturnsAsync(id);
            //Act
            var deletedcategoryId = await categoryService.DeleteCategory(id);
            //Asserts
            Assert.IsType<int>(deletedcategoryId);
            Assert.Equal(id, deletedcategoryId);
        }
        [Fact]
        public async Task Test_DeleteCategory_Returns_ArgumentException()
        {
            //Arrnage
            var id = 509;
            var categoryDetails = GetCategoryById(id);
            categorytRepositoryMock.Setup(p => p.GetCategoryById(id)).ReturnsAsync(categoryDetails);
            //Act
            var deletedcategoryId = await categoryService.DeleteCategory(id);
            //Assert
            Assert.IsType<int>(deletedcategoryId);
            Assert.Equal(-1, deletedcategoryId);
        }
        [Fact]
        public async Task Test_GetAllCategoriesWithProducts_Returns_ListCategoriesWithProducts()
        {
            //Arrange
            categorytRepositoryMock.Setup(p => p.GetAllCategoriesWithProducts()).ReturnsAsync(categoriesWithProducts);
            //Act
            var categoriesList = await categoryService.GetAllCategoriesWithProducts();
            //Assert
            Assert.NotNull(categoriesList);
            Assert.IsType<List<Category>>(categoriesList);
            Assert.Equal(categoriesWithProducts[0].Name, categoriesList[0].Name);
            Assert.Equal(categoriesWithProducts.Count, categoriesList.Count);
        }
        [Fact]
        public async Task Test_GetAllCategoriesWithProducts_Returns_EmptyResult()
        {
            //Arrange
            var categoryListData = new List<Category>();
            categorytRepositoryMock.Setup(p => p.GetAllCategoriesWithProducts()).ReturnsAsync(categoryListData);
            //Act
            var categoriesData = await categoryService.GetAllCategoriesWithProducts();
            //Assert
            Assert.IsType<List<Category>>(categoriesData);
            Assert.Equal(categoryListData.Count, categoriesData.Count);

        }
        [Fact]
        public async Task Test_GetCategoryWithProducts_Returns_ListCategoriesWithProducts()
        {
            //Arrange
            var categoryid = 1;
            var categoryWithProduct = GetCategoryWithProductsById(categoryid);
            categorytRepositoryMock.Setup(p => p.GetCategoryWithProducts(categoryid)).ReturnsAsync(categoryWithProduct);
            //Act
            var categoriesList = await categoryService.GetCategoryWithProducts(categoryid);
            //Assert
            Assert.NotNull(categoriesList);
            Assert.IsType<List<Category>>(categoriesList);
            Assert.Equal(categoryWithProduct[0].Name, categoriesList[0].Name);
            Assert.Equal(categoryWithProduct.Count, categoriesList.Count);
        }
        [Fact]
        public async Task Test_GetCategoryWithProducts_Returns_EmptyResult()
        {
            //Arrange
            var categoryid = 765;
            var categoryWithProduct = GetCategoryWithProductsById(categoryid);
            categorytRepositoryMock.Setup(p => p.GetCategoryWithProducts(categoryid)).ReturnsAsync(categoryWithProduct);
            //Act
            var categoriesData = await categoryService.GetCategoryWithProducts(categoryid);
            //Assert
            Assert.IsType<List<Category>>(categoriesData);
            Assert.Equal(categoryWithProduct.Count, categoriesData.Count);

        }

    }
}
