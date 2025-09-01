using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using VentasBros.Application.Services;
using VentasBros.Application.DTOs.Product;

namespace VentasBros.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] int? categoryId = null,
            [FromQuery] bool onlyActive = false)
        {
            var result = await _productService.GetPagedAsync(page, pageSize, search, categoryId, onlyActive);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Editor")]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto dto)
        {
            var product = await _productService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Editor")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDto dto)
        {
            var product = await _productService.UpdateAsync(id, dto);
            return Ok(product);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _productService.DeleteAsync(id);
            return NoContent();
        }
    }
}
