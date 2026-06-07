namespace WorkTrack.Domain.Enums;

public enum TicketAuditAction
{
	Created = 1,
	DetailsUpdated = 2,
	StatusChanged = 3,
	PriorityChanged = 4,
	Assigned = 5,
	Unassigned = 6
}