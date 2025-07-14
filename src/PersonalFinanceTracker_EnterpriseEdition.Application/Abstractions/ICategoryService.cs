using PersonalFinanceTracker_EnterpriseEdition.Application.DTOs.Categories;

namespace PersonalFinanceTracker_EnterpriseEdition.Application.Abstractions;

public interface ICategoryService
{
    Task<GetCategoryDto> CreateAsync(CreateCategoryDto dto, Guid userId);
    Task<GetCategoryDto> UpdateAsync(Guid id, UpdateCategoryDto dto, Guid userId);
    Task<bool> DeleteAsync(Guid id, Guid userId);
    Task<GetCategoryDto> GetByIdAsync(Guid id, Guid userId);
    Task<List<GetCategoryDto>> GetAllAsync(Guid userId);
} 