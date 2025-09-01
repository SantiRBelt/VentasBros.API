using VentasBros.Application.DTOs.Common;
using VentasBros.Application.DTOs.Product;

namespace VentasBros.Application.Services
{
    public interface IProductService
    {
        Task<ProductDto?> GetByIdAsync(int id);
        Task<PagedResult<ProductDto>> GetPagedAsync(int page, int pageSize, string? search = null, int? categoryId = null, bool onlyActive = false);
        Task<ProductDto> CreateAsync(CreateProductDto dto);
        Task<ProductDto> UpdateAsync(int id, UpdateProductDto dto);
        Task DeleteAsync(int id);
    }
}
