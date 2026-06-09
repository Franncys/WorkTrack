using Microsoft.EntityFrameworkCore;
using WorkTrack.Application.Common.Interfaces;
using WorkTrack.Domain.Entities;

namespace WorkTrack.Application.Tests;

internal sealed class TestApplicationDbContext
	: DbContext, IApplicationDbContext
{
	public TestApplicationDbContext(
		DbContextOptions<TestApplicationDbContext> options)
		: base(options)
	{
	}

	public DbSet<Project> Projects => Set<Project>();

	public DbSet<User> Users => Set<User>();
}