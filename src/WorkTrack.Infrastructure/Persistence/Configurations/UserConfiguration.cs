using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkTrack.Domain.Entities;

namespace WorkTrack.Infrastructure.Persistence.Configurations;

internal sealed class UserConfiguration
	: IEntityTypeConfiguration<User>
{
	public void Configure(EntityTypeBuilder<User> builder)
	{
		builder.ToTable("users");

		builder.HasKey(user => user.Id);

		builder.Property(user => user.Id)
			.ValueGeneratedNever();

		builder.Property(user => user.FullName)
			.HasMaxLength(150)
			.IsRequired();

		builder.Property(user => user.Email)
			.HasMaxLength(320)
			.IsRequired();

		builder.Property(user => user.PasswordHash)
			.HasMaxLength(500)
			.IsRequired();

		builder.Property(user => user.Role)
			.HasConversion<string>()
			.HasMaxLength(30)
			.IsRequired();

		builder.Property(user => user.IsActive)
			.IsRequired();

		builder.Property(user => user.CreatedAtUtc)
			.IsRequired();

		builder.Property(user => user.UpdatedAtUtc);

		builder.HasIndex(user => user.Email)
			.IsUnique();
	}
}