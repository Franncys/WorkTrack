namespace WorkTrack.Application.Projects;

public sealed record UpdateProjectRequest(
	string Name,
	string? Description);