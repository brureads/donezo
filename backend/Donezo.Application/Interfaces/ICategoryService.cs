using Donezo.Application.DTOs.Categories;

namespace Donezo.Application.Interfaces;

public interface ICategoryService
{
    Task<List<CategoryResponse>> GetAllAsync(Guid userId);

    Task<CategoryResponse> GetByIdAsync(Guid userId, Guid id);

    Task<CategoryResponse> CreateAsync(
        Guid userId, CategoryRequest request);

    Task<CategoryResponse> UpdateAsync(
        Guid userId, Guid id, CategoryRequest request);

    Task DeleteAsync(Guid userId, Guid id);
}