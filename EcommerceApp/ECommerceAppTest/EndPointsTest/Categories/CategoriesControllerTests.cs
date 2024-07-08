using AutoMapper;
using EcommerceApp.Controllers;
using EcommerceApp.Entities.APIResponses;
using EcommerceApp.Entities.DomainModels;
using EcommerceApp.Entities.DTOs;
using EcommerceApp.Entities.DTOs.Category;
using EcommerceApp.Entities.DTOs.Product;
using EcommerceApp.Mapping;
using EcommerceApp.Repositories.Contracts;
using EcommerceApp.Services.Contracts;
using ECommerceAppTest.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static EcommerceApp.Entities.APIResponses.APICategoriesResponses;

namespace ECommerceAppTest.EndPointsTest.Categories
{
    public class CategoriesControllerTests
    {
        private readonly Mock<ICategoryService> categoryRepositoryMock;
        private readonly CategoriesController categoriesController;
        private readonly IMapper mapper;
        private readonly Mock<ILogger<CategoriesController>> logger;

        public CategoriesControllerTests()
        {
            categoryRepositoryMock = new Mock<ICategoryService>();
            logger = new Mock<ILogger<CategoriesController>>();

            var _mapper = new MapperConfiguration(mc => mc.AddProfile(new AutoMapperProfile())).CreateMapper();
            mapper = _mapper;

            categoriesController = new CategoriesController(categoryRepositoryMock.Object, mapper, logger.Object);
        }

        //GetAllProducts Tests
        [Fact]
        public async Task Test_GetAllCategories_Returns_OKResponse()
        {
            //Arrange
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
            var categoriesDto = mapper.Map<List<CategoryResponseDTO>>(categories);

            categoryRepositoryMock.Setup(p => p.GetAllCategories()).ReturnsAsync(categories);
            //Act
            var categoriesData = await categoriesController.GetAllCategories();
            var categoriesWithOkObj = categoriesData as OkObjectResult;
            var categoriesList = categoriesWithOkObj?.Value as List<CategoryResponseDTO>;

            //Assert
            Assert.NotNull(categoriesData);
            Assert.IsType<OkObjectResult>(categoriesWithOkObj);
            Assert.Equal((int)HttpStatusCode.OK, categoriesWithOkObj.StatusCode);

            Assert.IsType<List<CategoryResponseDTO>>(categoriesList);
            Assert.Equal(categoriesDto[0].Id, categoriesList[0].Id);
            Assert.Equal(categoriesDto[0].Name, categoriesList[0].Name);
            Assert.Equal(2, categoriesList.Count);
        }
        [Fact]
        public async Task Test_GetAllProducts_Returns_OKResponseWithEmptyResult()
        {
            //Arrange
            var categories = new List<Category>();
            var count = categories.Count;

            categoryRepositoryMock.Setup(p => p.GetAllCategories()).ReturnsAsync(categories);
            //Act
            var categoriesData = await categoriesController.GetAllCategories();
            var categoriesWithOkObj = categoriesData as OkObjectResult;
            var categoriesList = categoriesWithOkObj?.Value as AllCategorysResponse;

            //Assert
            Assert.NotNull(categoriesData);
            Assert.IsType<OkObjectResult>(categoriesWithOkObj);
            Assert.IsType<AllCategorysResponse>(categoriesList);
            Assert.Equal((int)HttpStatusCode.OK, categoriesWithOkObj.StatusCode);

            //Additional assertions
            Assert.Equal(ResponseConstantsTest.NoRecord, categoriesList.Message);
            Assert.Equal(count, categoriesList.CategoryCount);

        }
        [Fact]
        public async Task Test_GetAllProducts_Returns_ServerErrorResponse()
        {
            //Arrange
            categoryRepositoryMock.Setup(p => p.GetAllCategories()).
                Throws(new Exception("Simulated internal server error"));

            //Act
            var categoryData = await categoriesController.GetAllCategories();
            var errorResponse = categoryData as ObjectResult;
            var errorContent = errorResponse?.Value as ProblemDetails;

            //Assert
            Assert.IsType<ObjectResult>(categoryData);
            Assert.IsType<ObjectResult>(errorResponse);
            Assert.Equal((int)HttpStatusCode.InternalServerError, errorResponse.StatusCode);
            Assert.Equal(ResponseConstantsTest.ServerError, errorContent?.Detail);
        }

