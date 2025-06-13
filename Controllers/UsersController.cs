using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyDotnetApp.DTOs;  // 使用 DTOs 命名空间
using MyDotnetApp.Services;
using System;
using System.Linq;
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
        
        /// <summary>
        /// Registers a new user in the system.
        /// </summary>
        /// <param name="registerDto">The user registration data transfer object containing registration information.</param>
        /// <returns>
        /// ActionResult containing UserRegisterResponseDto with registration confirmation details if successful;
        /// BadRequest with validation errors or exception message if registration fails.
        /// </returns>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<UserDto>> Register(UserRegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var response = await _userService.RegisterAsync(registerDto);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<UserLoginResponseDto>> Login(UserLoginDto loginDto)
        {
            Console.WriteLine("========== 收到登录请求 ==========");
            Console.WriteLine($"登录请求数据: Email={loginDto.Email}, Password长度={loginDto.Password?.Length ?? 0}");
            
            if (!ModelState.IsValid)
            {
                Console.WriteLine("登录请求验证失败:");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"- {error.ErrorMessage}");
                }
                return BadRequest(ModelState);
            }

            try
            {
                Console.WriteLine("调用UserService.LoginAsync...");
                var response = await _userService.LoginAsync(loginDto);
                Console.WriteLine($"登录成功: UserID={response.Id}, Token长度={response.Token.Length}");
                Console.WriteLine("========== 登录请求处理完成 ==========");
                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"登录失败: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"内部异常: {ex.InnerException.Message}");
                }
                Console.WriteLine("========== 登录请求处理失败 ==========");
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [Authorize] // 恢复这个特性，因为方法内部逻辑要求用户已登录
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