﻿using EcommerceApp.Entities.DomainModels;

namespace EcommerceApp.Repositories.Contracts
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllCategories();
        Task<Category?> GetCategoryById(int id);
        Task<Category?> AddCategory(Category category);
        Task<int> UpdateCategory(int id, Category category);
        Task<int> DeleteCategory(int id);
    }
}
