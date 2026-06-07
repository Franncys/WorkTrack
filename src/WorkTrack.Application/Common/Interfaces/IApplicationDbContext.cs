using Microsoft.EntityFrameworkCore;
using WorkTrack.Domain.Entities;

namespace WorkTrack.Application.Common.Interfaces;

public interface IApplicationDbContext
{
	DbSet<Project> Projects { get; }

	//DbSet<User> Users { get; }
	//DbSet<Ticket> Tickets { get; }
	//DbSet<TicketComment> TicketComments { get; }
	//DbSet<TicketAuditEntry> TicketAuditEntries { get; }

	Task<int> SaveChangesAsync(
		CancellationToken cancellationToken = default);
}