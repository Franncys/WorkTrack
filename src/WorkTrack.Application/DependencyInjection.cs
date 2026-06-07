using Microsoft.Extensions.DependencyInjection;
using WorkTrack.Application.Projects;

namespace WorkTrack.Application;

public static class DependencyInjection
{
	public static IServiceCollection AddApplication(
		this IServiceCollection services)
	{
		services.AddScoped<IProjectService, ProjectService>();

		return services;
	}
}