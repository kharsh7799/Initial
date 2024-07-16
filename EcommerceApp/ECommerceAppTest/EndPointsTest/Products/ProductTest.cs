using AutoMapper;
using EcommerceApp.Controllers;
using EcommerceApp.Entities.APIResponses;
using EcommerceApp.Entities.DomainModels;
using EcommerceApp.Entities.DTOs.Category;
using EcommerceApp.Entities.DTOs.Product;
using EcommerceApp.Mapping;
using EcommerceApp.Repositories.Contracts;
using EcommerceApp.Services.Contracts;
using ECommerceAppTest.Constants;
using ECommerceAppTest.DataSetUp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using static EcommerceApp.Entities.APIResponses.APICategoriesResponses;

namespace ECommerceAppTest.EndPointsTest.Products
{
    public class ProductTest
    {
        private readonly Mock<IProductService> productServiceMock;
        private readonly ProductsController productsController;
        private readonly IMapper mapper;
        private readonly Mock<ILogger<ProductsController>> logger;
        private readonly InitialProductDataSetUp productDataSetUp;
        private readonly List<Product> products;
        private readonly List<Category> category;
        private readonly ProductRequestDTO productRequestDTO;
        private readonly ProductResponseDTO productResponseDTO;

        public ProductTest()
        {
            productServiceMock =  new Mock<IProductService>();
            logger = new Mock<ILogger<ProductsController>>();

            var _mapper = new MapperConfiguration(mc => mc.AddProfile(new AutoMapperProfile())).CreateMapper();
            mapper = _mapper;

            productsController = new ProductsController(productServiceMock.Object, mapper, logger.Object);
            productDataSetUp = new InitialProductDataSetUp();
            this.products = productDataSetUp.ProductList;
            this.productRequestDTO=productDataSetUp.ProductRequestDTO;
            this.productResponseDTO=productDataSetUp.ProductResponseDTO;
            this.category = productDataSetUp.CategoryList;
        }
        private Product? GetProductById(int id)
        {
            return products?.Find(x => x.Id == id);
        }
        [Fact]
        public async Task Test_GetAllProducts_Returns_OKResponse()
        {
            //Arrange
            productServiceMock.Setup(p => p.GetAllProducts(null)).ReturnsAsync(products);
            var productDto = mapper.Map<List<ProductResponseDTO>>(products);

            //Act
            var productsData = await productsController.GetAllProducts(null);
            var productsWithOkObj = productsData as OkObjectResult;
            var ProductsList = productsWithOkObj?.Value as List<ProductResponseDTO>;

            //Assert
            Assert.IsType<OkObjectResult>(productsWithOkObj);
            Assert.Equal((int)HttpStatusCode.OK, productsWithOkObj.StatusCode);
            Assert.IsType<List<ProductResponseDTO>>(ProductsList);

            Assert.Equal(productDto[0].Id, ProductsList[0].Id);
            Assert.Equal(productDto[0].Name, ProductsList[0].Name);
            Assert.Equal(productDto[0].Rating, ProductsList[0].Rating);
            Assert.Equal(productDto[0].Price, ProductsList[0].Price);
            Assert.Equal(productDto.Count, ProductsList.Count);
        }
        [Fact]
        public async Task Test_GetAllProducts_Returns_NotFoundWithEmptyResult()
        {
            var productEmptyData = new List<Product>(); 
            //Arrange
            productServiceMock.Setup(p => p.GetAllProducts(null)).ReturnsAsync(productEmptyData);
            //Act
            var productsData = await productsController.GetAllProducts(null);
            var productsWithNotFoundObj = productsData as NotFoundObjectResult;
            var ProductsList = productsWithNotFoundObj?.Value as AllProductsResponse;

            //Assert
            Assert.IsType<NotFoundObjectResult>(productsWithNotFoundObj);
            Assert.Equal((int)HttpStatusCode.NotFound, productsWithNotFoundObj.StatusCode);
            Assert.Equal(ResponseConstantsTest.NoRecord, ProductsList?.Message);
            Assert.Equal(0, ProductsList?.ProductCount);

        }

