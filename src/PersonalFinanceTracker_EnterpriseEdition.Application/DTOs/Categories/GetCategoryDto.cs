using System;

namespace PersonalFinanceTracker_EnterpriseEdition.Application.DTOs.Categories;

public class GetCategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Color { get; set; } = default!;
} 