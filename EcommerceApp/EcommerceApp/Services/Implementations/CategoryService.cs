using EcommerceApp.Entities.DomainModels;
using EcommerceApp.Repositories.Contracts;
using EcommerceApp.Repositories.Implementation;
using EcommerceApp.Services.Contracts;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EcommerceApp.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }
        public async Task<Category?> AddCategory(Category category)
        {
            var categories = await categoryRepository.GetAllCategories();
            var isPresent = categories.Any(x => x.Name?.ToUpper()==category.Name?.ToUpper());
            if (isPresent) { return null; }
            return await categoryRepository.AddCategory(category);
        }
        public async Task<int> DeleteCategory(int id)
        {
            var categoryData =  await categoryRepository.GetCategoryById(id);
            if(categoryData == null)
            {
                return -1;
            }
            return await categoryRepository.DeleteCategory(categoryData);
        }
        public async Task<List<Category>> GetAllCategories()
        {
           return await categoryRepository.GetAllCategories();
        }
        public async Task<Category?> GetCategoryById(int id)
        {
            return await categoryRepository.GetCategoryById(id);
        }     
        public async Task<int> UpdateCategory(int id, Category category)
        {
            var categoryData = await categoryRepository.GetCategoryById(id);
            if(categoryData == null) {  return -1; }
            categoryData.Name = category.Name;
            return await categoryRepository.UpdateCategory(categoryData);
        }
        public async Task<List<Category>> GetAllCategoriesWithProducts()
        {
            return await categoryRepository.GetAllCategoriesWithProducts();
        }
        public async Task<List<Category>> GetCategoryWithProducts(int categoryId)
        {
         return await categoryRepository.GetCategoryWithProducts(categoryId);
        }
    }
}
