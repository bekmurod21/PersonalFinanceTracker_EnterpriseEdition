using PersonalFinanceTracker_EnterpriseEdition.Application.Abstractions;
using PersonalFinanceTracker_EnterpriseEdition.Application.DTOs.Categories;
using PersonalFinanceTracker_EnterpriseEdition.Domain.Entities;

namespace PersonalFinanceTracker_EnterpriseEdition.Application.Services;

public class CategoryService(IRepository<Category> categoryRepository, IAuditLogService auditLogService) : ICategoryService
{
    private readonly IRepository<Category> _categoryRepository = categoryRepository;
    private readonly IAuditLogService _auditLogService = auditLogService;

    public async Task<GetCategoryDto> CreateAsync(CreateCategoryDto dto, Guid userId)
    {
        var category = new Category
        {
            Name = dto.Name,
            Color = dto.Color,
            UserId = userId
        };
        await _categoryRepository.AddAsync(category);
        await _categoryRepository.SaveChangesAsync();
        await _auditLogService.LogCreateAsync(nameof(Category), category.Id, userId, category);
        return new GetCategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Color = category.Color
        };
    }

    public async Task<GetCategoryDto> UpdateAsync(Guid id, UpdateCategoryDto dto, Guid userId)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null || category.UserId != userId) throw new Exception("Category not found");
        var oldValue = new { category.Name, category.Color };
        category.Name = dto.Name;
        category.Color = dto.Color;
        await _categoryRepository.UpdateAsync(category);
        await _categoryRepository.SaveChangesAsync();
        await _auditLogService.LogUpdateAsync(nameof(Category), category.Id, userId, oldValue, category);
        return new GetCategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Color = category.Color
        };
    }

    public async Task<bool> DeleteAsync(Guid id, Guid userId)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null || category.UserId != userId) return false;
        var oldValue = new { category.Name, category.Color };
        await _categoryRepository.DeleteAsync(id);
        await _categoryRepository.SaveChangesAsync();
        await _auditLogService.LogDeleteAsync(nameof(Category), category.Id, userId, oldValue);
        return true;
    }

    public async Task<GetCategoryDto> GetByIdAsync(Guid id, Guid userId)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null || category.UserId != userId) return null;
        return new GetCategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Color = category.Color
        };
    }

    public async Task<List<GetCategoryDto>> GetAllAsync(Guid userId)
    {
        var categories = await _categoryRepository.GetAllAsync(c => c.UserId == userId);
        return categories.Select(c => new GetCategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            Color = c.Color
        }).ToList();
    }
} 