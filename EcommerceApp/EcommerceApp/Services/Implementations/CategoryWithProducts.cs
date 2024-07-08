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
        public async Task<List<Category>?> GetCategoryWithProductDetails(int? categoryId = null)
        {
            try
            {
                if (categoryId != null)
                {
                    if (categoryId > 0)
                    {
                        var catWithproductsBycatId = await categoryWithProductsRepo.GetCategoryWithProductDetails(categoryId);
                        return catWithproductsBycatId;
                    }
                    logger.LogInformation($"Invalid category id entered.");
                    throw new ArgumentNullException(nameof(categoryId));
                }
                var catWithproducts = await categoryWithProductsRepo.GetCategoryWithProductDetails();
                return catWithproducts;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
