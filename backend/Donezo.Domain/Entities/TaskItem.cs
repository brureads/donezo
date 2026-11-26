using Donezo.Domain.Enums;
using TaskStatus = Donezo.Domain.Enums.TaskStatus;

namespace Donezo.Domain.Entities;

public class TaskItem
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public TaskPriority Priority { get; set; } = TaskPriority.Medium;

    public TaskStatus Status { get; set; } = TaskStatus.Pending;

    public DateTime? DueDate { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid UserId { get; set; }

    public User User { get; set; } = null!;

    public Guid? CategoryId { get; set; }

    public Category? Category { get; set; }

    public bool IsOverdue =>
        DueDate.HasValue &&
        DueDate.Value < DateTime.UtcNow &&
        Status != TaskStatus.Completed;
}