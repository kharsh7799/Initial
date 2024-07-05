using System.ComponentModel.DataAnnotations;

namespace EcommerceApp.Entities.DomainModels
{
    /// <summary>
    /// Product Data Model
    /// </summary>
    public class Product
    {
        /// <summary>
        /// Product Id
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Product Name
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Product Price
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Product Rating
        /// </summary>
        public double Rating { get; set; }

        /// <summary>
        /// Category Id
        /// </summary>
        public int CategoryId { get; set; }


        //Navgitaion Properties
        /// <summary>
        /// Category
        /// </summary>
        public Category? Category { get; set; }
    }
}