        //GetProductByID Tests
        [Fact]
        public async Task Test_GetProductById_Returns_OKResponse()
        {
            //Arrange
            var id = 4;
            var product = GetProductById(id);
            productServiceMock.Setup(p => p.GetProductById(id)).ReturnsAsync(product);
            var productDto = mapper.Map<ProductResponseDTO>(product);

            //Act
            var productData = await productsController.GetProductById(id);
            var productWithOkObj = productData as OkObjectResult;
            var productResult = productWithOkObj?.Value as ProductResponseDTO;

            //Assert
            Assert.IsType<OkObjectResult>(productWithOkObj);
            Assert.Equal((int)HttpStatusCode.OK, productWithOkObj.StatusCode);
            Assert.IsType<ProductResponseDTO>(productResult);
            Assert.Equal(productDto.Id, productResult.Id);
            Assert.Equal(productDto.Name, productResult.Name);
            Assert.Equal(productDto.Rating, productResult.Rating);
            Assert.Equal(productDto.Price, productResult.Price);
            Assert.Equal(productDto.CategoryId, productResult.CategoryId);
        }
        [Fact]
        public async Task Test_GetProductById_Returns_NotFoundResponse()
        {
            //Arrange
            var id = 456;
            productServiceMock.Setup(p => p.GetProductById(id)).ReturnsAsync((Product?)null);

            //Act
            var productData = await productsController.GetProductById(id);
            var productNotFound = productData as NotFoundObjectResult;
            var ProductResponse = productNotFound?.Value as ProductNotFoundResponse;

            //Assert
            Assert.IsType<NotFoundObjectResult>(productNotFound);
            Assert.Equal((int)HttpStatusCode.NotFound, productNotFound.StatusCode);
            Assert.IsType<ProductNotFoundResponse>(ProductResponse);
            Assert.Equal(id, ProductResponse.ProductId);
            Assert.Equal(ResponseConstantsTest.NoRecordWithProductId, ProductResponse.Message);
        }
        [Theory]
        [InlineData(-976)]
        [InlineData(-80)]
        [InlineData(0)]
        public async Task Test_GetProductById_Returns_BadRequest_WhenInvalidIdProvided(int id)
        {
            //Arrange
            //Act
            var productData = await productsController.GetProductById(id); 
            var errorResponse = productData as BadRequestObjectResult;
            var errorContent = errorResponse?.Value as ProductIdInvalidResponse;

            //Assert
            Assert.IsType<BadRequestObjectResult>(productData);
            Assert.IsType<BadRequestObjectResult>(errorResponse);
            Assert.IsType<ProductIdInvalidResponse>(errorContent);
            Assert.Equal((int)HttpStatusCode.BadRequest, errorResponse.StatusCode);
            Assert.Equal(id, errorContent.ProductId);
            Assert.Equal(ResponseConstantsTest.NullOrInvalidProductId, errorContent.ErrorMessage);
            productServiceMock.Verify();
        }

        //GetProductsByCategoryId tests

        [Fact]
        public async Task Test_GetProductByCategoryId_Returns_OKResponse()
        {
            //Arrange
            var categoryId = 1;
            var productList = products.Where(x => x.CategoryId == categoryId).ToList();
            productServiceMock.Setup(p => p.GetProductByCategoryId(categoryId)).ReturnsAsync(productList);
            var productsDto = mapper.Map<List<ProductResponseDTO>>(productList);

            //Act
            var productsByCateId = await productsController.GetProductByCategoryId(categoryId);
            var productsByCatIdWithOkObj = productsByCateId as OkObjectResult;
            var ProductsResult = productsByCatIdWithOkObj?.Value as List<ProductResponseDTO>;

            //Assert
            Assert.IsType<OkObjectResult>(productsByCatIdWithOkObj);
            Assert.Equal((int)HttpStatusCode.OK, productsByCatIdWithOkObj.StatusCode);
            //Additional assertions
            Assert.IsType<List<ProductResponseDTO>>(ProductsResult);
            Assert.Equal(productsDto[0].Id, ProductsResult[0].Id);
            Assert.Equal(productsDto[0].Name, ProductsResult[0].Name);
            Assert.Equal(productsDto[0].Rating, ProductsResult[0].Rating);
            Assert.Equal(productsDto[0].Price, ProductsResult[0].Price);
            Assert.Equal(productsDto[0].CategoryId, ProductsResult[0].CategoryId);
            Assert.Equal(productList.Count,ProductsResult.Count);                     

        }
        [Fact]
        public async Task Test_GetProductByCategoryId_Returns_NotFoundResponse()
        {
            //Arrange
            var categoryId = 400;
            var productsData = new List<Product>();
            productServiceMock.Setup(p => p.GetProductByCategoryId(categoryId)).ReturnsAsync(productsData);

            //Act
            var productsByCateId = await productsController.GetProductByCategoryId(categoryId);
            var productsByCatINotFoundObj = productsByCateId as NotFoundObjectResult;
            var ProductsResult = productsByCatINotFoundObj?.Value as ProductNotFoundByCategory;

            //Assert
            Assert.IsType<NotFoundObjectResult>(productsByCatINotFoundObj);
            Assert.Equal((int)HttpStatusCode.NotFound, productsByCatINotFoundObj.StatusCode);
            Assert.IsType<ProductNotFoundByCategory>(ProductsResult);
            Assert.Equal(categoryId, ProductsResult.CategoryId);
            Assert.Equal(ResponseConstantsTest.NoRecordWithCategoryId, ProductsResult.Message);
        }
        [Theory]
        [InlineData(0)]
        [InlineData(-47)]
        public async Task Test_GetProductByCategoryId_Returns_BadRequest_WhenInvalidIdProvided(int categoryId)
        {
            //Arrange
            //Act
            var productData = await productsController.GetProductByCategoryId(categoryId);
            var errorResponse = productData as BadRequestObjectResult;
            var errorContent = errorResponse?.Value as CategoryErrorResponse;

            //Assert
            Assert.IsType<BadRequestObjectResult>(productData);
            Assert.IsType<BadRequestObjectResult>(errorResponse);
            Assert.IsType<CategoryErrorResponse>(errorContent);
            Assert.Equal((int)HttpStatusCode.BadRequest, errorResponse.StatusCode);
            Assert.Equal(categoryId, errorContent.CategoryId);
            Assert.Equal(ResponseConstantsTest.NullOrInvalidCategoryId, errorContent.ErrorMessage);
        }