        //GetCategoryById
        [Theory]
        [InlineData(4)]
        public async Task Test_GetProductById_Returns_OKResponse(int id)
        {
            //Arrange
            var category = new Category
            {
                Id = 4,
                Name = "Mobile",
            };
            var categoryDto = mapper.Map<CategoryResponseDTO>(category);
            categoryRepositoryMock.Setup(p => p.GetCategoryById(id)).ReturnsAsync(category);

            //Act
            var categoryData = await categoriesController.GetCategoryById(id);
            var pcategoryWithOkObj = categoryData as OkObjectResult;
            var categoryResult = pcategoryWithOkObj?.Value as CategoryResponseDTO;

            //Assert
            Assert.IsType<OkObjectResult>(pcategoryWithOkObj);
            Assert.Equal((int)HttpStatusCode.OK, pcategoryWithOkObj.StatusCode);

            //Additional assertions
            Assert.IsType<CategoryResponseDTO>(categoryResult);
            Assert.Equal(categoryDto.Id, categoryResult.Id);
            Assert.Equal(categoryDto.Name, categoryResult.Name);
        }
        [Theory]
        [InlineData(200)]
        public async Task Test_GetProductById_Returns_NotFoundResponse(int id)
        {
            //Arrange
            var category = new Category
            {
                Id = 4,
                Name = "Mobile",
            };
            var categoryDto = mapper.Map<CategoryResponseDTO>(category);
            categoryRepositoryMock.Setup(p => p.GetCategoryById(id)).ReturnsAsync((Category?)null);

            //Act
            var categoryData = await categoriesController.GetCategoryById(id);
            var categoryNotFound = categoryData as NotFoundObjectResult;
            var categoryResponse = categoryNotFound?.Value as CategoryNotFoundResponse;

            //Assert
            Assert.IsType<NotFoundObjectResult>(categoryNotFound);
            Assert.Equal((int)HttpStatusCode.NotFound, categoryNotFound.StatusCode);

            //Additional assertions
            Assert.IsType<CategoryNotFoundResponse>(categoryResponse);
            Assert.NotEqual(categoryDto.Id, categoryResponse.CategoryId);
            Assert.Equal(ResponseConstantsTest.NoRecordWithCategoryId, categoryResponse.Message);
        }
        [Theory]
        [InlineData(0)]
        public async Task Test_GetProductById_Returns_BadRequestForArgumetException(int id)
        {
            //Arrange
            categoryRepositoryMock.Setup(p => p.GetCategoryById(id)).
                Throws(new ArgumentNullException(nameof(id)));

            //Act
            var categoryData = await categoriesController.GetCategoryById(id); //valid id passed
            var errorResponse = categoryData as BadRequestObjectResult;
            var errorContent = errorResponse?.Value as CategoryErrorResponse;

            //Assert
            Assert.IsType<BadRequestObjectResult>(categoryData);
            Assert.IsType<BadRequestObjectResult>(errorResponse);
            Assert.IsType<CategoryErrorResponse>(errorContent);
            Assert.Equal((int)HttpStatusCode.BadRequest, errorResponse.StatusCode);
            Assert.Equal(id, errorContent.CategoryId);
            Assert.Equal(ResponseConstantsTest.NullOrInvalidCategoryId, errorContent.ErrorMessage);
        }
        [Theory]
        [InlineData(4)]
        public async Task Test_GetProductById_Returns_ServerErrorResponse(int id)
        {
            //Arrange
            categoryRepositoryMock.Setup(p => p.GetCategoryById(id)).
                Throws(new Exception("Simulated internal server error"));

            //Act
            var categoryData = await categoriesController.GetCategoryById(id); //valid id passed
            var errorResponse = categoryData as ObjectResult;
            var errorContent = errorResponse?.Value as ProblemDetails;

            //Assert
            Assert.IsType<ObjectResult>(categoryData);
            Assert.IsType<ObjectResult>(errorResponse);
            Assert.Equal((int)HttpStatusCode.InternalServerError, errorResponse.StatusCode);
            Assert.Equal(ResponseConstantsTest.ServerError, errorContent?.Detail);
        }

