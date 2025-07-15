using System;
using System.Threading.Tasks;
using Moq;
using PersonalFinanceTracker_EnterpriseEdition.Application.Abstractions;
using PersonalFinanceTracker_EnterpriseEdition.Application.DTOs.Categories;
using PersonalFinanceTracker_EnterpriseEdition.Application.Services;
using PersonalFinanceTracker_EnterpriseEdition.Domain.Entities;
using Xunit;

namespace PersonalFinanceTracker_EnterpriseEdition.Tests.Unit.Services;

public class CategoryServiceTests
{
    private readonly Mock<IRepository<Category>> _categoryRepoMock = new();
    private readonly Mock<IAuditLogService> _auditLogServiceMock = new();

    [Fact]
    public async Task CreateAsync_ShouldReturnCreatedCategoryDto()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dto = new CreateCategoryDto { Name = "TestCat", Color = "#fff" };
        _categoryRepoMock.Setup(r => r.AddAsync(It.IsAny<Category>())).ReturnsAsync((Category c) => c);
        _categoryRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
        var service = new CategoryService(_categoryRepoMock.Object, _auditLogServiceMock.Object);

        // Act
        var result = await service.CreateAsync(dto, userId);

        // Assert
        Assert.Equal(dto.Name, result.Name);
        Assert.Equal(dto.Color, result.Color);
        _categoryRepoMock.Verify(r => r.AddAsync(It.IsAny<Category>()), Times.Once);
        _auditLogServiceMock.Verify(a => a.LogCreateAsync(nameof(Category), It.IsAny<Guid>(), userId, It.IsAny<object>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowCustomException_WhenCategoryNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        _categoryRepoMock.Setup(r => r.GetByIdAsync(categoryId, null)).ReturnsAsync((Category)null);
        var service = new CategoryService(_categoryRepoMock.Object, _auditLogServiceMock.Object);
        var dto = new UpdateCategoryDto { Name = "Test", Color = "#fff" };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<PersonalFinanceTracker_EnterpriseEdition.Domain.Exceptions.CustomException>(() => service.UpdateAsync(categoryId, dto, userId));
        Assert.Equal(404, ex.StatusCode);
        Assert.Equal("Category not found", ex.Message);
    }
} 