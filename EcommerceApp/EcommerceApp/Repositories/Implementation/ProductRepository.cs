using EcommerceApp.Data;
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

                    return product;
                }
                return null;
            }
            catch(Exception)
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
                    return id;
                }
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
                    return product;
                }
                else
                {
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
                var productsByCategoryId = dbContext.Products.Where(x => x.CategoryId == categoryId);
                if (productsByCategoryId.Any())
                {
                    return await productsByCategoryId.ToListAsync();
                }
                else
                {
                    return null;
                }
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
                    return id;
                }
                return -1;
            }
            catch (Exception)
            {
                throw;
            }
           
        }
    }
}

