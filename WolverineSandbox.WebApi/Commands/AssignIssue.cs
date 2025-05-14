namespace WolverineSandbox.WebApi.Commands;

public record AssignIssue(Guid IssueId, Guid AssigneeId);
