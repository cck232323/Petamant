using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyDotnetApp.DTOs;  // 使用 DTOs 命名空间
using MyDotnetApp.Services;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyDotnetApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(UserRegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = await _userService.RegisterAsync(registerDto);
                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserLoginResponseDto>> Login(UserLoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var response = await _userService.LoginAsync(loginDto);
                // 存储token到cookie或session
                HttpContext.Response.Cookies.Append("AuthToken", response.Token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                });
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            try
            {
                // 获取当前登录用户的ID
                var claimValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(claimValue))
                {
                    return Unauthorized("无法识别当前用户");
                }

                if (!int.TryParse(claimValue, out int currentUserId))
                {
                    return BadRequest("用户ID格式无效");
                }

                // 记录调试信息
                Console.WriteLine($"请求的用户ID: {id}, 当前用户ID: {currentUserId}");

                // 只允许用户查看自己的资料，或者管理员查看任何用户的资料
                if (currentUserId != id && !User.IsInRole("Admin"))
                {
                    return Forbid();
                }

                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    return NotFound($"未找到ID为{id}的用户");
                }
                
                return Ok(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取用户资料时出错: {ex.Message}");
                return StatusCode(500, $"服务器错误: {ex.Message}");
            }
        }
        
    }
}