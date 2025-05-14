using System.Net.Mail;
using WolverineSandbox.Domain.Entities;
using WolverineSandbox.Domain.Repositories;

namespace WolverineSandbox.Domain.Events;

public static class IssueCreatedHandler
{
    // Wolverine assumes that the first argument is the message type,
    // but other arguments are inferred to be services (aka method injection).
    public static async Task Handle(IssueCreated created, IssueRepository repository)
    {
        Issue issue = repository.Get(created.Id);
        MailMessage message = await BuildEmailMessage(issue);
        using SmtpClient client = new();
        client.Send(message);
    }

    // This is a little helper method I made public
    // Wolverine will not expose this as a message handler.
    internal static Task<MailMessage> BuildEmailMessage(Issue issue)
    {
        // Build up a templated email message, with
        // some sort of async method to look up additional
        // data just so we can show off an async
        // Wolverine Handler.
        return Task.FromResult(new MailMessage());
    }
}
