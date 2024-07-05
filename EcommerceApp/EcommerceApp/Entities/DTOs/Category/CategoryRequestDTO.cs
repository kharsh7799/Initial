using System.ComponentModel.DataAnnotations;

namespace EcommerceApp.Entities.DTOs.Category
{
    /// <summary>
    /// Category Request DTO
    /// </summary>
    public class CategoryRequestDTO
    {
        /// <summary>
        /// Category Name
        /// </summary>
        [Required]
        public string? Name { get; set; }
    }
}
