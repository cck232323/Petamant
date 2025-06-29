using AutoMapper;
using MyDotnetApp.DTOs; // 确保只引用 DTOs 命名空间
using MyDotnetApp.Models;

namespace MyDotnetApp.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // 用户相关映射
            CreateMap<MyDotnetApp.DTOs.UserRegisterDto, User>(); // 明确指定命名空间
            CreateMap<User, MyDotnetApp.DTOs.UserDto>(); // 明确指定命名空间

            // 活动相关映射
            CreateMap<CreateActivityDto, Activity>();
            CreateMap<Activity, ActivityDto>()
                .ForMember(dest => dest.CreatorUserName, opt => opt.MapFrom(src => src.CreatorUser.UserName))
                .ForMember(dest => dest.RegistrationsCount, opt => opt.MapFrom(src => src.Registrations.Count))
                .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments)); 

            // 注册相关映射
            CreateMap<Registration, RegistrationDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName));

            // 评论相关映射
            CreateMap<CommentCreateDto, Comment>();

            CreateMap<Comment, CommentDto>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));
                // .ForMember(dest => dest.ActivityId, opt => opt.MapFrom(src => src.ActivityId));
                // .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                // .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                // .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content));
                
                
                
        }
    }
}