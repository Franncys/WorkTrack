using WorkTrack.Application.Common.Models;

namespace WorkTrack.Application.Projects;

public interface IProjectService
{
	Task<Result<ProjectDto>> CreateAsync(
		CreateProjectRequest request,
		CancellationToken cancellationToken = default);
}