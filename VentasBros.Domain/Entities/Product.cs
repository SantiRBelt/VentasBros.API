using System;
using System.Collections.Generic;

namespace VentasBros.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public Category Category { get; set; } = null!;
        public ICollection<Image> Images { get; set; } = new List<Image>();
    }
}
