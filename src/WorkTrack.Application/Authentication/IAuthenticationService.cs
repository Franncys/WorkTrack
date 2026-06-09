using WorkTrack.Application.Common.Models;

namespace WorkTrack.Application.Authentication;

public interface IAuthenticationService
{
	Task<Result<UserDto>> RegisterAsync(
		RegisterRequest request,
		CancellationToken cancellationToken = default);
}