        //AddCategory tests
        [Fact]
        public async Task Test_AddCategory_Returns_CreatedResponse()
        {
            //Arrange
            var categoryReqDTO = new CategoryRequestDTO
            {
                Name = "Mobile",
            };
            var categoryResDTO = new CategoryResponseDTO
            {
                Name = "Mobile",
            };
            var category = mapper.Map<Category>(categoryReqDTO);
            categoryRepositoryMock.Setup(p => p.AddCategory(It.IsAny<Category>())).ReturnsAsync(category);

            //Act
            var categoryData = await categoriesController.AddCategory(categoryReqDTO);
            var categoryWithCreatedResult = categoryData as CreatedAtActionResult;
            var categorysResult = categoryWithCreatedResult?.Value as CategoryResponseDTO;

            //Asserts
            Assert.NotNull(categoryData);
            Assert.NotNull(categoryWithCreatedResult);
            Assert.IsType<CreatedAtActionResult>(categoryWithCreatedResult);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(categoryData);
            Assert.Equal((int)HttpStatusCode.Created, createdAtActionResult.StatusCode);
            Assert.Equal(nameof(CategoriesController.GetCategoryById), createdAtActionResult.ActionName);
            Assert.Equal(categoryResDTO.Name, categorysResult?.Name);

        }
        [Fact]
        public async Task Test_AddCategoryt_Returns_BadRequestResponse() //If same category is already present
        {
            //Arrange
            var categoryReqDTO = new CategoryRequestDTO
            {
                Name = "Nike sneaker",
            };
            var category = mapper.Map<Category>(categoryReqDTO);
            categoryRepositoryMock.Setup(p => p.AddCategory(category)).ReturnsAsync((Category?)null);

            //Act
            var caregoryData = await categoriesController.AddCategory(categoryReqDTO);
            var caregoryWithBadReqResult = caregoryData as BadRequestObjectResult;
            var caregorysBadReqResult = caregoryWithBadReqResult?.Value as CategoryAddFailureResponse;

            Assert.IsType<BadRequestObjectResult>(caregoryData);
            Assert.IsType<BadRequestObjectResult>(caregoryWithBadReqResult);
            Assert.IsType<CategoryAddFailureResponse>(caregorysBadReqResult);
            Assert.Equal((int)HttpStatusCode.BadRequest, caregoryWithBadReqResult.StatusCode);
            Assert.Equal(categoryReqDTO.Name, caregorysBadReqResult.CategoryName);
            Assert.Equal(ResponseConstantsTest.CategoryAlreadyPresent, caregorysBadReqResult.ErrorMessage);
        }
        [Fact]
        public async Task Test_AddCategory_Returns_BadReqResponseForDBUpdateFailure() // Not existing category id provided
        {
            //Arrange
            var categoryReqDTO = new CategoryRequestDTO
            {
                Name = "Mobile"
            };
            categoryRepositoryMock.Setup(p => p.AddCategory(It.IsAny<Category>())).
            Throws(new DbUpdateException());

            //Act
            var categoryData = await categoriesController.AddCategory(categoryReqDTO);
            var categoryWithBadReqResult = categoryData as BadRequestObjectResult;
            var categorysBadReqResult = categoryWithBadReqResult?.Value as CategoryAddFailureResponse;

            Assert.IsType<BadRequestObjectResult>(categoryData);
            Assert.IsType<BadRequestObjectResult>(categoryWithBadReqResult);
            Assert.IsType<CategoryAddFailureResponse>(categorysBadReqResult);
            Assert.Equal((int)HttpStatusCode.BadRequest, categoryWithBadReqResult.StatusCode);
            Assert.Equal(categoryReqDTO.Name, categorysBadReqResult.CategoryName);
            Assert.Equal(ResponseConstantsTest.CategoryNotAdded, categorysBadReqResult.ErrorMessage);
        }
        [Fact]
        public async Task Test_AddCategory_Returns_ServerErrorResponse()
        {
            //Arrange
            var categoryReqDTO = new CategoryRequestDTO
            {
                Name = "Mobile"
            };
            categoryRepositoryMock.Setup(p => p.AddCategory(It.IsAny<Category>())).Throws(new Exception("Demo Exception"));

            //Act
            var categoryData = await categoriesController.AddCategory(categoryReqDTO);
            var categoryWithServerErrorResult = categoryData as ObjectResult;
            var categoryErrorResult = categoryWithServerErrorResult?.Value as ProblemDetails;

            Assert.IsType<ObjectResult>(categoryData);
            Assert.IsType<ObjectResult>(categoryWithServerErrorResult);
            Assert.IsType<ProblemDetails>(categoryErrorResult);
            Assert.Equal((int)HttpStatusCode.InternalServerError, categoryWithServerErrorResult.StatusCode);
            Assert.Equal(ResponseConstantsTest.ServerError, categoryErrorResult.Detail);
        }

