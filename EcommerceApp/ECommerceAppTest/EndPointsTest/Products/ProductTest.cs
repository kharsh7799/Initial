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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Net;
using static EcommerceApp.Entities.APIResponses.APICategoriesResponses;

namespace ECommerceAppTest.EndPointsTest.Products
{
    public class ProductTest
    {
        private readonly Mock<IProductService> productRepositoryMock;
        private readonly ProductsController productsController;
        private readonly IMapper mapper;
        private readonly Mock<ILogger<ProductsController>> logger;

        public ProductTest()
        {
            productRepositoryMock =  new Mock<IProductService>();
            logger = new Mock<ILogger<ProductsController>>();

            var _mapper = new MapperConfiguration(mc => mc.AddProfile(new AutoMapperProfile())).CreateMapper();
            mapper = _mapper;

            productsController = new ProductsController(productRepositoryMock.Object, mapper, logger.Object);
        }

        //GetAllProducts Tests
        [Fact]
        public async Task Test_GetAllProducts_Returns_OKResponse()
        {
            //Arrange
            var products = new List<Product>()
            {
              new Product
              {
                Id = 1,
                Name = "Nike sneaker",
                Price=15000,
                Rating=1,
                CategoryId=5,
              },
              new Product
              {
                Id = 2,
                Name = "Adidas sports",
                Price=2000,
                Rating=2,
                CategoryId=2,
              },
            };
            var productDto = mapper.Map<List<ProductResponseDTO>>(products);

            productRepositoryMock.Setup(p => p.GetAllProducts(null)).ReturnsAsync(products);
            //Act
            var productsData = await productsController.GetAllProducts(null);
            var productsWithOkObj = productsData as OkObjectResult;
            var ProductsList = productsWithOkObj?.Value as List<ProductResponseDTO>;

            //Assert
            Assert.IsType<OkObjectResult>(productsWithOkObj);
            Assert.Equal((int)HttpStatusCode.OK, productsWithOkObj.StatusCode);

            //Additional assertions
            Assert.IsType<List<ProductResponseDTO>>(ProductsList);
            Assert.Equal(productDto[0].Id, ProductsList[0].Id);
            Assert.Equal(productDto[0].Name, ProductsList[0].Name);
            Assert.Equal(productDto[0].Rating, ProductsList[0].Rating);
            Assert.Equal(productDto[0].Price, ProductsList[0].Price);
            Assert.Equal(2,ProductsList.Count);

        }
        [Fact]
        public async Task Test_GetAllProducts_Returns_OKResponseWithEmptyResult()
        {
            //Arrange
            var products = new List<Product>();
            var count = products.Count;

            productRepositoryMock.Setup(p => p.GetAllProducts(null)).ReturnsAsync(products);
            //Act
            var productsData = await productsController.GetAllProducts(null);
            var productsWithOkObj = productsData as OkObjectResult;
            var ProductsList = productsWithOkObj?.Value as AllProductsResponse;

            //Assert
            Assert.IsType<OkObjectResult>(productsWithOkObj);
            Assert.Equal((int)HttpStatusCode.OK, productsWithOkObj.StatusCode);

            //Additional assertions
            Assert.Equal(ResponseConstantsTest.NoRecord, ProductsList?.Message);
            Assert.Equal(count, ProductsList?.ProductCount);

        }
        [Fact]
        public async Task Test_GetAllProducts_Returns_ServerErrorResponse()
        {
            //Arrange
            productRepositoryMock.Setup(p => p.GetAllProducts(null)).
                Throws(new Exception("Simulated internal server error"));

            //Act
            var productData = await productsController.GetAllProducts(null);
            var errorResponse = productData as ObjectResult;
            var errorContent = errorResponse?.Value as ProblemDetails;
           
            //Assert
            Assert.IsType<ObjectResult>(productData);
            Assert.IsType<ObjectResult>(errorResponse);
            Assert.Equal((int)HttpStatusCode.InternalServerError, errorResponse.StatusCode);
            Assert.Equal(ResponseConstantsTest.ServerError, errorContent?.Detail);
        }

