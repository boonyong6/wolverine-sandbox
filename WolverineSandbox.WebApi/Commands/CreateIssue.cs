namespace WolverineSandbox.WebApi.Commands;

public record CreateIssue(Guid OriginatorId, string Title, string Description);
