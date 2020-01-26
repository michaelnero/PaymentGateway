using Infrastructure.Messaging;

namespace Infrastructure.EventSourcing
{
    public interface IVersionedEvent : IEvent
    {
        int Version { get; }
    }
}
