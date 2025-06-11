using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MyDotnetApp.Data;
using DtoModels = MyDotnetApp.DTOs;  // 为 DTOs 命名空间创建别名
using MyDotnetApp.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<DtoModels.UserDto> GetUserByIdAsync(int id)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                throw new Exception("用户不存在");
            }

            return _mapper.Map<DtoModels.UserDto>(user);
        }

        public async Task<DtoModels.UserDto> RegisterAsync(DtoModels.UserRegisterDto registerDto)
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

            return _mapper.Map<DtoModels.UserDto>(user);
        }

        public async Task<DtoModels.UserLoginResponseDto> LoginAsync(DtoModels.UserLoginDto loginDto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email); // 使用 Email 而不是 UserName

            if (user == null)
            {
                throw new Exception("邮箱或密码错误");
            }

            // 验证密码
            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                throw new Exception("邮箱或密码错误");
            }

            // 生成JWT令牌
            var token = GenerateJwtToken(user);

            return new DtoModels.UserLoginResponseDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Token = token
            };
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // 使用ID（整数）
                new Claim(ClaimTypes.Name, user.UserName), // 用户名（字符串）
                new Claim(ClaimTypes.Email, user.Email) // 添加电子邮件声明
            };
            
            var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is not configured");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var issuer = _configuration["Jwt:Issuer"] ?? "default_issuer";
            var audience = _configuration["Jwt:Audience"] ?? "default_audience";

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}