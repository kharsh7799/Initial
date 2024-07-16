using EcommerceApp.Entities.DomainModels;
using EcommerceApp.Entities.DTOs.Category;
using EcommerceApp.Entities.DTOs.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceAppTest.DataSetUp
{
    public class InitialProductDataSetUp
    {
        public List<Product> ProductList { get; set; }
        public ProductRequestDTO ProductRequestDTO { get; set; }
        public ProductResponseDTO ProductResponseDTO { get; set; }
        public Product ProductModel { get; set; }
        public List<Category> CategoryList { get; set; }
        public InitialProductDataSetUp()
        {
            this.ProductList = GetProductList();
            this.ProductRequestDTO = GetProductRequestDTO();
            this.ProductResponseDTO = GetProductResponseDTO();
            this.CategoryList = GetCategoryList();
            this.ProductModel= GetProductModel();
        }
        private List<Product> GetProductList()
        {
           return  new List<Product>
            {
              new Product
              {
                Id = 1,
                Name = "Nike sneaker",
                Price=15000,
                Rating=1,
                CategoryId=5,
              },
              new Product
              {
                Id = 2,
                Name = "Adidas sports",
                Price=20000,
                Rating=2,
                CategoryId=4,
              },
              new Product
              {
                Id = 3,
                Name = "Maggi",
                Price=150,
                Rating=4,
                CategoryId=3,
              },
              new Product
              {
                Id = 4,
                Name = "Vivo 10",
                Price=15000,
                Rating=3,
                CategoryId=1,
              },
              new Product
              {
                Id = 5,
                Name = "Redmi Note 9",
                Price=18000,
                Rating=4,
                CategoryId=1,
              }
            };

        }
        private ProductRequestDTO GetProductRequestDTO()
        {
           return new ProductRequestDTO()
            {
                Name = "Iphone 14",
                Price = 150000,
                Rating = 4,
                CategoryId = 1,
            };

        }
        private Product GetProductModel()
        {
            return new Product()
            {
                Name = "Iphone 14",
                Price = 150000,
                Rating = 4,
                CategoryId = 1,
            };

        }

        private ProductResponseDTO GetProductResponseDTO()
        {
            return new ProductResponseDTO()
            {
                Name = "Iphone 14",
                Price = 150000,
                Rating = 4,
                CategoryId = 1,
            };
        }
        private List<Category> GetCategoryList()
        {
           return  new List<Category>
            {
               new Category
               {
                   Id=1,
                   Name="Mobile"
               },
               new Category
               {
                   Id=3,
                   Name="Food"
               },
               new Category
               {
                   Id=4,
                   Name="Sport"
               },
               new Category
               {
                   Id=5,
                   Name="Shoes"
               }
            };
        }


    }
    public class InitialCategoryDataSetUp
    {
        public List<Category> CategoryList { get; set; }
        public CategoryRequestDTO CategoryRequestDTO { get; set; }
        public CategoryResponseDTO CategoryResponseDTO { get; set; }
        public List<Category> CategoriesWithProductsList { get; set; }

        public InitialCategoryDataSetUp()
        {
            this.CategoriesWithProductsList =GetCategoriesWithProductsList();
            this.CategoryRequestDTO=GetCategoryRequestDTO();
            this.CategoryResponseDTO=GetCategoryResponseDTO();
            this.CategoryList=GetCategoryList();
        }
        private List<Category> GetCategoriesWithProductsList()
        {
            return new List<Category>()
            {
              new Category
              {
                Id = 1,
                Name = "TV",
                Products = new List<Product>()
                {
                  new Product
                  {
                    Id = 1,
                    Name = "Nike sneaker",
                    Price=15000,
                    Rating=1,
                    CategoryId=5,
                  }
                }
              },
              new Category
              {
                Id = 2,
                Name = "Mobile",
                Products = new List<Product>()
                  {
                     new Product
                     {
                       Id = 2,
                       Name = "Adidas sports",
                       Price=2000,
                       Rating=2,
                       CategoryId=2,
                     }
                  }
              }
            };
        }
        private List<Category> GetCategoryList()
        {
           return new List<Category>
            {
               new Category
               {
                   Id=1,
                   Name="Mobile"
               },
               new Category
               {
                   Id=3,
                   Name="Food"
               },
               new Category
               {
                   Id=4,
                   Name="Sport"
               },
               new Category
               {
                   Id=5,
                   Name="Shoes"
               }
            };
        }
        private CategoryRequestDTO GetCategoryRequestDTO()
        {
            return new CategoryRequestDTO
            {
                Name = "Fashion",
            };

        }
        private CategoryResponseDTO GetCategoryResponseDTO()
        {
            return new CategoryResponseDTO
            {
                Name = "Fashion",
            };
        }


    }
   
}


