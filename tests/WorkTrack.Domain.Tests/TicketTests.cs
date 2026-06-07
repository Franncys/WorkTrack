using WorkTrack.Domain.Entities;
using WorkTrack.Domain.Enums;

namespace WorkTrack.Domain.Tests;

public sealed class TicketTests
{
	[Fact]
	public void Constructor_ShouldCreateTicketWithOpenStatus()
	{
		// Arrange
		var projectId = Guid.NewGuid();
		var createdByUserId = Guid.NewGuid();

		// Act
		var ticket = new Ticket(
			projectId,
			"Fix login bug",
			"Users cannot login with valid credentials.",
			TicketPriority.High,
			createdByUserId);

		// Assert
		Assert.Equal(projectId, ticket.ProjectId);
		Assert.Equal("Fix login bug", ticket.Title);
		Assert.Equal(TicketPriority.High, ticket.Priority);
		Assert.Equal(TicketStatus.Open, ticket.Status);
		Assert.Equal(createdByUserId, ticket.CreatedByUserId);
		Assert.Null(ticket.AssignedToUserId);
	}

	[Fact]
	public void Constructor_ShouldThrow_WhenTitleIsEmpty()
	{
		// Arrange
		var projectId = Guid.NewGuid();
		var createdByUserId = Guid.NewGuid();

		// Act
		var action = () => new Ticket(
			projectId,
			"",
			null,
			TicketPriority.Medium,
			createdByUserId);

		// Assert
		Assert.Throws<ArgumentException>(action);
	}

	[Fact]
	public void AssignTo_ShouldAssignTicketToUser()
	{
		// Arrange
		var ticket = CreateTicket();
		var assignedUserId = Guid.NewGuid();
		var performedByUserId = Guid.NewGuid();

		// Act
		ticket.AssignTo(
			assignedUserId,
			performedByUserId);

		// Assert
		Assert.Equal(
			assignedUserId,
			ticket.AssignedToUserId);
	}

	[Fact]
	public void AddComment_ShouldAddCommentToTicket()
	{
		// Arrange
		var ticket = CreateTicket();
		var authorId = Guid.NewGuid();

		// Act
		var comment = ticket.AddComment(
			authorId,
			"I am investigating the issue.");

		// Assert
		Assert.Single(ticket.Comments);
		Assert.Equal(ticket.Id, comment.TicketId);
		Assert.Equal(authorId, comment.AuthorId);
		Assert.Equal(
			"I am investigating the issue.",
			comment.Content);
	}

	[Fact]
	public void AddComment_ShouldThrow_WhenContentIsEmpty()
	{
		// Arrange
		var ticket = CreateTicket();

		// Act
		var action = () => ticket.AddComment(
			Guid.NewGuid(),
			" ");

		// Assert
		Assert.Throws<ArgumentException>(action);
	}

	[Fact]
	public void Constructor_ShouldCreateAuditEntry()
	{
		// Arrange
		var createdByUserId = Guid.NewGuid();

		// Act
		var ticket = new Ticket(
			Guid.NewGuid(),
			"Fix login bug",
			null,
			TicketPriority.High,
			createdByUserId);

		// Assert
		var auditEntry = Assert.Single(ticket.AuditEntries);

		Assert.Equal(
			TicketAuditAction.Created,
			auditEntry.Action);

		Assert.Equal(
			createdByUserId,
			auditEntry.PerformedByUserId);

		Assert.Null(auditEntry.OldValue);
		Assert.Equal(ticket.Title, auditEntry.NewValue);
	}

	[Fact]
	public void ChangeStatus_ShouldUpdateTicketStatus()
	{
		// Arrange
		var ticket = CreateTicket();
		var performedByUserId = Guid.NewGuid();

		// Act
		ticket.ChangeStatus(
			TicketStatus.InProgress,
			performedByUserId);

		// Assert
		Assert.Equal(TicketStatus.InProgress, ticket.Status);
	}

	[Fact]
	public void ChangeStatus_ShouldCreateAuditEntry()
	{
		// Arrange
		var ticket = CreateTicket();
		var performedByUserId = Guid.NewGuid();

		// Act
		ticket.ChangeStatus(
			TicketStatus.InProgress,
			performedByUserId);

		// Assert
		var auditEntry = ticket.AuditEntries.Last();

		Assert.Equal(
			TicketAuditAction.StatusChanged,
			auditEntry.Action);

		Assert.Equal(
			TicketStatus.Open.ToString(),
			auditEntry.OldValue);

		Assert.Equal(
			TicketStatus.InProgress.ToString(),
			auditEntry.NewValue);
	}

	[Fact]
	public void ChangeStatus_ShouldNotCreateAuditEntry_WhenStatusIsUnchanged()
	{
		// Arrange
		var ticket = CreateTicket();
		var initialAuditCount = ticket.AuditEntries.Count;

		// Act
		ticket.ChangeStatus(
			TicketStatus.Open,
			Guid.NewGuid());

		// Assert
		Assert.Equal(
			initialAuditCount,
			ticket.AuditEntries.Count);
	}

	private static Ticket CreateTicket()
	{
		return new Ticket(
			Guid.NewGuid(),
			"Fix login bug",
			"Users cannot login with valid credentials.",
			TicketPriority.High,
			Guid.NewGuid());
	}

	[Fact]
	public void Unassign_ShouldRemoveAssigneeAndCreateAuditEntry()
	{
		// Arrange
		var ticket = CreateTicket();
		var assignedUserId = Guid.NewGuid();
		var managerId = Guid.NewGuid();

		ticket.AssignTo(assignedUserId, managerId);

		// Act
		ticket.Unassign(managerId);

		// Assert
		Assert.Null(ticket.AssignedToUserId);

		var auditEntry = ticket.AuditEntries.Last();

		Assert.Equal(
			TicketAuditAction.Unassigned,
			auditEntry.Action);

		Assert.Equal(
			assignedUserId.ToString(),
			auditEntry.OldValue);

		Assert.Null(auditEntry.NewValue);
	}
}