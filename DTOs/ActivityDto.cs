using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyDotnetApp.DTOs
{
    public class ActivityDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Location { get; set; } = string.Empty;
        public int CreatorUserId { get; set; }
        public int CreatorId { get; set; }
        public string CreatorUserName { get; set; } = string.Empty;
        // 移除 public DateTime CreatedAt { get; set; }
        public int RegistrationsCount { get; set; }
        public List<RegistrationDto>? Registrations { get; set; }
        public UserDto Creator { get; set; } = null!;
        // public List<CommentDto>? Comments { get; set; } = new List<CommentDto>();
        public ICollection<CommentDto> Comments { get; set; } = new List<CommentDto>();
    }

    // public class ActivityCreateDto
    // {
    //     public string Title { get; set; } = string.Empty;
    //     public string Description { get; set; } = string.Empty;
    //     public DateTime Date { get; set; }
    //     public string Location { get; set; } = string.Empty;
    // }

    public class CreateActivityDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        public DateTime Date { get; set; }
        
        [Required]
        public string Location { get; set; } = string.Empty;
    }
}