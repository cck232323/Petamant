using MyDotnetApp.DTOs;
using MyDotnetApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyDotnetApp.Services
{
    public interface ICommentService
    {
        // 保留这一个方法，删除重复的定义
        Task<IEnumerable<CommentDto>> GetActivityCommentsAsync(int activityId);
        
        Task<CommentDto> CreateCommentAsync(CommentCreateDto commentDto);
        Task<CommentDto> CreateActivityCommentsAsync(CommentCreateDto commentDto, int activityId);
        Task<CommentDto> GetCommentByIdAsync(int commentId);
        Task DeleteCommentAsync(int commentId, int userId);
    }
}