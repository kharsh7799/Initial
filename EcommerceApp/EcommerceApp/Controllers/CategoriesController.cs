using AutoMapper;
using EcommerceApp.Constants;
using EcommerceApp.CustomAttributes;
using EcommerceApp.Entities.APIResponses;
using EcommerceApp.Entities.DomainModels;
using EcommerceApp.Entities.DTOs;
using EcommerceApp.Entities.DTOs.Category;
using EcommerceApp.Entities.DTOs.Product;
using EcommerceApp.Repositories.Contracts;
using EcommerceApp.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Construction;
using Microsoft.EntityFrameworkCore;
using System.Data;
using static EcommerceApp.Entities.APIResponses.APICategoriesResponses;

namespace EcommerceApp.Controllers
{
    /// <summary>categories Controller</summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService categoryService;
        private readonly IMapper mapper;
        private readonly ILogger<CategoriesController> logger;

        /// <summary>Categories Endpoints</summary>
        public CategoriesController(ICategoryService categoryService, IMapper mapper, ILogger<CategoriesController> logger)
        {
            this.categoryService = categoryService;
            this.mapper = mapper;
            this.logger = logger;
        }

        /// <summary> Gets a Categories List </summary>
        /// <returns>In case of success, Categories List will be rendered</returns>
        /// <response code="200">In case of success, Categories List will be rendered</response>
        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var categoriesData = await categoryService.GetAllCategories();
            if(categoriesData.Count > 0)
            {
                var categoriesResponseData = mapper.Map<List<CategoryResponseDTO>>(categoriesData);
                return Ok(categoriesResponseData);
            }
            return NotFound(new AllCategorysResponse { CategoryCount=0, Message =ResponseConstants.NoRecord });
        }

        /// <summary> Gets a category by category Id</summary>
        /// <param name="id"></param>
        /// <returns>Ok response with Category in case of success</returns>
        /// <response code="200">In case of success, Category by Category Id will be rendered</response>
        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetCategoryById([FromRoute] int id)
        {
            if (id <= 0)
            {
                return BadRequest(new CategoryErrorResponse { CategoryId = id, ErrorMessage = ResponseConstants.NullOrInvalidCategoryId });
            }
            var categoryData = await categoryService.GetCategoryById(id);
            if(categoryData != null)
            {
                var categoryResponseData = mapper.Map<CategoryResponseDTO>(categoryData);
                return Ok(categoryResponseData);
            }
            return NotFound(new CategoryNotFoundResponse { CategoryId = id, Message = ResponseConstants.NoRecordWithCategoryId });
        }

        /// <summary> Creates a new category</summary>
        /// <remarks> Request Body { "name": "Food" } </remarks>
        /// <returns>A newly created category</returns>
        /// <response code="201">In case of success, New Category will be Created</response>
        [HttpPost]
        [ValidateRequestModel]
        public async Task<IActionResult> AddCategory([FromBody] CategoryRequestDTO categoryReqDTO)
        {
            var addCategoryData = mapper.Map<Category>(categoryReqDTO);
            var categoryDomainData = await categoryService.AddCategory(addCategoryData);
            if(categoryDomainData != null)
            {
                var categoryResData = mapper.Map<CategoryResponseDTO>(categoryDomainData);
                return CreatedAtAction(nameof(GetCategoryById), new { id = categoryResData.Id }, categoryResData);
            }
            return BadRequest(new CategoryAddFailureResponse { CategoryName = categoryReqDTO.Name, ErrorMessage = ResponseConstants.CategoryAlreadyPresent });
        }

        /// <summary>Updates a category deatils by category Id</summary>
        ///  <remarks> Request Body { "name": "Food" } </remarks>
        /// <param name="id"></param>
        /// <returns>In case of success, category id of the updated product</returns>
        /// <response code="200">In case of succes, category deatils of the provided category id will be updated</response>
        [HttpPut]
        [Route("{id:int}")]
        [ValidateRequestModel]
        public async Task<IActionResult> UpdateCategory([FromRoute] int id, [FromBody] CategoryRequestDTO updatecategoryReqDTO)
        {
            if (id <= 0)
            {
                return BadRequest(new CategoryErrorResponse { CategoryId = id, ErrorMessage = ResponseConstants.NullOrInvalidCategoryId });
            }
            var updateCategoryData = mapper.Map<Category>(updatecategoryReqDTO);
            var updateCategoryId = await categoryService.UpdateCategory(id, updateCategoryData);
            if (updateCategoryId == -1)
            {
                return NotFound(new CategoryNotFoundResponse { CategoryId = id, Message = ResponseConstants.NoRecordWithCategoryId });
            }
            return Ok(new CategorySuccessResponse { CategoryId = updateCategoryId, Message = ResponseConstants.CategoryUpdateSuccess });
        }

        /// <summary> Deletes the Category by category id </summary>
        /// <param name="id"></param>
        /// <returns>In case of success, category id of the deleted product</returns>
        /// <response code="200">In case of success, Category with provided category id will be deleted</response>
        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteCategory([FromRoute] int id)
        {
            if (id <= 0)
            {
                return BadRequest(new CategoryErrorResponse { CategoryId = id, ErrorMessage = ResponseConstants.NullOrInvalidCategoryId });
            }
            var deleteCategoryId = await categoryService.DeleteCategory(id);
            if (deleteCategoryId == -1)
            {
                return NotFound(new CategoryNotFoundResponse { CategoryId = id, Message = ResponseConstants.NoRecordWithCategoryId });
            }
            return Ok(new CategorySuccessResponse { CategoryId = deleteCategoryId, Message = ResponseConstants.CategoryDeleteSuccess });
        }

        /// <summary> Gets categories list with product list</summary>
        /// <returns>Gets the list of categories with respective products</returns>
        /// <response code="200">In case of success, Categories list with their respective</response>
        [HttpGet]
        [Route("products")]
        public async Task<IActionResult> GetAllCategoriesWithProducts()
        {
            var categoryWithProdDetailsData = await categoryService.GetAllCategoriesWithProducts();
            if(categoryWithProdDetailsData.Count>0)
            {
                var categoryResponseData = mapper.Map<List<CategoryWithProductsResDTO>>(categoryWithProdDetailsData);
                return Ok(categoryResponseData);
            }
            return NotFound(new AllCategorysResponse { CategoryCount = 0, Message = ResponseConstants.NoRecord });
        }
        /// <summary> Gets category by Id with product list</summary>
        /// <returns>Gets the list of categories with respective products</returns>
        /// <response code="200">In case of success, Categories list with their respective</response>
        [HttpGet]
        [Route("{categoryId:int}/products")]
        public async Task<IActionResult> GetCategoryWithProducts([FromRoute] int categoryId)
        {
            if (categoryId <= 0)
            {
                return BadRequest(new CategoryErrorResponse { CategoryId = (int)categoryId, ErrorMessage = ResponseConstants.NullOrInvalidCategoryId });
            }
            var categoryWithProdDetailsData = await categoryService.GetCategoryWithProducts(categoryId);
            if (categoryWithProdDetailsData.Count > 0)
            {
                var categoryResponseData = mapper.Map<List<CategoryWithProductsResDTO>>(categoryWithProdDetailsData);
                return Ok(categoryResponseData);
            }
            return NotFound(new CategoryNotFoundResponse { CategoryId = (int)categoryId, Message = ResponseConstants.NoRecordWithCategoryId });
            
        }

    }
}
