using WolverineSandbox.Domain.Entities;
using WolverineSandbox.Domain.Events;
using WolverineSandbox.Domain.Repositories;

namespace WolverineSandbox.WebApi.Commands;

public class CreateIssueHandler
{
    private readonly IssueRepository _repository;

    public CreateIssueHandler(IssueRepository repository)
    {
        _repository = repository;
    }

    // The IssueCreated event message being returned will be
    // published as a new "cascaded" message by Wolverine after
    // the original message and any related middleware has
    // succeeded.
    public IssueCreated Handle(CreateIssue command)
    {
        Issue issue = new()
        {
            Title = command.Title,
            Description = command.Description,
            IsOpen = true,
            Opened = DateTimeOffset.Now,
            OriginatorId = command.OriginatorId,
        };

        _repository.Store(issue);

        return new IssueCreated(issue.Id);
    }
}
