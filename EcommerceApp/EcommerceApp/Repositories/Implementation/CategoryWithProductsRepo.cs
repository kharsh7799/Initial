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
        public CategoryWithProductsRepo(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
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

                return await categoriesWithproducts.ToListAsync();
            }
            else
            {
                return null;
            }
        }
    }
}
