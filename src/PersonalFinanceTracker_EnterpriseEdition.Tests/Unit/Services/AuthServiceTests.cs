using System;
using System.Threading.Tasks;
using Moq;
using Microsoft.Extensions.Configuration;
using PersonalFinanceTracker_EnterpriseEdition.Application.Abstractions;
using PersonalFinanceTracker_EnterpriseEdition.Application.DTOs.Users;
using PersonalFinanceTracker_EnterpriseEdition.Domain.Entities;
using PersonalFinanceTracker_EnterpriseEdition.Infrastructure.Services;
using Xunit;

namespace PersonalFinanceTracker_EnterpriseEdition.Tests;

public class AuthServiceTests
{
    private readonly Mock<IRepository<User>> _userRepoMock = new();
    private readonly Mock<IConfiguration> _configMock = new();

    [Fact]
    public async Task SignUpAsync_ShouldReturnCreatedUser()
    {
        // Arrange
        var dto = new SignUpDto { FirstName = "Ali", LastName = "Valiyev", Email = "ali@mail.com", UserName = "ali", Password = "12345" };
        _userRepoMock.Setup(r => r.Query(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .Returns((System.Linq.Expressions.Expression<Func<User, bool>> pred) => new User[0].AsQueryable());
        _userRepoMock.Setup(r => r.AddAsync(It.IsAny<User>())).ReturnsAsync((User u) => u);
        _userRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
        var service = new AuthService(_configMock.Object, _userRepoMock.Object);

        // Act
        var user = await service.SignUpAsync(dto);

        // Assert
        Assert.Equal(dto.Email, user.Email);
        Assert.Equal(dto.UserName, user.UserName);
        Assert.Equal(dto.FirstName, user.FirstName);
        Assert.Equal(dto.LastName, user.LastName);
        _userRepoMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
    }
} 