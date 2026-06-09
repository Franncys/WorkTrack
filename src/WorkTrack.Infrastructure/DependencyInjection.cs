using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WorkTrack.Infrastructure.Persistence;
using WorkTrack.Application.Common.Interfaces;
using WorkTrack.Infrastructure.Authentication;

namespace WorkTrack.Infrastructure;

public static class DependencyInjection
{
	public static IServiceCollection AddInfrastructure(
		this IServiceCollection services,
		IConfiguration configuration)
	{
		var connectionString =
			configuration.GetConnectionString("Database")
			?? throw new InvalidOperationException(
				"The database connection string was not configured.");

		services.AddDbContext<WorkTrackDbContext>(options =>
			options.UseNpgsql(connectionString));

		services.AddScoped<IApplicationDbContext>(
			serviceProvider =>
				serviceProvider.GetRequiredService<WorkTrackDbContext>());

		services.AddScoped<IPasswordHasher, PasswordHasher>();

		return services;
	}
}