        //GetProductByID Tests
        [Theory]
        [InlineData(4)]
        public async Task Test_GetProductById_Returns_OKResponse(int id)
        {
            //Arrange
            var product = new Product
            {
                Id = 4,
                Name = "LG",
                Price = 15000,
                Rating = 4,
                CategoryId = 3,
            };
            var productDto = mapper.Map<ProductResponseDTO>(product);

            productRepositoryMock.Setup(p => p.GetProductById(id)).ReturnsAsync(product);

            //Act
            var productData = await productsController.GetProductById(id);
            var productWithOkObj = productData as OkObjectResult;
            var productResult = productWithOkObj?.Value as ProductResponseDTO;

            //Assert
            Assert.IsType<OkObjectResult>(productWithOkObj);
            Assert.Equal((int)HttpStatusCode.OK, productWithOkObj.StatusCode);

            //Additional assertions
            Assert.IsType<ProductResponseDTO>(productResult);
            Assert.Equal(productDto.Id, productResult.Id);
            Assert.Equal(productDto.Name, productResult.Name);
            Assert.Equal(productDto.Rating, productResult.Rating);
            Assert.Equal(productDto.Price, productResult.Price);
            Assert.Equal(productDto.CategoryId, productResult.CategoryId);
        }
        [Theory]
        [InlineData(200)]
        public async Task Test_GetProductById_Returns_NotFoundResponse(int id)
        {
            //Arrange
            var product = new Product
            {
                Id = 4,
                Name = "LG",
                Price = 15000,
                Rating = 4,
                CategoryId = 3,
            };
            var productDto = mapper.Map<ProductResponseDTO>(product);

            productRepositoryMock.Setup(p => p.GetProductById(id)).ReturnsAsync((Product?)null);

            //Act
            var productData = await productsController.GetProductById(id);
            var productNotFound = productData as NotFoundObjectResult;
            var ProductResponse = productNotFound?.Value as ProductNotFoundResponse;

            //Assert
            Assert.IsType<NotFoundObjectResult>(productNotFound);
            Assert.Equal((int)HttpStatusCode.NotFound, productNotFound.StatusCode);

            //Additional assertions
            Assert.IsType<ProductNotFoundResponse>(ProductResponse);
            Assert.NotEqual(productDto.Id, ProductResponse.ProductId);
            Assert.Equal(ResponseConstantsTest.NoRecordWithProductId, ProductResponse.Message);
        }
        [Theory]
        [InlineData(0)]
        public async Task Test_GetProductById_Returns_BadRequestForArgumetException(int id)
        {
            //Arrange
            productRepositoryMock.Setup(p => p.GetProductById(id)).
                Throws(new ArgumentNullException(nameof(id)));

            //Act
            var productData = await productsController.GetProductById(id); 
            var errorResponse = productData as BadRequestObjectResult;
            var errorContent = errorResponse?.Value as ProductDeleteFailResponse;

            //Assert
            Assert.IsType<BadRequestObjectResult>(errorResponse);
            Assert.IsType<BadRequestObjectResult>(errorResponse);
            Assert.IsType<ProductDeleteFailResponse>(errorContent);
            Assert.Equal((int)HttpStatusCode.BadRequest, errorResponse.StatusCode);
            Assert.Equal(id, errorContent.ProductId);
            Assert.Equal(ResponseConstantsTest.NullOrInvalidProductId, errorContent.ErrorMessage);
        }
        [Theory]
        [InlineData(4)]
        public async Task Test_GetProductById_Returns_ServerErrorResponse(int id)
        {
            //Arrange
            productRepositoryMock.Setup(p => p.GetProductById(id)).
                Throws(new Exception("Simulated internal server error"));

            //Act
            var productData = await productsController.GetProductById(id); //valid id passed
            var errorResponse = productData as ObjectResult;
            var errorContent = errorResponse?.Value as ProblemDetails;

            //Assert
            Assert.IsType<ObjectResult>(productData); 
            Assert.IsType<ObjectResult>(errorResponse);
            Assert.Equal((int)HttpStatusCode.InternalServerError, errorResponse.StatusCode);
            Assert.Equal(ResponseConstantsTest.ServerError, errorContent?.Detail);
        }

        //GetProductsByCategoryId tests

