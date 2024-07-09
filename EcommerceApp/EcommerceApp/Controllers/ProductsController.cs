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
    /// <summary>
    /// Products Controller
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]

    public class ProductsController : ControllerBase
    {
        private readonly IProductService productService;
        private readonly IMapper mapper;
        private readonly ILogger<ProductsController> logger;

        /// <summary>
        /// Products Endpoints
        /// </summary>
        public ProductsController(IProductService productService, IMapper mapper,ILogger<ProductsController> logger) 
        {
            this.productService = productService;
            this.mapper = mapper;
            this.logger = logger;
        }

        // GET: api/<ProductsController>
        /// <summary>
        /// Get Products List
        /// </summary>
        /// <remarks>
        ///   URL
        ///   GET: api/Products OR 
        ///   GET: api/Products?filterByNameValue="Real Me Narzo 50"
        /// </remarks>
        /// <returns>Ok response with List of Products in case of success</returns>
        /// <response code="500">If error occurres</response>
        [HttpGet]
        public async Task<IActionResult> GetAllProducts([FromQuery] string? filterByNameValue)
        {
            try
            {
                var productsData = await productService.GetAllProducts(filterByNameValue);
                var productsResponseData = mapper.Map<List<ProductResponseDTO>>(productsData);
                if(productsResponseData.Count>0)
                {
                    return Ok(productsResponseData);
                }
                else
                {
                    return Ok(new AllProductsResponse { ProductCount = 0, Message = ResponseConstants.NoRecord });
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message,ex);
                return Problem (ResponseConstants.ServerError , statusCode: 500);
            }
        }

        // GET: api/<ProductsController>/1
        /// <summary>
        /// Get Product by product Id
        /// </summary>
        /// <remarks>
        ///   URL
        ///   GET: api/Products/1
        /// </remarks>
        /// <param name="id"></param>
        /// <returns>Ok response with Product in case of success</returns>
        /// <response code="404">If product with provided id is not found</response>
        /// <response code="500">If error occurres</response>
        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetProductById([FromRoute] int id)
        {
            try
            {
                var productsData = await productService.GetProductById(id);
                var productResponseData = mapper.Map<ProductResponseDTO>(productsData);
                if (productResponseData != null)
                {
                    return Ok(productResponseData);
                }
                else
                {
                    return NotFound(new ProductNotFoundResponse { ProductId = id, Message = ResponseConstants.NoRecordWithProductId });
                }
            }
            catch (Exception ex)
            {
                if (ex is ArgumentNullException)
                {
                    logger.LogError(ex.Message, ex);
                    return BadRequest(new ProductDeleteFailResponse { ProductId = id, ErrorMessage = ResponseConstants.NullOrInvalidProductId});
                }
                logger.LogError(ex.Message,ex);
                return Problem(ResponseConstants.ServerError, statusCode: 500);

            }
        }

        // GET: api/<ProductsController>/category/1
        /// <summary>
        /// Get Products list by category Id
        /// </summary>
        /// <remarks>
        ///   URL
        ///   GET: api/Products/category/1
        /// </remarks>
        /// <param name="categoryId"></param>
        /// <returns>Ok response with Products by category id in case of success</returns>
        /// <response code="404">If products with provided category id are not found</response>
        /// <response code="500">If error occurres</response>
        [HttpGet]
        [Route("category/{categoryId:int}")]
        public async Task<IActionResult> GetProductByCategoryId([FromRoute] int categoryId)
        {
            try
            {
                var productsByCategoryId = await productService.GetProductByCategoryId(categoryId);
                var productsByCategoryIdResData = mapper.Map<List<ProductResponseDTO>>(productsByCategoryId);
                if (productsByCategoryIdResData.Count>0)
                {
                    return Ok(productsByCategoryIdResData);
                }
                else
                {
                    return NotFound(new ProductNotFoundByCategory { CategoryId = categoryId, Message = ResponseConstants.NoRecordWithCategoryId });
                }
            }
            catch (Exception ex)
            {
                if (ex is ArgumentNullException)
                {
                    logger.LogError(ex.Message, ex);
                    return BadRequest(new CategoryErrorResponse { CategoryId = categoryId, ErrorMessage = ResponseConstants.NullOrInvalidCategoryId });
                }
                logger.LogError(ex.Message, ex);
                return Problem(ResponseConstants.ServerError,statusCode: 500);
            }
        }

        // POST: api/<ProductsController>
        /// <summary>
        /// Create new product
        /// </summary>
        /// <remarks>
        /// URL: POST /Products
        /// Request Body:
        ///     {
        ///        "name": "Realme Narzo 50",
        ///        "Price": 18000,
        ///        "Rating":4,
        ///        CategoryId:1
        ///     }
        /// </remarks>
        /// <returns>A newly created Product</returns>
        /// <response code="201">Returns the newly created product item</response>
        /// <response code="400">If invalid request object is provided or product is already present</response>
        /// <response code="500">If error occurres</response>
        /// 
        [HttpPost]
        [ValidateRequestModel]
        public async Task<IActionResult> AddProduct([FromBody] ProductRequestDTO productReqDTO)
        {
            try
            {
                var AddProductData = mapper.Map<Product>(productReqDTO);
                var productDomainData = await productService.AddProduct(AddProductData);
                var productResData = mapper.Map<ProductResponseDTO>(productDomainData);

                if (productResData != null)
                {
                    return CreatedAtAction(nameof(GetProductById), new { id = productResData.Id }, productResData);
                }
                else
                {
                    return BadRequest( new ProductAlreadyPresentResponse { ProductName = productReqDTO.Name, ErrorMessage = ResponseConstants.ProductAlreadyPresent });
                }
            }
            catch (Exception ex)
            {
                if(ex is DBConcurrencyException or DbUpdateException)
                {
                    logger.LogError(ex.Message, ex);
                    return BadRequest( new ProductAddFailureResponse { CategoryId = productReqDTO.CategoryId, ErrorMessage = ResponseConstants.ProductNotAdded });
                }
                logger.LogError(ex.Message, ex);
                return Problem(ResponseConstants.ServerError,statusCode:500);
            }
        }

        // PUT: api/<ProductsController>/1
        /// <summary>
        /// Update Product deatils by product Id
        /// </summary>
        ///  <remarks>
        ///   URL
        ///   PUT: api/Products/1
        ///   Request Body
        ///     {
        ///        "name": "Realme Narzo 50",
        ///        "Price": 18000,
        ///        "Rating":4,
        ///        CategoryId:1
        ///     }
        /// </remarks>
        /// <param name="id"></param>
        /// <returns>Ok response with product id in case of success</returns>
        /// <response code="400">If request Object provided</response>
        /// <response code="404">If products with provided product id is not found</response>
        /// <response code="500">If some error occurres</response>
        [HttpPut]
        [Route("{id:int}")]
        [ValidateRequestModel]
        public async Task<IActionResult> UpdateProduct([FromRoute] int id, [FromBody] ProductRequestDTO updateProductReqDTO)
        {
            try
            {
                var updateProductData = mapper.Map<Product>(updateProductReqDTO);
                var updateProductId = await productService.UpdateProduct(id, updateProductData);

                if (updateProductId == -1)
                {
                    return NotFound(new ProductNotFoundResponse { ProductId = id, Message = ResponseConstants.NoRecordWithProductId });
                }
                return Ok(new ProductSuccessResponse { ProductId = updateProductId, Message = ResponseConstants.ProductUpdateSuccess });

            }
            catch (Exception ex)
            {
                if(ex is DbUpdateException or DBConcurrencyException)
                {
                    logger.LogError(ex.Message, ex);
                    return BadRequest(new ProductUpdateFailResponse { ProductId = id, CategoryId = updateProductReqDTO.CategoryId, ErrorMessage = ResponseConstants.ProductUpdateFailed });
                }
                if(ex is ArgumentNullException)
                {
                    logger.LogError(ex.Message, ex);
                    return BadRequest(new ProductUpdateFailResponse { ProductId = id, ErrorMessage = ResponseConstants.NullOrInvalidProductId });
                }
                logger.LogError(ex.Message, ex);
                return Problem(ResponseConstants.ServerError,statusCode:500);
            }
        }

        // DELETE: api/<ProductsController>/1
        /// <summary>
        /// Delete Product deatils by product Id
        /// </summary>
        /// <remarks>
        ///   URL
        ///   DELETE: api/Products/1
        /// </remarks>
        /// <param name="id"></param>
        /// <returns>Ok response with Product id in case of success</returns>
        /// <response code="404">If products with provided product id is not found</response>
        /// <response code="500">If some error occurres</response>
        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] int id)
        {
            try
            {
                var deleteProductId = await productService.DeleteProduct(id);
                if (deleteProductId == -1)
                {
                    return NotFound(new ProductNotFoundResponse { ProductId = id, Message = ResponseConstants.NoRecordWithProductId });
                }
                return Ok(new ProductSuccessResponse { ProductId = deleteProductId, Message = ResponseConstants.ProductDeleteSuccess });
            }
            catch (Exception ex)
            {
                if (ex is ArgumentNullException)
                { 
                  logger.LogError(ex.Message, ex);
                  return BadRequest(new ProductDeleteFailResponse { ProductId =id, ErrorMessage = ResponseConstants.NullOrInvalidProductId });
                }
                logger.LogError(ex.Message, ex);
                return Problem(ResponseConstants.ServerError,statusCode:500);
            }
        }

    }
}
