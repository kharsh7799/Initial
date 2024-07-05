using AutoMapper;
using EcommerceApp.Constants;
using EcommerceApp.CustomAttributes;
using EcommerceApp.Entities.DomainModels;
using EcommerceApp.Entities.DTOs.Category;
using EcommerceApp.Entities.DTOs.Product;
using EcommerceApp.Repositories.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Construction;
using Microsoft.EntityFrameworkCore;
using System.Data;
using static EcommerceApp.Entities.APIResponses.APICategoriesResponses;

namespace EcommerceApp.Controllers
{
    /// <summary>
    /// categories Controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository categoryRepo;
        private readonly IMapper mapper;
        private readonly ILogger<CategoriesController> logger;
        /// <summary>
        /// Categories Endpoints
        /// </summary>
        public CategoriesController(ICategoryRepository categoryRepo, IMapper mapper, ILogger<CategoriesController> logger)
        {
            this.categoryRepo = categoryRepo;
            this.mapper = mapper;
            this.logger = logger;
        }

        // GET: api/<CategoriesController>
        /// <summary>
        /// Get Categories List
        /// </summary>
        /// <remarks>
        ///   URL
        ///   GET: api/Categories
        /// </remarks>
        /// <returns>Ok response with List of categories in case of success</returns>
        /// <response code="500">If error occurres</response>
        /// 
        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                var categoriesData = await categoryRepo.GetAllCategories();
                var categoriesResponseData = mapper.Map<List<CategoryResponseDTO>>(categoriesData);
                if (categoriesResponseData.Count > 0)
                {
                    return Ok(categoriesResponseData);
                }
                else
                {
                    return Ok(new AllCategorysResponse { CategoryCount=0, Message =ResponseConstants.NoRecord });
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex);
                return Problem(ResponseConstants.ServerError,statusCode:500);
            }
        }

        // GET: api/<CategoriesController>/1
        /// <summary>
        /// Get category by product Id
        /// </summary>
        /// <remarks>
        ///   URL
        ///   GET: api/Categories/1
        /// </remarks>
        /// <param name="id"></param>
        /// <returns>Ok response with Category in case of success</returns>
        /// <response code="404">If category with provided id is not found</response>
        /// <response code="500">If error occurres</response>
        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetCategoryById([FromRoute] int id)
        {
            try
            {
                var categoryData = await categoryRepo.GetCategoryById(id);
                var categoryResponseData = mapper.Map<CategoryResponseDTO>(categoryData);
                if (categoryResponseData != null)
                {
                    return Ok(categoryResponseData);
                }
                else
                {
                    return NotFound(new CategoryNotFoundResponse { CategoryId = id, Message = ResponseConstants.NoRecordWithCategoryId });
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex);
                return Problem(ResponseConstants.ServerError,statusCode:500);
            }
        }

        // POST: api/<CategoriesController>
        /// <summary>
        /// Create new category
        /// </summary>
        /// <remarks>
        /// URL:POST api/categories
        /// Request Body:
        ///     {
        ///        "name": "Food",
        ///     }
        /// </remarks>
        /// <returns>A newly created category</returns>
        /// <response code="201">Returns the newly created category item</response>
        /// <response code="400">If invalid request object is provided or Category is already present</response>
        /// <response code="500">If error occurres</response>
        [HttpPost]
        [ValidateRequestModel]
        public async Task<IActionResult> AddCategory([FromBody] CategoryRequestDTO categoryReqDTO)
        {
            try
            {
                var addCategoryData = mapper.Map<Category>(categoryReqDTO);
                var categoryDomainData = await categoryRepo.AddCategory(addCategoryData);

                var categoryResData = mapper.Map<CategoryResponseDTO>(categoryDomainData);
                if (categoryResData != null)
                {
                    return CreatedAtAction(nameof(GetCategoryById), new { id = categoryResData.Id }, categoryResData);
                }
                else
                {
                    return BadRequest(new CategoryAddFailureResponse { CategoryName = categoryReqDTO.Name, ErrorMessage = ResponseConstants.CategoryAlreadyPresent });
                }
            }
            catch (Exception ex)
            {
                if (ex is DBConcurrencyException or DbUpdateException)
                {
                    logger.LogError(ex.Message, ex);
                    return BadRequest(new CategoryAddFailureResponse { CategoryName = categoryReqDTO.Name, ErrorMessage = ResponseConstants.CategoryNotAdded });
                }
                logger.LogError(ex.Message, ex);
                return Problem(ResponseConstants.ServerError,statusCode:500);
            }
        }

        // PUT: api/<CategoriesController>/1
        /// <summary>
        /// Update category deatils by category Id
        /// </summary>
        ///  <remarks>
        ///   URL
        ///   PUT: api/categories/1
        ///   Request Body
        ///     {
        ///        "name": "Food",
        ///     }
        /// </remarks>
        /// <param name="id"></param>
        /// <returns>Ok response with category id in case of success</returns>
        /// <response code="400">If request Object provided</response>
        /// <response code="404">If products with provided product id is not found</response>
        /// <response code="500">If some error occurres</response>
        [HttpPut]
        [Route("{id:int}")]
        [ValidateRequestModel]
        public async Task<IActionResult> UpdateCategory([FromRoute] int id, [FromBody] CategoryRequestDTO updatecategoryReqDTO)
        {
            try
            {
                var updateCategoryData = mapper.Map<Category>(updatecategoryReqDTO);
                var updateCategoryId = await categoryRepo.UpdateCategory(id, updateCategoryData);

                if (updateCategoryId == -1)
                {
                    return NotFound(new CategoryNotFoundResponse { CategoryId = id, Message = ResponseConstants.NoRecordWithCategoryId });
                }
                return Ok(new CategorySuccessResponse { CategoryId = updateCategoryId, Message = ResponseConstants.CategoryUpdateSuccess });

            }
            catch (Exception ex)
            {
                if (ex is DbUpdateException or DBConcurrencyException)
                {
                    logger.LogError(ex.Message, ex);
                    return BadRequest(new CategoryUpdateFailResponse { CategoryId = id, ErrorMessage = ResponseConstants.CategoryUpdateFailed});
                }
                logger.LogError(ex.Message, ex);
                return Problem(ResponseConstants.ServerError,statusCode:500);
            }
        }

        // DELETE: api/<CategoriesController>/1
        /// <summary>
        /// Delete category deatils by category Id
        /// </summary>
        /// <remarks>
        ///   URL
        ///   DELETE: api/categories/1
        /// </remarks>
        /// <param name="id"></param>
        /// <returns>Ok response with category id in case of success</returns>
        /// <response code="404">If products with provided category id is not found</response>
        /// <response code="500">If some error occurres</response>
        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteCategory([FromRoute] int id)
        {
            try
            {
                var deleteCategoryId = await categoryRepo.DeleteCategory(id);
                if (deleteCategoryId == -1)
                {
                    return NotFound(new CategoryNotFoundResponse { CategoryId = id, Message = ResponseConstants.NoRecordWithCategoryId });
                }
                return Ok(new CategorySuccessResponse { CategoryId = deleteCategoryId, Message = ResponseConstants.CategoryDeleteSuccess });
            }
            catch (Exception ex)
            {
                if (ex.InnerException is DBConcurrencyException)
                {
                    logger.LogError(ex.Message, ex);
                    return BadRequest(new CategoryDeleteFailResponse { CategoryId = id, ErrorMessage = ResponseConstants.CategoryDeleteFailed });
                }
                logger.LogError(ex.Message, ex);
                return Problem(ResponseConstants.ServerError, statusCode: 500);
            }
        }
    }
}
