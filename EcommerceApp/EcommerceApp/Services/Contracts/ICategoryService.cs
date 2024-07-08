using EcommerceApp.Entities.DomainModels;

namespace EcommerceApp.Services.Contracts
{
    public interface ICategoryService
    {
        Task<List<Category>> GetAllCategories();
        Task<Category?> GetCategoryById(int id);
        Task<Category?> AddCategory(Category category);
        Task<int> UpdateCategory(int id, Category category);
        Task<int> DeleteCategory(int id);
    }
}
