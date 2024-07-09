using AutoMapper;
using EcommerceApp.Controllers;
using EcommerceApp.Entities.DomainModels;
using EcommerceApp.Entities.DTOs;
using EcommerceApp.Entities.DTOs.Category;
using EcommerceApp.Mapping;
using EcommerceApp.Repositories.Contracts;
using EcommerceApp.Services.Contracts;
using ECommerceAppTest.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static EcommerceApp.Entities.APIResponses.APICategoriesResponses;

namespace ECommerceAppTest.EndPointsTest.CategoryWithProducts
{
    public class CategoryWithProductsControllerTests
    {
        private readonly Mock<ICategoryWithProductsService> categoryWithProductsServiceMock;
        private readonly CategoryWithProductsController categoryWithProductsController;
        private readonly IMapper mapper;
        private readonly Mock<ILogger<CategoryWithProductsController>> logger;

        public CategoryWithProductsControllerTests()
        {
            categoryWithProductsServiceMock = new Mock<ICategoryWithProductsService>();
            logger = new Mock<ILogger<CategoryWithProductsController>>();

            var _mapper = new MapperConfiguration(mc => mc.AddProfile(new AutoMapperProfile())).CreateMapper();
            mapper = _mapper;

            categoryWithProductsController = new CategoryWithProductsController(categoryWithProductsServiceMock.Object, mapper, logger.Object);
        }
        [Fact]
        public async Task Test_GetCategoryWithProductDetails_Returns_OKResponse()
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
                    CategoryId=5,
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
            var categoriesDto = mapper.Map<List<CategoryWithProductsResDTO>>(categories);
            categoryWithProductsServiceMock.Setup(p => p.GetCategoryWithProductDetails(null)).ReturnsAsync(categories);

            //Act
            var categoryWithproductsData = await categoryWithProductsController.GetCategoryWithProductDetails(null);
            var categoryWithproductsDataOkObj = categoryWithproductsData as OkObjectResult;
            var categoryWithproductsDataResult = categoryWithproductsDataOkObj?.Value as List<CategoryWithProductsResDTO>;

            //Assert
            Assert.IsType<OkObjectResult>(categoryWithproductsData);
            Assert.IsType<OkObjectResult>(categoryWithproductsDataOkObj);
            Assert.Equal((int)HttpStatusCode.OK, categoryWithproductsDataOkObj.StatusCode);

            //Additional assertions
            Assert.IsType<List<CategoryWithProductsResDTO>>(categoryWithproductsDataResult);
            Assert.Equal(categoriesDto[0].Id, categoryWithproductsDataResult[0].Id);
            Assert.Equal(categoriesDto[0].Name, categoryWithproductsDataResult[0].Name);

        }
        [Fact]
        public async Task Test_GetCategoryWithProductDetails_EmptyResultResponse()
        {
            //Arrange
            categoryWithProductsServiceMock.Setup(p => p.GetCategoryWithProductDetails(null)).ReturnsAsync((List<Category>?)null);

            //Act
            var categoryData = await categoryWithProductsController.GetCategoryWithProductDetails(null);
            var noDataResult = categoryData as NoContentResult;

            //Assert
            Assert.IsType<NoContentResult>(categoryData);
            Assert.IsType<NoContentResult>(noDataResult);
            Assert.Equal((int)HttpStatusCode.NoContent, noDataResult.StatusCode);
        }
        [Theory]
        [InlineData(100)]
        public async Task Test_GetCategoryWithProductDetails_NotFoundResponse(int categoryId)
        {
            //Arrange
            categoryWithProductsServiceMock.Setup(p => p.GetCategoryWithProductDetails(categoryId)).ReturnsAsync((List<Category>?)null);

            //Act
            var categoryData = await categoryWithProductsController.GetCategoryWithProductDetails(categoryId);
            var notFoundDataResult = categoryData as NotFoundObjectResult;
            var notFoundDataResultWithValue = notFoundDataResult?.Value as CategoryNotFoundResponse;
            //Assert
            Assert.IsType<NotFoundObjectResult>(categoryData);
            Assert.IsType<NotFoundObjectResult>(notFoundDataResult);
            Assert.Equal((int)HttpStatusCode.NotFound, notFoundDataResult.StatusCode);
            Assert.IsType<CategoryNotFoundResponse>(notFoundDataResultWithValue);
            Assert.Equal(categoryId, notFoundDataResultWithValue.CategoryId);
            Assert.Equal(ResponseConstantsTest.NoRecordWithCategoryId, notFoundDataResultWithValue.Message);


        }
        [Theory]
        [InlineData(0)]
        public async Task Test_GetCategoryWithProductDetails_Returns_BadRequestForArgumetException(int categoryid)
        {
            //Arrange
            categoryWithProductsServiceMock.Setup(p => p.GetCategoryWithProductDetails(categoryid)).
                Throws(new ArgumentNullException(nameof(categoryid)));

            //Act
            var categoryData = await categoryWithProductsController.GetCategoryWithProductDetails(categoryid); //valid id passed
            var errorResponse = categoryData as BadRequestObjectResult;
            var errorContent = errorResponse?.Value as CategoryErrorResponse;

            //Assert
            Assert.IsType<BadRequestObjectResult>(categoryData);
            Assert.IsType<BadRequestObjectResult>(errorResponse);
            Assert.IsType<CategoryErrorResponse>(errorContent);
            Assert.Equal((int)HttpStatusCode.BadRequest, errorResponse.StatusCode);
            Assert.Equal(categoryid, errorContent.CategoryId);
            Assert.Equal(ResponseConstantsTest.NullOrInvalidCategoryId, errorContent.ErrorMessage);
        }
        [Fact]
        public async Task Test_GetCategoryWithProductDetails_Returns_ServerErrorResponse()
        {
            //Arrange
            categoryWithProductsServiceMock.Setup(p => p.GetCategoryWithProductDetails(null)).
                Throws(new Exception("Simulated internal server error"));

            //Act
            var categoryData = await categoryWithProductsController.GetCategoryWithProductDetails(null);
            var errorResponse = categoryData as ObjectResult;
            var errorContent = errorResponse?.Value as ProblemDetails;

            //Assert
            Assert.IsType<ObjectResult>(categoryData);
            Assert.IsType<ObjectResult>(errorResponse);
            Assert.Equal((int)HttpStatusCode.InternalServerError, errorResponse.StatusCode);
            Assert.Equal(ResponseConstantsTest.ServerError, errorContent?.Detail);
        }


    }
}
