using System.ComponentModel.DataAnnotations;
using Donezo.Domain.Enums;
using TaskStatus = Donezo.Domain.Enums.TaskStatus;

namespace Donezo.Application.DTOs.Tasks;

public class TaskRequest
{
    [Required]
    [StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [EnumDataType(typeof(TaskPriority))]
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;

    [EnumDataType(typeof(TaskStatus))]
    public TaskStatus Status { get; set; } = TaskStatus.Pending;

    public DateTime? DueDate { get; set; }

    public Guid? CategoryId { get; set; }
}