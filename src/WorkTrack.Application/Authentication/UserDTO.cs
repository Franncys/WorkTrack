using WorkTrack.Domain.Enums;

namespace WorkTrack.Application.Authentication;

public sealed record UserDto(
	Guid Id,
	string FullName,
	string Email,
	UserRole Role,
	bool IsActive,
	DateTime CreatedAtUtc);