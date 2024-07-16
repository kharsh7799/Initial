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
using ECommerceAppTest.DataSetUp;
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
using System.Web.Http.Results;
using static EcommerceApp.Entities.APIResponses.APICategoriesResponses;

namespace ECommerceAppTest.EndPointsTest.Categories
{
    public class CategoriesControllerTests
    {
        private readonly Mock<ICategoryService> categoryServiceMock;
        private readonly CategoriesController categoriesController;
        private readonly IMapper mapper;
        private readonly Mock<ILogger<CategoriesController>> logger;
        private readonly InitialCategoryDataSetUp categoryDataSetUp;
        private readonly List<Category> categories;
        private readonly List<Category> categoriesWithProducts;
        private readonly CategoryRequestDTO categoryRequestDTO;
        private readonly CategoryResponseDTO categoryResponseDTO;


        public CategoriesControllerTests()
        {
            categoryServiceMock = new Mock<ICategoryService>();
            logger = new Mock<ILogger<CategoriesController>>();

            var _mapper = new MapperConfiguration(mc => mc.AddProfile(new AutoMapperProfile())).CreateMapper();
            mapper = _mapper;

            categoriesController = new CategoriesController(categoryServiceMock.Object, mapper, logger.Object);
            categoryDataSetUp = new InitialCategoryDataSetUp();
            categories = categoryDataSetUp.CategoryList;
            categoriesWithProducts = categoryDataSetUp.CategoriesWithProductsList;
            categoryRequestDTO = categoryDataSetUp.CategoryRequestDTO;
            categoryResponseDTO= categoryDataSetUp.CategoryResponseDTO;
        }

