using Microsoft.AspNetCore.Mvc;
using WorkTrack.Application.Common.Models;

namespace WorkTrack.Api.Extensions;

public static class ErrorMappingExtensions
{
	public static ObjectResult ToProblemDetails(
		this Error error,
		HttpContext httpContext)
	{
		var statusCode = error.Type switch
		{
			ErrorType.Validation =>
				StatusCodes.Status400BadRequest,

			ErrorType.NotFound =>
				StatusCodes.Status404NotFound,

			ErrorType.Conflict =>
				StatusCodes.Status409Conflict,

			_ => StatusCodes.Status500InternalServerError
		};

		var title = error.Type switch
		{
			ErrorType.Validation =>
				"Validation error",

			ErrorType.NotFound =>
				"Resource not found",

			ErrorType.Conflict =>
				"Resource conflict",

			_ => "Unexpected error"
		};

		var problemDetails = new ProblemDetails
		{
			Title = title,
			Detail = error.Message,
			Status = statusCode,
			Instance = httpContext.Request.Path
		};

		problemDetails.Extensions["code"] = error.Code;

		return new ObjectResult(problemDetails)
		{
			StatusCode = statusCode
		};
	}
}