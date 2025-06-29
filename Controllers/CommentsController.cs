using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyDotnetApp.DTOs;
using Microsoft.EntityFrameworkCore;
using MyDotnetApp.Data;
using MyDotnetApp.Models;
using System.Security.Claims;
using MyDotnetApp.Services;

namespace MyDotnetApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // [AllowAnonymous]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly ApplicationDbContext _context; // 添加 _context 字段

        public CommentsController(ICommentService commentService, ApplicationDbContext context)
        {
            _commentService = commentService;
            _context = context; // 初始化 _context
        }

        [HttpGet("activity/{activityId}")]
        public async Task<ActionResult<IEnumerable<CommentDto>>> GetActivityComments(int activityId)
        {
            var comments = await _commentService.GetActivityCommentsAsync(activityId);
            return Ok(comments);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<CommentDto>> CreateComment(CommentCreateDto commentDto)
        {
            var comment = await _commentService.CreateCommentAsync(commentDto);
            return CreatedAtAction(nameof(GetActivityComments), new { activityId = comment.ActivityId }, comment);
        }

        [HttpPost("sql")] // 修改为 HttpPost，因为这是创建操作
        [Authorize]
        public async Task<ActionResult> CreateActivityCommentLegacy(CommentCreateDto commentDto)
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

            var comment = new Comment
            {
                Content = commentDto.Content,
                UserId = userId,
                ActivityId = commentDto.ActivityId,
                UserName = commentDto.UserName,
                // 添加对楼中楼功能的支持
                ParentCommentId = commentDto.ParentCommentId,
                ReplyToUserId = commentDto.ReplyToUserId,
                ReplyToUserName = commentDto.ReplyToUserName
            };
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return Ok(new { message = "评论成功" });
        }
    }
}