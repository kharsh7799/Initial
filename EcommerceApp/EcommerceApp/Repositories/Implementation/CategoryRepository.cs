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
        public async Task<Category> AddCategory(Category category)
        {
            await dbContext.Categories.AddAsync(category);
            await dbContext.SaveChangesAsync();
            return category;
        }
        public async Task<int> DeleteCategory(Category category)
        {
            dbContext.Categories.Remove(category);
            await dbContext.SaveChangesAsync();
            return category.Id;
        }
        public async Task<List<Category>> GetAllCategories()
        {
            return await dbContext.Categories.ToListAsync();
        }
        public async Task<Category?> GetCategoryById(int id)
        {
            return await dbContext.Categories.FindAsync(id);
        }
        public async Task<int> UpdateCategory(Category category)
        {
            await dbContext.SaveChangesAsync();
            return category.Id;
        }
        public async Task<List<Category>> GetAllCategoriesWithProducts()
        {
         return await dbContext.Categories.Include(cat => cat.Products).ToListAsync();
        }
        public async Task<List<Category>> GetCategoryWithProducts(int categoryId)
        {
            var categories = dbContext.Categories.Include(cat => cat.Products);
            return  await categories.Where(x => x.Id == categoryId).ToListAsync();
        }

    }
}

