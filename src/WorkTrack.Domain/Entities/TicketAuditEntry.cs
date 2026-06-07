using WorkTrack.Domain.Common;
using WorkTrack.Domain.Enums;

namespace WorkTrack.Domain.Entities;

public sealed class TicketAuditEntry : BaseEntity
{
	public Guid TicketId { get; private set; }

	public Ticket? Ticket { get; private set; }

	public Guid PerformedByUserId { get; private set; }

	public User? PerformedByUser { get; private set; }

	public TicketAuditAction Action { get; private set; }

	public string? OldValue { get; private set; }

	public string? NewValue { get; private set; }
	private TicketAuditEntry()
	{
	}

	public TicketAuditEntry(
		Guid ticketId,
		Guid performedByUserId,
		TicketAuditAction action,
		string? oldValue,
		string? newValue)
	{
		if (ticketId == Guid.Empty)
		{
			throw new ArgumentException(
				"Ticket id is required.",
				nameof(ticketId));
		}

		if (performedByUserId == Guid.Empty)
		{
			throw new ArgumentException(
				"User id is required.",
				nameof(performedByUserId));
		}

		TicketId = ticketId;
		PerformedByUserId = performedByUserId;
		Action = action;
		OldValue = NormalizeValue(oldValue);
		NewValue = NormalizeValue(newValue);
	}

	private static string? NormalizeValue(string? value)
	{
		return string.IsNullOrWhiteSpace(value)
			? null
			: value.Trim();
	}
}