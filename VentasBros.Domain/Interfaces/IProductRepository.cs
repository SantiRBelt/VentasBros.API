using System.Collections.Generic;
using System.Threading.Tasks;
using VentasBros.Domain.Entities;

namespace VentasBros.Domain.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<Product?> GetByIdWithImagesAsync(int id);
        Task<(IEnumerable<Product> Items, int Total)> GetPagedWithFiltersAsync(
            int page,
            int pageSize,
            string? search = null,
            int? categoryId = null,
            bool onlyActive = false,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            string sort = "createdAt",
            string direction = "desc");
    }
}
