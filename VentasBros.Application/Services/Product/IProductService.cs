using System.Threading.Tasks;
using VentasBros.Application.DTOs.Common;
using VentasBros.Application.DTOs.Product;

namespace VentasBros.Application.Services
{
    public interface IProductService
    {
        Task<ProductDto?> GetByIdAsync(int id);
        Task<PagedResult<ProductDto>> GetPagedAsync(ProductFilterDto filter);
        Task<PagedResult<ProductDto>> GetCatalogPagedAsync(CatalogFilterDto filter);
        Task<ProductDto> CreateAsync(CreateProductDto dto);
        Task<ProductDto> UpdateAsync(int id, UpdateProductDto dto);
        Task DeleteAsync(int id);
    }
}
