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

		var validationError = Validate(
			normalizedName,
			request.Description);

		if (validationError is not null)
		{
			return Result<ProjectDto>.Failure(validationError);
		}

		var projectAlreadyExists =
			await _dbContext.Projects.AnyAsync(
				project => project.Name.ToLower() ==
						   normalizedName!.ToLower(),
				cancellationToken);

		if (projectAlreadyExists)
		{
			return Result<ProjectDto>.Failure(
				new Error(
					"Projects.NameAlreadyExists",
					"A project with the same name already exists.",
					ErrorType.Conflict));
		}

		var project = new Project(
			normalizedName!,
			request.Description);

		_dbContext.Projects.Add(project);

		await _dbContext.SaveChangesAsync(cancellationToken);

		return Result<ProjectDto>.Success(
			MapToDto(project));
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

		if (description?.Trim().Length >
			MaximumDescriptionLength)
		{
			return new Error(
				"Projects.DescriptionTooLong",
				$"Project description cannot exceed {MaximumDescriptionLength} characters.",
				ErrorType.Validation);
		}

		return null;
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