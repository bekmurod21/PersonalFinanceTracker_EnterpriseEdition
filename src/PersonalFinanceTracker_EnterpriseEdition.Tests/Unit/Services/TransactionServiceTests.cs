using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using PersonalFinanceTracker_EnterpriseEdition.Application.Abstractions;
using PersonalFinanceTracker_EnterpriseEdition.Application.DTOs.Transactions;
using PersonalFinanceTracker_EnterpriseEdition.Application.Services;
using PersonalFinanceTracker_EnterpriseEdition.Domain.Entities;
using Xunit;

namespace PersonalFinanceTracker_EnterpriseEdition.Tests;

public class TransactionServiceTests
{
    private readonly Mock<IRepository<Transaction>> _transactionRepoMock = new();
    private readonly Mock<IRepository<Category>> _categoryRepoMock = new();
    private readonly Mock<IAuditLogService> _auditLogServiceMock = new();
    private readonly IMemoryCache _memoryCache = new MemoryCache(new MemoryCacheOptions());

    [Fact]
    public async Task CreateAsync_ShouldReturnCreatedTransactionDto()
    {
        // Arrange
        var service = new TransactionService(_transactionRepoMock.Object, _categoryRepoMock.Object, _auditLogServiceMock.Object, _memoryCache);
        var userId = Guid.NewGuid();
        var dto = new CreateTransactionDto { Amount = 100, Type = Domain.Enums.TransactionType.Income, CategoryId = Guid.NewGuid(), Note = "Test" };
        _transactionRepoMock.Setup(r => r.AddAsync(It.IsAny<Transaction>())).ReturnsAsync((Transaction t) => t);
        _transactionRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
        _categoryRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Category { Id = dto.CategoryId, Name = "TestCat" });

        // Act
        var result = await service.CreateAsync(dto, userId);

        // Assert
        Assert.Equal(dto.Amount, result.Amount);
        Assert.Equal(dto.Type, result.Type);
        Assert.Equal(dto.CategoryId, result.CategoryId);
        Assert.Equal("TestCat", result.CategoryName);
    }

    [Fact]
    public async Task GetMonthlySummaryAsync_ShouldReturnCorrectSummary()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var year = DateTime.UtcNow.Year;
        var month = DateTime.UtcNow.Month;
        var transactions = new List<Transaction>
        {
            new Transaction { UserId = userId, CreatedAt = new DateTime(year, month, 1), Amount = 200, Type = Domain.Enums.TransactionType.Income },
            new Transaction { UserId = userId, CreatedAt = new DateTime(year, month, 2), Amount = 50, Type = Domain.Enums.TransactionType.Expense }
        };
        _transactionRepoMock.Setup(r => r.Query(It.IsAny<System.Linq.Expressions.Expression<Func<Transaction, bool>>>()))
            .Returns((System.Linq.Expressions.Expression<Func<Transaction, bool>> pred) => transactions.AsQueryable().Where(pred));
        var service = new TransactionService(_transactionRepoMock.Object, _categoryRepoMock.Object, _auditLogServiceMock.Object, _memoryCache);

        // Act
        var summary = await service.GetMonthlySummaryAsync(userId, year, month);

        // Assert
        Assert.Equal(200, summary.TotalIncome);
        Assert.Equal(50, summary.TotalExpense);
        Assert.Equal(150, summary.Balance);
    }

    [Fact]
    public async Task GetMonthlyTrendAsync_ShouldReturnCorrectTrend()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var transactions = new List<Transaction>
        {
            new Transaction { UserId = userId, CreatedAt = now.AddMonths(-1), Amount = 100, Type = Domain.Enums.TransactionType.Income },
            new Transaction { UserId = userId, CreatedAt = now.AddMonths(-1), Amount = 40, Type = Domain.Enums.TransactionType.Expense },
            new Transaction { UserId = userId, CreatedAt = now, Amount = 200, Type = Domain.Enums.TransactionType.Income },
            new Transaction { UserId = userId, CreatedAt = now, Amount = 50, Type = Domain.Enums.TransactionType.Expense }
        };
        _transactionRepoMock.Setup(r => r.Query(It.IsAny<System.Linq.Expressions.Expression<Func<Transaction, bool>>>()))
            .Returns((System.Linq.Expressions.Expression<Func<Transaction, bool>> pred) => transactions.AsQueryable().Where(pred));
        var service = new TransactionService(_transactionRepoMock.Object, _categoryRepoMock.Object, _auditLogServiceMock.Object, _memoryCache);

        // Act
        var trend = await service.GetMonthlyTrendAsync(userId, 2);

        // Assert
        Assert.Equal(2, trend.Count);
        Assert.Contains(trend, t => t.Income == 100 && t.Expense == 40);
        Assert.Contains(trend, t => t.Income == 200 && t.Expense == 50);
    }
} 