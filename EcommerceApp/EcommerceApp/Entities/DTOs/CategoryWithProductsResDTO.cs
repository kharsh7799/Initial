using EcommerceApp.Entities.DTOs.Product;

namespace EcommerceApp.Entities.DTOs
{
    /// <summary>
    /// Category With Products Response DTO
    /// </summary>
    public class CategoryWithProductsResDTO
    {
        /// <summary>
        /// Category  Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Category Name
        /// </summary>
        public string? Name { get; set; }

        //Navigation Properties
        /// <summary>
        /// Products
        /// </summary>
        public ICollection<ProductResponseDTO>? Products { get; set; }
    }
}
