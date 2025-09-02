using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VentasBros.Application.DTOs.Category;
using VentasBros.Application.Services;
using VentasBros.Domain.Entities;
using VentasBros.Domain.Interfaces;

namespace VentasBros.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<CategoryDto>> GetActiveAsync(bool flat = false)
        {
            var categories = await _categoryRepository.GetActiveAsync();
            var categoryDtos = categories.Select(MapToDto).ToList();

            if (flat)
            {
                return categoryDtos;
            }

            // Organizar en árbol jerárquico
            return BuildCategoryTree(categoryDtos);
        }

        public async Task<CategoryDto?> GetByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            return category != null ? MapToDto(category) : null;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return categories.Select(MapToDto);
        }

        public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
        {
            // Validar que el nombre no exista
            if (await NameExistsAsync(dto.Name))
                throw new ArgumentException("Ya existe una categoría con ese nombre");

            // Validar que el padre exista si se especifica
            if (dto.ParentId.HasValue)
            {
                var parent = await _categoryRepository.GetByIdAsync(dto.ParentId.Value);
                if (parent == null || !parent.IsActive)
                    throw new ArgumentException("La categoría padre no existe o no está activa");
            }

            var category = new Category
            {
                Name = dto.Name,
                ParentId = dto.ParentId,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            var createdCategory = await _categoryRepository.AddAsync(category);
            return MapToDto(createdCategory);
        }

        public async Task<CategoryDto> UpdateAsync(int id, UpdateCategoryDto dto)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                throw new ArgumentException("Categoría no encontrada");

            // Validar que el nombre no exista (excluyendo la categoría actual)
            if (await NameExistsAsync(dto.Name, id))
                throw new ArgumentException("Ya existe una categoría con ese nombre");

            // Validar que no se establezca como padre de sí misma o de sus descendientes
            if (dto.ParentId.HasValue)
            {
                if (dto.ParentId == id)
                    throw new ArgumentException("Una categoría no puede ser padre de sí misma");

                if (await IsDescendantAsync(id, dto.ParentId.Value))
                    throw new ArgumentException("No se puede establecer como padre a un descendiente");

                var parent = await _categoryRepository.GetByIdAsync(dto.ParentId.Value);
                if (parent == null || !parent.IsActive)
                    throw new ArgumentException("La categoría padre no existe o no está activa");
            }

            category.Name = dto.Name;
            category.ParentId = dto.ParentId;
            category.IsActive = dto.IsActive;

            var updatedCategory = await _categoryRepository.UpdateAsync(category);
            return MapToDto(updatedCategory);
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                throw new ArgumentException("Categoría no encontrada");

            // Verificar si tiene productos asociados
            if (category.Products.Any())
                throw new InvalidOperationException("No se puede eliminar una categoría que tiene productos asociados");

            // Verificar si tiene categorías hijas activas
            var children = await GetByParentIdAsync(id);
            if (children.Any(c => c.IsActive))
                throw new InvalidOperationException("No se puede eliminar una categoría que tiene subcategorías activas");

            await _categoryRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<CategoryDto>> GetByParentIdAsync(int? parentId)
        {
            var categories = await _categoryRepository.GetByParentIdAsync(parentId);
            return categories.Select(MapToDto);
        }

        public async Task<bool> NameExistsAsync(string name, int? excludeId = null)
        {
            var category = await _categoryRepository.GetByNameAsync(name);
            return category != null && (excludeId == null || category.Id != excludeId);
        }

        private async Task<bool> IsDescendantAsync(int ancestorId, int descendantId)
        {
            var descendant = await _categoryRepository.GetByIdAsync(descendantId);
            
            while (descendant?.ParentId != null)
            {
                if (descendant.ParentId == ancestorId)
                    return true;
                
                descendant = await _categoryRepository.GetByIdAsync(descendant.ParentId.Value);
            }
            
            return false;
        }

        private static CategoryDto MapToDto(Category category)
        {
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                ParentId = category.ParentId,
                ParentName = category.Parent?.Name,
                IsActive = category.IsActive,
                CreatedAt = category.CreatedAt,
                Children = new List<CategoryDto>()
            };
        }

        private static IEnumerable<CategoryDto> BuildCategoryTree(List<CategoryDto> categories)
        {
            var categoryDict = categories.ToDictionary(c => c.Id);
            var rootCategories = new List<CategoryDto>();

            foreach (var category in categories)
            {
                if (category.ParentId.HasValue && categoryDict.ContainsKey(category.ParentId.Value))
                {
                    categoryDict[category.ParentId.Value].Children.Add(category);
                }
                else
                {
                    rootCategories.Add(category);
                }
            }

            return rootCategories;
        }
    }
}
