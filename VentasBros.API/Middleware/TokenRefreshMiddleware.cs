using Microsoft.AspNetCore.Authorization;
using VentasBros.Application.Interfaces;

namespace VentasBros.API.Middleware
{
    public class TokenRefreshMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public TokenRefreshMiddleware(RequestDelegate next, IServiceScopeFactory serviceScopeFactory)
        {
            _next = next;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Solo procesar si la ruta requiere autorización
            var endpoint = context.GetEndpoint();
            var requiresAuth = endpoint?.Metadata.GetMetadata<AuthorizeAttribute>() != null;

            if (requiresAuth && context.Request.Headers.ContainsKey("Authorization"))
            {
                var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                if (authHeader?.StartsWith("Bearer ") == true)
                {
                    var token = authHeader.Substring("Bearer ".Length).Trim();
                    
                    using var scope = _serviceScopeFactory.CreateScope();
                    var jwtService = scope.ServiceProvider.GetRequiredService<IJwtService>();
                    
                    // Verificar si el token está activo y actualizar actividad
                    if (await jwtService.IsTokenActiveAsync(token))
                    {
                        await jwtService.UpdateTokenActivityAsync(token);
                        
                        // Opcional: Generar nuevo token si está cerca de expirar
                        var refreshResponse = await jwtService.RefreshTokenAsync(token);
                        if (refreshResponse.Token != token) // Si se generó un nuevo token
                        {
                            context.Response.Headers.Add("X-New-Token", refreshResponse.Token);
                            context.Response.Headers.Add("X-Token-Expires-At", refreshResponse.ExpiresAt.ToString("o"));
                        }
                    }
                }
            }

            await _next(context);
        }
    }
}

