using WorkTrack.Domain.Common;
using WorkTrack.Domain.Enums;

namespace WorkTrack.Domain.Entities;

public sealed class User : BaseEntity
{
	public string FullName { get; private set; } = string.Empty;

	public string Email { get; private set; } = string.Empty;

	public string PasswordHash { get; private set; } = string.Empty;

	public UserRole Role { get; private set; }

	public bool IsActive { get; private set; }

	private User()
	{
	}

	public User(
		string fullName,
		string email,
		string passwordHash,
		UserRole role)
	{
		if (string.IsNullOrWhiteSpace(fullName))
		{
			throw new ArgumentException("Full name is required.", nameof(fullName));
		}

		if (string.IsNullOrWhiteSpace(email))
		{
			throw new ArgumentException("Email is required.", nameof(email));
		}

		if (string.IsNullOrWhiteSpace(passwordHash))
		{
			throw new ArgumentException("Password hash is required.", nameof(passwordHash));
		}

		FullName = fullName.Trim();
		Email = NormalizeEmail(email);
		PasswordHash = passwordHash;
		Role = role;
		IsActive = true;
	}

	public void UpdateProfile(string fullName)
	{
		if (string.IsNullOrWhiteSpace(fullName))
		{
			throw new ArgumentException("Full name is required.", nameof(fullName));
		}

		FullName = fullName.Trim();
		MarkAsUpdated();
	}

	public void ChangeRole(UserRole role)
	{
		if (Role == role)
		{
			return;
		}

		Role = role;
		MarkAsUpdated();
	}

	public void ChangePasswordHash(string passwordHash)
	{
		if (string.IsNullOrWhiteSpace(passwordHash))
		{
			throw new ArgumentException("Password hash is required.", nameof(passwordHash));
		}

		PasswordHash = passwordHash;
		MarkAsUpdated();
	}

	public void Deactivate()
	{
		if (!IsActive)
		{
			return;
		}

		IsActive = false;
		MarkAsUpdated();
	}

	public void Reactivate()
	{
		if (IsActive)
		{
			return;
		}

		IsActive = true;
		MarkAsUpdated();
	}

	private static string NormalizeEmail(string email)
	{
		return email.Trim().ToLowerInvariant();
	}
}