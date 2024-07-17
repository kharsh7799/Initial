using EcommerceApp.Entities.DomainModels;
using EcommerceApp.Entities.DTOs.Product;
using EcommerceApp.Repositories.Contracts;
using EcommerceApp.Services.Contracts;
using EcommerceApp.Services.Implementations;
using ECommerceAppTest.DataSetUp;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Runtime.InteropServices;

namespace ECommerceAppTest.ServicesTests
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly Mock<ICategoryRepository> categoryRepository;
        private readonly IProductService productService;
        private readonly InitialProductDataSetUp initialProductDataSetUp;
        private readonly List<Product> products; 
        private readonly Product productModel;
        private readonly List<Category> categories;
        public ProductServiceTests()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            categoryRepository = new Mock<ICategoryRepository>();
            productService = new ProductService(_productRepositoryMock.Object, categoryRepository.Object);
            initialProductDataSetUp = new InitialProductDataSetUp();
            products=initialProductDataSetUp.ProductList;
            productModel=initialProductDataSetUp.ProductModel;
            categories =initialProductDataSetUp.CategoryList;
        }
        private Product? GetProductById(int id)
        {
            return products?.Find(x => x.Id == id);
        }
        private Category? GetCategoryById(int id)
        {
            return categories?.Find(x => x.Id == id);
        }

        [Fact]
        public async Task Test_GetAllProducts_Returns_ProductList()
        {
            //Arrange
            _productRepositoryMock.Setup(p => p.GetAllProducts()).ReturnsAsync(products);
            //Act
            var productsData = await productService.GetAllProducts();

            //Assert
            Assert.NotNull(productsData);
            Assert.IsType<List<Product>>(productsData);
            Assert.Equal(products[0].Id, productsData[0].Id);
            Assert.Equal(products[0].Name, productsData[0].Name);
            Assert.Equal(products[0].Rating, productsData[0].Rating);
            Assert.Equal(products[0].Price, productsData[0].Price);
            Assert.Equal(products.Count, productsData.Count);

        }
        [Fact]
        public async Task Test_GetAllProducts_Returns_ProductListByName()
        {
            var name = "Nike sneaker";
            var productsByname = products.Where(x => x.Name.Contains(name)).ToList();
            //Arrange
            _productRepositoryMock.Setup(p => p.GetProductsByName(name)).ReturnsAsync(productsByname);
            //Act
            var productsData = await productService.GetAllProducts(name);

            //Assert
            Assert.NotNull(productsData);
            Assert.IsType<List<Product>>(productsData);
            Assert.Equal(productsByname[0].Id, productsData[0].Id);
            Assert.Equal(productsByname[0].Name, productsData[0].Name);
            Assert.Equal(productsByname[0].Rating, productsData[0].Rating);
            Assert.Equal(productsByname[0].Price, productsData[0].Price);
            Assert.Equal(productsByname.Count, productsData.Count);

        }
        [Fact]
        public async Task Test_GetAllProducts_Returns_EmptyProductList()
        {
            //Arrange
            var productList = new List<Product>();
            _productRepositoryMock.Setup(p => p.GetAllProducts()).ReturnsAsync(productList);
            //Act
            var productsData = await productService.GetAllProducts(null);
            //Assert
            Assert.IsType<List<Product>>(productsData);
            Assert.Equal(productList.Count, productsData.Count);

        }
        [Fact]
        public async Task Test_GetProductById_Returns_Product()
        {
            //Arrange
            var id = 4;
            var product = GetProductById(id);
            _productRepositoryMock.Setup(p => p.GetProductById(id)).ReturnsAsync(product);
            //Act
            var productData = await productService.GetProductById(id);
            //Assert
            Assert.NotNull(productData);
            Assert.IsType<Product>(productData);
            Assert.Equal(product?.Id, productData.Id);
            Assert.Equal(product?.Name, productData.Name);
            Assert.Equal(product?.Rating, productData.Rating);
            Assert.Equal(product?.Price, productData.Price);
            Assert.Equal(product?.CategoryId, productData.CategoryId);
        }
        [Fact]
        public async Task Test_GetProductById_Returns_NullOrNoProduct()
        {
            //Arrange
            var id = 460;
            var product = GetProductById(id);
            _productRepositoryMock.Setup(p => p.GetProductById(id)).ReturnsAsync(product);
            //Act
            var productData = await productService.GetProductById(id);
            //Assert
            Assert.Null(productData);
        }
        [Fact]
        public async Task Test_GetProductByCategoryId_Returns_Product()
        {
            //Arrange
            var categoryId = 1;
            var productList = products.Where(x => x.CategoryId == categoryId).ToList();
            _productRepositoryMock.Setup(p => p.GetProductByCategoryId(categoryId)).ReturnsAsync(productList);
            //Act
            var productsData = await productService.GetProductByCategoryId(categoryId);
            //Assert
            Assert.NotNull(productsData);
            Assert.IsType<List<Product>>(productsData);
            Assert.Equal(productList[0].Id, productsData[0].Id);
            Assert.Equal(productList[0].Name, productsData[0].Name);
            Assert.Equal(productList[0].Rating, productsData[0].Rating);
            Assert.Equal(productList[0].Price, productsData[0].Price);
            Assert.Equal(categoryId, productsData[0].CategoryId);
        }
        [Fact]
        public async Task Test_GetProductByCategoryId_Returns_EmptyResult()
        {
            var categoryId = 515;
            var productList = products.Where(x => x.CategoryId == categoryId).ToList();
            _productRepositoryMock.Setup(p => p.GetProductByCategoryId(categoryId)).ReturnsAsync(productList);
            //Act
            var productsData = await productService.GetProductByCategoryId(categoryId);
            //Assert
            Assert.NotNull(productsData);
            Assert.IsType<List<Product>>(productsData);
            Assert.Equal(productList.Count, productsData.Count);
        }
        [Fact]
        public async Task Test_AddProduct_Returns_CreatedProduct()
        {
            //Arrange
             _productRepositoryMock.Setup(p => p.GetAllProducts()).ReturnsAsync(products);
            _productRepositoryMock.Setup(p => p.AddProduct(It.IsAny<Product>())).ReturnsAsync(productModel);
            //Act
            var productData = await productService.AddProduct(productModel);
            //Asserts
            Assert.NotNull(productData);
            Assert.IsType<Product>(productData);
            Assert.Equal(productModel.Name, productData.Name);
            Assert.Equal(productModel.Price, productData.Price);
            Assert.Equal(productModel.Rating, productData.Rating);
            Assert.Equal(productModel.CategoryId, productData.CategoryId);

        }
        [Fact]
        public async Task Test_AddProduct_Returns_NullWhenProductIsAlradyPresent()
        {
            //Arrange
            var productToAdd = new Product
            {
                Name = "Nike sneaker",
                Price = 15000,
                Rating = 1,
                CategoryId = 5,
            };
            //Act
            _productRepositoryMock.Setup(p => p.GetAllProducts()).ReturnsAsync(products);
            var productData = await productService.AddProduct(productToAdd);
            //Asserts
            Assert.Null(productData);
        }
        [Fact]
        public async Task Test_UpdateProduct_Returns_UpdatedProductId()
        {
            //Arrange
            var id = 4;
            var product = GetProductById(id);
            var category = GetCategoryById(productModel.CategoryId);
            _productRepositoryMock.Setup(p => p.GetProductById(id)).ReturnsAsync(product);
            categoryRepository.Setup(p => p.GetCategoryById(productModel.CategoryId)).ReturnsAsync(category);
            _productRepositoryMock.Setup(p => p.UpdateProduct(It.IsAny<Product>())).ReturnsAsync(id);
            //Act
            var productData = await productService.UpdateProduct(id, productModel);
            //Asserts
            Assert.IsType<int>(productData);
            Assert.Equal(id, productData);
        }
        [Fact]
        public async Task Test_UpdateProduct_Returns_WhenNoIdIsFound()
        {
            var id = 400;
            productModel.CategoryId = 126;
            var product = GetProductById(id);
            _productRepositoryMock.Setup(p => p.GetProductById(id)).ReturnsAsync(product);
            //Act
            var productData = await productService.UpdateProduct(id, productModel);
            Assert.IsType<int>(productData);
            Assert.Equal(-1, productData);
        }
        [Fact]
        public async Task Test_UpdateProduct_Returns_WhenCategoryIdIsNotPresent()
        {
            var id = 4;
            productModel.CategoryId = 126;
            var product = GetProductById(id);
            _productRepositoryMock.Setup(p => p.GetProductById(id)).ReturnsAsync(product);
            var category = GetCategoryById(productModel.CategoryId);
            categoryRepository.Setup(p => p.GetCategoryById(productModel.CategoryId)).ReturnsAsync(category);

            //Act
            var productData = await productService.UpdateProduct(id, productModel);
            Assert.IsType<int>(productData);
            Assert.Equal(-2, productData);
        }
        [Fact]
        public async Task Test_DeleteProduct_Returns_DeletedProductId()
        {
            //Arrange
            var id = 4;
            var product = GetProductById(id);
            _productRepositoryMock.Setup(p => p.GetProductById(id)).ReturnsAsync(product);
            _productRepositoryMock.Setup(p => p.DeleteProduct(It.IsAny<Product>())).ReturnsAsync(id);
            //Act
            var deletedProductId = await productService.DeleteProduct(id);
            //Asserts
            Assert.IsType<int>(deletedProductId);
            Assert.Equal(id, deletedProductId);
        }
        [Fact]
        public async Task Test_DeleteProduct_Returns_WhenNoIdIsFound()
        {
            var id = 311;
            var product = GetProductById(id);
            _productRepositoryMock.Setup(p => p.GetProductById(id)).ReturnsAsync(product);
            //Act
            var deletedProductId = await productService.DeleteProduct(id);
            //Asserts
            Assert.IsType<int>(deletedProductId);
            Assert.Equal(-1, deletedProductId);
        }
    }
}
