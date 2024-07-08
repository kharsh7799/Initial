using EcommerceApp.Entities.DomainModels;

namespace EcommerceApp.Services.Contracts
{
    public interface ICategoryWithProductsService
    {
        Task<List<Category>?> GetCategoryWithProductDetails(int? categoryid = null);

    }
}
