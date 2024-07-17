using EcommerceApp.Data;
using EcommerceApp.Entities.DomainModels;
using EcommerceApp.Repositories.Contracts;
using EcommerceApp.Repositories.Implementation;
using EcommerceApp.Services.Implementations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq;

namespace ECommerceAppTest.Repositories
{
    public class ProductRepositoryTest
    {
        [Fact]
        public async Task Test_GetAllProducts_Returns_ProductList()
        {
            //Arranage
            var options = new DbContextOptionsBuilder<AppDbContext>()
               .UseInMemoryDatabase(databaseName: "GetAllProducts_ProductList")
               .Options;

            using (var _dbContext = new AppDbContext(options))
            {
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
                await _dbContext.Products.AddRangeAsync(products);
                await _dbContext.SaveChangesAsync();
                var productRepository = new ProductRepository(_dbContext);
                var productListData = await productRepository.GetAllProducts();

                //Asserts
                Assert.NotNull(productListData);
                Assert.IsType<List<Product>>(productListData);
                Assert.Equal(products[0].Id, productListData[0].Id);
                Assert.Equal(products[0].Name, productListData[0].Name);
                Assert.Equal(products[0].Rating, productListData[0].Rating);
                Assert.Equal(products[0].Price, productListData[0].Price);
                Assert.Equal(products.Count, productListData.Count);
            }
        }
        [Fact]
        public async Task Test_GetAllProducts_Returns_ProductListByName()
        {
            //Arranage
            var options = new DbContextOptionsBuilder<AppDbContext>()
               .UseInMemoryDatabase(databaseName: "GetAllProductsByName_ProductList")
               .Options;

            using (var _dbContext = new AppDbContext(options))
            {
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
                await _dbContext.Products.AddRangeAsync(products);
                await _dbContext.SaveChangesAsync();
                var productRepository = new ProductRepository(_dbContext);
                var productListData = await productRepository.GetProductsByName("Nike sneaker");

                //Asserts
                Assert.NotNull(productListData);
                Assert.IsType<List<Product>>(productListData);
                Assert.Equal(products[0].Id, productListData[0].Id);
                Assert.Equal(products[0].Name, productListData[0].Name);
                Assert.Equal(products[0].Rating, productListData[0].Rating);
                Assert.Equal(products[0].Price, productListData[0].Price);
                Assert.Single(productListData);
            }
        }
        [Fact]
        public async Task Test_GetAllProducts_Returns_EmptyResult()
        {
            //Arranage
            var options = new DbContextOptionsBuilder<AppDbContext>()
               .UseInMemoryDatabase(databaseName: "ProductList_EmptyResult")
               .Options;

            using (var _dbContext = new AppDbContext(options))
            {
                var products = new List<Product>();
          
                await _dbContext.Products.AddRangeAsync(products);
                await _dbContext.SaveChangesAsync();
                var productRepository = new ProductRepository(_dbContext);
                var productListData = await productRepository.GetAllProducts();

                //Asserts
                Assert.NotNull(productListData);
                Assert.IsType<List<Product>>(productListData);
                Assert.Equal(products.Count, productListData.Count);
            }
        }
        [Theory]
        [InlineData(1)]
        public async Task Test_GetProductById_Returns_Product(int id)
        {
            //Arranage
            var options = new DbContextOptionsBuilder<AppDbContext>()
               .UseInMemoryDatabase(databaseName: "GetProductById_Product")
               .Options;

            using (var _dbContext = new AppDbContext(options))
            {
                var product = new Product
                {
                    Id = 1,
                    Name = "Nike sneaker",
                    Price = 15000,
                    Rating = 1,
                    CategoryId = 5,
                };
                await _dbContext.Products.AddAsync(product);
                await _dbContext.SaveChangesAsync();
                var productRepository = new ProductRepository(_dbContext);
                var productData = await productRepository.GetProductById(id);

                //Asserts
                Assert.NotNull(productData);
                Assert.IsType<Product>(productData);
                Assert.Equal(id, productData.Id);
                Assert.Equal(product.Name, productData.Name);
                Assert.Equal(product.Rating, productData.Rating);
                Assert.Equal(product.Price, productData.Price);
            }
        }
        [Theory]
        [InlineData(2)]
        public async Task Test_GetProductById_Returns_NullOrNoProduct(int id)
        {
            //Arranage
            var options = new DbContextOptionsBuilder<AppDbContext>()
               .UseInMemoryDatabase(databaseName: "GetProductById_NullProduct")
               .Options;

            using (var _dbContext = new AppDbContext(options))
            {
                var product = new Product
                {
                    Id = 1,
                    Name = "Nike sneaker",
                    Price = 15000,
                    Rating = 1,
                    CategoryId = 5,
                };
                await _dbContext.Products.AddAsync(product);
                await _dbContext.SaveChangesAsync();
                var productRepository = new ProductRepository(_dbContext);
                var productData = await productRepository.GetProductById(id);

                //Asserts
                Assert.Null(productData);
                Assert.NotEqual(id, product.Id);
       
            }
        }
        [Theory]
        [InlineData(5)]
        public async Task Test_GetProductByCategoryId_Returns_ProductList(int categoryId)
        {
            //Arranage
            var options = new DbContextOptionsBuilder<AppDbContext>()
               .UseInMemoryDatabase(databaseName: "GetProductByCategoryId_Product")
               .Options;

            using (var _dbContext = new AppDbContext(options))
            {
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
                Name = "Maggi",
                Price=120,
                Rating=2,
                CategoryId=3,
              }
            };
                await _dbContext.Products.AddRangeAsync(products);
                await _dbContext.SaveChangesAsync();
                var productRepository = new ProductRepository(_dbContext);
                var productData = await productRepository.GetProductByCategoryId(categoryId);