        //AddProduct tests
        [Fact]
        public async Task Test_AddProduct_Returns_CreatedResponse()
        {
            //Arrange
            var productModel = mapper.Map<Product>(productRequestDTO);
            productServiceMock.Setup(p => p.IsCategoryPresent(productRequestDTO.CategoryId)).ReturnsAsync(category.Exists(x => x.Id == productRequestDTO.CategoryId));
            productServiceMock.Setup(p => p.AddProduct(It.IsAny<Product>())).ReturnsAsync(productModel);
            //Act
            var productData = await productsController.AddProduct(productRequestDTO);
            var productWithCreatedResult = productData as CreatedAtActionResult;
            var ProductsResult = productWithCreatedResult?.Value  as ProductResponseDTO;

            //Asserts
            Assert.NotNull(productData);
            Assert.NotNull(productWithCreatedResult);
            Assert.IsType<CreatedAtActionResult>(productWithCreatedResult);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(productData);
            Assert.Equal((int)HttpStatusCode.Created, createdAtActionResult.StatusCode);
            Assert.Equal(nameof(ProductsController.GetProductById), createdAtActionResult.ActionName);
            Assert.Equal(productResponseDTO.Name, ProductsResult?.Name);
            Assert.Equal(productResponseDTO.Price, ProductsResult?.Price);
            Assert.Equal(productResponseDTO.Rating, ProductsResult?.Rating);
            Assert.Equal(productResponseDTO.CategoryId, ProductsResult?.CategoryId);
        }
        [Fact]
        public async Task Test_AddProduct_Returns_BadRequestResponse_WhenProductIsAlreadyPresent() //If same product is already present
        {
            //Arrange
            var productModel = mapper.Map<Product>(productRequestDTO);
            productServiceMock.Setup(p => p.IsCategoryPresent(productRequestDTO.CategoryId)).ReturnsAsync(category.Exists(x => x.Id == productRequestDTO.CategoryId));
            productServiceMock.Setup(p => p.AddProduct(productModel)).ReturnsAsync((Product?)null);

            //Act
            var productData = await productsController.AddProduct(productRequestDTO);
            var productWithBadReqResult = productData as BadRequestObjectResult;
            var ProductsBadReqResult = productWithBadReqResult?.Value as ProductAlreadyPresentResponse;

            Assert.IsType<BadRequestObjectResult>(productData);
            Assert.IsType<BadRequestObjectResult>(productWithBadReqResult);     
            Assert.IsType<ProductAlreadyPresentResponse>(ProductsBadReqResult);
            Assert.Equal((int)HttpStatusCode.BadRequest, productWithBadReqResult.StatusCode);
            Assert.Equal(productRequestDTO.Name, ProductsBadReqResult.ProductName);
            Assert.Equal(ResponseConstantsTest.ProductAlreadyPresent, ProductsBadReqResult.ErrorMessage);
        }
        [Theory]
        [InlineData(225)]
        [InlineData(500)]
        public async Task Test_AddProduct_Returns_NotFoundResponse_WhenCategoryIdIsNotPresent(int categoryId) // Not existing category id provided
        {
            //Arrange
            productRequestDTO.CategoryId = categoryId;
            productServiceMock.Setup(p => p.IsCategoryPresent(productRequestDTO.CategoryId)).ReturnsAsync(category.Exists(x =>x.Id==productRequestDTO.CategoryId));

            //Act
            var productData = await productsController.AddProduct(productRequestDTO);
            var productWithNotFoundResult = productData as NotFoundObjectResult;
            var ProductsResult = productWithNotFoundResult?.Value as ProductAddFailureResponse;

            Assert.IsType<NotFoundObjectResult>(productData);
            Assert.IsType<NotFoundObjectResult>(productWithNotFoundResult);
            Assert.IsType<ProductAddFailureResponse>(ProductsResult);
            Assert.Equal((int)HttpStatusCode.NotFound, productWithNotFoundResult.StatusCode);
            Assert.Equal(productRequestDTO.CategoryId, ProductsResult.CategoryId);
            Assert.Equal(ResponseConstantsTest.CategoryIdNotPresent, ProductsResult.ErrorMessage);
        }

