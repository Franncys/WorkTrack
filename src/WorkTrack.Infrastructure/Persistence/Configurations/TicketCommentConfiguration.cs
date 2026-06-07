using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkTrack.Domain.Entities;

namespace WorkTrack.Infrastructure.Persistence.Configurations;

internal sealed class TicketCommentConfiguration
	: IEntityTypeConfiguration<TicketComment>
{
	public void Configure(
		EntityTypeBuilder<TicketComment> builder)
	{
		builder.ToTable("ticket_comments");

		builder.HasKey(comment => comment.Id);

		builder.Property(comment => comment.Id)
			.ValueGeneratedNever();

		builder.Property(comment => comment.Content)
			.HasMaxLength(4000)
			.IsRequired();

		builder.Property(comment => comment.CreatedAtUtc)
			.IsRequired();

		builder.Property(comment => comment.UpdatedAtUtc);

		builder.HasIndex(comment => comment.TicketId);

		builder.HasIndex(comment => comment.AuthorId);

		builder.HasOne(comment => comment.Author)
			.WithMany()
			.HasForeignKey(comment => comment.AuthorId)
			.OnDelete(DeleteBehavior.Restrict);
	}
}