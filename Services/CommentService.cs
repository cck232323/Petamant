// using AutoMapper;
// using Microsoft.EntityFrameworkCore;
// using MyDotnetApp.Data;
// using MyDotnetApp.DTOs;
// using MyDotnetApp.Models;
// namespace MyDotnetApp.Services
// {
//     public class CommentService : ICommentService
//     {
//         private readonly ApplicationDbContext _context;
//         private readonly IMapper _mapper;


//         public CommentService(ApplicationDbContext context, IMapper mapper)
//         {
//             _context = context;
//             _mapper = mapper;
//         }


//         //         }
//         public async Task<IEnumerable<CommentDto>> CreateActivityCommentsAsync(int commentId)
//         {
//             var comments = await _context.Comments
//                 .Include(a => a.Content)
//                 .Include(a => a.User)

//                 .ToListAsync();
//             return _mapper.Map<IEnumerable<CommentDto>>(comments);
//         }
//     }
// }
