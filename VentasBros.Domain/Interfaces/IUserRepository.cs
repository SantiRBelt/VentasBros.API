using System.Collections.Generic;
using System.Threading.Tasks;
using VentasBros.Domain.Entities;

namespace VentasBros.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByEmailAsync(string email);
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> CreateAsync(User user);
        Task<User> UpdateAsync(User user);
        Task DeleteAsync(int id);
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> EmailExistsAsync(string email);
    }
}
