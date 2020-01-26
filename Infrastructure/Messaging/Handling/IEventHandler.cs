using System.Threading.Tasks;

namespace Infrastructure.Messaging.Handling
{
    public interface IEventHandler
    {
    }

    public interface IEventHandler<T> : IEventHandler where T : IEvent
    {
        Task HandleAsync(T @event);
    }
}
