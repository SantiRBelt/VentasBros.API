using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using VentasBros.Application.DTOs.User;
using VentasBros.Application.Interfaces;
using VentasBros.Domain.Entities;

namespace VentasBros.Application.Services.Security
{
    public class JwtService : IJwtService
    {
        private readonly IJwtTokenRepository _jwtTokenRepository;
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;

        public JwtService(IJwtTokenRepository jwtTokenRepository, IConfiguration configuration, IUserRepository userRepository)
        {
            _jwtTokenRepository = jwtTokenRepository;
            _configuration = configuration;
            _userRepository = userRepository;
        }

        public async Task<string> GenerateTokenAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKey = _configuration["JwtSettings:SecretKey"]
                ?? throw new InvalidOperationException("JWT SecretKey no configurado");

            var key = Encoding.UTF8.GetBytes(secretKey);
            var expirationMinutes = int.Parse(_configuration["JwtSettings:ExpirationMinutes"] ?? "15");
            var expiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                        new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                        new Claim(JwtRegisteredClaimNames.Email, user.Email),
                        new Claim(ClaimTypes.Role, user.Role),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat,
                            DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                            ClaimValueTypes.Integer64)
                    }),
                Expires = expiresAt,
                Issuer = _configuration["JwtSettings:Issuer"],
                Audience = _configuration["JwtSettings:Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // Guardar token usando el repositorio
            var jwtToken = new JwtToken
            {
                Token = tokenString,
                UserId = user.Id,
                ExpiresAt = expiresAt,
                LastAccessedAt = DateTime.UtcNow,
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow
            };

            await _jwtTokenRepository.SaveTokenAsync(jwtToken);
            return tokenString;
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(string currentToken)
        {
            var storedToken = await _jwtTokenRepository.GetTokenAsync(currentToken);
            if (storedToken == null || storedToken.IsRevoked)
                throw new UnauthorizedAccessException("Token inválido");

            // Verificar si el token ha expirado por inactividad (15 minutos)
            if (await _jwtTokenRepository.IsTokenExpiredByInactivityAsync(currentToken, 15))
                throw new UnauthorizedAccessException("Token expirado por inactividad");

            var user = await _userRepository.GetByIdAsync(storedToken.UserId);
            if (user == null || !user.IsActive)
                throw new UnauthorizedAccessException("Usuario inválido");

            // Revocar el token actual
            await _jwtTokenRepository.RevokeTokenAsync(currentToken);

            // Generar nuevo token
            var newToken = await GenerateTokenAsync(user);

            return new AuthResponseDto
            {
                Token = newToken,
                User = MapUserToDto(user),
                ExpiresAt = DateTime.UtcNow.AddMinutes(15)
            };
        }

        public async Task<bool> IsTokenActiveAsync(string token)
        {
            var storedToken = await _jwtTokenRepository.GetTokenAsync(token);
            if (storedToken == null || storedToken.IsRevoked)
                return false;

            // Verificar expiración por inactividad
            return !await _jwtTokenRepository.IsTokenExpiredByInactivityAsync(token, 15);
        }

        public async Task UpdateTokenActivityAsync(string token)
        {
            await _jwtTokenRepository.UpdateLastAccessedAsync(token);
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            return await _jwtTokenRepository.IsTokenValidAsync(token);
        }

        public async Task RevokeTokenAsync(string token)
        {
            await _jwtTokenRepository.RevokeTokenAsync(token);
        }

        public async Task RevokeAllUserTokensAsync(int userId)
        {
            await _jwtTokenRepository.RevokeAllUserTokensAsync(userId);
        }

        public ClaimsPrincipal? GetPrincipalFromToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var secretKey = _configuration["JwtSettings:SecretKey"];

                if (string.IsNullOrWhiteSpace(secretKey))
                    return null;

                var key = Encoding.UTF8.GetBytes(secretKey);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["JwtSettings:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["JwtSettings:Audience"],
                    ValidateLifetime = false,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                return principal;
            }
            catch
            {
                return null;
            }
        }

        private static UserDto MapUserToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };
        }
    }
}

