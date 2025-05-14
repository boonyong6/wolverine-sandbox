using WolverineSandbox.Domain.Entities;

namespace WolverineSandbox.Domain.Repositories;

public class IssueRepository
{
    private readonly Dictionary<Guid, Issue> _issues = [];

    public void Store(Issue issue)
    {
        _issues[issue.Id] = issue;
    }

    public Issue Get(Guid id)
    {
        if (_issues.TryGetValue(id, out Issue? issue))
        {
            return issue;
        }

        throw new ArgumentOutOfRangeException(nameof(id), "Issue does not exist.");
    }
}