        [Theory]
        [InlineData(5)]
        public async Task Test_GetProductByCategoryId_Returns_OKResponse(int categoryId)
        {
            //Arrange
            var products = new List<Product>()
            {
              new Product
              {
                Id = 1,
                Name = "Nike sneaker",
                Price=15000,
                Rating=1,
                CategoryId=5,
              },
              new Product
              {
                Id = 2,
                Name = "Adidas sports",
                Price=2000,
                Rating=2,
                CategoryId=5,
              },
              new Product
              {
                Id = 3,
                Name = "LG",
                Price = 15000,
                Rating = 4,
                CategoryId = 5,
              }
            };
            var productDto = mapper.Map<List<ProductResponseDTO>>(products);


            productRepositoryMock.Setup(p => p.GetProductByCategoryId(categoryId)).ReturnsAsync(products);

            //Act
            var productsByCateId= await productsController.GetProductByCategoryId(categoryId);
            var productsByCatIdWithOkObj = productsByCateId as OkObjectResult;
            var ProductsResult = productsByCatIdWithOkObj?.Value as List<ProductResponseDTO>;

            //Assert
            Assert.IsType<OkObjectResult>(productsByCatIdWithOkObj);
            Assert.Equal((int)HttpStatusCode.OK, productsByCatIdWithOkObj.StatusCode);

            //Additional assertions
            Assert.IsType<List<ProductResponseDTO>>(ProductsResult);
            Assert.Equal(productDto[0].Id, ProductsResult[0].Id);
            Assert.Equal(productDto[0].Name, ProductsResult[0].Name);
            Assert.Equal(productDto[0].Rating, ProductsResult[0].Rating);
            Assert.Equal(productDto[0].Price, ProductsResult[0].Price);
            Assert.Equal(productDto[0].CategoryId, ProductsResult[0].CategoryId);


        }
        [Theory]
        [InlineData(400)]
        public async Task Test_GetProductByCategoryId_Returns_NotFoundResponse(int categoryId)
        {
            //Arrange
            var products = new List<Product>()
            {
              new Product
              {
                Id = 1,
                Name = "Nike sneaker",
                Price=15000,
                Rating=1,
                CategoryId=5,
              },
              new Product
              {
                Id = 2,
                Name = "Adidas sports",
                Price=2000,
                Rating=2,
                CategoryId=5,
              },
              new Product
              {
                Id = 3,
                Name = "LG",
                Price = 15000,
                Rating = 4,
                CategoryId = 5,
              }
            };
            var productDto = mapper.Map<List<ProductResponseDTO>>(products);

            productRepositoryMock.Setup(p => p.GetProductByCategoryId(categoryId)).ReturnsAsync((List<Product>?)null);

            //Act
            var productsByCateId = await productsController.GetProductByCategoryId(categoryId);
            var productsByCatINotFoundObj = productsByCateId as NotFoundObjectResult;
            var ProductsResult = productsByCatINotFoundObj?.Value as ProductNotFoundByCategory;

            //Assert
            Assert.IsType<NotFoundObjectResult>(productsByCatINotFoundObj);
            Assert.Equal((int)HttpStatusCode.NotFound, productsByCatINotFoundObj.StatusCode);

            //Additional assertions
            Assert.IsType<ProductNotFoundByCategory>(ProductsResult);
            Assert.NotEqual(productDto[0].CategoryId, ProductsResult.CategoryId);
            Assert.Equal(ResponseConstantsTest.NoRecordWithCategoryId, ProductsResult.Message);
        }
        [Theory]
        [InlineData(0)]
        public async Task Test_GetProductByCategoryId_Returns_BadRequestForArgumetException(int categoryId)
        {
            //Arrange
            productRepositoryMock.Setup(p => p.GetProductByCategoryId(categoryId)).
                Throws(new ArgumentNullException(nameof(categoryId)));

            //Act
            var productData = await productsController.GetProductByCategoryId(categoryId);
            var errorResponse = productData as BadRequestObjectResult;
            var errorContent = errorResponse?.Value as CategoryErrorResponse;

            //Assert
            Assert.IsType<BadRequestObjectResult>(errorResponse);
            Assert.IsType<BadRequestObjectResult>(errorResponse);
            Assert.IsType<CategoryErrorResponse>(errorContent);
            Assert.Equal((int)HttpStatusCode.BadRequest, errorResponse.StatusCode);
            Assert.Equal(categoryId, errorContent.CategoryId);
            Assert.Equal(ResponseConstantsTest.NullOrInvalidCategoryId, errorContent.ErrorMessage);
        }
        [Theory]
        [InlineData(5)]
        public async Task Test_GetProductByCategoryId_Returns_ErrorResponse(int categoryId)
        {
            //Arrange
            productRepositoryMock.Setup(p => p.GetProductByCategoryId(categoryId)).
            Throws(new Exception("Simulated internal server error"));
            //Act
            var productsByCateId = await productsController.GetProductByCategoryId(categoryId);
            var productsByCatIdErrorResponse = productsByCateId as ObjectResult;
            var ProductsErrorContent = productsByCatIdErrorResponse?.Value as ProblemDetails;

            //Assert
            Assert.IsType<ObjectResult>(productsByCateId);
            Assert.IsType<ObjectResult>(productsByCatIdErrorResponse);
            Assert.Equal((int)HttpStatusCode.InternalServerError, productsByCatIdErrorResponse.StatusCode);
            Assert.Equal(ResponseConstantsTest.ServerError, ProductsErrorContent?.Detail);
        }

