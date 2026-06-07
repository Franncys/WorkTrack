
using WorkTrack.Domain.Common;

namespace WorkTrack.Domain.Entities;

public sealed class Project : BaseEntity
{
	public string Name { get; private set; } = string.Empty;

	public string? Description { get; private set; }

	public IReadOnlyCollection<Ticket> Tickets => _tickets.AsReadOnly();

	private readonly List<Ticket> _tickets = [];

	private Project()
	{
	}

	public Project(string name, string? description)
	{
		if (string.IsNullOrWhiteSpace(name))
		{
			throw new ArgumentException("Project name is required.", nameof(name));
		}

		Name = name.Trim();
		Description = description?.Trim();
	}

	public void UpdateDetails(string name, string? description)
	{
		if (string.IsNullOrWhiteSpace(name))
		{
			throw new ArgumentException("Project name is required.", nameof(name));
		}

		Name = name.Trim();
		Description = description?.Trim();

		MarkAsUpdated();
	}
}