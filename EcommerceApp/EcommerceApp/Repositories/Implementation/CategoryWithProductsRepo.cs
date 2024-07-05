using EcommerceApp.Data;
using EcommerceApp.Entities.DomainModels;
using EcommerceApp.Repositories.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApp.Repositories.Implementation
{
    public class CategoryWithProductsRepo : ICategoryWithProductsRepo
    {
        private readonly AppDbContext dbContext;
        private readonly ILogger<CategoryWithProductsRepo> logger;

        public CategoryWithProductsRepo(AppDbContext dbContext, ILogger<CategoryWithProductsRepo> logger)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }
        public async Task<List<Category>?> GetCategoryWithProductDetails(int? categoryid = null)
        {
            var categoriesWithproducts = dbContext.Categories.Include(cat => cat.Products);
         
            if (categoriesWithproducts.Any())
            {
                if (categoryid != null)
                {
                    var categoryWithProducts = categoriesWithproducts.Where(x => x.Id == categoryid);
                    return await categoryWithProducts.ToListAsync();
                }

                logger.LogInformation($"category with id = {categoryid} fetched successfully");
                return await categoriesWithproducts.ToListAsync();
            }
            else
            {
                logger.LogInformation($"category with id = {categoryid} is not found.");
                return null;
            }
        }
    }
}
