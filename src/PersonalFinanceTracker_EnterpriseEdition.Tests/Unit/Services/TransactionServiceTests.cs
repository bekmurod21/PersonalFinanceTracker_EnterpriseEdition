using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using PersonalFinanceTracker_EnterpriseEdition.Application.Abstractions;
using PersonalFinanceTracker_EnterpriseEdition.Application.DTOs.Transactions;
using PersonalFinanceTracker_EnterpriseEdition.Application.Services;
using PersonalFinanceTracker_EnterpriseEdition.Domain.Entities;
using Xunit;
using MockQueryable.Moq;

namespace PersonalFinanceTracker_EnterpriseEdition.Tests.Unit.Services;

public class TransactionServiceTests
{
    private readonly Mock<IRepository<Transaction>> _transactionRepoMock = new();
    private readonly Mock<IRepository<Category>> _categoryRepoMock = new();
    private readonly Mock<IAuditLogService> _auditLogServiceMock = new();
    private readonly IDistributedCache _distributedCache = new MemoryDistributedCache(new Microsoft.Extensions.Options.OptionsWrapper<MemoryDistributedCacheOptions>(new MemoryDistributedCacheOptions()));

    [Fact]
    public async Task CreateAsync_ShouldReturnCreatedTransactionDto()
    {
        // Arrange
        var service = new TransactionService(_transactionRepoMock.Object, _categoryRepoMock.Object, _auditLogServiceMock.Object, _distributedCache);
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var dto = new CreateTransactionDto { Amount = 100, Type = Domain.Enums.TransactionType.Income, CategoryId = categoryId, Note = "Test" };
        _transactionRepoMock.Setup(r => r.AddAsync(It.IsAny<Transaction>())).ReturnsAsync((Transaction t) => t);
        _transactionRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
        _categoryRepoMock.Setup(r => r.GetByIdAsync(categoryId, null)).ReturnsAsync(new Category { Id = categoryId, Name = "TestCat", UserId = userId });

        // Act
        var result = await service.CreateAsync(userId, dto);

        // Assert
        Assert.Equal(dto.Amount, result.Amount);
        Assert.Equal(dto.Type, result.Type);
        Assert.Equal(dto.CategoryId, result.Category.Id);
        Assert.Equal("TestCat", result.Category.Name);
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
        _transactionRepoMock.Setup(r => r.Query(It.IsAny<Expression<Func<Transaction, bool>>>(), null))
            .Returns((Expression<Func<Transaction, bool>> pred, string[] includes) => transactions.AsQueryable().Where(pred).BuildMockDbSet().Object);
        var service = new TransactionService(_transactionRepoMock.Object, _categoryRepoMock.Object, _auditLogServiceMock.Object, _distributedCache);

        // Act
        var summary = await service.GetMonthlySummaryAsync(userId,year, month);

        // Assert
        Assert.Equal(200, summary.TotalIncome);
        Assert.Equal(50, summary.TotalExpense);
        Assert.Equal(150, summary.Balance);
    }

    [Fact]
    public async Task GetMonthlyTrendAsync_ShouldReturnCorrectTrend()
    {
        // Arrange
        var userId = Guid.NewGuid(); // har safar unique
        var now = DateTime.UtcNow;
        var transactions = new List<Transaction>
        {
            new Transaction { UserId = userId, CreatedAt = now.AddMonths(-1), Amount = 100, Type = Domain.Enums.TransactionType.Income },
            new Transaction { UserId = userId, CreatedAt = now.AddMonths(-1), Amount = 40, Type = Domain.Enums.TransactionType.Expense },
            new Transaction { UserId = userId, CreatedAt = now, Amount = 200, Type = Domain.Enums.TransactionType.Income },
            new Transaction { UserId = userId, CreatedAt = now, Amount = 50, Type = Domain.Enums.TransactionType.Expense }
        };
        _transactionRepoMock.Setup(r => r.Query(It.IsAny<Expression<Func<Transaction, bool>>>(), null))
            .Returns((Expression<Func<Transaction, bool>> pred, string[] includes) => transactions.AsQueryable().Where(pred).BuildMockDbSet().Object);
        var distributedCache = new MemoryDistributedCache(new Microsoft.Extensions.Options.OptionsWrapper<MemoryDistributedCacheOptions>(new MemoryDistributedCacheOptions()));
        var service = new TransactionService(_transactionRepoMock.Object, _categoryRepoMock.Object, _auditLogServiceMock.Object, distributedCache);

        // Act
        var trend = await service.GetMonthlyTrendAsync(userId,2);

        // Assert
        Assert.Equal(2, trend.Count);
        Assert.Contains(trend, t => t.Income == 100 && t.Expense == 40);
        Assert.Contains(trend, t => t.Income == 200 && t.Expense == 50);
    }
} 