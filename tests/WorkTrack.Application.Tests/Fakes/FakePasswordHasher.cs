using WorkTrack.Application.Common.Interfaces;

namespace WorkTrack.Application.Tests.Fakes;

internal sealed class FakePasswordHasher
	: IPasswordHasher
{
	public string Hash(string password)
	{
		return $"hashed::{password}";
	}

	public bool Verify(
		string passwordHash,
		string providedPassword)
	{
		return passwordHash ==
			   $"hashed::{providedPassword}";
	}
}