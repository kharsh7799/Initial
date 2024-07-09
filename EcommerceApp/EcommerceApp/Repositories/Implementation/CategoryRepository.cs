using EcommerceApp.Data;
using EcommerceApp.Entities.DomainModels;
using EcommerceApp.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApp.Repositories.Implementation
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext dbContext;
        public CategoryRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<Category?> AddCategory(Category category)
        {
            try
            {
                //verifying record is already present or not
                var isAdd = await dbContext.Categories.AnyAsync(x => x.Name == category.Name);

                if (!isAdd)
                {
                    await dbContext.Categories.AddAsync(category);
                    var categoryIsadded = await dbContext.SaveChangesAsync();
                    return category;
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> DeleteCategory(int id)
        {
            try
            {
                var category = await dbContext.Categories.FindAsync(id);
                if (category != null)
                {
                    dbContext.Remove(category);
                    var isSuccess = await dbContext.SaveChangesAsync();
                    return id;
                }
                return -1;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<List<Category>> GetAllCategories()
        {
            var categories = await dbContext.Categories.ToListAsync();
            return categories;
        }

        public async Task<Category?> GetCategoryById(int id)
        {
            var category = await dbContext.Categories.FindAsync(id);
            if (category != null)
            {
                return category;
            }
            else
            {
                return null;
            }
        }

        public async Task<int> UpdateCategory(int id, Category category)
        {
            try
            {
                var categoryData = await dbContext.Categories.FindAsync(id);
                if (categoryData != null)
                {
                    categoryData.Name = category.Name;
                    var isSuccess = await dbContext.SaveChangesAsync();
                    return id;
                }
                return -1;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
