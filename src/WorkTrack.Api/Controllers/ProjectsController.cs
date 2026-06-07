using Microsoft.AspNetCore.Mvc;
using WorkTrack.Application.Common.Models;
using WorkTrack.Application.Projects;

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
	public async Task<IActionResult> Create(
		CreateProjectRequest request,
		CancellationToken cancellationToken)
	{
		var result = await _projectService.CreateAsync(
			request,
			cancellationToken);

		if (!result.IsSuccess)
		{
			return MapError(result.Error!);
		}

		var project = result.Value!;

		return CreatedAtAction(
			nameof(GetById),
			new { id = project.Id },
			project);
	}

	[HttpGet("{id:guid}")]
	[ApiExplorerSettings(IgnoreApi = true)]
	public IActionResult GetById(Guid id)
	{
		return NotFound();
	}

	private IActionResult MapError(Error error)
	{
		var problemDetails = new ProblemDetails
		{
			Title = GetTitle(error.Type),
			Detail = error.Message,
			Status = GetStatusCode(error.Type),
			Instance = HttpContext.Request.Path
		};

		problemDetails.Extensions["code"] = error.Code;

		return StatusCode(
			problemDetails.Status.Value,
			problemDetails);
	}

	private static int GetStatusCode(ErrorType errorType)
	{
		return errorType switch
		{
			ErrorType.Validation =>
				StatusCodes.Status400BadRequest,

			ErrorType.NotFound =>
				StatusCodes.Status404NotFound,

			ErrorType.Conflict =>
				StatusCodes.Status409Conflict,

			_ => StatusCodes.Status500InternalServerError
		};
	}

	private static string GetTitle(ErrorType errorType)
	{
		return errorType switch
		{
			ErrorType.Validation =>
				"Validation error",

			ErrorType.NotFound =>
				"Resource not found",

			ErrorType.Conflict =>
				"Resource conflict",

			_ => "Unexpected error"
		};
	}
}