        //UpdateCategory tests
        [Theory]
        [InlineData(1)]
        public async Task Test_UpdateCategory_Returns_OKResponse(int id)
        {
            //Arrange
            var categoryReqDTO = new CategoryRequestDTO
            {
                Name = "Mobile",
            };
            categoryRepositoryMock.Setup(p => p.UpdateCategory(id, It.IsAny<Category>())).ReturnsAsync(id);

            //Act
            var categoryData = await categoriesController.UpdateCategory(id, categoryReqDTO);
            var categoryUpdateOkObjResult = categoryData as OkObjectResult;
            var categorysSuccessResult = categoryUpdateOkObjResult?.Value as CategorySuccessResponse;

            //Asserts
            Assert.NotNull(categoryData);
            Assert.NotNull(categoryUpdateOkObjResult);
            Assert.IsType<OkObjectResult>(categoryUpdateOkObjResult);
            Assert.IsType<CategorySuccessResponse>(categorysSuccessResult);
            Assert.Equal((int)HttpStatusCode.OK, categoryUpdateOkObjResult.StatusCode);

            //Additional assertions
            Assert.Equal(id, categorysSuccessResult.CategoryId);
            Assert.Equal(ResponseConstantsTest.CategoryUpdateSuccess, categorysSuccessResult.Message);
        }
        [Theory]
        [InlineData(78)]
        public async Task Test_UpdateCategory_Returns_NotFoundResponse(int id)
        {
            var returnValue = -1;
            //Arrange
            var categoryReqDTO = new CategoryRequestDTO
            {
                Name = "Mobile",
            };

            categoryRepositoryMock.Setup(p => p.UpdateCategory(id, It.IsAny<Category>())).ReturnsAsync(returnValue);

            //Act
            var categoryData = await categoriesController.UpdateCategory(id, categoryReqDTO);
            var categoryUpdateNotFoundResult = categoryData as NotFoundObjectResult;
            var categorysUpdateFailResult = categoryUpdateNotFoundResult?.Value as CategoryNotFoundResponse;

            //Asserts
            Assert.NotNull(categoryData);
            Assert.NotNull(categoryUpdateNotFoundResult);
            Assert.IsType<NotFoundObjectResult>(categoryUpdateNotFoundResult);
            Assert.IsType<CategoryNotFoundResponse>(categorysUpdateFailResult);
            Assert.Equal((int)HttpStatusCode.NotFound, categoryUpdateNotFoundResult.StatusCode);

            //Additional assertions
            Assert.Equal(id, categorysUpdateFailResult.CategoryId);
            Assert.Equal(ResponseConstantsTest.NoRecordWithCategoryId, categorysUpdateFailResult.Message);
        }
        [Theory]
        [InlineData(1)]
        public async Task Test_UpdateCategory_Returns_BadRequestResponse(int id) 
        {
            //Arrange
            var categoryReqDTO = new CategoryRequestDTO
            {
                Name = "Mobile",
            };

            categoryRepositoryMock.Setup(p => p.UpdateCategory(id, It.IsAny<Category>())).Throws(new DbUpdateException());

            //Act
            var categoryData = await categoriesController.UpdateCategory(id, categoryReqDTO);
            var categoryUpdateBadRequestResult = categoryData as BadRequestObjectResult;
            var categoryUpdateFailResult = categoryUpdateBadRequestResult?.Value as CategoryUpdateFailResponse;

            //Asserts
            Assert.NotNull(categoryData);
            Assert.NotNull(categoryUpdateBadRequestResult);
            Assert.IsType<BadRequestObjectResult>(categoryUpdateBadRequestResult);
            Assert.IsType<CategoryUpdateFailResponse>(categoryUpdateFailResult);
            Assert.Equal((int)HttpStatusCode.BadRequest, categoryUpdateBadRequestResult.StatusCode);

            //Additional assertions
            Assert.Equal(id, categoryUpdateFailResult.CategoryId);
            Assert.Equal(ResponseConstantsTest.CategoryUpdateFailed, categoryUpdateFailResult.ErrorMessage);
        }
        [Theory]
        [InlineData(0)]
        public async Task Test_UpdateCategory_Returns_BadRequestForArgumentException(int id) //For Null, zero or less than zero product id
        {
            //Arrange
            var categoryReqDTO = new CategoryRequestDTO
            {
                Name = "Mobile",
            };
            categoryRepositoryMock.Setup(p => p.UpdateCategory(id, It.IsAny<Category>())).
                Throws(new ArgumentNullException(nameof(id)));

            //Act
            var categoryData = await categoriesController.UpdateCategory(id, categoryReqDTO);
            var categoryUpdateBadRequestResult = categoryData as BadRequestObjectResult;
            var categoryErrorResult = categoryUpdateBadRequestResult?.Value as CategoryErrorResponse;

            //Asserts
            Assert.NotNull(categoryData);
            Assert.NotNull(categoryUpdateBadRequestResult);
            Assert.IsType<BadRequestObjectResult>(categoryUpdateBadRequestResult);
            Assert.IsType<CategoryErrorResponse>(categoryErrorResult);
            Assert.Equal((int)HttpStatusCode.BadRequest, categoryUpdateBadRequestResult.StatusCode);

            //Additional assertions
            Assert.Equal(id, categoryErrorResult.CategoryId);
            Assert.Equal(ResponseConstantsTest.NullOrInvalidCategoryId, categoryErrorResult.ErrorMessage);
        }
        [Theory]
        [InlineData(1)]
        public async Task Test_UpdateCategory_Returns_ServerErrorResponse(int id)
        {
            //Arrange
            var categoryReqDTO = new CategoryRequestDTO
            {
                Name = "Mobile",
            };
            categoryRepositoryMock.Setup(p => p.UpdateCategory(id, It.IsAny<Category>())).Throws(new Exception());

            //Act
            var categoryData = await categoriesController.UpdateCategory(id, categoryReqDTO);
            var categoryUpdateObjResult = categoryData as ObjectResult;
            var categoryErrorResult = categoryUpdateObjResult?.Value as ProblemDetails;

            //Asserts
            Assert.NotNull(categoryData);
            Assert.NotNull(categoryUpdateObjResult);
            Assert.IsType<ObjectResult>(categoryUpdateObjResult);
            Assert.IsType<ProblemDetails>(categoryErrorResult);
            Assert.Equal((int)HttpStatusCode.InternalServerError, categoryUpdateObjResult.StatusCode);
            Assert.Equal(ResponseConstantsTest.ServerError, categoryErrorResult.Detail);
        }

