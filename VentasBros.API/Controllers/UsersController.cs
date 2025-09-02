using Application.DTOs.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using VentasBros.Application.DTOs.User;
using VentasBros.Application.Interfaces;

namespace VentasBros.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("getAllUsers")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                return Ok(await _userService.GetAllAsync());
            }catch (ArgumentException ex)
            {
                return BadRequest(new ApiException(ex));
            }
        }

        [HttpPost("registerUser")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerDto)
        {
            try
            {
                return Ok(await _userService.RegisterAsync(registerDto));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiException(ex));
            }
        }

        [HttpGet("getUsersById/{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var currentUserId = int.Parse(User.FindFirst("sub")?.Value ?? "0");
            var userRole = User.FindFirst("role")?.Value;
            if (userRole != "Admin" && currentUserId != id)
                return Forbid();
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound();
            return Ok(user);
        }

        [HttpPut("updateUserById/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto updateDto)
        {
            try
            {
                var currentUserId = int.Parse(User.FindFirst("sub")?.Value ?? "0");
                var userRole = User.FindFirst("role")?.Value;
                // Solo admin puede actualizar otros usuarios, o el usuario puede actualizar su propio perfil
                if (userRole != "Admin" && currentUserId != id)
                    return Forbid();
                return Ok(await _userService.UpdateAsync(id, updateDto));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpDelete("deleteUserById/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                await _userService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}
