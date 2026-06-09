using Microsoft.AspNetCore.Mvc;
using WorkTrack.Api.Extensions;
using WorkTrack.Application.Authentication;

namespace WorkTrack.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthenticationController : ControllerBase
{
	private readonly IAuthenticationService
		_authenticationService;

	public AuthenticationController(
		IAuthenticationService authenticationService)
	{
		_authenticationService = authenticationService;
	}

	[HttpPost("register")]
	[ProducesResponseType<UserDto>(
		StatusCodes.Status201Created)]
	[ProducesResponseType<ProblemDetails>(
		StatusCodes.Status400BadRequest)]
	[ProducesResponseType<ProblemDetails>(
		StatusCodes.Status409Conflict)]
	public async Task<ActionResult<UserDto>> Register(
		RegisterRequest request,
		CancellationToken cancellationToken)
	{
		var result =
			await _authenticationService.RegisterAsync(
				request,
				cancellationToken);

		if (!result.IsSuccess)
		{
			return result.Error!
				.ToProblemDetails(HttpContext);
		}

		return StatusCode(
			StatusCodes.Status201Created,
			result.Value);
	}
}