        private Category? GetCategoryById(int id)
        {
            return categories.Find(x => x.Id == id);
        }
        [Fact]
        public async Task Test_GetAllCategories_Returns_OKResponse()
        {
            //Arrange
            categoryServiceMock.Setup(p => p.GetAllCategories()).ReturnsAsync(categories);
            var categoriesDto = mapper.Map<List<CategoryResponseDTO>>(categories);
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
            Assert.Equal(categories.Count, categoriesList.Count);
        }
        [Fact]
        public async Task Test_GetAllCategories_Returns_NotFoundResponse_WithEmptyResult()
        {
            //Arrange
            var categoryList = new List<Category>();
            categoryServiceMock.Setup(p => p.GetAllCategories()).ReturnsAsync(categoryList);
            //Act
            var categoriesData = await categoriesController.GetAllCategories();
            var categoriesWithNotFoundObj = categoriesData as NotFoundObjectResult;
            var categoriesListResult = categoriesWithNotFoundObj?.Value as AllCategorysResponse;

            //Assert
            Assert.NotNull(categoriesData);
            Assert.IsType<NotFoundObjectResult>(categoriesWithNotFoundObj);
            Assert.IsType<AllCategorysResponse>(categoriesListResult);
            Assert.Equal((int)HttpStatusCode.NotFound, categoriesWithNotFoundObj.StatusCode);

            //Additional assertions
            Assert.Equal(ResponseConstantsTest.NoRecord, categoriesListResult.Message);
            Assert.Equal(categoryList.Count, categoriesListResult.CategoryCount);

        }
        //GetCategoryById
        [Fact]
        public async Task Test_GetCategoryById_Returns_OKResponse()
        {
            //Arrange
            var id = 3;
            var category = GetCategoryById(id);
            categoryServiceMock.Setup(p => p.GetCategoryById(id)).ReturnsAsync(category);
            var categoryDto = mapper.Map<CategoryResponseDTO>(category);
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
        [Fact]
        public async Task Test_GetCategoryById_Returns_NotFoundResponse()
        {
            //Arrange
            var id = 560;
            categoryServiceMock.Setup(p => p.GetCategoryById(id)).ReturnsAsync((Category?)null);
            //Act
            var categoryData = await categoriesController.GetCategoryById(id);
            var categoryNotFound = categoryData as NotFoundObjectResult;
            var categoryResult = categoryNotFound?.Value as CategoryNotFoundResponse;

            //Assert
            Assert.IsType<NotFoundObjectResult>(categoryNotFound);
            Assert.Equal((int)HttpStatusCode.NotFound, categoryNotFound.StatusCode);
            //Additional assertions
            Assert.IsType<CategoryNotFoundResponse>(categoryResult);
            Assert.Equal(id, categoryResult.CategoryId);
            Assert.Equal(ResponseConstantsTest.NoRecordWithCategoryId, categoryResult.Message);
        }
        [Theory]
        [InlineData(0)]
        [InlineData(-4000)]
        public async Task Test_GetCategoryById_Returns_BadRequest_WhenInvlaidIdProvided(int id)
        {
            //Arrange
            //Act
            var categoryData = await categoriesController.GetCategoryById(id); 
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

        //AddCategory tests
        [Fact]
        public async Task Test_AddCategory_Returns_CreatedResponse()
        {
            //Arrange
            var categoryModel = mapper.Map<Category>(categoryRequestDTO);
            categoryServiceMock.Setup(p => p.AddCategory(It.IsAny<Category>())).ReturnsAsync(categoryModel);

            //Act
            var categoryData = await categoriesController.AddCategory(categoryRequestDTO);
            var categoryWithCreatedResult = categoryData as CreatedAtActionResult;
            var categorysResult = categoryWithCreatedResult?.Value as CategoryResponseDTO;

            //Asserts
            Assert.NotNull(categoryData);
            Assert.NotNull(categoryWithCreatedResult);
            Assert.IsType<CreatedAtActionResult>(categoryWithCreatedResult);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(categoryData);
            Assert.Equal((int)HttpStatusCode.Created, createdAtActionResult.StatusCode);
            Assert.Equal(nameof(CategoriesController.GetCategoryById), createdAtActionResult.ActionName);
            Assert.Equal(categoryResponseDTO.Name, categorysResult?.Name);

        }
        [Fact]
        public async Task Test_AddCategoryt_Returns_BadRequest_WhenProductIsAlreadyPresent() //If same category is already present
        {
            //Arrange
            categoryRequestDTO.Name = "Food";
            categoryServiceMock.Setup(p => p.AddCategory(It.IsAny<Category>())).ReturnsAsync((Category?)null);

            //Act
            var caregoryData = await categoriesController.AddCategory(categoryRequestDTO);
            var caregoryWithBadReqResult = caregoryData as BadRequestObjectResult;
            var caregorysBadReqResult = caregoryWithBadReqResult?.Value as CategoryAddFailureResponse;

            Assert.IsType<BadRequestObjectResult>(caregoryData);
            Assert.IsType<BadRequestObjectResult>(caregoryWithBadReqResult);
            Assert.IsType<CategoryAddFailureResponse>(caregorysBadReqResult);
            Assert.Equal((int)HttpStatusCode.BadRequest, caregoryWithBadReqResult.StatusCode);
            Assert.Equal(categoryRequestDTO.Name, caregorysBadReqResult.CategoryName);
            Assert.Equal(ResponseConstantsTest.CategoryAlreadyPresent, caregorysBadReqResult.ErrorMessage);
        }

        //UpdateCategory tests
        [Fact]
        public async Task Test_UpdateCategory_Returns_OKResponse()
        {
            //Arrange
            var id = 4;
            categoryServiceMock.Setup(p => p.UpdateCategory(id, It.IsAny<Category>())).ReturnsAsync(id);

            //Act
            var categoryData = await categoriesController.UpdateCategory(id, categoryRequestDTO);
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
        [Fact]
        public async Task Test_UpdateCategory_Returns_NotFoundResponse()
        {
            var id = 356;
            var returnValue = -1;
            //Arrange
            categoryServiceMock.Setup(p => p.UpdateCategory(id, It.IsAny<Category>())).ReturnsAsync(returnValue);

            //Act
            var categoryData = await categoriesController.UpdateCategory(id, categoryRequestDTO);
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
        [InlineData(0)]
        [InlineData(-27)]
        public async Task Test_UpdateCategory_Returns_BadRequest_WhenInvalidIdProvided(int id) //For Null, zero or less than zero product id
        {
            //Arrange
            //Act
            var categoryData = await categoriesController.UpdateCategory(id, categoryRequestDTO);
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

        //DeleteCategpory tests
        [Fact]
        public async Task Test_DeleteCategory_Returns_OKResponse()
        {
            //Arrange
            var idToDelete = 4;
            categoryServiceMock.Setup(p => p.DeleteCategory(idToDelete)).ReturnsAsync(idToDelete);

            //Act
            var categoryData = await categoriesController.DeleteCategory(idToDelete);
            var categoryDeleteOkObjResult = categoryData as OkObjectResult;
            var categorysSuccessResult = categoryDeleteOkObjResult?.Value as CategorySuccessResponse;

            //Asserts
            Assert.NotNull(categoryData);
            Assert.NotNull(categoryDeleteOkObjResult);
            Assert.IsType<OkObjectResult>(categoryDeleteOkObjResult);
            Assert.IsType<CategorySuccessResponse>(categorysSuccessResult);
            Assert.Equal((int)HttpStatusCode.OK, categoryDeleteOkObjResult.StatusCode);

            //Additional assertions
            Assert.Equal(idToDelete, categorysSuccessResult.CategoryId);
            Assert.Equal(ResponseConstantsTest.CategoryDeleteSuccess, categorysSuccessResult.Message);
        }
        [Fact]
        public async Task Test_DeleteCategory_Returns_NotFoundResponse()
        {
            var id = 90;
            var returnValue = -1;
            //Arrange
            categoryServiceMock.Setup(p => p.DeleteCategory(id)).ReturnsAsync(returnValue);

            //Act
            var categoryData = await categoriesController.DeleteCategory(id);
            var categoryDeleteNotFoundResult = categoryData as NotFoundObjectResult;
            var categorysDeleteFailResult = categoryDeleteNotFoundResult?.Value as CategoryNotFoundResponse;

            //Asserts
            Assert.NotNull(categoryData);
            Assert.NotNull(categoryDeleteNotFoundResult);
            Assert.IsType<NotFoundObjectResult>(categoryDeleteNotFoundResult);
            Assert.IsType<CategoryNotFoundResponse>(categorysDeleteFailResult);
            Assert.Equal((int)HttpStatusCode.NotFound, categoryDeleteNotFoundResult.StatusCode);

            //Additional assertions
            Assert.Equal(id, categorysDeleteFailResult.CategoryId);
            Assert.Equal(ResponseConstantsTest.NoRecordWithCategoryId, categorysDeleteFailResult.Message);
        }
        [Theory]
        [InlineData(0)]
        [InlineData(-75)]
        public async Task Test_DeleteCategory_Returns_BadRequestResponse_WhenInvlaidIdPriovided(int id)
        {
            //Arrange
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

        [Fact]
        public async Task Test_GetCategoriesWithProducts_Returns_OKResponse()
        {
            //Arrange
            categoryServiceMock.Setup(p => p.GetAllCategoriesWithProducts()).ReturnsAsync(categoriesWithProducts);
            var categoriesDto = mapper.Map<List<CategoryWithProductsResDTO>>(categoriesWithProducts);

            //Act
            var categoryWithproductsData = await categoriesController.GetAllCategoriesWithProducts();
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
        public async Task Test_GetCategoriesWithProducts_EmptyResultResponse()
        {
            //Arrange
            var categoriesWithProductsEmpty = new List<Category>();
            categoryServiceMock.Setup(p => p.GetAllCategoriesWithProducts()).ReturnsAsync(categoriesWithProductsEmpty);

            //Act
            var categoryData = await categoriesController.GetAllCategoriesWithProducts();
            var notFoundResult = categoryData as NotFoundObjectResult;
            var categoryResult = notFoundResult?.Value as AllCategorysResponse;

            //Assert
            Assert.IsType<NotFoundObjectResult>(categoryData);
            Assert.IsType<NotFoundObjectResult>(notFoundResult);
            Assert.Equal((int)HttpStatusCode.NotFound, notFoundResult.StatusCode);
            Assert.IsType<AllCategorysResponse>(categoryResult);
            Assert.Equal(ResponseConstantsTest.NoRecord, categoryResult.Message);
            Assert.Equal(categoriesWithProductsEmpty.Count, categoryResult.CategoryCount);


        }
        [Fact]
        public async Task Test_GetCategoryWithProducts_OKResponse()
        {
            //Arrange
            var categoryId = 1;
            var categoryWithproduct = categoriesWithProducts.Where(x => x.Id == categoryId).ToList();
            categoryServiceMock.Setup(p => p.GetCategoryWithProducts(categoryId)).ReturnsAsync(categoryWithproduct);

            //Act
            var categoryData = await categoriesController.GetCategoryWithProducts(categoryId);
            var okObjDataResult = categoryData as OkObjectResult;
            var categoryWithProductsData = okObjDataResult?.Value as List<CategoryWithProductsResDTO>;
            //Assert
            Assert.IsType<OkObjectResult>(categoryData);
            Assert.IsType<OkObjectResult>(okObjDataResult);
            Assert.Equal((int)HttpStatusCode.OK, okObjDataResult.StatusCode);
            Assert.IsType<List<CategoryWithProductsResDTO>>(categoryWithProductsData);
            Assert.Equal(categoryId, categoryWithProductsData[0].Id);
        }
        [Fact]
        public async Task Test_GetCategoryWithProducts_NotFoundResponse()
        {
            //Arrange
            var categoryId = 421;
            var categoryWithproduct = categoriesWithProducts.Where(x => x.Id == categoryId).ToList();
            categoryServiceMock.Setup(p => p.GetCategoryWithProducts(categoryId)).ReturnsAsync(categoryWithproduct);

            //Act
            var categoryData = await categoriesController.GetCategoryWithProducts(categoryId);
            var NotFoundData = categoryData as NotFoundObjectResult;
            var errorContent = NotFoundData?.Value as CategoryNotFoundResponse;

            //Assert
            Assert.IsType<NotFoundObjectResult>(categoryData);
            Assert.IsType<NotFoundObjectResult>(NotFoundData);
            Assert.IsType<CategoryNotFoundResponse>(errorContent);
            Assert.Equal((int)HttpStatusCode.NotFound, NotFoundData.StatusCode);
            Assert.Equal(categoryId, errorContent.CategoryId);
            Assert.Equal(ResponseConstantsTest.NoRecordWithCategoryId, errorContent.Message);
        }
        [Theory]
        [InlineData(0)]
        [InlineData(-20)]
        public async Task Test_GetCategoryWithProducts_BadRequest_WhenInvalidcategoryIdProvided(int categoryId)
        {
            //Arrange
            //Act
            var categoryData = await categoriesController.GetCategoryWithProducts(categoryId);
            var errorResponse = categoryData as BadRequestObjectResult;
            var errorContent = errorResponse?.Value as CategoryErrorResponse;

            //Assert
            Assert.IsType<BadRequestObjectResult>(categoryData);
            Assert.IsType<BadRequestObjectResult>(errorResponse);
            Assert.Equal((int)HttpStatusCode.BadRequest, errorResponse.StatusCode);
            Assert.Equal(categoryId, errorContent?.CategoryId);
            Assert.Equal(ResponseConstantsTest.NullOrInvalidCategoryId, errorContent?.ErrorMessage);
        }

    }
}
