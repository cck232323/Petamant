using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyDotnetApp.DTOs;
using MyDotnetApp.Services;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyDotnetApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ActivitiesController : ControllerBase
    {
        private readonly IActivityService _activityService;

        public ActivitiesController(IActivityService activityService)
        {
            _activityService = activityService;
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