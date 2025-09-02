using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VentasBros.Application.DTOs.User;
using VentasBros.Application.Interfaces;
using VentasBros.Application.Services.Security;
using VentasBros.Domain.Entities;

namespace VentasBros.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHashService _passwordHashService;
        private readonly IJwtService _jwtService;

        public UserService(
            IUserRepository userRepository,
            IPasswordHashService passwordHashService,
            IJwtService jwtService)
        {
            _userRepository = userRepository;
            _passwordHashService = passwordHashService;
            _jwtService = jwtService;
        }

        public async Task<ApiResponse<UserDto?>> GetByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return new ApiResponse<UserDto?> { Success = false, Message = "Usuario no encontrado", Data = null };

            return new ApiResponse<UserDto?> { Success = true, Message = "Usuario encontrado", Data = MapToDto(user) };
        }

        public async Task<ApiResponse<UserDto?>> GetByUsernameAsync(string username)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null)
                return new ApiResponse<UserDto?> { Success = false, Message = "Usuario no encontrado", Data = null };

            return new ApiResponse<UserDto?> { Success = true, Message = "Usuario encontrado", Data = MapToDto(user) };
        }

        public async Task<ApiResponse<IEnumerable<UserDto>>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return new ApiResponse<IEnumerable<UserDto>> { Success = true, Message = "Usuarios obtenidos", Data = users.Select(MapToDto) };
        }

        public async Task<ApiResponse<UserDto>> RegisterAsync(RegisterUserDto registerDto)
        {
            if (registerDto.Password != registerDto.ConfirmPassword)
                return new ApiResponse<UserDto> { Success = false, Message = "Las contraseñas no coinciden", Data = null };

            //if (await _userRepository.UsernameExistsAsync(registerDto.Username))
            //    return new ApiResponse<UserDto> { Success = false, Message = "El nombre de usuario ya existe", Data = null };

            //if (await _userRepository.EmailExistsAsync(registerDto.Email))
            //    return new ApiResponse<UserDto> { Success = false, Message = "El email ya está registrado", Data = null };

            User user = new User
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = _passwordHashService.HashPassword(registerDto.Password),
                Role = registerDto.Role,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            User createdUser = await _userRepository.CreateAsync(user);
            return new ApiResponse<UserDto> { Success = true, Message = "Usuario registrado correctamente", Data = MapToDto(createdUser) };
        }

        public async Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto loginDto)
        {
            User? user = await _userRepository.GetByEmailAsync(loginDto.Email);

            if (user == null || !user.IsActive)
                return new ApiResponse<AuthResponseDto> { Success = false, Message = "Credenciales inválidas", Data = null };

            if (!_passwordHashService.VerifyPassword(loginDto.Password, user.PasswordHash))
                return new ApiResponse<AuthResponseDto> { Success = false, Message = "Credenciales inválidas", Data = null };

            var token = await _jwtService.GenerateTokenAsync(user);

            return new ApiResponse<AuthResponseDto>
            {
                Success = true,
                Message = "Login exitoso",
                Data = new AuthResponseDto
                {
                    Token = token,
                    User = MapToDto(user),
                    ExpiresAt = DateTime.UtcNow.AddHours(24)
                }
            };
        }

        public async Task<ApiResponse<UserDto>> UpdateAsync(int id, UpdateUserDto updateDto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return new ApiResponse<UserDto> { Success = false, Message = "Usuario no encontrado", Data = null };

            var existingUser = await _userRepository.GetByUsernameAsync(updateDto.Username);
            if (existingUser != null && existingUser.Id != id)
                return new ApiResponse<UserDto> { Success = false, Message = "El nombre de usuario ya existe", Data = null };

            existingUser = await _userRepository.GetByEmailAsync(updateDto.Email);
            if (existingUser != null && existingUser.Id != id)
                return new ApiResponse<UserDto> { Success = false, Message = "El email ya está registrado", Data = null };

            user.Username = updateDto.Username;
            user.Email = updateDto.Email;
            user.Role = updateDto.Role;
            user.IsActive = updateDto.IsActive;

            var updatedUser = await _userRepository.UpdateAsync(user);
            return new ApiResponse<UserDto> { Success = true, Message = "Usuario actualizado correctamente", Data = MapToDto(updatedUser) };
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            await _userRepository.DeleteAsync(id);
            return new ApiResponse<bool> { Success = true, Message = "Usuario eliminado correctamente", Data = true };
        }

        public async Task<ApiResponse<bool>> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto)
        {
            User? user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return new ApiResponse<bool> { Success = false, Message = "Usuario no encontrado", Data = false };

            if (!_passwordHashService.VerifyPassword(changePasswordDto.CurrentPassword, user.PasswordHash))
                return new ApiResponse<bool> { Success = false, Message = "Contraseña actual incorrecta", Data = false };

            if (changePasswordDto.NewPassword != changePasswordDto.ConfirmNewPassword)
                return new ApiResponse<bool> { Success = false, Message = "Las contraseñas nuevas no coinciden", Data = false };

            user.PasswordHash = _passwordHashService.HashPassword(changePasswordDto.NewPassword);
            await _userRepository.UpdateAsync(user);

            return new ApiResponse<bool> { Success = true, Message = "Contraseña cambiada correctamente", Data = true };
        }

        public async Task<ApiResponse<bool>> UsernameExistsAsync(string username)
        {
            var exists = await _userRepository.UsernameExistsAsync(username);
            return new ApiResponse<bool> { Success = true, Message = exists ? "El nombre de usuario existe" : "El nombre de usuario no existe", Data = exists };
        }

        public async Task<ApiResponse<bool>> EmailExistsAsync(string email)
        {
            var exists = await _userRepository.EmailExistsAsync(email);
            return new ApiResponse<bool> { Success = true, Message = exists ? "El email existe" : "El email no existe", Data = exists };
        }

        private static UserDto MapToDto(User user)
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

        public async Task<ApiResponse<AuthResponseDto>> RefreshTokenAsync(RefreshTokenDto refreshTokenDto)
        {
            try
            {
                var refreshedAuth = await _jwtService.RefreshTokenAsync(refreshTokenDto.Token);
                return new ApiResponse<AuthResponseDto>
                {
                    Success = true,
                    Message = "Token renovado exitosamente",
                    Data = refreshedAuth
                };
            }
            catch (UnauthorizedAccessException ex)
            {
                return new ApiResponse<AuthResponseDto>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        public async Task<ApiResponse<bool>> LogoutAsync(string token)
        {
            try
            {
                await _jwtService.RevokeTokenAsync(token);
                return new ApiResponse<bool>
                {
                    Success = true,
                    Message = "Logout exitoso",
                    Data = true
                };
            }
            catch (Exception)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Error durante el logout",
                    Data = false
                };
            }
        }

    }
}
