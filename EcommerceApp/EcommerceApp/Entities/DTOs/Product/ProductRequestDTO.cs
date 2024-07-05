using System.ComponentModel.DataAnnotations;

namespace EcommerceApp.Entities.DTOs.Product
{
    /// <summary>
    /// Product Request DTO
    /// </summary>
    public class ProductRequestDTO
    {
        /// <summary>
        /// Product Name
        /// </summary>
        [Required(ErrorMessage = "Name filed is required")]
        public string? Name { get; set; }

        /// <summary>
        /// Product Price
        /// </summary>
        [Required(ErrorMessage = "Price filed is required")]
        [DataType(DataType.Currency)]
        [RegularExpression(@"^₹?\s?\d{1,3}(?:,?\d{3})*(?:\.\d{1,2})?$", ErrorMessage = "Entered price is not valid")]
        
        public decimal Price { get; set; }

        /// <summary>
        /// Product Rating
        /// </summary>
        [Required(ErrorMessage = "Rating field is required")]
        [Range(0,5, ErrorMessage ="Rating should be betwwen 0 to 5")]
        public double Rating { get; set; }

        /// <summary>
        /// Product Category Id
        /// </summary>
        [Required(ErrorMessage = "CategoryId field is required")]
        public int CategoryId { get; set; }
    }
}
