using System.ComponentModel;
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
        [Range(1.00, double.MaxValue, ErrorMessage = "Entered price is not valid")]
        [DefaultValue(1.00)]
        public decimal Price { get; set; }

        /// <summary>
        /// Product Rating
        /// </summary>
        [Required(ErrorMessage = "Rating field is required")]
        [Range(0,5, ErrorMessage ="Rating should be betwwen 0 to 5")]
        [DefaultValue(1)]
        public double Rating { get; set; }

        /// <summary>
        /// Product Category Id
        /// </summary>
        [Required(ErrorMessage = "CategoryId field is required")]
        [Range(1,int.MaxValue,ErrorMessage ="Please Enter valid category Id")]
        [DefaultValue(1)]
        public int CategoryId { get; set; }
    }
}
