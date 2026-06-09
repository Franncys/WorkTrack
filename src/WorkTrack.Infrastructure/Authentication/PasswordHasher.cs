using Microsoft.AspNetCore.Identity;
using WorkTrack.Application.Common.Interfaces;

namespace WorkTrack.Infrastructure.Authentication;

internal sealed class PasswordHasher : IPasswordHasher
{
	private readonly PasswordHasher<object> _passwordHasher = new();

	public string Hash(string password)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(password);

		return _passwordHasher.HashPassword(
			new object(),
			password);
	}

	public bool Verify(
		string passwordHash,
		string providedPassword)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);
		ArgumentException.ThrowIfNullOrWhiteSpace(providedPassword);

		var result = _passwordHasher.VerifyHashedPassword(
			new object(),
			passwordHash,
			providedPassword);

		return result is
			PasswordVerificationResult.Success or
			PasswordVerificationResult.SuccessRehashNeeded;
	}
}