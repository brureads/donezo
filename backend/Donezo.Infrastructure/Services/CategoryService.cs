using Donezo.Application.DTOs.Categories;
using Donezo.Application.Interfaces;
using Donezo.Domain.Entities;
using Donezo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Donezo.Infrastructure.Services;

public class CategoryService : ICategoryService
{
    private readonly DonezoDbContext _context;

    public CategoryService(DonezoDbContext context)
    {
        _context = context;
    }

    public async Task<List<CategoryResponse>> GetAllAsync(
        Guid userId)
    {
        return await _context.Categories
            .AsNoTracking()
            .Where(c => c.UserId == userId)
            .OrderBy(c => c.Name)
            .Select(c => new CategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                CreatedAt = c.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<CategoryResponse> GetByIdAsync(
        Guid userId, Guid id)
    {
        var category = await FindAsync(userId, id);

        return ToResponse(category);
    }

    public async Task<CategoryResponse> CreateAsync(
        Guid userId, CategoryRequest request)
    {
        var category = new Category
        {
            Name = ValidateName(request.Name),
            UserId = userId
        };

        _context.Categories.Add(category);

        await _context.SaveChangesAsync();

        return ToResponse(category);
    }

    public async Task<CategoryResponse> UpdateAsync(
        Guid userId, Guid id, CategoryRequest request)
    {
        var category = await FindAsync(userId, id);

        category.Name = ValidateName(request.Name);

        await _context.SaveChangesAsync();

        return ToResponse(category);
    }

    public async Task DeleteAsync(Guid userId, Guid id)
    {
        var category = await FindAsync(userId, id);

        _context.Categories.Remove(category);

        await _context.SaveChangesAsync();
    }

    private async Task<Category> FindAsync(
        Guid userId, Guid id)
    {
        return await _context.Categories
            .FirstOrDefaultAsync(c =>
                c.Id == id && c.UserId == userId)
            ?? throw new KeyNotFoundException(
                "Categoria não encontrada.");
    }

    private static string ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "O nome da categoria é obrigatório.");

        return name.Trim();
    }

    private static CategoryResponse ToResponse(
        Category category)
    {
        return new CategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            CreatedAt = category.CreatedAt
        };
    }
}