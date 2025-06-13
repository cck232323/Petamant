using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // 添加这一行
using MyDotnetApp.Data;
using MyDotnetApp.DTOs;
using MyDotnetApp.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyDotnetApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RegistrationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RegistrationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult> RegisterForActivity(RegistrationCreateDto registrationDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userId == 0)
            {
                return Unauthorized("用户未认证");
            }

            var activity = await _context.Activities.FindAsync(registrationDto.ActivityId);
            if (activity == null)
            {
                return NotFound("活动不存在");
            }

            // 检查用户是否已经报名
            var existingRegistration = await _context.Registrations
                .FirstOrDefaultAsync(r => r.UserId == userId && r.ActivityId == registrationDto.ActivityId);
            
            if (existingRegistration != null)
            {
                return BadRequest("您已经报名参加此活动");
            }

            var registration = new Registration
            {
                UserId = userId,
                ActivityId = registrationDto.ActivityId,
                PetInfo = registrationDto.PetInfo
            };

            _context.Registrations.Add(registration);
            await _context.SaveChangesAsync();

            return Ok(new { message = "报名成功" });
        }
    }
}
// 在RegistrationsController.cs中
