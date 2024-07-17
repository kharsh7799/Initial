using EcommerceApp.Entities.DomainModels;
using EcommerceApp.Repositories.Contracts;
using EcommerceApp.Repositories.Implementation;
using EcommerceApp.Services.Contracts;
using System.Linq.Expressions;

namespace EcommerceApp.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository productRepository;
        private readonly ICategoryRepository categoryRepository;

        public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            this.productRepository = productRepository;
            this.categoryRepository = categoryRepository;
        }
        public async Task<Product?> AddProduct(Product product)
        {
            var products = await productRepository.GetAllProducts();
            var isPresent = products.Any(x => x.Name?.ToUpper() == product.Name?.ToUpper());
            if(isPresent)
            {
                return null;
            }
            return await productRepository.AddProduct(product);
        }
        public async Task<int> DeleteProduct(int id)
        {
            var productData = await productRepository.GetProductById(id);
            if (productData == null)
            {
                return -1;
            }
           return await productRepository.DeleteProduct(productData);
        }
        public async Task<List<Product>> GetAllProducts(string? filterByNameValue = null)
        {
            if(filterByNameValue == null)
            {
                return await productRepository.GetAllProducts();
            }
            return await productRepository.GetProductsByName(filterByNameValue);
        }
        public async Task<List<Product>> GetProductByCategoryId(int categoryId)
        {
            return await productRepository.GetProductByCategoryId(categoryId);
        }
        public async Task<Product?> GetProductById(int id)
        {
            return await productRepository.GetProductById(id);
        }
        public async Task<bool> IsCategoryPresent(int categoryId)
        {
            var category = await categoryRepository.GetCategoryById(categoryId);
            if (category == null)
            {
                return false;
            }
            return category.Id == categoryId;
        }
        public async Task<int> UpdateProduct(int id, Product product)
        {
            var productData = await productRepository.GetProductById(id);
            if (productData == null)
            {
                return -1;
            }
            var isPresent = await IsCategoryPresent(product.CategoryId);
            if (!isPresent)
            {
                return -2;
            }
            productData.Name = product.Name;
            productData.Price = product.Price;
            productData.Rating = product.Rating;
            productData.CategoryId = product.CategoryId;
            return await productRepository.UpdateProduct(productData);
        }
    }
}