        //Update Product

        [Fact]
        public async Task Test_UpdateProduct_Returns_OKResponse()
        {
            //Arrange
            var id = 4;
            productServiceMock.Setup(p => p.UpdateProduct(id,It.IsAny<Product>())).ReturnsAsync(id);

            //Act
            var productData = await productsController.UpdateProduct(id,productRequestDTO);
            var productUpdateOkObjResult = productData as OkObjectResult;
            var ProductsSuccessResult = productUpdateOkObjResult?.Value as ProductSuccessResponse;

            //Asserts
            Assert.NotNull(productData);
            Assert.NotNull(productUpdateOkObjResult);
            Assert.IsType<OkObjectResult>(productUpdateOkObjResult);
            Assert.IsType<ProductSuccessResponse>(ProductsSuccessResult);
            Assert.Equal((int)HttpStatusCode.OK, productUpdateOkObjResult.StatusCode);

            //Additional assertions
            Assert.Equal(id, ProductsSuccessResult.ProductId);
            Assert.Equal(ResponseConstantsTest.ProductUpdateSuccess, ProductsSuccessResult.Message);
        }
        [Fact]
        public async Task Test_UpdateProduct_Returns_NotFoundResponse_WhenProvidedIdIsNotPresent()
        {
            var id = 400;
            var returnValue = -1;
            //Arrange
            productServiceMock.Setup(p => p.UpdateProduct(id, It.IsAny<Product>())).ReturnsAsync(returnValue);

            //Act
            var productData = await productsController.UpdateProduct(id, productRequestDTO);
            var productUpdateNotFoundResult = productData as NotFoundObjectResult;
            var ProductsUpdateFailResult = productUpdateNotFoundResult?.Value as ProductNotFoundResponse;

            //Asserts
            Assert.NotNull(productData);
            Assert.NotNull(productUpdateNotFoundResult);
            Assert.IsType<NotFoundObjectResult>(productUpdateNotFoundResult);
            Assert.IsType<ProductNotFoundResponse>(ProductsUpdateFailResult);
            Assert.Equal((int)HttpStatusCode.NotFound, productUpdateNotFoundResult.StatusCode);

            //Additional assertions
            Assert.Equal(id, ProductsUpdateFailResult.ProductId);
            Assert.Equal(ResponseConstantsTest.NoRecordWithProductId, ProductsUpdateFailResult.Message);
        }
        [Fact]
        public async Task Test_UpdateProduct_Returns_NotFound_WhenProvidedCategoryId_IsNotPresentInSystem()
        {
            //Arrange
            var id = 1;
            var returnValue = -2;
            productRequestDTO.CategoryId = 400;
            productServiceMock.Setup(p => p.UpdateProduct(id, It.IsAny<Product>())).ReturnsAsync(returnValue);

            //Act
            var productData = await productsController.UpdateProduct(id, productRequestDTO);
            var productUpdateNotFoundResult = productData as NotFoundObjectResult;
            var ProductUpdateFailResult = productUpdateNotFoundResult?.Value as ProductUpdateFailResponse;

            //Asserts
            Assert.NotNull(productData);
            Assert.NotNull(productUpdateNotFoundResult);
            Assert.IsType<NotFoundObjectResult>(productUpdateNotFoundResult);
            Assert.IsType<ProductUpdateFailResponse>(ProductUpdateFailResult);
            Assert.Equal((int)HttpStatusCode.NotFound, productUpdateNotFoundResult.StatusCode);

            //Additional assertions
            Assert.Equal(id, ProductUpdateFailResult.ProductId);
            Assert.Equal(productRequestDTO.CategoryId, ProductUpdateFailResult.CategoryId);
            Assert.Equal(ResponseConstantsTest.CategoryIdNotPresent, ProductUpdateFailResult.ErrorMessage);
        }
        [Theory]
        [InlineData(0)]
        [InlineData(-121)]
        public async Task Test_UpdateProduct_Returns_BadRequest_WhenInvalidIdIsProvided(int id)
        {
            //Arrange
            //Act
            var productData = await productsController.UpdateProduct(id, productRequestDTO);
            var productUpdateBadRequestResult = productData as BadRequestObjectResult;
            var productErrorResult = productUpdateBadRequestResult?.Value as ProductUpdateFailResponse;

            //Asserts
            Assert.NotNull(productData);
            Assert.NotNull(productUpdateBadRequestResult);
            Assert.IsType<BadRequestObjectResult>(productUpdateBadRequestResult);
            Assert.IsType<ProductUpdateFailResponse>(productErrorResult);
            Assert.Equal((int)HttpStatusCode.BadRequest, productUpdateBadRequestResult.StatusCode);

            //Additional assertions
            Assert.Equal(id, productErrorResult.ProductId);
            Assert.Equal(ResponseConstantsTest.NullOrInvalidProductId, productErrorResult.ErrorMessage);
        }

