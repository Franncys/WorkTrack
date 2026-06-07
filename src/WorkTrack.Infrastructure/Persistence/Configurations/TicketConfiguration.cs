using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkTrack.Domain.Entities;

namespace WorkTrack.Infrastructure.Persistence.Configurations;

internal sealed class TicketConfiguration
	: IEntityTypeConfiguration<Ticket>
{
	public void Configure(EntityTypeBuilder<Ticket> builder)
	{
		builder.ToTable("tickets");

		builder.HasKey(ticket => ticket.Id);

		builder.Property(ticket => ticket.Id)
			.ValueGeneratedNever();

		builder.Property(ticket => ticket.Title)
			.HasMaxLength(200)
			.IsRequired();

		builder.Property(ticket => ticket.Description)
			.HasMaxLength(5000);

		builder.Property(ticket => ticket.Status)
			.HasConversion<string>()
			.HasMaxLength(30)
			.IsRequired();

		builder.Property(ticket => ticket.Priority)
			.HasConversion<string>()
			.HasMaxLength(30)
			.IsRequired();

		builder.Property(ticket => ticket.CreatedAtUtc)
			.IsRequired();

		builder.Property(ticket => ticket.UpdatedAtUtc);

		builder.HasIndex(ticket => ticket.ProjectId);

		builder.HasIndex(ticket => ticket.Status);

		builder.HasIndex(ticket => ticket.Priority);

		builder.HasIndex(ticket => ticket.AssignedToUserId);

		builder.HasOne(ticket => ticket.CreatedByUser)
			.WithMany()
			.HasForeignKey(ticket => ticket.CreatedByUserId)
			.OnDelete(DeleteBehavior.Restrict);

		builder.HasOne(ticket => ticket.AssignedToUser)
			.WithMany()
			.HasForeignKey(ticket => ticket.AssignedToUserId)
			.OnDelete(DeleteBehavior.SetNull);

		builder.HasMany(ticket => ticket.Comments)
			.WithOne(comment => comment.Ticket)
			.HasForeignKey(comment => comment.TicketId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasMany(ticket => ticket.AuditEntries)
			.WithOne(entry => entry.Ticket)
			.HasForeignKey(entry => entry.TicketId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}