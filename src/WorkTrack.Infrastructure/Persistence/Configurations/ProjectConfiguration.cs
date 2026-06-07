using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkTrack.Domain.Entities;

namespace WorkTrack.Infrastructure.Persistence.Configurations;

internal sealed class ProjectConfiguration
	: IEntityTypeConfiguration<Project>
{
	public void Configure(EntityTypeBuilder<Project> builder)
	{
		builder.ToTable("projects");

		builder.HasKey(project => project.Id);

		builder.Property(project => project.Id)
			.ValueGeneratedNever();

		builder.Property(project => project.Name)
			.HasMaxLength(150)
			.IsRequired();

		builder.Property(project => project.Description)
			.HasMaxLength(2000);

		builder.Property(project => project.CreatedAtUtc)
			.IsRequired();

		builder.Property(project => project.UpdatedAtUtc);

		builder.HasIndex(project => project.Name);

		builder.HasMany(project => project.Tickets)
			.WithOne(ticket => ticket.Project)
			.HasForeignKey(ticket => ticket.ProjectId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}