namespace VentasBros.Application.Services.Security
{
    public interface IPasswordHashService
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
    }
}
