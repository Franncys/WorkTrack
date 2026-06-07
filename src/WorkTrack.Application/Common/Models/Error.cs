namespace WorkTrack.Application.Common.Models;

public sealed record Error(
	string Code,
	string Message,
	ErrorType Type);