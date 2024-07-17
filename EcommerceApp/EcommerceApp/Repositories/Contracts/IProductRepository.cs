using EcommerceApp.Entities.DomainModels;

namespace EcommerceApp.Repositories.Contracts
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllProducts();
        Task<List<Product>> GetProductsByName(string filterByNameValue);
        Task<Product?> GetProductById(int id);
        Task<List<Product>> GetProductByCategoryId(int categoryId);
        Task<Product> AddProduct(Product product);
        Task<int> UpdateProduct(Product product);
        Task<int> DeleteProduct(Product product);
    }
}
