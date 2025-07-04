using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyDotnetApp.DTOs;
using MyDotnetApp.Services;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using MyDotnetApp.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
namespace MyDotnetApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous] // 添加这一行，允许匿名访问
  
    public class ActivitiesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // public CommentsController(ApplicationDbContext context)
        // {
        //     _context = context;
        // }
        private readonly IActivityService _activityService;

        public ActivitiesController(IActivityService activityService, ApplicationDbContext context)
        {
            _activityService = activityService;
            _context = context;
            // _context = activityService.GetDbContext(); // 获取 ApplicationDbContext 实例
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ActivityDto>>> GetActivities([FromQuery] string? searchTerm)
        {
            var activities = await _activityService.GetActivitiesAsync(searchTerm);
            return Ok(activities);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ActivityDto>> GetActivity(int id)
        {
            try
            {
                var activity = await _activityService.GetActivityByIdAsync(id);
                return Ok(activity);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ActivityDto>> CreateActivity(CreateActivityDto createActivityDto)
        {
            try
            {
                // 获取当前用户ID
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                if (userId == 0)
                {
                    return Unauthorized("用户未认证");
                }

                var activity = await _activityService.CreateActivityAsync(createActivityDto, userId);
                return Ok(activity);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}/registrations")]
        public async Task<ActionResult<IEnumerable<RegistrationDto>>> GetActivityRegistrations(int id)
        {
            try
            {
                var registrations = await _activityService.GetActivityRegistrationsAsync(id);
                return Ok(registrations);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        // 在ActivitiesController.cs中添加
        [HttpGet("{id}/comments")]
        public async Task<ActionResult<IEnumerable<CommentDto>>> GetActivityComments(int id)
        {
            // 只获取顶层评论
            var comments = await _context.Comments
                .Where(c => c.ActivityId == id && c.ParentCommentId == null)
                .Include(c => c.User)
                .Include(c => c.Replies)
                    .ThenInclude(r => r.User)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    UserId = c.UserId,
                    UserName = c.User.UserName,
                    ActivityId = c.ActivityId,
                    Replies = c.Replies.Select(r => new CommentDto
                    {
                        Id = r.Id,
                        Content = r.Content,
                        CreatedAt = r.CreatedAt,
                        UserId = r.UserId,
                        UserName = r.User.UserName,
                        ActivityId = r.ActivityId,
                        ParentCommentId = r.ParentCommentId,
                        ReplyToUserId = r.ReplyToUserId,
                        ReplyToUserName = r.ReplyToUserName
                    }).OrderBy(r => r.CreatedAt).ToList()
                })
                .ToListAsync();

            return Ok(comments);
        }
        [HttpGet("user/{userId}/created")]
        public async Task<ActionResult<IEnumerable<ActivityDto>>> GetUserCreatedActivities(int userId)
        {
            try
            {
                var activities = await _activityService.GetUserCreatedActivitiesAsync(userId);
                return Ok(activities);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("user/{userId}/registered")]
        public async Task<ActionResult<IEnumerable<ActivityDto>>> GetUserRegisteredActivities(int userId)
        {
            try
            {
                var activities = await _activityService.GetUserRegisteredActivitiesAsync(userId);
                return Ok(activities);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteActivity(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                await _activityService.DeleteActivityAsync(id, userId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}