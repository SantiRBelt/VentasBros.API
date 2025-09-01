using System;

namespace VentasBros.Domain.Entities
{
    public class JwtToken
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public User User { get; set; } = null!;
    }
}
