using System.Threading.Tasks;
using VentasBros.Domain.Entities;

namespace VentasBros.Application.Interfaces
{
    public interface IJwtTokenRepository
    {
        Task SaveTokenAsync(JwtToken token);
        Task<JwtToken?> GetTokenAsync(string token);
        Task RevokeTokenAsync(string token);
        Task RevokeAllUserTokensAsync(int userId);
        Task<bool> IsTokenValidAsync(string token);
        Task UpdateLastAccessedAsync(string token);
        Task<bool> IsTokenExpiredByInactivityAsync(string token, int inactivityMinutes = 15);
        Task CleanupExpiredTokensAsync();
    }
}

