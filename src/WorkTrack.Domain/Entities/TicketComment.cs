using WorkTrack.Domain.Common;

namespace WorkTrack.Domain.Entities;

public sealed class TicketComment : BaseEntity
{
	public Guid TicketId { get; private set; }

	public Ticket? Ticket { get; private set; }

	public Guid AuthorId { get; private set; }

	public User? Author { get; private set; }

	public string Content { get; private set; } = string.Empty;

	private TicketComment()
	{
	}

	public TicketComment(
		Guid ticketId,
		Guid authorId,
		string content)
	{
		if (ticketId == Guid.Empty)
		{
			throw new ArgumentException(
				"Ticket id is required.",
				nameof(ticketId));
		}

		if (authorId == Guid.Empty)
		{
			throw new ArgumentException(
				"Author id is required.",
				nameof(authorId));
		}

		if (string.IsNullOrWhiteSpace(content))
		{
			throw new ArgumentException(
				"Comment content is required.",
				nameof(content));
		}

		TicketId = ticketId;
		AuthorId = authorId;
		Content = content.Trim();
	}

	public void UpdateContent(string content)
	{
		if (string.IsNullOrWhiteSpace(content))
		{
			throw new ArgumentException(
				"Comment content is required.",
				nameof(content));
		}

		Content = content.Trim();
		MarkAsUpdated();
	}
}