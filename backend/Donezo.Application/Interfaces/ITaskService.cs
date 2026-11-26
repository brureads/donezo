using Donezo.Application.DTOs.Tasks;
using Donezo.Domain.Enums;
using TaskStatus = Donezo.Domain.Enums.TaskStatus;

namespace Donezo.Application.Interfaces;

public interface ITaskService
{
    Task<List<TaskResponse>> GetAllAsync(
        Guid userId,
        TaskStatus? status,
        TaskPriority? priority,
        Guid? categoryId);

    Task<TaskResponse> GetByIdAsync(
        Guid userId, Guid id);

    Task<TaskResponse> CreateAsync(
        Guid userId, TaskRequest request);

    Task<TaskResponse> UpdateAsync(
        Guid userId, Guid id, TaskRequest request);

    Task DeleteAsync(Guid userId, Guid id);
}