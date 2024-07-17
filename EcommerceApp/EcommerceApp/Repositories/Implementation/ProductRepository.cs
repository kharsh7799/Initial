﻿using EcommerceApp.Data;
using EcommerceApp.Entities.DomainModels;
using EcommerceApp.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApp.Repositories.Implementation
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext dbContext;

        public ProductRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<Product?> GetProductById(int id)
        {
            return await dbContext.Products.FindAsync(id);
        }
        public async Task<List<Product>> GetProductByCategoryId(int categoryId)
        {
            return await dbContext.Products.Where(x => x.CategoryId == categoryId).ToListAsync();
        }
        public async Task<List<Product>> GetAllProducts()
        {
           return await dbContext.Products.ToListAsync();
        }
        public async Task<List<Product>> GetProductsByName(string filterByNameValue)
        {
            var products = dbContext.Products.AsQueryable();
            return await products.Where(x => x.Name.Contains(filterByNameValue)).ToListAsync();
        }
        public async Task<Product> AddProduct(Product product)
        {
            await dbContext.Products.AddAsync(product);
            await dbContext.SaveChangesAsync();
            return product;
        }
        public async Task<int> UpdateProduct(Product product)
        {
            await dbContext.SaveChangesAsync();
            return product.Id;
        }
        public async Task<int> DeleteProduct(Product product)
        {
             dbContext.Products.Remove(product);
             await dbContext.SaveChangesAsync();
             return product.Id;
         
        }
    }
}

