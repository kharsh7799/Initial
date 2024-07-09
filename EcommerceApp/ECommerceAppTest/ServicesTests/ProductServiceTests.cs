using AutoMapper;
using EcommerceApp.Controllers;
using EcommerceApp.Entities.APIResponses;
using EcommerceApp.Entities.DomainModels;
using EcommerceApp.Entities.DTOs.Product;
using EcommerceApp.Repositories.Contracts;
using EcommerceApp.Services.Contracts;
using EcommerceApp.Services.Implementations;
using ECommerceAppTest.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceAppTest.ServicesTests
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly IProductService productService;
        private readonly Mock<ILogger<ProductService>> logger;
        public ProductServiceTests()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            logger = new Mock<ILogger<ProductService>>();
            productService = new ProductService(_productRepositoryMock.Object, logger.Object);
        }
        [Fact]
        public async Task Test_GetAllProducts_Returns_ProductList()
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
            _productRepositoryMock.Setup(p => p.GetAllProducts(null)).ReturnsAsync(products);
            //Act
            var productsData = await productService.GetAllProducts(null);

            //Assert
            Assert.NotNull(productsData);
            Assert.IsType<List<Product>>(productsData);
            Assert.Equal(products[0].Id, productsData[0].Id);
            Assert.Equal(products[0].Name, productsData[0].Name);
            Assert.Equal(products[0].Rating, productsData[0].Rating);
            Assert.Equal(products[0].Price, productsData[0].Price);
            Assert.Equal(2, productsData.Count);

        }
        [Fact]
        public async Task Test_GetAllProducts_Returns_EmptyProductList()
        {
            //Arrange
            var products = new List<Product>();
            _productRepositoryMock.Setup(p => p.GetAllProducts(null)).ReturnsAsync(products);
            //Act
            var productsData = await productService.GetAllProducts(null);
            //Assert
            Assert.IsType<List<Product>>(productsData);
            Assert.Equal(products.Count, productsData.Count);

        }
        [Theory]
        [InlineData(4)]
        public async Task Test_GetProductById_Returns_Product(int id)
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
            _productRepositoryMock.Setup(p => p.GetProductById(id)).ReturnsAsync(product);
            //Act
            var productData = await productService.GetProductById(id);
            //Assert
            Assert.NotNull(productData);
            Assert.IsType<Product>(productData);
            Assert.Equal(product.Id, productData.Id);
            Assert.Equal(product.Name, productData.Name);
            Assert.Equal(product.Rating, productData.Rating);
            Assert.Equal(product.Price, productData.Price);
            Assert.Equal(product.CategoryId, productData.CategoryId);
        }
        [Theory]
        [InlineData(-9)]
        public async Task Test_GetProductById_Returns_ArgumentException(int id)
        {
            //Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(()=> productService.GetProductById(id));
            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
            Assert.Equal(nameof(id), exception.ParamName);
        }
        [Theory]
        [InlineData(4)]
        public async Task Test_GetProductByCategoryId_Returns_Product(int categoryId)
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
                CategoryId=4,
              },
              new Product
              {
                Id = 2,
                Name = "Adidas sports",
                Price=2000,
                Rating=2,
                CategoryId=4,
              },
            };
            _productRepositoryMock.Setup(p => p.GetProductByCategoryId(categoryId)).ReturnsAsync(products);
            //Act
            var productsData = await productService.GetProductByCategoryId(categoryId);
            //Assert
            Assert.NotNull(productsData);
            Assert.IsType<List<Product>>(productsData);
            Assert.Equal(products[0].Id, productsData[0].Id);
            Assert.Equal(products[0].Name, productsData[0].Name);
            Assert.Equal(products[0].Rating, productsData[0].Rating);
            Assert.Equal(products[0].Price, productsData[0].Price);
            Assert.Equal(categoryId, productsData[0].CategoryId);
        }
        [Theory]
        [InlineData(-9)]
        public async Task Test_GetProductByCategoryId_Returns_ArgumentException(int categoryId)
        {
            //Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => productService.GetProductByCategoryId(categoryId));
            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
            Assert.Equal(nameof(categoryId), exception.ParamName);
        }
        [Fact]
        public async Task Test_AddProduct_Returns_CreatedProduct()
        {
            //Arrange
            var product = new Product
            {
                Name = "Nike sneaker",
                Price = 15000,
                Rating = 1,
                CategoryId = 5,
            };
            _productRepositoryMock.Setup(p => p.AddProduct(It.IsAny<Product>())).ReturnsAsync(product);
            //Act
            var productData = await productService.AddProduct(product);
            //Asserts
            Assert.NotNull(productData);
            Assert.IsType<Product>(productData);
            Assert.Equal(product.Name, productData.Name);
            Assert.Equal(product.Price, productData.Price);
            Assert.Equal(product.Rating, productData.Rating);
            Assert.Equal(product.CategoryId, productData.CategoryId);

        }
        [Fact]
        public async Task Test_AddProduct_Returns_Exception()
        {
            //Arrange
            var product = new Product
            {
                Name = "Nike sneaker",
                Price = 15000,
                Rating = 1,
                CategoryId = 5,
            };
            _productRepositoryMock.Setup(p => p.AddProduct(It.IsAny<Product>())).Throws(new DbUpdateException());
            //Act
            var exception = await Assert.ThrowsAsync<DbUpdateException>(() => productService.AddProduct(product));
            Assert.NotNull(exception);
            Assert.IsType<DbUpdateException>(exception);
        }
        [Theory]
        [InlineData(4)]

        public async Task Test_UpdateProduct_Returns_UpdatedProductId(int id)
        {
            //Arrange
            var product = new Product
            {
                Name = "Nike sneaker",
                Price = 15000,
                Rating = 1,
                CategoryId = 5,
            };
            _productRepositoryMock.Setup(p => p.UpdateProduct(id, It.IsAny<Product>())).ReturnsAsync(id);
            //Act
            var productData = await productService.UpdateProduct(id, product);
            //Asserts
            Assert.IsType<int>(productData);
            Assert.Equal(id, productData);
        }
        [Theory]
        [InlineData(0)]
        public async Task Test_UpdateProduct_Returns_ArgumentException(int id)
        {
            var product = new Product
            {
                Name = "Nike sneaker",
                Price = 15000,
                Rating = 1,
                CategoryId = 5,
            };
            //Act
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => productService.UpdateProduct(id, product));
            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
            Assert.Equal(nameof(id), exception.ParamName);
        }
        [Theory]
        [InlineData(4)]
        public async Task Test_DeleteProduct_Returns_DeletedProductId(int id)
        {
            //Arrange
            _productRepositoryMock.Setup(p => p.DeleteProduct(id)).ReturnsAsync(id);
            //Act
            var deletedProductId = await productService.DeleteProduct(id);
            //Asserts
            Assert.IsType<int>(deletedProductId);
            Assert.Equal(id, deletedProductId);
        }
        [Theory]
        [InlineData(0)]
        public async Task Test_DeleteProduct_Returns_ArgumentException(int id)
        {
            //Act
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => productService.DeleteProduct(id));
            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
            Assert.Equal(nameof(id), exception.ParamName);
        }
    }
}
