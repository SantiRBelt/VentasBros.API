using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using VentasBros.Application.Interfaces;
using VentasBros.Domain.Entities;
using VentasBros.Infrastructure.Data;

namespace VentasBros.Infrastructure.Repositories
{
    public class JwtTokenRepository : IJwtTokenRepository
    {
        private readonly ApplicationDbContext _context;

        public JwtTokenRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SaveTokenAsync(JwtToken token)
        {
            _context.JwtTokens.Add(token);
            await _context.SaveChangesAsync();
        }

        public async Task<JwtToken?> GetTokenAsync(string token)
        {
            return await _context.JwtTokens
                .FirstOrDefaultAsync(t => t.Token == token);
        }

        public async Task RevokeTokenAsync(string token)
        {
            var jwtToken = await GetTokenAsync(token);
            if (jwtToken != null)
            {
                jwtToken.IsRevoked = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task RevokeAllUserTokensAsync(int userId)
        {
            var userTokens = await _context.JwtTokens
                .Where(t => t.UserId == userId && !t.IsRevoked)
                .ToListAsync();

            foreach (var token in userTokens)
            {
                token.IsRevoked = true;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsTokenValidAsync(string token)
        {
            var jwtToken = await GetTokenAsync(token);
            return jwtToken != null &&
                   !jwtToken.IsRevoked &&
                   jwtToken.ExpiresAt > DateTime.UtcNow;
        }

        public async Task UpdateLastAccessedAsync(string token)
        {
            var jwtToken = await GetTokenAsync(token);
            if (jwtToken != null && !jwtToken.IsRevoked)
            {
                jwtToken.LastAccessedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsTokenExpiredByInactivityAsync(string token, int inactivityMinutes = 15)
        {
            var jwtToken = await GetTokenAsync(token);
            if (jwtToken == null || jwtToken.IsRevoked)
                return true;

            var inactivityThreshold = DateTime.UtcNow.AddMinutes(-inactivityMinutes);
            return jwtToken.LastAccessedAt < inactivityThreshold;
        }

        public async Task CleanupExpiredTokensAsync()
        {
            var expiredTokens = await _context.JwtTokens
                .Where(t => t.ExpiresAt < DateTime.UtcNow || t.IsRevoked)
                .ToListAsync();

            if (expiredTokens.Any())
            {
                _context.JwtTokens.RemoveRange(expiredTokens);
                await _context.SaveChangesAsync();
            }
        }
    }
}

