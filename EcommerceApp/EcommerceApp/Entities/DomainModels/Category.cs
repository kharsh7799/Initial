using System.ComponentModel.DataAnnotations;

namespace EcommerceApp.Entities.DomainModels
{
    /// <summary>
    /// Category Data Model
    /// </summary>
    public class Category
    {
        /// <summary>
        /// Category Id
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Category Name
        /// </summary>
        public string? Name { get; set; }

        //Navigation Properties
        /// <summary>
        /// Products
        /// </summary>
        public ICollection<Product>? Products { get; set; }
    }
}
