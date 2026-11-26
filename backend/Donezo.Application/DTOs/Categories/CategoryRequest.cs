using System.ComponentModel.DataAnnotations;

namespace Donezo.Application.DTOs.Categories;

public class CategoryRequest
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;
}