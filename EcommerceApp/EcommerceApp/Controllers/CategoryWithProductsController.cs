using AutoMapper;
using EcommerceApp.Constants;
using EcommerceApp.CustomAttributes;
using EcommerceApp.Entities.DTOs;
using EcommerceApp.Entities.DTOs.Category;
using EcommerceApp.Repositories.Contracts;
using EcommerceApp.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static EcommerceApp.Entities.APIResponses.APICategoriesResponses;

namespace EcommerceApp.Controllers
{
    /// <summary>
    /// Category With Products Controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryWithProductsController : ControllerBase
    {
        private readonly ICategoryWithProductsService categoryWithProdService;
        private readonly IMapper mapper;
        private readonly ILogger<CategoryWithProductsController> logger;

        /// <summary>
        ///  Category With Products Endpoints
        /// </summary>
        public CategoryWithProductsController(ICategoryWithProductsService categoryWithProdService, IMapper mapper, ILogger<CategoryWithProductsController> logger)
        {
            this.categoryWithProdService = categoryWithProdService;
            this.mapper = mapper;
            this.logger = logger;
        }

        /// <summary>
        /// Get category with products
        /// </summary>
        /// <remarks>
        ///   URL
        ///   GET: api/CategoryWithProducts  OR
        ///   GET: api/CategoryWithProducts?id=1
        /// </remarks>
        /// <returns>Ok response of category with products in case of success</returns>
        /// <response code="204">If data is not available</response>
        /// <response code="404">If category with provided id is not found</response>
        /// <response code="500">If error occurres</response>
        [HttpGet]
        public async Task<IActionResult> GetCategoryWithProductDetails([FromQuery] int? categoryid)
        {
            try
            {
                var categoryWithProdDetailsData = await categoryWithProdService.GetCategoryWithProductDetails(categoryid);
                var categoryResponseData = mapper.Map<List<CategoryWithProductsResDTO>>(categoryWithProdDetailsData);
                if (categoryResponseData.Any())
                {
                    return Ok(categoryResponseData);
                }
                else
                {
                    if (categoryid != null)
                    {
                        return NotFound(new CategoryNotFoundResponse { CategoryId = (int)categoryid, Message =ResponseConstants.NoRecordWithCategoryId });
                    }
                  return NoContent();
                }
            }
            catch (Exception ex)
            {
                if (ex is ArgumentNullException)
                {
                    logger.LogError(ex.Message, ex);
                    return BadRequest(new CategoryErrorResponse { CategoryId = (int)categoryid, ErrorMessage = ResponseConstants.NullOrInvalidCategoryId });
                }
                logger.LogError(ex.Message, ex);
                return Problem(ResponseConstants.ServerError,statusCode:500);
            }
        }
    }
}
