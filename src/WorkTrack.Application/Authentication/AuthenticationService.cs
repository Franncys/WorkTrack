using Microsoft.EntityFrameworkCore;
using WorkTrack.Application.Common.Interfaces;
using WorkTrack.Application.Common.Models;
using WorkTrack.Domain.Entities;
using WorkTrack.Domain.Enums;

namespace WorkTrack.Application.Authentication;

internal sealed class AuthenticationService
	: IAuthenticationService
{
	private const int MaximumFullNameLength = 150;
	private const int MaximumEmailLength = 320;
	private const int MinimumPasswordLength = 8;
	private const int MaximumPasswordLength = 128;

	private readonly IApplicationDbContext _dbContext;
	private readonly IPasswordHasher _passwordHasher;

	public AuthenticationService(
		IApplicationDbContext dbContext,
		IPasswordHasher passwordHasher)
	{
		_dbContext = dbContext;
		_passwordHasher = passwordHasher;
	}

	public async Task<Result<UserDto>> RegisterAsync(
		RegisterRequest request,
		CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(request);

		var normalizedFullName = request.FullName?.Trim();
		var normalizedEmail = NormalizeEmail(request.Email);

		var validationError = Validate(
			normalizedFullName,
			normalizedEmail,
			request.Password);

		if (validationError is not null)
		{
			return Result<UserDto>.Failure(validationError);
		}

		var emailAlreadyExists = await _dbContext.Users
			.AsNoTracking()
			.AnyAsync(
				user => user.Email == normalizedEmail,
				cancellationToken);

		if (emailAlreadyExists)
		{
			return Result<UserDto>.Failure(
				new Error(
					"Authentication.EmailAlreadyExists",
					"An account with this email address already exists.",
					ErrorType.Conflict));
		}

		var passwordHash = _passwordHasher.Hash(
			request.Password);

		var user = new User(
			normalizedFullName!,
			normalizedEmail!,
			passwordHash,
			UserRole.Developer);

		_dbContext.Users.Add(user);

		await _dbContext.SaveChangesAsync(
			cancellationToken);

		return Result<UserDto>.Success(
			MapToDto(user));
	}

	private static Error? Validate(
		string? fullName,
		string? email,
		string? password)
	{
		if (string.IsNullOrWhiteSpace(fullName))
		{
			return new Error(
				"Authentication.FullNameRequired",
				"Full name is required.",
				ErrorType.Validation);
		}

		if (fullName.Length > MaximumFullNameLength)
		{
			return new Error(
				"Authentication.FullNameTooLong",
				$"Full name cannot exceed {MaximumFullNameLength} characters.",
				ErrorType.Validation);
		}

		if (string.IsNullOrWhiteSpace(email))
		{
			return new Error(
				"Authentication.EmailRequired",
				"Email is required.",
				ErrorType.Validation);
		}

		if (email.Length > MaximumEmailLength)
		{
			return new Error(
				"Authentication.EmailTooLong",
				$"Email cannot exceed {MaximumEmailLength} characters.",
				ErrorType.Validation);
		}

		if (!IsValidEmail(email))
		{
			return new Error(
				"Authentication.InvalidEmail",
				"The email address is not valid.",
				ErrorType.Validation);
		}

		if (string.IsNullOrWhiteSpace(password))
		{
			return new Error(
				"Authentication.PasswordRequired",
				"Password is required.",
				ErrorType.Validation);
		}

		if (password.Length < MinimumPasswordLength)
		{
			return new Error(
				"Authentication.PasswordTooShort",
				$"Password must contain at least {MinimumPasswordLength} characters.",
				ErrorType.Validation);
		}

		if (password.Length > MaximumPasswordLength)
		{
			return new Error(
				"Authentication.PasswordTooLong",
				$"Password cannot exceed {MaximumPasswordLength} characters.",
				ErrorType.Validation);
		}

		if (!password.Any(char.IsUpper))
		{
			return new Error(
				"Authentication.PasswordRequiresUppercase",
				"Password must contain at least one uppercase letter.",
				ErrorType.Validation);
		}

		if (!password.Any(char.IsLower))
		{
			return new Error(
				"Authentication.PasswordRequiresLowercase",
				"Password must contain at least one lowercase letter.",
				ErrorType.Validation);
		}

		if (!password.Any(char.IsDigit))
		{
			return new Error(
				"Authentication.PasswordRequiresDigit",
				"Password must contain at least one number.",
				ErrorType.Validation);
		}

		return null;
	}

	private static string? NormalizeEmail(string? email)
	{
		return string.IsNullOrWhiteSpace(email)
			? null
			: email.Trim().ToLowerInvariant();
	}

	private static bool IsValidEmail(string email)
	{
		return System.Net.Mail.MailAddress.TryCreate(
			email,
			out var parsedEmail) &&
			string.Equals(
				parsedEmail.Address,
				email,
				StringComparison.OrdinalIgnoreCase);
	}

	private static UserDto MapToDto(User user)
	{
		return new UserDto(
			user.Id,
			user.FullName,
			user.Email,
			user.Role,
			user.IsActive,
			user.CreatedAtUtc);
	}
}