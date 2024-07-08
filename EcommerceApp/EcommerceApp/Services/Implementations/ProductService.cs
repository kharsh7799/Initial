using EcommerceApp.Entities.DomainModels;
using EcommerceApp.Repositories.Contracts;
using EcommerceApp.Services.Contracts;
using System.Linq.Expressions;

namespace EcommerceApp.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository productRepository;
        private readonly ILogger<ProductService> logger;

        public ProductService(IProductRepository productRepository, ILogger<ProductService> logger)
        {
            this.productRepository = productRepository;
            this.logger = logger;
        }
        public async Task<Product?> AddProduct(Product product)
        {
            try
            {
                var productAdd = await productRepository.AddProduct(product);
                logger.LogInformation($"Product {product.Name} added successfully");
                return productAdd;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<int> DeleteProduct(int id)
        {
            try
            {
                if (id > 0) 
                { 
                    var deletedproduct = await productRepository.DeleteProduct(id);
                    return deletedproduct;
                }
                logger.LogInformation($"Invalid product id entered.");
                throw new ArgumentNullException(nameof(id));
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<List<Product>> GetAllProducts(string? filterByNameValue = null)
        {
            return await productRepository.GetAllProducts(filterByNameValue);
        }
        public async Task<List<Product>?> GetProductByCategoryId(int categoryId)
        {
            try
            {
                if (categoryId > 0)
                {
                    var productList = await productRepository.GetProductByCategoryId(categoryId);
                    logger.LogInformation($"Product with categoryId = {categoryId} fetched successfully");
                    return productList;
                }
                logger.LogInformation($"Invalid category id entered.");
                throw new ArgumentNullException(nameof(categoryId));
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<Product?> GetProductById(int id)
        {      
            try
            {
                if(id > 0)
                {
                    var product = await productRepository.GetProductById(id);
                    logger.LogInformation($"Product data with product id = {id} fetched successfully");
                    return product;
                }
                logger.LogInformation($"Invalid product id entered.");
                throw new ArgumentNullException(nameof(id));
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<int> UpdateProduct(int id, Product product)
        {
            try
            {
                if(id>0)
                {
                    var updatedProduct = await productRepository.UpdateProduct(id, product);
                    return updatedProduct;
                }
                logger.LogInformation($"Invalid product id entered.");
                throw new ArgumentNullException(nameof(id));
            }
            catch (Exception)
            {
                throw;
            }   
        }
    }
}
