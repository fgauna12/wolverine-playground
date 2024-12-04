using Marten;
using Wolverine.Attributes;

namespace WolverinePlayground.Web;

public static class MeowCreateIssueHandler
{
    // The IssueCreated event message being returned will be
    // published as a new "cascaded" message by Wolverine after
    // the original message and any related middleware has
    // succeeded
    [WolverineHandler]
    public static IssueCreated Handle(CreateIssue command, IDocumentSession session)
    {
        var issue = new Issue
        {
            Title = command.Title,
            Description = command.Description,
            IsOpen = true,
            Opened = DateTimeOffset.Now,
            OriginatorId = command.OriginatorId
        };

        session.Store(issue);

        return new IssueCreated(issue.Id);
    }
}