using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyDotnetApp.Data;
using MyDotnetApp.DTOs;
using MyDotnetApp.Models;

namespace MyDotnetApp.Services
{
    public class ActivityService : IActivityService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ActivityService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ActivityDto>> GetActivitiesAsync(string? searchTerm = null)
        {
            var query = _context.Activities
                .Include(a => a.CreatorUser)
                .Include(a => a.Registrations)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(a => 
                    a.Title.Contains(searchTerm) || 
                    a.Description.Contains(searchTerm) ||
                    a.Location.Contains(searchTerm));
            }

            var activities = await query.ToListAsync();
            return _mapper.Map<IEnumerable<ActivityDto>>(activities);
        }

        public async Task<ActivityDto> GetActivityByIdAsync(int id)
        {
            var activity = await _context.Activities
                .Include(a => a.CreatorUser)
                .Include(a => a.Registrations)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (activity == null)
            {
                throw new Exception("活动不存在");
            }

            return _mapper.Map<ActivityDto>(activity);
        }

        public async Task<ActivityDto> CreateActivityAsync(CreateActivityDto createActivityDto, int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new Exception("用户不存在");
            }

            var activity = new Activity
            {
                Title = createActivityDto.Title,
                Description = createActivityDto.Description,
                Date = createActivityDto.Date,
                Location = createActivityDto.Location,
                CreatorUserId = userId
                // 不要设置 CreatedAt
            };

            _context.Activities.Add(activity);
            await _context.SaveChangesAsync();

            // 返回创建的活动
            return new ActivityDto
            {
                Id = activity.Id,
                Title = activity.Title,
                Description = activity.Description,
                Date = activity.Date,
                Location = activity.Location,
                CreatorUserId = activity.CreatorUserId,
                CreatorId = activity.CreatorUserId,
                CreatorUserName = user.UserName,
                // 不要设置 CreatedAt
                RegistrationsCount = 0
            };
        }

        public async Task<IEnumerable<RegistrationDto>> GetActivityRegistrationsAsync(int activityId)
        {
            var registrations = await _context.Registrations
                .Include(r => r.User)
                .Where(r => r.ActivityId == activityId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<RegistrationDto>>(registrations);
        }

        public async Task<IEnumerable<ActivityDto>> GetUserCreatedActivitiesAsync(int userId)
        {
            var activities = await _context.Activities
                .Include(a => a.CreatorUser)
                .Include(a => a.Registrations)
                .Where(a => a.CreatorUserId == userId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ActivityDto>>(activities);
        }

        public async Task<IEnumerable<ActivityDto>> GetUserRegisteredActivitiesAsync(int userId)
        {
            var activities = await _context.Activities
                .Include(a => a.CreatorUser)
                .Include(a => a.Registrations)
                .Where(a => a.Registrations.Any(r => r.UserId == userId))
                .ToListAsync();

            return _mapper.Map<IEnumerable<ActivityDto>>(activities);
        }

        public async Task DeleteActivityAsync(int id, int userId)
        {
            var activity = await _context.Activities
                .FirstOrDefaultAsync(a => a.Id == id);

            if (activity == null)
            {
                throw new Exception("活动不存在");
            }

            if (activity.CreatorUserId != userId)
            {
                throw new Exception("只有活动创建者才能删除活动");
            }

            _context.Activities.Remove(activity);
            await _context.SaveChangesAsync();
        }
    }
}