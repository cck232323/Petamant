using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyDotnetApp.Data;
using MyDotnetApp.DTOs;
using MyDotnetApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyDotnetApp.Services
{
    public class CommentService : ICommentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CommentService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CommentDto>> GetActivityCommentsAsync(int activityId)
        {
            // 只获取顶层评论（没有父评论的评论）
            var comments = await _context.Comments
                .Include(c => c.User)
                .Include(c => c.Replies)
                    .ThenInclude(r => r.User)
                .Where(c => c.ActivityId == activityId && c.ParentCommentId == null)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<CommentDto>>(comments);
        }

        public async Task<CommentDto> CreateCommentAsync(CommentCreateDto commentDto)
        {
            var comment = _mapper.Map<Comment>(commentDto);
            
            // 如果是回复评论，设置回复信息
            if (commentDto.ParentCommentId.HasValue)
            {
                var parentComment = await _context.Comments
                    .FirstOrDefaultAsync(c => c.Id == commentDto.ParentCommentId.Value);
                
                if (parentComment == null)
                {
                    throw new Exception("父评论不存在");
                }
                
                // 确保回复的是顶层评论或二级评论
                comment.ParentCommentId = parentComment.ParentCommentId ?? parentComment.Id;
            }

            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();

            // 重新获取包含用户信息的完整评论
            var createdComment = await _context.Comments
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == comment.Id);

            return _mapper.Map<CommentDto>(createdComment);
        }

        // 修改返回类型为 Task<CommentDto>
        public async Task<CommentDto> CreateActivityCommentsAsync(CommentCreateDto commentDto, int activityId)
        {
            // 检查活动是否存在
            var activity = await _context.Activities.FindAsync(activityId);
            if (activity == null)
            {
                throw new Exception("活动不存在");
            }

            var comment = _mapper.Map<Comment>(commentDto);
            comment.ActivityId = activityId;
            
            // 如果是回复评论，设置回复信息
            if (commentDto.ParentCommentId.HasValue)
            {
                var parentComment = await _context.Comments
                    .FirstOrDefaultAsync(c => c.Id == commentDto.ParentCommentId.Value);
                
                if (parentComment == null)
                {
                    throw new Exception("父评论不存在");
                }
                
                // 确保回复的是顶层评论或二级评论
                comment.ParentCommentId = parentComment.ParentCommentId ?? parentComment.Id;
                comment.ReplyToUserId = commentDto.ReplyToUserId;
                comment.ReplyToUserName = commentDto.ReplyToUserName;
            }

            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();

            // 重新获取包含用户信息的完整评论
            var createdComment = await _context.Comments
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == comment.Id);

            return _mapper.Map<CommentDto>(createdComment);
        }

        public async Task<CommentDto> GetCommentByIdAsync(int commentId)
        {
            var comment = await _context.Comments
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == commentId);
                
            if (comment == null)
            {
                throw new Exception("评论不存在");
            }
            
            return _mapper.Map<CommentDto>(comment);
        }

        public async Task DeleteCommentAsync(int commentId, int userId)
        {
            var comment = await _context.Comments
                .FirstOrDefaultAsync(c => c.Id == commentId && c.UserId == userId);
                
            if (comment == null)
            {
                throw new Exception("评论不存在或您无权删除此评论");
            }
            
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
        }
    }

    // public interface ICommentService
    // {
    //     Task<IEnumerable<CommentDto>> GetActivityCommentsAsync(int activityId);
    //     Task<CommentDto> CreateCommentAsync(CommentCreateDto commentDto);
    //     Task<CommentDto> GetCommentByIdAsync(int commentId);
    //     Task DeleteCommentAsync(int commentId, int userId);
    //     Task<CommentDto> CreateActivityCommentsAsync(CommentCreateDto commentDto, int activityId);
    // }
}