namespace WorkTrack.Application.Projects;

public sealed record ProjectDto(
	Guid Id,
	string Name,
	string? Description,
	DateTime CreatedAtUtc,
	DateTime? UpdatedAtUtc);