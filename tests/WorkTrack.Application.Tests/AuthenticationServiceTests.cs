using Microsoft.EntityFrameworkCore;
using WorkTrack.Application.Authentication;
using WorkTrack.Application.Common.Models;
using WorkTrack.Application.Tests.Fakes;
using WorkTrack.Domain.Enums;

namespace WorkTrack.Application.Tests;

public sealed class AuthenticationServiceTests
{
	[Fact]
	public async Task RegisterAsync_ShouldCreateDeveloperUser()
	{
		// Arrange
		await using var dbContext = CreateDbContext();

		var service = new AuthenticationService(
			dbContext,
			new FakePasswordHasher());

		var request = new RegisterRequest(
			"Francisco Matos",
			"Francisco@Email.COM",
			"Password1");

		// Act
		var result = await service.RegisterAsync(request);

		// Assert
		Assert.True(result.IsSuccess);
		Assert.NotNull(result.Value);

		Assert.Equal(
			"Francisco Matos",
			result.Value.FullName);

		Assert.Equal(
			"francisco@email.com",
			result.Value.Email);

		Assert.Equal(
			UserRole.Developer,
			result.Value.Role);

		var savedUser = await dbContext.Users.SingleAsync();

		Assert.Equal(
			"hashed::Password1",
			savedUser.PasswordHash);
	}

	[Fact]
	public async Task RegisterAsync_ShouldReturnConflict_WhenEmailExists()
	{
		// Arrange
		await using var dbContext = CreateDbContext();

		var service = new AuthenticationService(
			dbContext,
			new FakePasswordHasher());

		await service.RegisterAsync(
			new RegisterRequest(
				"First User",
				"user@email.com",
				"Password1"));

		// Act
		var result = await service.RegisterAsync(
			new RegisterRequest(
				"Second User",
				"USER@EMAIL.COM",
				"Password2"));

		// Assert
		Assert.False(result.IsSuccess);
		Assert.NotNull(result.Error);

		Assert.Equal(
			ErrorType.Conflict,
			result.Error.Type);

		Assert.Equal(
			1,
			await dbContext.Users.CountAsync());
	}

	[Fact]
	public async Task RegisterAsync_ShouldReturnValidationError_WhenPasswordIsWeak()
	{
		// Arrange
		await using var dbContext = CreateDbContext();

		var service = new AuthenticationService(
			dbContext,
			new FakePasswordHasher());

		var request = new RegisterRequest(
			"Francisco Matos",
			"francisco@email.com",
			"password");

		// Act
		var result = await service.RegisterAsync(request);

		// Assert
		Assert.False(result.IsSuccess);
		Assert.NotNull(result.Error);

		Assert.Equal(
			ErrorType.Validation,
			result.Error.Type);

		Assert.Empty(dbContext.Users);
	}

	[Fact]
	public async Task RegisterAsync_ShouldReturnValidationError_WhenEmailIsInvalid()
	{
		// Arrange
		await using var dbContext = CreateDbContext();

		var service = new AuthenticationService(
			dbContext,
			new FakePasswordHasher());

		var request = new RegisterRequest(
			"Francisco Matos",
			"not-an-email",
			"Password1");

		// Act
		var result = await service.RegisterAsync(request);

		// Assert
		Assert.False(result.IsSuccess);
		Assert.NotNull(result.Error);

		Assert.Equal(
			ErrorType.Validation,
			result.Error.Type);

		Assert.Empty(dbContext.Users);
	}

	private static TestApplicationDbContext CreateDbContext()
	{
		var options =
			new DbContextOptionsBuilder<TestApplicationDbContext>()
				.UseInMemoryDatabase(
					Guid.NewGuid().ToString())
				.Options;

		return new TestApplicationDbContext(options);
	}
}