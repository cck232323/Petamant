using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MyDotnetApp.Data;
using MyDotnetApp.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using MyDotnetApp.DTOs;

namespace MyDotnetApp.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UserService(ApplicationDbContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);
            // var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user == null)
            {
                throw new Exception("用户不存在");
            }

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> RegisterAsync(UserRegisterDto registerDto)
        {
            // 检查用户名是否已存在
            if (await _context.Users.AnyAsync(u => u.UserName == registerDto.UserName))
            {
                throw new Exception("用户名已存在");
            }

            // 检查邮箱是否已存在
            if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
            {
                throw new Exception("邮箱已被注册");
            }

            // 创建新用户
            var user = _mapper.Map<User>(registerDto);
            
            // 哈希密码
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserLoginResponseDto> LoginAsync(UserLoginDto loginDto)
        {
            Console.WriteLine("========== 开始登录流程 ==========");
            Console.WriteLine($"登录请求: Email={loginDto.Email}, Password长度={loginDto.Password?.Length ?? 0}");
            
            try
            {
                // 验证输入
                if (string.IsNullOrEmpty(loginDto.Email))
                {
                    Console.WriteLine("登录失败: 邮箱为空");
                    throw new Exception("邮箱不能为空");
                }
                
                if (string.IsNullOrEmpty(loginDto.Password))
                {
                    Console.WriteLine("登录失败: 密码为空");
                    throw new Exception("密码不能为空");
                }
                
                Console.WriteLine($"正在查找用户: Email={loginDto.Email}");
                
                // 查找用户 - 优先使用Email
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == loginDto.Email);
                    
                // 如果通过Email找不到用户，尝试使用Email作为用户名查找
                if (user == null)
                {
                    Console.WriteLine($"通过Email未找到用户，尝试使用Email作为用户名查找");
                    user = await _context.Users
                        .FirstOrDefaultAsync(u => u.UserName == loginDto.Email);
                }

                if (user == null)
                {
                    Console.WriteLine($"登录失败: 未找到用户 Email={loginDto.Email}");
                    throw new Exception("邮箱或密码错误");
                }

                Console.WriteLine($"找到用户: ID={user.Id}, UserName={user.UserName}, Email={user.Email}");
                
                // 检查密码哈希是否为空
                if (string.IsNullOrEmpty(user.PasswordHash))
                {
                    Console.WriteLine($"错误: 用户 {user.Id} 的密码哈希为空");
                    throw new Exception("用户账户配置错误，请联系管理员");
                }

                // 验证密码
                Console.WriteLine("正在验证密码...");
                bool passwordValid = false;
                try
                {
                    passwordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"密码验证过程中发生异常: {ex.Message}");
                    throw new Exception("密码验证失败，请联系管理员");
                }
                
                if (!passwordValid)
                {
                    Console.WriteLine($"登录失败: 密码验证失败 UserID={user.Id}");
                    throw new Exception("邮箱或密码错误");
                }

                Console.WriteLine($"密码验证成功: UserID={user.Id}");

                // 生成JWT令牌
                Console.WriteLine("正在生成JWT令牌...");
                string token = "";
                try
                {
                    token = GenerateJwtToken(user);
                    Console.WriteLine($"JWT令牌生成成功，长度: {token.Length}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"生成JWT令牌时发生错误: {ex.Message}");
                    throw new Exception("生成认证令牌失败，请联系管理员");
                }

                // 创建响应对象
                var response = new UserLoginResponseDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Token = token
                };
                
                Console.WriteLine($"登录成功: UserID={user.Id}, UserName={user.UserName}");
                Console.WriteLine("========== 登录流程结束 ==========");
                
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"登录过程中发生异常: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"内部异常: {ex.InnerException.Message}");
                }
                Console.WriteLine("========== 登录流程异常结束 ==========");
                throw; // 重新抛出异常，让控制器处理
            }
        }

        private string GenerateJwtToken(User user)
        {
            Console.WriteLine("开始生成JWT令牌...");
            
            try
            {
                // 检查配置
                var jwtKey = _configuration["Jwt:Key"];
                Console.WriteLine($"JWT Key配置: {(string.IsNullOrEmpty(jwtKey) ? "未配置" : "已配置，长度: " + jwtKey.Length)}");
                
                if (string.IsNullOrEmpty(jwtKey))
                {
                    Console.WriteLine("警告: JWT Key未配置，使用默认密钥");
                    jwtKey = "DefaultSecretKeyForDevelopmentOnly12345678901234567890"; // 默认密钥，仅用于开发
                }
                
                var issuer = _configuration["Jwt:Issuer"];
                Console.WriteLine($"JWT Issuer配置: {(string.IsNullOrEmpty(issuer) ? "未配置，使用默认值" : issuer)}");
                issuer = issuer ?? "default_issuer";
                
                var audience = _configuration["Jwt:Audience"];
                Console.WriteLine($"JWT Audience配置: {(string.IsNullOrEmpty(audience) ? "未配置，使用默认值" : audience)}");
                audience = audience ?? "default_audience";
                
                // 创建声明
                Console.WriteLine($"为用户 {user.Id} 创建JWT声明");
                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email),
                    // new Claim("nameid", user.Id.ToString()), // 添加简单的nameid声明
                    // new Claim("sub", user.Id.ToString())     // 添加sub声明
                };
                
                // 创建密钥
                Console.WriteLine("创建签名密钥...");
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                
                // 创建令牌
                Console.WriteLine("创建JWT令牌描述符...");
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = creds,
                    Issuer = issuer,
                    Audience = audience
                };
                
                Console.WriteLine($"令牌过期时间: {tokenDescriptor.Expires}");
                
                // 生成令牌
                Console.WriteLine("生成JWT令牌...");
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);
                
                Console.WriteLine($"JWT令牌生成成功，长度: {tokenString.Length}");
                return tokenString;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"生成JWT令牌时发生错误: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"内部异常: {ex.InnerException.Message}");
                }
                throw;
            }
        }
    }
}