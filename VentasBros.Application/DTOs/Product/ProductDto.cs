using System;
using System.Collections.Generic;

namespace VentasBros.Application.DTOs.Product
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ImageDto> Images { get; set; } = new();
    }

    public class CreateProductDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class UpdateProductDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public bool IsActive { get; set; }
    }

    public class ImageDto
    {
        public int Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public bool IsMain { get; set; }
    }
}
