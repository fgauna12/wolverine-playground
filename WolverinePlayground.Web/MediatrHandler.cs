using MediatR;

namespace WolverinePlayground.Web;

public interface IWolverineIgnore {}
public record FakeCommand(Guid Id) : IRequest;
public record FakeEvent(Guid Id) : INotification;

public class MediatrHandler: INotificationHandler<FakeEvent>, IRequestHandler<FakeCommand>, IWolverineIgnore
{
    public Task Handle(FakeEvent notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task Handle(FakeCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}