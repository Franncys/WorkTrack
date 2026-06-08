using WorkTrack.Application.Common.Models;

namespace WorkTrack.Application.Projects;

public interface IProjectService
{
	Task<Result<ProjectDto>> CreateAsync(
		CreateProjectRequest request,
		CancellationToken cancellationToken = default);

	Task<IReadOnlyCollection<ProjectDto>> GetAllAsync(
		CancellationToken cancellationToken = default);

	Task<Result<ProjectDto>> GetByIdAsync(
		Guid id,
		CancellationToken cancellationToken = default);

	Task<Result<ProjectDto>> UpdateAsync(
		Guid id,
		UpdateProjectRequest request,
		CancellationToken cancellationToken = default);

	Task<Result<bool>> DeleteAsync(
		Guid id,
		CancellationToken cancellationToken = default);
}