using Microsoft.EntityFrameworkCore;
using WorkTrack.Application.Common.Interfaces;
using WorkTrack.Application.Common.Models;
using WorkTrack.Domain.Entities;

namespace WorkTrack.Application.Projects;

internal sealed class ProjectService : IProjectService
{
	private const int MaximumNameLength = 150;
	private const int MaximumDescriptionLength = 2000;

	private readonly IApplicationDbContext _dbContext;

	public ProjectService(IApplicationDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public async Task<Result<ProjectDto>> CreateAsync(
		CreateProjectRequest request,
		CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(request);

		var normalizedName = request.Name?.Trim();
		var normalizedDescription =
			NormalizeOptionalText(request.Description);

		var validationError = Validate(
			normalizedName,
			normalizedDescription);

		if (validationError is not null)
		{
			return Result<ProjectDto>.Failure(validationError);
		}

		var projectAlreadyExists =
			await ProjectNameExistsAsync(
				normalizedName!,
				excludedProjectId: null,
				cancellationToken);

		if (projectAlreadyExists)
		{
			return ProjectNameConflict();
		}

		var project = new Project(
			normalizedName!,
			normalizedDescription);

		_dbContext.Projects.Add(project);

		await _dbContext.SaveChangesAsync(cancellationToken);

		return Result<ProjectDto>.Success(
			MapToDto(project));
	}

	public async Task<IReadOnlyCollection<ProjectDto>> GetAllAsync(
		CancellationToken cancellationToken = default)
	{
		return await _dbContext.Projects
			.AsNoTracking()
			.OrderBy(project => project.Name)
			.Select(project => new ProjectDto(
				project.Id,
				project.Name,
				project.Description,
				project.CreatedAtUtc,
				project.UpdatedAtUtc))
			.ToListAsync(cancellationToken);
	}

	public async Task<Result<ProjectDto>> GetByIdAsync(
		Guid id,
		CancellationToken cancellationToken = default)
	{
		var project = await _dbContext.Projects
			.AsNoTracking()
			.Where(project => project.Id == id)
			.Select(project => new ProjectDto(
				project.Id,
				project.Name,
				project.Description,
				project.CreatedAtUtc,
				project.UpdatedAtUtc))
			.SingleOrDefaultAsync(cancellationToken);

		if (project is null)
		{
			return ProjectNotFound(id);
		}

		return Result<ProjectDto>.Success(project);
	}

	public async Task<Result<ProjectDto>> UpdateAsync(
		Guid id,
		UpdateProjectRequest request,
		CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(request);

		var normalizedName = request.Name?.Trim();
		var normalizedDescription =
			NormalizeOptionalText(request.Description);

		var validationError = Validate(
			normalizedName,
			normalizedDescription);

		if (validationError is not null)
		{
			return Result<ProjectDto>.Failure(validationError);
		}

		var project = await _dbContext.Projects
			.SingleOrDefaultAsync(
				project => project.Id == id,
				cancellationToken);

		if (project is null)
		{
			return ProjectNotFound(id);
		}

		var projectNameAlreadyExists =
			await ProjectNameExistsAsync(
				normalizedName!,
				id,
				cancellationToken);

		if (projectNameAlreadyExists)
		{
			return ProjectNameConflict();
		}

		project.UpdateDetails(
			normalizedName!,
			normalizedDescription);

		await _dbContext.SaveChangesAsync(cancellationToken);

		return Result<ProjectDto>.Success(
			MapToDto(project));
	}

	public async Task<Result<bool>> DeleteAsync(
		Guid id,
		CancellationToken cancellationToken = default)
	{
		var project = await _dbContext.Projects
			.Include(project => project.Tickets)
			.SingleOrDefaultAsync(
				project => project.Id == id,
				cancellationToken);

		if (project is null)
		{
			return Result<bool>.Failure(
				new Error(
					"Projects.NotFound",
					$"Project '{id}' was not found.",
					ErrorType.NotFound));
		}

		if (project.Tickets.Count > 0)
		{
			return Result<bool>.Failure(
				new Error(
					"Projects.HasTickets",
					"A project containing tickets cannot be deleted.",
					ErrorType.Conflict));
		}

		_dbContext.Projects.Remove(project);

		await _dbContext.SaveChangesAsync(cancellationToken);

		return Result<bool>.Success(true);
	}

	private async Task<bool> ProjectNameExistsAsync(
		string name,
		Guid? excludedProjectId,
		CancellationToken cancellationToken)
	{
		return await _dbContext.Projects
			.AsNoTracking()
			.AnyAsync(
				project =>
					project.Name.ToLower() == name.ToLower() &&
					(!excludedProjectId.HasValue ||
					 project.Id != excludedProjectId.Value),
				cancellationToken);
	}

	private static Error? Validate(
		string? name,
		string? description)
	{
		if (string.IsNullOrWhiteSpace(name))
		{
			return new Error(
				"Projects.NameRequired",
				"Project name is required.",
				ErrorType.Validation);
		}

		if (name.Length > MaximumNameLength)
		{
			return new Error(
				"Projects.NameTooLong",
				$"Project name cannot exceed {MaximumNameLength} characters.",
				ErrorType.Validation);
		}

		if (description?.Length > MaximumDescriptionLength)
		{
			return new Error(
				"Projects.DescriptionTooLong",
				$"Project description cannot exceed {MaximumDescriptionLength} characters.",
				ErrorType.Validation);
		}

		return null;
	}

	private static string? NormalizeOptionalText(string? value)
	{
		return string.IsNullOrWhiteSpace(value)
			? null
			: value.Trim();
	}

	private static Result<ProjectDto> ProjectNotFound(Guid id)
	{
		return Result<ProjectDto>.Failure(
			new Error(
				"Projects.NotFound",
				$"Project '{id}' was not found.",
				ErrorType.NotFound));
	}

	private static Result<ProjectDto> ProjectNameConflict()
	{
		return Result<ProjectDto>.Failure(
			new Error(
				"Projects.NameAlreadyExists",
				"A project with the same name already exists.",
				ErrorType.Conflict));
	}

	private static ProjectDto MapToDto(Project project)
	{
		return new ProjectDto(
			project.Id,
			project.Name,
			project.Description,
			project.CreatedAtUtc,
			project.UpdatedAtUtc);
	}
}