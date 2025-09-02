using System.Security.Claims;
using System.Threading.Tasks;
using VentasBros.Application.DTOs.User;
using VentasBros.Domain.Entities;

namespace VentasBros.Application.Interfaces
{
    public interface IJwtService
    {
        Task<string> GenerateTokenAsync(User user);
        Task<AuthResponseDto> RefreshTokenAsync(string currentToken);
        Task<bool> ValidateTokenAsync(string token);
        Task RevokeTokenAsync(string token);
        Task RevokeAllUserTokensAsync(int userId);
        ClaimsPrincipal? GetPrincipalFromToken(string token);
        Task<bool> IsTokenActiveAsync(string token);
        Task UpdateTokenActivityAsync(string token);
    }
}
