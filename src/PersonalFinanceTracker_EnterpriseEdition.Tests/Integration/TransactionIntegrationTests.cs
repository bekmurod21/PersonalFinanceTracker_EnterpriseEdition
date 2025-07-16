using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceTracker_EnterpriseEdition.Application.Abstractions;
using PersonalFinanceTracker_EnterpriseEdition.Application.DTOs.Transactions;
using PersonalFinanceTracker_EnterpriseEdition.Application.Services;
using PersonalFinanceTracker_EnterpriseEdition.Domain.Entities;
using PersonalFinanceTracker_EnterpriseEdition.Infrastructure.Persistense;
using Xunit;

namespace PersonalFinanceTracker_EnterpriseEdition.Tests.Integration;

public class TransactionIntegrationTests
{
    private ApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task Transaction_CRUD_And_Summary_Integration()
    {
        // Arrange
        var db = CreateDbContext();
        var transactionRepo = new Repository<Transaction>(db);
        var categoryRepo = new Repository<Category>(db);
        var auditLogService = new Moq.Mock<IAuditLogService>().Object;
        var cache = new Moq.Mock<ICacheService>().Object;
        var userId = Guid.NewGuid();
        var category = new Category { Id = Guid.NewGuid(), Name = "TestCat", Color = "#fff", UserId = userId };
        db.Categories.Add(category);
        db.SaveChanges();
        var service = new TransactionService(transactionRepo, categoryRepo, auditLogService, cache);

        // Create
        var createDto = new CreateTransactionDto { Amount = 100, Type = Domain.Enums.TransactionType.Income, CategoryId = category.Id, Note = "Test" };
        var created = await service.CreateAsync(userId,createDto);
        Assert.Equal(100, created.Amount);
        Assert.Equal(category.Id, created.Category.Id);

        // Update
        db.Entry(db.Transactions.First()).Reload();
        var dbTransaction = db.Transactions.Find(created.Id)!;
        var updateDto = new UpdateTransactionDto { 
            Amount = 150, 
            Type = Domain.Enums.TransactionType.Income, 
            CategoryId = category.Id, 
            Note = "Updated", 
            RowVersion = dbTransaction.RowVersion ?? new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 } 
        };
        var updated = await service.UpdateAsync(userId, created.Id, updateDto);
        Assert.Equal(150, updated.Amount);
        Assert.Equal("Updated", updated.Note);

        // Summary
        var now = DateTime.UtcNow;
        var summary = await service.GetMonthlySummaryAsync(userId, now.Year, now.Month);
        Assert.Equal(150, summary.TotalIncome);
        Assert.Equal(0, summary.TotalExpense);
        Assert.Equal(150, summary.Balance);

        // Delete
        var deleted = await service.DeleteAsync(userId,created.Id);
        Assert.True(deleted);
    }
} 