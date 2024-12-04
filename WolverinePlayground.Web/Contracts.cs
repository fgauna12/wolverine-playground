using Wolverine.Attributes;

namespace WolverinePlayground.Web;

[WolverineMessage]
public record CreateIssue(Guid OriginatorId, string Title, string Description);
public record AssignIssue(Guid IssueId, Guid AssigneeId);
public record IssueCreated(Guid Id);
public record IssueAssigned(Guid Id);

public class Issue
{
    public Guid Id { get; } = Guid.NewGuid();

    public Guid? AssigneeId { get; set; }
    public Guid? OriginatorId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public bool IsOpen { get; set; }

    public DateTimeOffset Opened { get; set; }

    public IList<IssueTask> Tasks { get; set; }
        = new List<IssueTask>();
}

public class IssueTask
{
    public IssueTask(string title, string description)
    {
        Title = title;
        Description = description;
        Id = Guid.NewGuid();
    }

    public Guid Id { get; set; }

    public string Title { get; set; }
    public string Description { get; set; }
    public DateTimeOffset? Started { get; set; }
    public DateTimeOffset Finished { get; set; }
}