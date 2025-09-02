using System.Collections.Generic;
using System.Threading.Tasks;
using VentasBros.Application.DTOs.Category;

namespace VentasBros.Application.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetActiveAsync(bool flat = false);
        Task<CategoryDto?> GetByIdAsync(int id);
        Task<IEnumerable<CategoryDto>> GetAllAsync();
        Task<CategoryDto> CreateAsync(CreateCategoryDto dto);
        Task<CategoryDto> UpdateAsync(int id, UpdateCategoryDto dto);
        Task DeleteAsync(int id);
        Task<IEnumerable<CategoryDto>> GetByParentIdAsync(int? parentId);
        Task<bool> NameExistsAsync(string name, int? excludeId = null);
    }
}
