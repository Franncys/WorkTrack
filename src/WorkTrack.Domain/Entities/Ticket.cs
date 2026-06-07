using WorkTrack.Domain.Common;
using WorkTrack.Domain.Enums;

namespace WorkTrack.Domain.Entities;

public sealed class Ticket : BaseEntity
{
	private readonly List<TicketComment> _comments = [];
	private readonly List<TicketAuditEntry> _auditEntries = [];

	public Guid ProjectId { get; private set; }

	public Project? Project { get; private set; }

	public string Title { get; private set; } = string.Empty;

	public string? Description { get; private set; }

	public TicketStatus Status { get; private set; }

	public TicketPriority Priority { get; private set; }

	public Guid CreatedByUserId { get; private set; }

	public User? CreatedByUser { get; private set; }

	public Guid? AssignedToUserId { get; private set; }

	public User? AssignedToUser { get; private set; }

	public IReadOnlyCollection<TicketComment> Comments =>
		_comments.AsReadOnly();

	public IReadOnlyCollection<TicketAuditEntry> AuditEntries =>
		_auditEntries.AsReadOnly();

	private Ticket()
	{
	}

	public Ticket(
		Guid projectId,
		string title,
		string? description,
		TicketPriority priority,
		Guid createdByUserId)
	{
		if (projectId == Guid.Empty)
		{
			throw new ArgumentException(
				"Project id is required.",
				nameof(projectId));
		}

		ValidateTitle(title);

		if (createdByUserId == Guid.Empty)
		{
			throw new ArgumentException(
				"Created by user id is required.",
				nameof(createdByUserId));
		}

		ProjectId = projectId;
		Title = title.Trim();
		Description = NormalizeOptionalText(description);
		Priority = priority;
		Status = TicketStatus.Open;
		CreatedByUserId = createdByUserId;

		AddAuditEntry(
			createdByUserId,
			TicketAuditAction.Created,
			null,
			Title);
	}

	public void UpdateDetails(
		string title,
		string? description,
		Guid performedByUserId)
	{
		ValidateUserId(performedByUserId);
		ValidateTitle(title);

		var normalizedTitle = title.Trim();
		var normalizedDescription = NormalizeOptionalText(description);

		if (Title == normalizedTitle &&
			Description == normalizedDescription)
		{
			return;
		}

		var oldValue = $"Title: {Title}; Description: {Description}";
		var newValue =
			$"Title: {normalizedTitle}; Description: {normalizedDescription}";

		Title = normalizedTitle;
		Description = normalizedDescription;

		MarkAsUpdated();

		AddAuditEntry(
			performedByUserId,
			TicketAuditAction.DetailsUpdated,
			oldValue,
			newValue);
	}

	public void ChangeStatus(
		TicketStatus status,
		Guid performedByUserId)
	{
		ValidateUserId(performedByUserId);

		if (Status == status)
		{
			return;
		}

		var previousStatus = Status;

		Status = status;
		MarkAsUpdated();

		AddAuditEntry(
			performedByUserId,
			TicketAuditAction.StatusChanged,
			previousStatus.ToString(),
			status.ToString());
	}

	public void ChangePriority(
		TicketPriority priority,
		Guid performedByUserId)
	{
		ValidateUserId(performedByUserId);

		if (Priority == priority)
		{
			return;
		}

		var previousPriority = Priority;

		Priority = priority;
		MarkAsUpdated();

		AddAuditEntry(
			performedByUserId,
			TicketAuditAction.PriorityChanged,
			previousPriority.ToString(),
			priority.ToString());
	}

	public void AssignTo(
		Guid userId,
		Guid performedByUserId)
	{
		ValidateUserId(userId);
		ValidateUserId(performedByUserId);

		if (AssignedToUserId == userId)
		{
			return;
		}

		var previousAssignee = AssignedToUserId;

		AssignedToUserId = userId;
		MarkAsUpdated();

		AddAuditEntry(
			performedByUserId,
			TicketAuditAction.Assigned,
			previousAssignee?.ToString(),
			userId.ToString());
	}

	public void Unassign(Guid performedByUserId)
	{
		ValidateUserId(performedByUserId);

		if (AssignedToUserId is null)
		{
			return;
		}

		var previousAssignee = AssignedToUserId;

		AssignedToUserId = null;
		MarkAsUpdated();

		AddAuditEntry(
			performedByUserId,
			TicketAuditAction.Unassigned,
			previousAssignee.ToString(),
			null);
	}

	public TicketComment AddComment(
		Guid authorId,
		string content)
	{
		ValidateUserId(authorId);

		var comment = new TicketComment(
			Id,
			authorId,
			content);

		_comments.Add(comment);

		return comment;
	}

	private void AddAuditEntry(
		Guid performedByUserId,
		TicketAuditAction action,
		string? oldValue,
		string? newValue)
	{
		var entry = new TicketAuditEntry(
			Id,
			performedByUserId,
			action,
			oldValue,
			newValue);

		_auditEntries.Add(entry);
	}

	private static void ValidateTitle(string title)
	{
		if (string.IsNullOrWhiteSpace(title))
		{
			throw new ArgumentException(
				"Ticket title is required.",
				nameof(title));
		}
	}

	private static void ValidateUserId(Guid userId)
	{
		if (userId == Guid.Empty)
		{
			throw new ArgumentException(
				"User id is required.",
				nameof(userId));
		}
	}

	private static string? NormalizeOptionalText(string? value)
	{
		return string.IsNullOrWhiteSpace(value)
			? null
			: value.Trim();
	}
}