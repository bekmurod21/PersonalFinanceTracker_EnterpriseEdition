using System;
using System.Threading.Tasks;
using Moq;
using Microsoft.Extensions.Configuration;
using PersonalFinanceTracker_EnterpriseEdition.Application.Abstractions;
using PersonalFinanceTracker_EnterpriseEdition.Application.DTOs.Users;
using PersonalFinanceTracker_EnterpriseEdition.Domain.Entities;
using PersonalFinanceTracker_EnterpriseEdition.Infrastructure.Services;
using Xunit;
using System.Linq;
using PersonalFinanceTracker_EnterpriseEdition.Application.Helpers;
using System.Linq.Expressions;
using MockQueryable.Moq;

namespace PersonalFinanceTracker_EnterpriseEdition.Tests.Unit.Services;

public class AuthServiceTests
{
    private readonly Mock<IRepository<User>> _userRepoMock = new();
    private readonly Mock<IConfiguration> _configMock = new();

    [Fact]
    public async Task SignUpAsync_ShouldReturnCreatedUser()
    {
        // Arrange
        var dto = new SignUpDto { Email = "ali@mail.com", Password = "12345" };
        _userRepoMock.Setup(r => r.Query(It.IsAny<Expression<Func<User, bool>>>(), null))
            .Returns((Expression<Func<User, bool>> pred, string[] includes) => (new User[0]).AsQueryable().BuildMockDbSet().Object);
        _userRepoMock.Setup(r => r.AddAsync(It.IsAny<User>())).ReturnsAsync((User u) => u);
        _userRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
        var service = new AuthService(_configMock.Object, _userRepoMock.Object);

        // Act
        var user = await service.SignUpAsync(dto);

        // Assert
        Assert.Equal(dto.Email, user.Email);
        Assert.Equal(dto.Username, user.Username);
        _userRepoMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task SignUpAsync_ShouldThrowCustomException_WhenUserExists()
    {
        // Arrange
        var dto = new SignUpDto { Email = "ali@mail.com", Username = "ali", Password = "12345" };
        _userRepoMock.Setup(r => r.Query(It.IsAny<Expression<Func<User, bool>>>(), null))
            .Returns((Expression<Func<User, bool>> pred, string[] includes) => (new[] { new User() }).AsQueryable().BuildMockDbSet().Object);
        var service = new AuthService(_configMock.Object, _userRepoMock.Object);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<PersonalFinanceTracker_EnterpriseEdition.Domain.Exceptions.CustomException>(() => service.SignUpAsync(dto));
        Assert.Equal(409, ex.StatusCode);
        Assert.Equal("User already exists", ex.Message);
    }

    [Fact]
    public async Task SignInAsync_ShouldThrowCustomException_WhenUserNotFound()
    {
        // Arrange
        var dto = new SignInDto { EmailOrUserName = "notfound", Password = "12345" };
        _userRepoMock.Setup(r => r.Query(It.IsAny<Expression<Func<User, bool>>>(), null))
            .Returns((Expression<Func<User, bool>> pred, string[] includes) => (new User[0]).AsQueryable().BuildMockDbSet().Object);
        var service = new AuthService(_configMock.Object, _userRepoMock.Object);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<PersonalFinanceTracker_EnterpriseEdition.Domain.Exceptions.CustomException>(() => service.SignInAsync(dto));
        Assert.Equal(404, ex.StatusCode);
        Assert.Equal("User not found", ex.Message);
    }

    [Fact]
    public async Task SignInAsync_ShouldThrowCustomException_WhenPasswordIncorrect()
    {
        // Arrange
        var dto = new SignInDto { EmailOrUserName = "ali", Password = "wrong" };
        var user = new User { Email = "ali@mail.com", Username = "ali", Password = "12345".Hash() };
        _userRepoMock.Setup(r => r.Query(It.IsAny<Expression<Func<User, bool>>>(), null))
            .Returns((Expression<Func<User, bool>> pred, string[] includes) => (new[] { user }).AsQueryable().BuildMockDbSet().Object);
        var service = new AuthService(_configMock.Object, _userRepoMock.Object);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<PersonalFinanceTracker_EnterpriseEdition.Domain.Exceptions.CustomException>(() => service.SignInAsync(dto));
        Assert.Equal(401, ex.StatusCode);
        Assert.Equal("Password is incorrect", ex.Message);
    }
} 