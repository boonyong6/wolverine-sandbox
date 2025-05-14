namespace WolverineSandbox.Domain.Entities;

public class IssueTask(string title, string description)
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Title { get; set; } = title;
    public string Description { get; set; } = description;
    public DateTimeOffset? Started { get; set; }
    public DateTimeOffset Finished { get; set; }
}
