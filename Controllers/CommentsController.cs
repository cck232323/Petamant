using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyDotnetApp.DTOs;

using Microsoft.EntityFrameworkCore; // 添加这一行
using MyDotnetApp.Data;

using MyDotnetApp.Models;
using System.Security.Claims;

namespace MyDotnetApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // [AllowAnonymous]
    public class CommentsController : ControllerBase
    {
        // private readonly ICommentsService _commentService;

        // public CommentsController(ICommentsService commentService)
        // {
        //     _commentService = commentService;
        // }
        private readonly ApplicationDbContext _context;

        public CommentsController(ApplicationDbContext context)
        {
            _context = context;
            // _context = context;
        }

        [HttpPost]
        [Authorize]
        [HttpGet("sql")]
        // [AllowAnonymous] // 允许匿名访问
        public async Task<ActionResult> CreateActivityComment(CommentCreateDto commentDto)

        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            Console.WriteLine("User ID: " + userId);
            Console.WriteLine("Comment DTO: " + commentDto.Content + ", Activity ID: " + commentDto.ActivityId+ ", User name: " + commentDto.UserName);
            if (userId == 0)
            {
                return Unauthorized("用户未认证");
            }

            var activity = await _context.Activities.FindAsync(commentDto.ActivityId);
            if (activity == null)
            {
                return NotFound("活动不存在");
            }

            // var activity = await _context.Comments.CreateActivityComment(CommentCreateDto.ActivityId);
            // var UserName = await _context.Users
            //     .FromSqlRaw("SELECT UserName FROM \"Users\"")
            //     .ToListAsync();
            var comment = new Comment
            {
                Content = commentDto.Content,
                UserId = userId,
                ActivityId = commentDto.ActivityId,
                UserName = commentDto.UserName // 假设 CommentCreateDto 包含 UserName 字段

            };
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return Ok(new { message = "评论成功" });
            // if (activity == null)
            // {
            //     return NotFound("活动不存在");
            // }
        }
    }
}