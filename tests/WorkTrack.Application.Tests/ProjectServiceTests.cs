using Microsoft.EntityFrameworkCore;
using WorkTrack.Application.Common.Models;
using WorkTrack.Application.Projects;
using WorkTrack.Domain.Entities;

namespace WorkTrack.Application.Tests;

public sealed class ProjectServiceTests
{
	[Fact]
	public async Task CreateAsync_ShouldCreateProject()
	{
		// Arrange
		await using var dbContext = CreateDbContext();

		var service = new ProjectService(dbContext);

		var request = new CreateProjectRequest(
			"WorkTrack",
			"Internal ticket management system.");

		// Act
		var result = await service.CreateAsync(request);

		// Assert
		Assert.True(result.IsSuccess);
		Assert.NotNull(result.Value);
		Assert.Equal("WorkTrack", result.Value.Name);

		var savedProject =
			await dbContext.Projects.SingleAsync();

		Assert.Equal("WorkTrack", savedProject.Name);
	}

	[Fact]
	public async Task CreateAsync_ShouldReturnValidationError_WhenNameIsEmpty()
	{
		// Arrange
		await using var dbContext = CreateDbContext();

		var service = new ProjectService(dbContext);

		var request = new CreateProjectRequest(
			" ",
			null);

		// Act
		var result = await service.CreateAsync(request);

		// Assert
		Assert.False(result.IsSuccess);
		Assert.NotNull(result.Error);
		Assert.Equal(
			ErrorType.Validation,
			result.Error.Type);

		Assert.Empty(dbContext.Projects);
	}

	[Fact]
	public async Task CreateAsync_ShouldReturnConflict_WhenNameAlreadyExists()
	{
		// Arrange
		await using var dbContext = CreateDbContext();

		var service = new ProjectService(dbContext);

		await service.CreateAsync(
			new CreateProjectRequest(
				"WorkTrack",
				null));

		// Act
		var result = await service.CreateAsync(
			new CreateProjectRequest(
				"worktrack",
				"Duplicate project."));

		// Assert
		Assert.False(result.IsSuccess);
		Assert.NotNull(result.Error);
		Assert.Equal(
			ErrorType.Conflict,
			result.Error.Type);

		Assert.Equal(
			1,
			await dbContext.Projects.CountAsync());
	}

	private static TestApplicationDbContext CreateDbContext()
	{
		var options =
			new DbContextOptionsBuilder<TestApplicationDbContext>()
				.UseInMemoryDatabase(Guid.NewGuid().ToString())
				.Options;

		return new TestApplicationDbContext(options);
	}

	[Fact]
	public async Task GetAllAsync_ShouldReturnProjectsOrderedByName()
	{
		// Arrange
		await using var dbContext = CreateDbContext();

		dbContext.Projects.AddRange(
			new Project("Zulu", null),
			new Project("Alpha", null),
			new Project("Beta", null));

		await dbContext.SaveChangesAsync();

		var service = new ProjectService(dbContext);

		// Act
		var projects = await service.GetAllAsync();

		// Assert
		Assert.Equal(3, projects.Count);

		Assert.Equal(
			["Alpha", "Beta", "Zulu"],
			projects.Select(project => project.Name));
	}

	[Fact]
	public async Task GetByIdAsync_ShouldReturnProject()
	{
		// Arrange
		await using var dbContext = CreateDbContext();

		var project = new Project(
			"WorkTrack",
			"Ticket management application.");

		dbContext.Projects.Add(project);

		await dbContext.SaveChangesAsync();

		var service = new ProjectService(dbContext);

		// Act
		var result = await service.GetByIdAsync(project.Id);

		// Assert
		Assert.True(result.IsSuccess);
		Assert.NotNull(result.Value);
		Assert.Equal(project.Id, result.Value.Id);
		Assert.Equal("WorkTrack", result.Value.Name);
	}

	[Fact]
	public async Task GetByIdAsync_ShouldReturnNotFound_WhenProjectDoesNotExist()
	{
		// Arrange
		await using var dbContext = CreateDbContext();

		var service = new ProjectService(dbContext);

		// Act
		var result = await service.GetByIdAsync(
			Guid.NewGuid());

		// Assert
		Assert.False(result.IsSuccess);
		Assert.NotNull(result.Error);
		Assert.Equal(
			ErrorType.NotFound,
			result.Error.Type);
	}

	[Fact]
	public async Task UpdateAsync_ShouldUpdateProject()
	{
		// Arrange
		await using var dbContext = CreateDbContext();

		var project = new Project(
			"Old Name",
			"Old description.");

		dbContext.Projects.Add(project);

		await dbContext.SaveChangesAsync();

		var service = new ProjectService(dbContext);

		var request = new UpdateProjectRequest(
			"New Name",
			"New description.");

		// Act
		var result = await service.UpdateAsync(
			project.Id,
			request);

		// Assert
		Assert.True(result.IsSuccess);
		Assert.NotNull(result.Value);
		Assert.Equal("New Name", result.Value.Name);
		Assert.Equal(
			"New description.",
			result.Value.Description);
		Assert.NotNull(result.Value.UpdatedAtUtc);
	}

	[Fact]
	public async Task UpdateAsync_ShouldReturnConflict_WhenNameBelongsToAnotherProject()
	{
		// Arrange
		await using var dbContext = CreateDbContext();

		var firstProject = new Project(
			"WorkTrack",
			null);

		var secondProject = new Project(
			"Other Project",
			null);

		dbContext.Projects.AddRange(
			firstProject,
			secondProject);

		await dbContext.SaveChangesAsync();

		var service = new ProjectService(dbContext);

		// Act
		var result = await service.UpdateAsync(
			secondProject.Id,
			new UpdateProjectRequest(
				"worktrack",
				null));

		// Assert
		Assert.False(result.IsSuccess);
		Assert.NotNull(result.Error);
		Assert.Equal(
			ErrorType.Conflict,
			result.Error.Type);
	}

	[Fact]
	public async Task DeleteAsync_ShouldDeleteProject()
	{
		// Arrange
		await using var dbContext = CreateDbContext();

		var project = new Project(
			"Temporary Project",
			null);

		dbContext.Projects.Add(project);

		await dbContext.SaveChangesAsync();

		var service = new ProjectService(dbContext);

		// Act
		var result = await service.DeleteAsync(project.Id);

		// Assert
		Assert.True(result.IsSuccess);

		Assert.False(
			await dbContext.Projects.AnyAsync(
				existingProject =>
					existingProject.Id == project.Id));
	}

	[Fact]
	public async Task DeleteAsync_ShouldReturnNotFound_WhenProjectDoesNotExist()
	{
		// Arrange
		await using var dbContext = CreateDbContext();

		var service = new ProjectService(dbContext);

		// Act
		var result = await service.DeleteAsync(
			Guid.NewGuid());

		// Assert
		Assert.False(result.IsSuccess);
		Assert.NotNull(result.Error);
		Assert.Equal(
			ErrorType.NotFound,
			result.Error.Type);
	}
}