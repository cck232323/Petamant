using AutoMapper;
using MyDotnetApp.DTOs;
using MyDotnetApp.Models;

namespace MyDotnetApp.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            // User 映射
            CreateMap<User, DTOs.UserDto>();
            // CreateMap<UserRegisterDto, User>();
            CreateMap<Activity, DTOs.ActivityDto>();
            CreateMap<Registration, DTOs.RegistrationDto>();

            // Activity 映射
            CreateMap<Activity, DTOs.ActivityDto>()
                .ForMember(dest => dest.CreatorUserName, opt => opt.MapFrom(src => src.CreatorUser.UserName))
                .ForMember(dest => dest.RegistrationsCount, opt => opt.MapFrom(src => src.Registrations.Count))
                // 移除或注释掉 CreatedAt 的映射
                // .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.CreatorId, opt => opt.MapFrom(src => src.CreatorUserId));
            CreateMap<CreateActivityDto, Activity>();

            // Registration 映射
            CreateMap<Registration, DTOs.RegistrationDto>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));
            CreateMap<RegistrationCreateDto, Registration>();

            // Comment 映射
            CreateMap<Comment, CommentDto>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));
            CreateMap<CommentCreateDto, Comment>();
        }
    }
}