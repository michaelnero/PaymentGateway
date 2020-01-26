using System;

namespace Infrastructure.Messaging
{
    public interface IEvent
    {
        Guid SourceId { get; }
    }

    
}
