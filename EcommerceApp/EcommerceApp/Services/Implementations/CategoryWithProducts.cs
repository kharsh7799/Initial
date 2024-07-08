using EcommerceApp.Entities.DomainModels;
using EcommerceApp.Repositories.Contracts;
using EcommerceApp.Services.Contracts;

namespace EcommerceApp.Services.Implementations
{
    public class CategoryWithProducts : ICategoryWithProductsService
    {
        private readonly ICategoryWithProductsRepo categoryWithProductsRepo;
        private readonly ILogger<CategoryWithProducts> logger;

        public CategoryWithProducts(ICategoryWithProductsRepo categoryWithProductsRepo, ILogger<CategoryWithProducts> logger)
        {
            this.categoryWithProductsRepo = categoryWithProductsRepo;
            this.logger = logger;
        }
        public Task<List<Category>?> GetCategoryWithProductDetails(int? categoryid = null)
        {
            try
            {
                if (categoryid != null)
                {
                    if (categoryid > 0)
                    {
                        var catWithproductsBycatId = categoryWithProductsRepo.GetCategoryWithProductDetails(categoryid);
                        return catWithproductsBycatId;
                    }
                    logger.LogInformation($"Invalid category id entered.");
                    throw new ArgumentNullException(nameof(categoryid));
                }
                var catWithproducts = categoryWithProductsRepo.GetCategoryWithProductDetails();
                return catWithproducts;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
