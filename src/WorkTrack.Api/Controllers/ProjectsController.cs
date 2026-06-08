using Microsoft.AspNetCore.Mvc;
using WorkTrack.Application.Projects;
using WorkTrack.Api.Extensions;

namespace WorkTrack.Api.Controllers;

[ApiController]
[Route("api/projects")]
public sealed class ProjectsController : ControllerBase
{
	private readonly IProjectService _projectService;

	public ProjectsController(
		IProjectService projectService)
	{
		_projectService = projectService;
	}

	[HttpPost]
	[ProducesResponseType<ProjectDto>(
		StatusCodes.Status201Created)]
	[ProducesResponseType<ProblemDetails>(
		StatusCodes.Status400BadRequest)]
	[ProducesResponseType<ProblemDetails>(
		StatusCodes.Status409Conflict)]
	public async Task<ActionResult<ProjectDto>> Create(
		CreateProjectRequest request,
		CancellationToken cancellationToken)
	{
		var result = await _projectService.CreateAsync(
			request,
			cancellationToken);

		if (!result.IsSuccess)
		{
			return result.Error!.ToProblemDetails(HttpContext);
		}

		var project = result.Value!;

		return CreatedAtAction(
			nameof(GetById),
			new { id = project.Id },
			project);
	}

	[HttpGet]
	[ProducesResponseType<IReadOnlyCollection<ProjectDto>>(
		StatusCodes.Status200OK)]
	public async Task<ActionResult<IReadOnlyCollection<ProjectDto>>>
		GetAll(CancellationToken cancellationToken)
	{
		var projects = await _projectService.GetAllAsync(
			cancellationToken);

		return Ok(projects);
	}

	[HttpGet("{id:guid}")]
	[ProducesResponseType<ProjectDto>(
		StatusCodes.Status200OK)]
	[ProducesResponseType<ProblemDetails>(
		StatusCodes.Status404NotFound)]
	public async Task<ActionResult<ProjectDto>> GetById(
		Guid id,
		CancellationToken cancellationToken)
	{
		var result = await _projectService.GetByIdAsync(
			id,
			cancellationToken);

		if (!result.IsSuccess)
		{
			return result.Error!.ToProblemDetails(HttpContext);
		}

		return Ok(result.Value);
	}

	[HttpPut("{id:guid}")]
	[ProducesResponseType<ProjectDto>(
		StatusCodes.Status200OK)]
	[ProducesResponseType<ProblemDetails>(
		StatusCodes.Status400BadRequest)]
	[ProducesResponseType<ProblemDetails>(
		StatusCodes.Status404NotFound)]
	[ProducesResponseType<ProblemDetails>(
		StatusCodes.Status409Conflict)]
	public async Task<ActionResult<ProjectDto>> Update(
		Guid id,
		UpdateProjectRequest request,
		CancellationToken cancellationToken)
	{
		var result = await _projectService.UpdateAsync(
			id,
			request,
			cancellationToken);

		if (!result.IsSuccess)
		{
			return result.Error!.ToProblemDetails(HttpContext);
		}

		return Ok(result.Value);
	}

	[HttpDelete("{id:guid}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType<ProblemDetails>(
		StatusCodes.Status404NotFound)]
	[ProducesResponseType<ProblemDetails>(
		StatusCodes.Status409Conflict)]
	public async Task<IActionResult> Delete(
		Guid id,
		CancellationToken cancellationToken)
	{
		var result = await _projectService.DeleteAsync(
			id,
			cancellationToken);

		if (!result.IsSuccess)
		{
			return result.Error!.ToProblemDetails(HttpContext);
		}

		return NoContent();
	}
}