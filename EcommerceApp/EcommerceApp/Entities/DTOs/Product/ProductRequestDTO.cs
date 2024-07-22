﻿using System.ComponentModel;
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
        [Required(ErrorMessage = "Name field is required")]
        public string? Name { get; set; }

        /// <summary>
        /// Product Price
        /// </summary>
        [Range(1.00, double.MaxValue, ErrorMessage = "Entered price is not valid")]
        [Required(ErrorMessage = "Price field is required")]
        [DefaultValue(1.00)]
        public decimal Price { get; set; }

        /// <summary>
        /// Product Rating
        /// </summary>
        [Required(ErrorMessage = "Rating field is required")]
        [Range(1, 5, ErrorMessage = "Rating should be betwwen 0 to 5")]
        [DefaultValue(1)]
        public double Rating { get; set; }

        /// <summary>
        /// Product Category Id
        /// </summary>
        [Range(1,int.MaxValue,ErrorMessage ="Please Enter valid category Id")]
        [Required(ErrorMessage = "CategoryId field is required")]
        [DefaultValue(1)]
        public int CategoryId { get; set; }
    }
}
