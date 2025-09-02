using Application.DTOs.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VentasBros.Application.DTOs.Category;
using VentasBros.Application.Services;

namespace VentasBros.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("getAllCategories")]
        public async Task<IActionResult> GetCategories([FromQuery] bool flat = false)
        {
            try
            {
                var categories = await _categoryService.GetAllAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiException(ex));
            }
        }

        [HttpGet("getActiveCategoriesTree")]
        public async Task<IActionResult> GetActiveCategories([FromQuery] bool flat = false)
        {
            try
            {
                var categories = await _categoryService.GetActiveAsync(flat);
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiException(ex));
            }
        }

        [HttpGet("getCategoryById/{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            try
            {
                var category = await _categoryService.GetByIdAsync(id);
                if (category == null)
                    return NotFound();

                return Ok(category);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiException(ex));
            }
        }

        [HttpGet("getCategoriesByParent/{parentId?}")]
        public async Task<IActionResult> GetCategoriesByParent(int? parentId = null)
        {
            try
            {
                var categories = await _categoryService.GetByParentIdAsync(parentId);
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiException(ex));
            }
        }

        [HttpPost("createCategory")]
        [Authorize(Roles = "Admin,Editor")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto dto)
        {
            try
            {
                var category = await _categoryService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiException(ex));
            }
        }

        [HttpPut("updateCategoryById/{id}")]
        [Authorize(Roles = "Admin,Editor")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryDto dto)
        {
            try
            {
                var category = await _categoryService.UpdateAsync(id, dto);
                return Ok(category);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiException(ex));
            }
        }

        [HttpDelete("deleteCategoryById/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                await _categoryService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiException(ex));
            }
        }

        [HttpGet("checkCategoryNameExists/{name}")]
        public async Task<IActionResult> CheckCategoryName(string name, [FromQuery] int? excludeId = null)
        {
            try
            {
                var exists = await _categoryService.NameExistsAsync(name, excludeId);
                return Ok(new { exists });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiException(ex));
            }
        }
    }
}
