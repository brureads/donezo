using Donezo.Application.DTOs.Tasks;
using Donezo.Application.Interfaces;
using Donezo.Domain.Entities;
using Donezo.Domain.Enums;
using Donezo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using TaskStatus = Donezo.Domain.Enums.TaskStatus;

namespace Donezo.Infrastructure.Services;

public class TaskService : ITaskService
{
    private readonly DonezoDbContext _context;

    public TaskService(DonezoDbContext context)
    {
        _context = context;
    }

    public async Task<List<TaskResponse>> GetAllAsync(
        Guid userId,
        TaskStatus? status,
        TaskPriority? priority,
        Guid? categoryId)
    {
        var query = _context.Tasks
            .AsNoTracking()
            .Where(t => t.UserId == userId);

        if (status.HasValue)
            query = query.Where(t => t.Status == status.Value);

        if (priority.HasValue)
            query = query.Where(t => t.Priority == priority.Value);

        if (categoryId.HasValue)
            query = query.Where(t => t.CategoryId == categoryId.Value);

        var tasks = await query
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        return tasks.Select(ToResponse).ToList();
    }

    public async Task<TaskResponse> GetByIdAsync(
        Guid userId, Guid id)
    {
        return ToResponse(await FindAsync(userId, id));
    }

    public async Task<TaskResponse> CreateAsync(
        Guid userId, TaskRequest request)
    {
        ValidateRequest(request);

        await ValidateCategoryAsync(
            userId, request.CategoryId);

        var task = new TaskItem
        {
            Title = request.Title.Trim(),
            Description = request.Description?.Trim(),
            Priority = request.Priority,
            Status = TaskStatus.Pending,
            DueDate = NormalizeDate(request.DueDate),
            CategoryId = request.CategoryId,
            UserId = userId
        };

        _context.Tasks.Add(task);

        await _context.SaveChangesAsync();

        return ToResponse(task);
    }

    public async Task<TaskResponse> UpdateAsync(
        Guid userId, Guid id, TaskRequest request)
    {
        ValidateRequest(request);

        var task = await FindAsync(userId, id);

        await ValidateCategoryAsync(
            userId, request.CategoryId);

        task.Title = request.Title.Trim();
        task.Description = request.Description?.Trim();
        task.Priority = request.Priority;
        task.Status = request.Status;
        task.DueDate = NormalizeDate(request.DueDate);
        task.CategoryId = request.CategoryId;

        await _context.SaveChangesAsync();

        return ToResponse(task);
    }

    public async Task DeleteAsync(Guid userId, Guid id)
    {
        var task = await FindAsync(userId, id);

        _context.Tasks.Remove(task);

        await _context.SaveChangesAsync();
    }

    private async Task<TaskItem> FindAsync(
        Guid userId, Guid id)
    {
        return await _context.Tasks
            .FirstOrDefaultAsync(t =>
                t.Id == id && t.UserId == userId)
            ?? throw new KeyNotFoundException(
                "Tarefa não encontrada.");
    }

    private async Task ValidateCategoryAsync(
        Guid userId, Guid? categoryId)
    {
        if (!categoryId.HasValue)
            return;

        var exists = await _context.Categories
            .AnyAsync(c =>
                c.Id == categoryId.Value &&
                c.UserId == userId);

        if (!exists)
            throw new ArgumentException(
                "Categoria inválida.");
    }

    private static void ValidateRequest(TaskRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            throw new ArgumentException(
                "O título da tarefa é obrigatório.");

        if (!Enum.IsDefined(request.Priority))
            throw new ArgumentException(
                "Prioridade inválida.");

        if (!Enum.IsDefined(request.Status))
            throw new ArgumentException(
                "Status inválido.");
    }

    private static DateTime? NormalizeDate(DateTime? date)
    {
        if (!date.HasValue)
            return null;

        return date.Value.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(date.Value, DateTimeKind.Utc)
            : date.Value.ToUniversalTime();
    }

    private static TaskResponse ToResponse(TaskItem task)
    {
        return new TaskResponse
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Priority = task.Priority,
            Status = task.Status,
            DueDate = task.DueDate,
            CreatedAt = task.CreatedAt,
            CategoryId = task.CategoryId,
            IsOverdue = task.IsOverdue
        };
    }
}