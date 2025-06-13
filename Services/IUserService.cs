// using System.Threading.Tasks;
using MyDotnetApp.DTOs;  // 使用 DTOs 命名空间

namespace MyDotnetApp.Services
{
    public interface IUserService
    {
        Task<UserDto> GetUserByIdAsync(int id);
        Task<UserDto> RegisterAsync(UserRegisterDto registerDto);
        Task<UserLoginResponseDto> LoginAsync(UserLoginDto loginDto);
    }
}