                //Asserts
                Assert.NotNull(productData);
                Assert.IsType<List<Product>>(productData);
                Assert.Equal(2, productData.Count);
            }
        }
        [Theory]
        [InlineData(2)]
        public async Task Test_GetProductByCategoryId_Returns_NullOrNoProduct(int categoryId)
        {
            //Arranage
            var options = new DbContextOptionsBuilder<AppDbContext>()
               .UseInMemoryDatabase(databaseName: "GetProductByCategoryId_NullProduct")
               .Options;

            using (var _dbContext = new AppDbContext(options))
            {
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
                Name = "Maggi",
                Price=120,
                Rating=2,
                CategoryId=3,
              }
            };
                await _dbContext.Products.AddRangeAsync(products);
                await _dbContext.SaveChangesAsync();
                var productRepository = new ProductRepository(_dbContext);
                var productData = await productRepository.GetProductByCategoryId(categoryId);

                //Asserts
                Assert.Empty(productData);
                foreach (var product in products)
                {
                    Assert.NotEqual(categoryId, product.CategoryId);
                }

            }
        }
        [Fact]
        public async Task Test_AddProduct_Returns_CreatedProduct()
        {
            //Arranage
            var options = new DbContextOptionsBuilder<AppDbContext>()
               .UseInMemoryDatabase(databaseName: "AddProduct_CreatedProduct")
               .Options;

            using (var _dbContext = new AppDbContext(options))
            {
                var product = new Product
                {
                    Name = "Nike sneaker",
                    Price = 15000,
                    Rating = 1,
                    CategoryId = 5,
                };
                var productRepository = new ProductRepository(_dbContext);
                var productData = await productRepository.AddProduct(product);

                //Asserts
                Assert.NotNull(productData);
                Assert.IsType<Product>(productData);
                Assert.Equal(1, productData.Id);
                Assert.Equal(product.Name, productData.Name);
                Assert.Equal(product.Rating, productData.Rating);
                Assert.Equal(product.Price, productData.Price);
            }
        }
        [Fact]
        public async Task Test_AddProduct_Returns_Exception()
        {
            //Arranage
            var options = new DbContextOptionsBuilder<AppDbContext>()
               .UseInMemoryDatabase(databaseName: "AddProduct_ThrowsException")
               .Options;

            using(var _dbContext = new AppDbContext(options))
            {
                var productToAdd = new Product
                {
                    Id=1,
                    Name = "Noodles",
                    Price = 15000,
                    Rating = 1,
                    CategoryId = 2,
                };
                await _dbContext.Products.AddAsync(productToAdd);
                await _dbContext.SaveChangesAsync();
            }
            using (var _dbContext = new AppDbContext(options))
            {
                var productToAdd = new Product
                {
                    Id=1,
                    Name = "Maggi",
                    Price = 15000,
                    Rating = 1,
                    CategoryId = 3,
                };
                var productRepository = new ProductRepository(_dbContext);
                var exception = await Assert.ThrowsAsync<ArgumentException>(() => productRepository.AddProduct(productToAdd));
                //Asserts
                Assert.NotNull(exception);
                Assert.IsType<ArgumentException>(exception);

            }
        }
        [Fact]
        public async Task Test_UpdateProduct_Returns_ProductId()
        {
            //Arranage
            var options = new DbContextOptionsBuilder<AppDbContext>()
               .UseInMemoryDatabase(databaseName: "UpdateProduct_ProductId")
               .Options;

            using (var _dbContext = new AppDbContext(options))
            {
                var product = new Product
                {
                    Id=4,
                    Name = "Noodles",
                    Price = 200,
                    Rating = 1,
                    CategoryId = 3,
                };
                await _dbContext.Products.AddAsync(product);
                await _dbContext.SaveChangesAsync();
                var productToUpdate = new Product
                {
                    Id = 4,
                    Name = "Maggi",
                    Price = 150,
                    Rating = 3,
                    CategoryId = 3,
                };
                var productRepository = new ProductRepository(_dbContext);
                var productId = await productRepository.UpdateProduct(productToUpdate);
                //Asserts
                Assert.IsType<int>(productId);
                Assert.Equal(productId, productToUpdate.Id);
            }
        }
        [Fact]
        public async Task Test_DeleteProduct_Returns_ProductId()
        {
            //Arranage
            var options = new DbContextOptionsBuilder<AppDbContext>()
               .UseInMemoryDatabase(databaseName: "DeleteProduct_ProductId")
               .Options;

            using (var _dbContext = new AppDbContext(options))
            {
                var product = new Product
                {
                    Id = 4,
                    Name = "Nike sneaker",
                    Price = 15000,
                    Rating = 1,
                    CategoryId = 5,
                };
                await _dbContext.Products.AddAsync(product);
                await _dbContext.SaveChangesAsync();
                var productRepository = new ProductRepository(_dbContext);
                var productId = await productRepository.DeleteProduct(product);
                var productData = await productRepository.GetProductById(productId);

                //Asserts
                Assert.IsType<int>(productId);
                Assert.Equal(product.Id, productId);
                Assert.Null(productData);
            }
        }
    }
}
