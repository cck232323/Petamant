using MyDotnetApp.DTOs;

namespace MyDotnetApp.Services
{
    public interface IActivityService
    {
        Task<IEnumerable<ActivityDto>> GetActivitiesAsync(string? searchTerm = null);
        Task<ActivityDto> GetActivityByIdAsync(int id);
        
        // 修改这一行，使用 CreateActivityDto 而不是 ActivityCreateDto
        Task<ActivityDto> CreateActivityAsync(CreateActivityDto createActivityDto, int userId);
        
        Task<IEnumerable<RegistrationDto>> GetActivityRegistrationsAsync(int activityId);
        Task<IEnumerable<ActivityDto>> GetUserCreatedActivitiesAsync(int userId);
        Task<IEnumerable<ActivityDto>> GetUserRegisteredActivitiesAsync(int userId);
        Task DeleteActivityAsync(int id, int userId);
    }
}