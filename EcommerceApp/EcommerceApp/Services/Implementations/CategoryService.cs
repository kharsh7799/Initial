using EcommerceApp.Entities.DomainModels;
using EcommerceApp.Repositories.Contracts;
using EcommerceApp.Services.Contracts;

namespace EcommerceApp.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository categoryRepository;
        private readonly ILogger<CategoryService> logger;

        public CategoryService(ICategoryRepository categoryRepository, ILogger<CategoryService> logger)
        {
            this.categoryRepository = categoryRepository;
            this.logger = logger;
        }
        public Task<Category?> AddCategory(Category category)
        {
            try
            {
                var categoryData = categoryRepository.AddCategory(category);
                return categoryData;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Task<int> DeleteCategory(int id)
        {
            try
            {
                if (id > 0)
                {
                  var deletedCategoryData = categoryRepository.DeleteCategory(id);
                  return deletedCategoryData;
                }
                logger.LogInformation($"Invalid category id entered.");
                throw new ArgumentNullException(nameof(id));
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Task<List<Category>> GetAllCategories()
        {
           return categoryRepository.GetAllCategories();
        }

        public Task<Category?> GetCategoryById(int id)
        {
            try
            {
                if (id > 0)
                {
                    var categoryData = categoryRepository.GetCategoryById(id);
                    return categoryData;
                }
                logger.LogInformation($"Invalid category id entered.");
                throw new ArgumentNullException(nameof(id));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task<int> UpdateCategory(int id, Category category)
        {
            try
            {
                if (id > 0)
                {
                    var updatedCategoryData = categoryRepository.DeleteCategory(id);
                    return updatedCategoryData;
                }
                logger.LogInformation($"Invalid category id entered.");
                throw new ArgumentNullException(nameof(id));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
