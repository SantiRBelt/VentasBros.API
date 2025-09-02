using System.Collections.Generic;
using System.Threading.Tasks;
using VentasBros.Application.DTOs.User;

namespace VentasBros.Application.Interfaces
{
    public interface IUserService
    {
        Task<ApiResponse<UserDto?>> GetByIdAsync(int id);
        Task<ApiResponse<UserDto?>> GetByUsernameAsync(string username);
        Task<ApiResponse<IEnumerable<UserDto>>> GetAllAsync();
        Task<ApiResponse<UserDto>> RegisterAsync(RegisterUserDto registerDto);
        Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto loginDto);
        Task<ApiResponse<AuthResponseDto>> RefreshTokenAsync(RefreshTokenDto refreshTokenDto);
        Task<ApiResponse<bool>> LogoutAsync(string token);
        Task<ApiResponse<UserDto>> UpdateAsync(int id, UpdateUserDto updateDto);
        Task<ApiResponse<bool>> DeleteAsync(int id);
        Task<ApiResponse<bool>> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto);
        Task<ApiResponse<bool>> UsernameExistsAsync(string username);
        Task<ApiResponse<bool>> EmailExistsAsync(string email);
    }
}
