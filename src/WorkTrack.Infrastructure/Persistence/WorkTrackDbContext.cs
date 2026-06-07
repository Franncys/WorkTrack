using Microsoft.EntityFrameworkCore;
using WorkTrack.Domain.Entities;
using WorkTrack.Application.Common.Interfaces;

namespace WorkTrack.Infrastructure.Persistence;

public sealed class WorkTrackDbContext : DbContext, IApplicationDbContext
{
	public DbSet<User> Users => Set<User>();

	public DbSet<Project> Projects => Set<Project>();

	public DbSet<Ticket> Tickets => Set<Ticket>();

	public DbSet<TicketComment> TicketComments =>
		Set<TicketComment>();

	public DbSet<TicketAuditEntry> TicketAuditEntries =>
		Set<TicketAuditEntry>();

	public WorkTrackDbContext(
		DbContextOptions<WorkTrackDbContext> options)
		: base(options)
	{
	}

	protected override void OnModelCreating(
		ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.ApplyConfigurationsFromAssembly(
			typeof(WorkTrackDbContext).Assembly);
	}
}