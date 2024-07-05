using EcommerceApp.Entities.DomainModels;

namespace EcommerceApp.Repositories.Contracts
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllProducts(string? filterByNameValue = null);
        Task<Product?> GetProductById(int id);

        Task<List<Product>?> GetProductByCategoryId(int categoryId);
        Task<Product?> AddProduct(Product product);
        Task<int> UpdateProduct(int id, Product product);
        Task<int> DeleteProduct(int id);
    }
}
