using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkTrack.Domain.Entities;

namespace WorkTrack.Infrastructure.Persistence.Configurations;

internal sealed class TicketAuditEntryConfiguration
	: IEntityTypeConfiguration<TicketAuditEntry>
{
	public void Configure(
		EntityTypeBuilder<TicketAuditEntry> builder)
	{
		builder.ToTable("ticket_audit_entries");

		builder.HasKey(entry => entry.Id);

		builder.Property(entry => entry.Id)
			.ValueGeneratedNever();

		builder.Property(entry => entry.Action)
			.HasConversion<string>()
			.HasMaxLength(50)
			.IsRequired();

		builder.Property(entry => entry.OldValue)
			.HasMaxLength(4000);

		builder.Property(entry => entry.NewValue)
			.HasMaxLength(4000);

		builder.Property(entry => entry.CreatedAtUtc)
			.IsRequired();

		builder.Property(entry => entry.UpdatedAtUtc);

		builder.HasIndex(entry => entry.TicketId);

		builder.HasIndex(entry => entry.PerformedByUserId);

		builder.HasOne(entry => entry.PerformedByUser)
			.WithMany()
			.HasForeignKey(entry => entry.PerformedByUserId)
			.OnDelete(DeleteBehavior.Restrict);
	}
}