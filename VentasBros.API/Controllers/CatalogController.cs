using Application.DTOs.Common;
using Microsoft.AspNetCore.Mvc;
using VentasBros.Application.DTOs.Product;
using VentasBros.Application.Services;

namespace VentasBros.API.Controllers
{
    [ApiController]
    [Route("catalog")]
    public class CatalogController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public CatalogController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        /// <summary>
        /// Obtiene el listado público de productos activos con filtros (POST)
        /// </summary>
        [HttpPost("searchProducts")]
        public async Task<IActionResult> GetProducts([FromBody] CatalogFilterDto filter)
        {
            try
            {
                var result = await _productService.GetCatalogPagedAsync(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiException(ex));
            }
        }

        /// <summary>
        /// Obtiene el listado público de productos activos con parámetros query (GET)
        /// </summary>
        [HttpGet("getAllProducts")]
        public async Task<IActionResult> GetProductsQuery(
            [FromQuery] string? search = null,
            [FromQuery] int? categoryId = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] string sort = "createdAt",
            [FromQuery] string dir = "desc",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var filter = new CatalogFilterDto
                {
                    Search = search,
                    CategoryId = categoryId,
                    MinPrice = minPrice,
                    MaxPrice = maxPrice,
                    Sort = sort,
                    Direction = dir,
                    Page = page,
                    PageSize = pageSize
                };

                var result = await _productService.GetCatalogPagedAsync(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiException(ex));
            }
        }

        /// <summary>
        /// Obtiene el detalle de un producto activo específico
        /// </summary>
        [HttpGet("getProductById/{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            try
            {
                var product = await _productService.GetByIdAsync(id);

                if (product == null || !product.IsActive)
                    return NotFound();

                return Ok(product);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiException(ex));
            }
        }

        /// <summary>
        /// Obtiene las categorías activas en formato árbol o plano
        /// </summary>
        [HttpGet("getAllCategories")]
        public async Task<IActionResult> GetCategories([FromQuery] bool flat = false)
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
    }
}

