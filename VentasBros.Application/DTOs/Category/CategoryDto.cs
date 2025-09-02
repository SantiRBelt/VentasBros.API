using System;
using System.Collections.Generic;

namespace VentasBros.Application.DTOs.Category
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? ParentId { get; set; }
        public string? ParentName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<CategoryDto> Children { get; set; } = new();
    }

    public class CreateCategoryDto
    {
        public string Name { get; set; } = string.Empty;
        public int? ParentId { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class UpdateCategoryDto
    {
        public string Name { get; set; } = string.Empty;
        public int? ParentId { get; set; }
        public bool IsActive { get; set; }
    }
}
