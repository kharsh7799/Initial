using EcommerceApp.CustomExceptions;
using EcommerceApp.Data;
using EcommerceApp.Entities.DomainModels;
using EcommerceApp.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;

namespace EcommerceApp.Repositories.Implementation
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext dbContext;
        private readonly ILogger<ProductRepository> logger;

        public ProductRepository(AppDbContext dbContext,ILogger<ProductRepository> logger)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }
        public async Task<Product?> AddProduct(Product product)
        {
            try
            {
                //verifying record is already present or not
                var isAdd = await dbContext.Products.AnyAsync(x => x.Name == product.Name);

                if (!isAdd)
                {
                    await dbContext.Products.AddAsync(product);
                    var productIsadded = await dbContext.SaveChangesAsync();

                    logger.LogInformation($"Product {product.Name} added successfully");
                    return product;
                }
                logger.LogInformation($"Product {product.Name} is already available in system. add another another");
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> DeleteProduct(int id)
        {
            try
            {
                var product = await dbContext.Products.FindAsync(id);
                if (product != null)
                {
                    dbContext.Remove(product);
                    var isSuccess = await dbContext.SaveChangesAsync();
                    if (isSuccess > 0)
                    {
                        logger.LogInformation($"Product Id {id} deleted successfully");

                        return id;
                    }
                }
                logger.LogInformation($"Product with id = {id} is not found.");
                return -1;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Product>> GetAllProducts(string? filterByNameValue = null)
        {
            var products = dbContext.Products.AsQueryable();
            if (!string.IsNullOrEmpty(filterByNameValue))
            {
                products = products.Where(x => x.Name.Contains(filterByNameValue));
            }
            return await products.ToListAsync();
        }

        public async Task<Product?> GetProductById(int id)
        {
            try
            {
                var product = await dbContext.Products.FindAsync(id);
                if (product != null)
                {
                    logger.LogInformation($"Product with id = {id} fetched successfully");
                    return product;
                }
                else
                {
                    logger.LogInformation($"Product with id = {id} is not found.");
                    return null;
                }
            }
            catch (Exception)
            {
                throw;
            }
        
        }
        public async Task<List<Product>?> GetProductByCategoryId(int categoryId)
        {
            if(categoryId > 0)
            {
                var productsByCategoryId = dbContext.Products.Where(x => x.CategoryId == categoryId);
                if (productsByCategoryId.Any())
                {
                    logger.LogInformation($"Product with categoryId = {categoryId} fetched successfully");
                    return await productsByCategoryId.ToListAsync();
                }
                else
                {
                    logger.LogInformation($"Product with id = {categoryId} is not found.");
                    return null;
                }
            }
            return null;
        }

        public async Task<int> UpdateProduct(int id, Product product)
        {
            try
            {
                var productData = await dbContext.Products.FindAsync(id);
                if (productData != null)
                {
                    productData.Name = product.Name;
                    productData.Price = product.Price;
                    productData.Rating = product.Rating;
                    productData.CategoryId = product.CategoryId;
                    var isSuccess = await dbContext.SaveChangesAsync();
                    if (isSuccess > 0)
                    {
                        logger.LogInformation($"Product {product.Name} updated successfully");
                        return id;
                    }
                }
                logger.LogInformation($"Product with id = {id} is not found.");
                return -1;
            }
            catch (Exception)
            {
                throw;
            }
           
        }
    }
}

