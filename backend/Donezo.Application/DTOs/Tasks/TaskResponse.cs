using Donezo.Domain.Enums;
using TaskStatus = Donezo.Domain.Enums.TaskStatus;

namespace Donezo.Application.DTOs.Tasks;

public class TaskResponse
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public TaskPriority Priority { get; set; }

    public TaskStatus Status { get; set; }

    public DateTime? DueDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CategoryId { get; set; }

    public bool IsOverdue { get; set; }
}