        //DeleteCategpory tests
        [Theory]
        [InlineData(4)]
        public async Task Test_DeleteCategory_Returns_OKResponse(int id)
        {
            //Arrange
            categoryRepositoryMock.Setup(p => p.DeleteCategory(id)).ReturnsAsync(id);

            //Act
            var categoryData = await categoriesController.DeleteCategory(id);
            var categoryDeleteOkObjResult = categoryData as OkObjectResult;
            var categorysSuccessResult = categoryDeleteOkObjResult?.Value as CategorySuccessResponse;

            //Asserts
            Assert.NotNull(categoryData);
            Assert.NotNull(categoryDeleteOkObjResult);
            Assert.IsType<OkObjectResult>(categoryDeleteOkObjResult);
            Assert.IsType<CategorySuccessResponse>(categorysSuccessResult);
            Assert.Equal((int)HttpStatusCode.OK, categoryDeleteOkObjResult.StatusCode);

            //Additional assertions
            Assert.Equal(id, categorysSuccessResult.CategoryId);
            Assert.Equal(ResponseConstantsTest.CategoryDeleteSuccess, categorysSuccessResult.Message);
        }
        [Theory]
        [InlineData(78)]
        public async Task Test_DeleteCategory_Returns_NotFoundResponse(int id)
        {
            var returnValue = -1;
            //Arrange
            categoryRepositoryMock.Setup(p => p.DeleteCategory(id)).ReturnsAsync(returnValue);

            //Act
            var categoryData = await categoriesController.DeleteCategory(id);
            var categoryDeleteNotFoundResult = categoryData as NotFoundObjectResult;
            var categorysUpdateFailResult = categoryDeleteNotFoundResult?.Value as CategoryNotFoundResponse;

            //Asserts
            Assert.NotNull(categoryData);
            Assert.NotNull(categoryDeleteNotFoundResult);
            Assert.IsType<NotFoundObjectResult>(categoryDeleteNotFoundResult);
            Assert.IsType<CategoryNotFoundResponse>(categorysUpdateFailResult);
            Assert.Equal((int)HttpStatusCode.NotFound, categoryDeleteNotFoundResult.StatusCode);

            //Additional assertions
            Assert.Equal(id, categorysUpdateFailResult.CategoryId);
            Assert.Equal(ResponseConstantsTest.NoRecordWithCategoryId, categorysUpdateFailResult.Message);
        }
        [Theory]
        [InlineData(0)]
        public async Task Test_DeleteCategory_Returns_BadRequestResponse(int id)
        {
            //Arrange
            categoryRepositoryMock.Setup(p => p.DeleteCategory(id)).Throws(new ArgumentNullException(nameof(id)));

            //Act
            var categoryData = await categoriesController.DeleteCategory(id);
            var categoryDeleteBadRequestResult = categoryData as BadRequestObjectResult;
            var categoryDeleteFailResult = categoryDeleteBadRequestResult?.Value as CategoryErrorResponse;

            //Asserts
            Assert.NotNull(categoryData);
            Assert.NotNull(categoryDeleteBadRequestResult);
            Assert.IsType<BadRequestObjectResult>(categoryDeleteBadRequestResult);
            Assert.IsType<CategoryErrorResponse>(categoryDeleteFailResult);
            Assert.Equal((int)HttpStatusCode.BadRequest, categoryDeleteBadRequestResult.StatusCode);

            //Additional assertions
            Assert.Equal(id, categoryDeleteFailResult.CategoryId);
            Assert.Equal(ResponseConstantsTest.NullOrInvalidCategoryId, categoryDeleteFailResult.ErrorMessage);
        }
        [Theory]
        [InlineData(4)]
        public async Task Test_DeleteCategory_Returns_ServerErrorResponse(int id)
        {
            //Arrange
            categoryRepositoryMock.Setup(p => p.DeleteCategory(id)).Throws(new Exception());

            //Act
            var categoryData = await categoriesController.DeleteCategory(id);
            var categoryDeleteObjResult = categoryData as ObjectResult;
            var categoryErrorResult = categoryDeleteObjResult?.Value as ProblemDetails;

            //Asserts
            Assert.NotNull(categoryData);
            Assert.NotNull(categoryDeleteObjResult);
            Assert.IsType<ObjectResult>(categoryDeleteObjResult);
            Assert.IsType<ProblemDetails>(categoryErrorResult);
            Assert.Equal((int)HttpStatusCode.InternalServerError, categoryDeleteObjResult.StatusCode);
            Assert.Equal(ResponseConstantsTest.ServerError, categoryErrorResult.Detail);
        }
    }
}
