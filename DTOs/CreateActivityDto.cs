using System;
using System.ComponentModel.DataAnnotations;

public class CreateActivityDto
{
    [Required(ErrorMessage = "标题不能为空")]
    public string Title { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "描述不能为空")]
    public string Description { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "日期不能为空")]
    public DateTime Date { get; set; }
    
    [Required(ErrorMessage = "地点不能为空")]
    public string Location { get; set; } = string.Empty;
}