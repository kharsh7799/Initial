using EcommerceApp.Entities.DomainModels;

namespace EcommerceApp.Repositories.Contracts
{
    public interface ICategoryWithProductsRepo
    {
        Task<List<Category>?> GetCategoryWithProductDetails(int? categoryid = null);
    }
}
