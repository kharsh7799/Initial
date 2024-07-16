using AutoMapper;
using EcommerceApp.Constants;
using EcommerceApp.CustomAttributes;
using EcommerceApp.Entities.APIResponses;
using EcommerceApp.Entities.DomainModels;
using EcommerceApp.Entities.DTOs.Product;
using EcommerceApp.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using static EcommerceApp.Entities.APIResponses.APICategoriesResponses;

namespace EcommerceApp.Controllers
{
    /// <summary> Products Controller </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]

    public class ProductsController : ControllerBase
    {
        private readonly IProductService productService;
        private readonly IMapper mapper;
        private readonly ILogger<ProductsController> logger;

        /// <summary> Products Endpoints </summary>
        public ProductsController(IProductService productService, IMapper mapper,ILogger<ProductsController> logger) 
        {
            this.productService = productService;
            this.mapper = mapper;
            this.logger = logger;
        }

        /// <summary> Gets Product List </summary>
        /// <returns>In case of success response,Products List is rendered</returns>
        /// <response code="200">In case of success response,Products List is rendered</response>
        [HttpGet]
        public async Task<IActionResult> GetAllProducts([FromQuery] string? filterByNameValue)
        {
            var productsData = await productService.GetAllProducts(filterByNameValue);
            if (productsData.Count > 0)
            {
              var productsResponseData = mapper.Map<List<ProductResponseDTO>>(productsData);
              return Ok(productsResponseData);
            }
            return NotFound(new AllProductsResponse { ProductCount = 0, Message = ResponseConstants.NoRecord });
        }

        /// <summary>Gets Product by product Id</summary>
        /// <param name="id"></param>
        /// <returns>In case of success response,Product with respective product Id is rendered</returns>
        /// <response code="200">In case of success response,Product with respective product Id is rendered</response>
        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetProductById([FromRoute] int id) // check id at start
        {
            if(id <= 0)
            {
                return BadRequest(new ProductIdInvalidResponse { ProductId = id, ErrorMessage = ResponseConstants.NullOrInvalidProductId });
            }
            var productsData = await productService.GetProductById(id);
            if (productsData != null)
            {
                var productResponseData = mapper.Map<ProductResponseDTO>(productsData);
                return Ok(productResponseData);
            }
            return NotFound(new ProductNotFoundResponse { ProductId = id, Message = ResponseConstants.NoRecordWithProductId });
        }

        /// <summary>Get Product list by category Id</summary>
        /// <param name="categoryId"></param>
        /// <returns>In case of success response,Product List with respective Category Id is rendered</returns>
        /// <response code="200">In case of success response,Product List with respective Category Id is rendered</response>
        [HttpGet]
        [Route("category/{categoryId:int}")]
        public async Task<IActionResult> GetProductByCategoryId([FromRoute] int categoryId)
        {
            if (categoryId <= 0)
            {
                return BadRequest(new CategoryErrorResponse { CategoryId = categoryId, ErrorMessage = ResponseConstants.NullOrInvalidCategoryId });
            }
            var productsByCategoryId = await productService.GetProductByCategoryId(categoryId);
            if (productsByCategoryId.Count > 0)
            {
                var productsByCategoryIdResData = mapper.Map<List<ProductResponseDTO>>(productsByCategoryId);
                return Ok(productsByCategoryIdResData);
            }
            return NotFound(new ProductNotFoundByCategory { CategoryId = categoryId, Message = ResponseConstants.NoRecordWithCategoryId });
        }

        /// <summary>Creates a new product</summary>
        /// <remarks> Request Body: { "name": "Realme Narzo 50","Price": 18000,"Rating":4, CategoryId:1 } </remarks>
        /// <returns>Newly Created Product</returns>
        /// <response code="201">In case of success, New Product will be Created</response>
        [HttpPost]
        [ValidateRequestModel]
        public async Task<IActionResult> AddProduct([FromBody] ProductRequestDTO productReqDTO)
        {
            var isPresent = await productService.IsCategoryPresent(productReqDTO.CategoryId);
            if (!isPresent)
            {
                return NotFound(new ProductAddFailureResponse { CategoryId = productReqDTO.CategoryId, ErrorMessage = ResponseConstants.CategoryIdNotPresent });
            }
            var AddProductData = mapper.Map<Product>(productReqDTO);
            var productDomainData = await productService.AddProduct(AddProductData);
            if(productDomainData != null)
            {
                var productResData = mapper.Map<ProductResponseDTO>(productDomainData);
                return CreatedAtAction(nameof(GetProductById), new { id = productResData.Id }, productResData);
            }
            return BadRequest(new ProductAlreadyPresentResponse { ProductName = productReqDTO.Name, ErrorMessage = ResponseConstants.ProductAlreadyPresent });
        }

        /// <summary> Updates Product deatils by product Id </summary>
        /// <remarks> Request Body { "name": "Realme Narzo 50","Price": 18000, "Rating":4, CategoryId:1 } </remarks>
        /// <param name="id"></param>
        /// <returns>In case of success, product id of the updated product</returns>
        /// <response code="200">In case of succes, product deatils of the provided product id will be updated</response>
        [HttpPut]
        [Route("{id:int}")]
        [ValidateRequestModel]
        public async Task<IActionResult> UpdateProduct([FromRoute] int id, [FromBody] ProductRequestDTO updateProductReqDTO)
        {
            if(id <= 0)
            {
                return BadRequest(new ProductUpdateFailResponse { ProductId = id, ErrorMessage = ResponseConstants.NullOrInvalidProductId });
            }
            var updateProductData = mapper.Map<Product>(updateProductReqDTO);
            var updateProductId = await productService.UpdateProduct(id, updateProductData);
            if (updateProductId == -1)
            {
                return NotFound(new ProductNotFoundResponse { ProductId = id, Message = ResponseConstants.NoRecordWithProductId });
            }  
            else if (updateProductId == -2)
            {
                return NotFound(new ProductUpdateFailResponse { ProductId = id, CategoryId = updateProductReqDTO.CategoryId, ErrorMessage = ResponseConstants.CategoryIdNotPresent });
            }
            return Ok(new ProductSuccessResponse { ProductId = updateProductId, Message = ResponseConstants.ProductUpdateSuccess });
        }

        /// <summary> Deletes the Product by product Id </summary>
        /// <param name="id"></param>
        /// <returns>In case of success, Product Id of the deleted product</returns>
        /// <response code="200">In case of success, Product with provided product id will be deleted</response>
        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] int id)
        {
            if (id <= 0)
            {
                return BadRequest(new ProductDeleteFailResponse { ProductId = id, ErrorMessage = ResponseConstants.NullOrInvalidProductId });
            }
            var deleteProductId = await productService.DeleteProduct(id);

            if (deleteProductId == -1)
            {
                return NotFound(new ProductNotFoundResponse { ProductId = id, Message = ResponseConstants.NoRecordWithProductId });
            }
            return Ok(new ProductSuccessResponse { ProductId = deleteProductId, Message = ResponseConstants.ProductDeleteSuccess });
        }

    }
}
