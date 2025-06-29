using MyDotnetApp.DTOs;

namespace MyDotnetApp.Services
{
    public interface ICommentService
    //CreateActivityCommentsAsync
    {
        Task<CommentDto> CreateActivityCommentsAsync(CommentCreateDto commentDto, int userId);
        Task<IEnumerable<CommentDto>> GetActivityCommentsAsync(int activityId);
        // Add other methods if needed
        Task<CommentDto> GetCommentByIdAsync(int commentId);
        Task DeleteCommentAsync(int commentId, int userId);
        
    }
}