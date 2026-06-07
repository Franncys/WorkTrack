using WorkTrack.Domain.Entities;
using WorkTrack.Domain.Enums;

namespace WorkTrack.Domain.Tests;

public sealed class UserTests
{
	[Fact]
	public void Constructor_ShouldCreateActiveUser()
	{
		// Act
		var user = new User(
			"Francisco Matos",
			"francisco@email.com",
			"hashed-password",
			UserRole.Developer);

		// Assert
		Assert.Equal("Francisco Matos", user.FullName);
		Assert.Equal("francisco@email.com", user.Email);
		Assert.Equal("hashed-password", user.PasswordHash);
		Assert.Equal(UserRole.Developer, user.Role);
		Assert.True(user.IsActive);
	}

	[Fact]
	public void Constructor_ShouldNormalizeEmail()
	{
		// Act
		var user = new User(
			"Francisco Matos",
			" Francisco@Email.COM ",
			"hashed-password",
			UserRole.Developer);

		// Assert
		Assert.Equal("francisco@email.com", user.Email);
	}

	[Fact]
	public void Constructor_ShouldThrow_WhenEmailIsEmpty()
	{
		// Act
		var action = () => new User(
			"Francisco Matos",
			"",
			"hashed-password",
			UserRole.Developer);

		// Assert
		Assert.Throws<ArgumentException>(action);
	}

	[Fact]
	public void ChangeRole_ShouldUpdateUserRole()
	{
		// Arrange
		var user = CreateUser();

		// Act
		user.ChangeRole(UserRole.Manager);

		// Assert
		Assert.Equal(UserRole.Manager, user.Role);
	}

	[Fact]
	public void Deactivate_ShouldMakeUserInactive()
	{
		// Arrange
		var user = CreateUser();

		// Act
		user.Deactivate();

		// Assert
		Assert.False(user.IsActive);
	}

	[Fact]
	public void Reactivate_ShouldMakeUserActive()
	{
		// Arrange
		var user = CreateUser();
		user.Deactivate();

		// Act
		user.Reactivate();

		// Assert
		Assert.True(user.IsActive);
	}

	private static User CreateUser()
	{
		return new User(
			"Francisco Matos",
			"francisco@email.com",
			"hashed-password",
			UserRole.Developer);
	}
}