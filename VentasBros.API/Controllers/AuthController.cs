using Application.DTOs.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VentasBros.Application.DTOs.User;
using VentasBros.Application.Interfaces;

namespace VentasBros.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var result = await _userService.LoginAsync(loginDto);
                if (!result.Success)
                    return Unauthorized(new { message = result.Message });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiException(ex));
            }
        }

        [HttpPost("refresh")]
        [Authorize]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
        {
            try
            {
                var result = await _userService.RefreshTokenAsync(refreshTokenDto);
                if (!result.Success)
                    return Unauthorized(new { message = result.Message });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiException(ex));
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                if (authHeader?.StartsWith("Bearer ") == true)
                {
                    var token = authHeader.Substring("Bearer ".Length).Trim();
                    var result = await _userService.LogoutAsync(token);

                    if (result.Success)
                        return Ok(new { message = result.Message });
                    else
                        return BadRequest(new { message = result.Message });
                }

                return BadRequest(new { message = "Token no encontrado" });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiException(ex));
            }
        }

        [HttpPost("changeUserPassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("sub")?.Value ?? "0");
                var result = await _userService.ChangePasswordAsync(userId, changePasswordDto);

                if (!result.Success)
                    return BadRequest(new { message = result.Message });

                return Ok(new { message = result.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiException(ex));
            }
        }

        [HttpGet("checkUsernameExists/{username}")]
        public async Task<IActionResult> CheckUsername(string username)
        {
            try
            {
                var result = await _userService.UsernameExistsAsync(username);
                return Ok(new { exists = result.Data });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiException(ex));
            }
        }

        [HttpGet("checkEmailExists/{email}")]
        public async Task<IActionResult> CheckEmail(string email)
        {
            try
            {
                var result = await _userService.EmailExistsAsync(email);
                return Ok(new { exists = result.Data });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiException(ex));
            }
        }
    }
}
