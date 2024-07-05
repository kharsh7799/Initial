namespace EcommerceApp.Entities.DTOs.Product
{
    /// <summary>
    /// Product Response DTO
    /// </summary>
    public class ProductResponseDTO
    {
        /// <summary>
        /// Product Id
        /// </summary>
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
        /// Product CategoryId
        /// </summary>
        public int CategoryId { get; set; }
    }
}
