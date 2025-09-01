using System;

namespace VentasBros.Domain.Entities
{
    public class Image
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Url { get; set; } = string.Empty;
        public bool IsMain { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public Product Product { get; set; } = null!;
    }
}