        //DeleteProduct tests
        [Fact]
        public async Task Test_DeleteProduct_Returns_OKResponse()
        {
            //Arrange
            var id = 5;
            productServiceMock.Setup(p => p.DeleteProduct(id)).ReturnsAsync(id);

            //Act
            var productData = await productsController.DeleteProduct(id);
            var productDeleteOkObjResult = productData as OkObjectResult;
            var ProductsSuccessResult = productDeleteOkObjResult?.Value as ProductSuccessResponse;

            //Asserts
            Assert.NotNull(productData);
            Assert.NotNull(productDeleteOkObjResult);
            Assert.IsType<OkObjectResult>(productDeleteOkObjResult);
            Assert.IsType<ProductSuccessResponse>(ProductsSuccessResult);
            Assert.Equal((int)HttpStatusCode.OK, productDeleteOkObjResult.StatusCode);

            //Additional assertions
            Assert.Equal(id, ProductsSuccessResult.ProductId);
            Assert.Equal(ResponseConstantsTest.ProductDeleteSuccess, ProductsSuccessResult.Message);
        }
        [Fact]
        public async Task Test_DeleteProduct_Returns_NotFoundResponse()
        {
            var id = 210;
            var returnValue = -1;
            //Arrange
            productServiceMock.Setup(p => p.DeleteProduct(id)).ReturnsAsync(returnValue);

            //Act
            var productData = await productsController.DeleteProduct(id);
            var productDeleteNotFoundResult = productData as NotFoundObjectResult;
            var ProductsUpdateFailResult = productDeleteNotFoundResult?.Value as ProductNotFoundResponse;

            //Asserts
            Assert.NotNull(productData);
            Assert.NotNull(productDeleteNotFoundResult);
            Assert.IsType<NotFoundObjectResult>(productDeleteNotFoundResult);
            Assert.IsType<ProductNotFoundResponse>(ProductsUpdateFailResult);
            Assert.Equal((int)HttpStatusCode.NotFound, productDeleteNotFoundResult.StatusCode);

            //Additional assertions
            Assert.Equal(id, ProductsUpdateFailResult.ProductId);
            Assert.Equal(ResponseConstantsTest.NoRecordWithProductId, ProductsUpdateFailResult.Message);
        }
        [Theory]
        [InlineData(0)]
        [InlineData(-65)]
        public async Task Test_DeleteProduct_Returns_BadRequestResponse_WhenInvalidIdProvided(int id) 
        {
            //Arrange
            //Act
            var productData = await productsController.DeleteProduct(id);
            var productDeleteBadRequestResult = productData as BadRequestObjectResult;
            var ProductDeleteFailResult = productDeleteBadRequestResult?.Value as ProductDeleteFailResponse;

            //Asserts
            Assert.NotNull(productData);
            Assert.NotNull(productDeleteBadRequestResult);
            Assert.IsType<BadRequestObjectResult>(productDeleteBadRequestResult);
            Assert.IsType<ProductDeleteFailResponse>(ProductDeleteFailResult);
            Assert.Equal((int)HttpStatusCode.BadRequest, productDeleteBadRequestResult.StatusCode);

            //Additional assertions
            Assert.Equal(id, ProductDeleteFailResult.ProductId);
            Assert.Equal(ResponseConstantsTest.NullOrInvalidProductId, ProductDeleteFailResult.ErrorMessage);
        }
    }
   
}