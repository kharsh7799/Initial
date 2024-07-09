using EcommerceApp.Entities.DomainModels;
using EcommerceApp.Repositories.Contracts;
using EcommerceApp.Services.Contracts;
using EcommerceApp.Services.Implementations;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceAppTest.ServicesTests
{
    public class CategoryWithProductsTests
    {
        private readonly Mock<ICategoryWithProductsRepo> categoryWithProductsRepo;
        private readonly ICategoryWithProductsService categoryWithProductsService;
        private readonly Mock<ILogger<CategoryWithProducts>> logger;

        public CategoryWithProductsTests() 
        {
            categoryWithProductsRepo = new Mock<ICategoryWithProductsRepo>();
            logger = new Mock<ILogger<CategoryWithProducts>>();
            categoryWithProductsService = new CategoryWithProducts(categoryWithProductsRepo.Object, logger.Object);
        }
        [Fact]
        public async Task Test_GetCategoryWithProductDetails_Returns_ListCategoriesWithProducts()
        {
            //Arrange
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
            categoryWithProductsRepo.Setup(p => p.GetCategoryWithProductDetails(null)).ReturnsAsync(categories);
            //Act
            var categoriesList = await categoryWithProductsService.GetCategoryWithProductDetails();
            //Assert
            Assert.NotNull(categoriesList);
            Assert.IsType<List<Category>>(categoriesList);
            Assert.Equal(categories[0].Name, categoriesList[0].Name);
            Assert.Equal(categories[1].Name, categoriesList[1].Name);
            Assert.Equal(categories.Count, categoriesList.Count);

        }
        [Theory]
        [InlineData(1)]
        public async Task Test_GetCategoryWithProductDetails_Returns_EmptyProductList(int categoryId)
        {
            //Arrange
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
            };
            categoryWithProductsRepo.Setup(p => p.GetCategoryWithProductDetails(categoryId)).ReturnsAsync(categories);
            //Act
            var categoriesData = await categoryWithProductsService.GetCategoryWithProductDetails(categoryId);
            //Assert
            Assert.IsType<List<Category>>(categoriesData);
            Assert.Equal(categories.Count, categoriesData.Count);

        }
        [Theory]
        [InlineData(0)]
        public async Task Test_GetCategoryWithProductDetails_Returns_ArgumentException(int categoryId)
        {
            //Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => categoryWithProductsService.GetCategoryWithProductDetails(categoryId));
            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
            Assert.Equal(nameof(categoryId), exception.ParamName);
        }
    }
}
