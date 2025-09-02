using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using VentasBros.API.Mappings;
using VentasBros.API.Middleware;
using VentasBros.Application.Interfaces;
using VentasBros.Application.Services;
using VentasBros.Application.Services.Security;
using VentasBros.Domain.Interfaces;
using VentasBros.Infrastructure.Data;
using VentasBros.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Repositories
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IJwtTokenRepository, JwtTokenRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Configura CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policy =>
        {
            policy.WithOrigins(
                "http://localhost:3000",
                "http://localhost:4200",      // Tu frontend Angular
                "http://localhost:5173",
                "https://localhost:7157",     // Tu API en HTTPS
                "http://localhost:5262"
                ).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
        });
});

// Services
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPasswordHashService, PasswordHashService>();
builder.Services.AddScoped<IJwtService, JwtService>();


// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(secretKey)
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}else
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowSpecificOrigin");
app.UseAuthentication();
app.UseMiddleware<TokenRefreshMiddleware>();
app.UseAuthorization();
app.MapControllers();

app.Run();
