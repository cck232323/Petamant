using MyDotnetApp.DTOs;

namespace MyDotnetApp.Services
{
    public interface IRegistrationService
    {
        Task<RegistrationDto> RegisterAsync(RegistrationCreateDto registrationDto, int userId);
        Task<IEnumerable<RegistrationDto>> GetUserRegistrationsAsync(int userId);
        Task CancelRegistrationAsync(int id, int userId);
    }
}