        //AddProduct tests
        [Fact]
        public async Task Test_AddProduct_Returns_CreatedResponse()
        {
            //Arrange
            var productReqDTO = new ProductRequestDTO
            {
                Name = "Nike sneaker",
                Price = 15000,
                Rating = 1,
                CategoryId = 5,
            };
            var productResDTO = new ProductResponseDTO
            {
                Name = "Nike sneaker",
                Price = 15000,
                Rating = 1,
                CategoryId = 5,
            };

            var product = mapper.Map<Product>(productReqDTO);

            productRepositoryMock.Setup(p => p.AddProduct(It.IsAny<Product>())).ReturnsAsync(product);
                
            //Act
            var productData= await productsController.AddProduct(productReqDTO);
            var productWithCreatedResult = productData as CreatedAtActionResult;
            var ProductsResult = productWithCreatedResult?.Value  as ProductResponseDTO;

            //Asserts
            Assert.NotNull(productData);
            Assert.NotNull(productWithCreatedResult);
            Assert.IsType<CreatedAtActionResult>(productWithCreatedResult);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(productData);
            Assert.Equal((int)HttpStatusCode.Created, createdAtActionResult.StatusCode);
            Assert.Equal(nameof(ProductsController.GetProductById), createdAtActionResult.ActionName);
            Assert.Equal(productResDTO.Name, ProductsResult?.Name);
            Assert.Equal(productResDTO.Price, ProductsResult?.Price);
            Assert.Equal(productResDTO.Rating, ProductsResult?.Rating);
            Assert.Equal(productResDTO.CategoryId, ProductsResult?.CategoryId);

        }
        [Fact]
        public async Task Test_AddProduct_Returns_BadRequestResponse() //If same product is already present
        {
            //Arrange
            var productReqDTO = new ProductRequestDTO
            {
                Name = "Nike sneaker",
                Price = 15000,
                Rating = 1,
                CategoryId = 5,
            };
            var product = mapper.Map<Product>(productReqDTO);
            productRepositoryMock.Setup(p => p.AddProduct(product)).ReturnsAsync((Product?)null);

            //Act
            var productData = await productsController.AddProduct(productReqDTO);
            var productWithBadReqResult = productData as BadRequestObjectResult;
            var ProductsBadReqResult = productWithBadReqResult?.Value as ProductAlreadyPresentResponse;

            Assert.IsType<BadRequestObjectResult>(productData);
            Assert.IsType<BadRequestObjectResult>(productWithBadReqResult);     
            Assert.IsType<ProductAlreadyPresentResponse>(ProductsBadReqResult);
            Assert.Equal((int)HttpStatusCode.BadRequest, productWithBadReqResult.StatusCode);
            Assert.Equal(productReqDTO.Name, ProductsBadReqResult.ProductName);
            Assert.Equal(ResponseConstantsTest.ProductAlreadyPresent, ProductsBadReqResult.ErrorMessage);
        }
        [Fact]
        public async Task Test_AddProduct_Returns_BadReqResponseForDBUpdateFailure() // Not existing category id provided
        {
            //Arrange
            var productReqDTO = new ProductRequestDTO
            {
                Name = "Maggi",
                Price = 100,
                Rating = 1,
                CategoryId = 55,
            };
            productRepositoryMock.Setup(p => p.AddProduct(It.IsAny<Product>())).
            Throws(new DbUpdateException());

            //Act
            var productData = await productsController.AddProduct(productReqDTO);
            var productWithBadReqResult = productData as BadRequestObjectResult;
            var ProductsBadReqResult = productWithBadReqResult?.Value as ProductAddFailureResponse;

            Assert.IsType<BadRequestObjectResult>(productData);
            Assert.IsType<BadRequestObjectResult>(productWithBadReqResult);
            Assert.IsType<ProductAddFailureResponse>(ProductsBadReqResult);
            Assert.Equal((int)HttpStatusCode.BadRequest, productWithBadReqResult.StatusCode);
            Assert.Equal(productReqDTO.CategoryId, ProductsBadReqResult.CategoryId);
            Assert.Equal(ResponseConstantsTest.ProductNotAdded, ProductsBadReqResult.ErrorMessage);
        }
        [Fact]
        public async Task Test_AddProduct_Returns_ServerErrorResponse()
        {
            //Arrange
            var productReqDTO = new ProductRequestDTO
            {
                Name = "Nike sneaker",
                Price = 15000,
                Rating = 1,
                CategoryId = 5,
            };
            productRepositoryMock.Setup(p => p.AddProduct(It.IsAny<Product>())).Throws(new Exception("Demo Exception"));

            //Act
            var productData = await productsController.AddProduct(productReqDTO);
            var productWithServerErrorResult = productData as ObjectResult;
            var productErrorResult = productWithServerErrorResult?.Value as ProblemDetails;

            Assert.IsType<ObjectResult>(productData);
            Assert.IsType<ObjectResult>(productWithServerErrorResult);
            Assert.IsType<ProblemDetails>(productErrorResult);
            Assert.Equal((int)HttpStatusCode.InternalServerError, productWithServerErrorResult.StatusCode);
            Assert.Equal(ResponseConstantsTest.ServerError, productErrorResult.Detail);
        }

