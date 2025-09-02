using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using VentasBros.Application.Services;
using VentasBros.Application.DTOs.Product;
using Application.DTOs.Common;

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

        [HttpPost("searchProducts")]
        public async Task<IActionResult> GetProducts([FromBody] ProductFilterDto filter)
        {
            try
            {
                var result = await _productService.GetPagedAsync(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiException(ex));
            }
        }

        [HttpGet("getProductById/{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            try
            {
                var product = await _productService.GetByIdAsync(id);
                if (product == null)
                    return NotFound();

                return Ok(product);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiException(ex));
            }
        }

        [HttpPost("createProduct")]
        [Authorize(Roles = "Admin,Editor")]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto dto)
        {
            try
            {
                var product = await _productService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiException(ex));
            }
        }

        [HttpPut("updateProductById/{id}")]
        [Authorize(Roles = "Admin,Editor")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDto dto)
        {
            try
            {
                var product = await _productService.UpdateAsync(id, dto);
                return Ok(product);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiException(ex));
            }
        }

        [HttpDelete("deleteProductById/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                await _productService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiException(ex));
            }
        }
    }
}
