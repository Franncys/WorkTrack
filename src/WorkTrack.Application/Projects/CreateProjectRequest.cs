namespace WorkTrack.Application.Projects;

public sealed record CreateProjectRequest(
	string Name,
	string? Description);