        //Update Product

        [Theory]
        [InlineData(4)]
        public async Task Test_UpdateProduct_Returns_OKResponse(int id)
        {
            //Arrange
            var productReqDTO = new ProductRequestDTO
            {
                Name = "Nike sneaker",
                Price = 15000,
                Rating = 1,
                CategoryId = 5,
            };
            productRepositoryMock.Setup(p => p.UpdateProduct(id,It.IsAny<Product>())).ReturnsAsync(id);

            //Act
            var productData = await productsController.UpdateProduct(id,productReqDTO);
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
        [Theory]
        [InlineData(78)]
        public async Task Test_UpdateProduct_Returns_NotFoundResponse(int id)
        {
            var returnValue = -1;
            //Arrange
            var productReqDTO = new ProductRequestDTO
            {
                Name = "Nike sneaker",
                Price = 15000,
                Rating = 1,
                CategoryId = 5,
            };
            productRepositoryMock.Setup(p => p.UpdateProduct(id, It.IsAny<Product>())).ReturnsAsync(returnValue);

            //Act
            var productData = await productsController.UpdateProduct(id, productReqDTO);
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
        [Theory]
        [InlineData(4)]
        public async Task Test_UpdateProduct_Returns_BadRequestResponse(int id) //non existing category Id
        {
            //Arrange
            var productReqDTO = new ProductRequestDTO
            {
                Name = "Nike sneaker",
                Price = 15000,
                Rating = 1,
                CategoryId = 92,
            };

            productRepositoryMock.Setup(p => p.UpdateProduct(id, It.IsAny<Product>())).Throws(new DbUpdateException());

            //Act
            var productData = await productsController.UpdateProduct(id, productReqDTO);
            var productUpdateBadRequestResult = productData as BadRequestObjectResult;
            var ProductUpdateFailResult = productUpdateBadRequestResult?.Value as ProductUpdateFailResponse;

            //Asserts
            Assert.NotNull(productData);
            Assert.NotNull(productUpdateBadRequestResult);
            Assert.IsType<BadRequestObjectResult>(productUpdateBadRequestResult);
            Assert.IsType<ProductUpdateFailResponse>(ProductUpdateFailResult);
            Assert.Equal((int)HttpStatusCode.BadRequest, productUpdateBadRequestResult.StatusCode);

            //Additional assertions
            Assert.Equal(id, ProductUpdateFailResult.ProductId);
            Assert.Equal(productReqDTO.CategoryId, ProductUpdateFailResult.CategoryId);
            Assert.Equal(ResponseConstantsTest.ProductUpdateFailed, ProductUpdateFailResult.ErrorMessage);
        }
        [Theory]
        [InlineData(0)]
        public async Task Test_UpdateProduct_Returns_BadRequestForArgumentException(int id) //non existing category Id
        {
            //Arrange
            var productReqDTO = new ProductRequestDTO
            {
                Name = "Nike sneaker",
                Price = 15000,
                Rating = 1,
                CategoryId = 92,
            };
            productRepositoryMock.Setup(p => p.UpdateProduct(id, It.IsAny<Product>())).
                Throws(new ArgumentNullException(nameof(id)));

            //Act
            var productData = await productsController.UpdateProduct(id, productReqDTO);
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
        [Theory]
        [InlineData(4)]
        public async Task Test_UpdateProduct_Returns_ServerErrorResponse(int id)
        {
            //Arrange
            var productReqDTO = new ProductRequestDTO
            {
                Name = "Nike sneaker",
                Price = 15000,
                Rating = 1,
                CategoryId = 5,
            };
            productRepositoryMock.Setup(p => p.UpdateProduct(id, It.IsAny<Product>())).Throws(new Exception());

            //Act
            var productData = await productsController.UpdateProduct(id, productReqDTO);
            var productUpdateObjResult = productData as ObjectResult;
            var productsErrorResult = productUpdateObjResult?.Value as ProblemDetails;

            //Asserts
            Assert.NotNull(productData);
            Assert.NotNull(productUpdateObjResult);
            Assert.IsType<ObjectResult>(productUpdateObjResult);
            Assert.IsType<ProblemDetails>(productsErrorResult);
            Assert.Equal((int)HttpStatusCode.InternalServerError, productUpdateObjResult.StatusCode);
            Assert.Equal(ResponseConstantsTest.ServerError, productsErrorResult.Detail);
        }

        //DeleteProduct tests
        [Theory]
        [InlineData(4)]
        public async Task Test_DeleteProduct_Returns_OKResponse(int id)
        {
            //Arrange
            productRepositoryMock.Setup(p => p.DeleteProduct(id)).ReturnsAsync(id);

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
        [Theory]
        [InlineData(78)]
        public async Task Test_DeleteProduct_Returns_NotFoundResponse(int id)
        {
            var returnValue = -1;
            //Arrange
            productRepositoryMock.Setup(p => p.DeleteProduct(id)).ReturnsAsync(returnValue);

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
        public async Task Test_DeleteProduct_Returns_BadRequestResponse(int id) 
        {
            //Arrange
            productRepositoryMock.Setup(p => p.DeleteProduct(id)).Throws(new ArgumentNullException(nameof(id)));

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
        [Theory]
        [InlineData(4)]
        public async Task Test_DeleteProduct_Returns_ServerErrorResponse(int id)
        {
            //Arrange
            productRepositoryMock.Setup(p => p.DeleteProduct(id)).Throws(new Exception());

            //Act
            var productData = await productsController.DeleteProduct(id);
            var productDeleteObjResult = productData as ObjectResult;
            var productsErrorResult = productDeleteObjResult?.Value as ProblemDetails;

            //Asserts
            Assert.NotNull(productData);
            Assert.NotNull(productDeleteObjResult);
            Assert.IsType<ObjectResult>(productDeleteObjResult);
            Assert.IsType<ProblemDetails>(productsErrorResult);
            Assert.Equal((int)HttpStatusCode.InternalServerError, productDeleteObjResult.StatusCode);
            Assert.Equal(ResponseConstantsTest.ServerError, productsErrorResult.Detail);
        }
    }
   
}