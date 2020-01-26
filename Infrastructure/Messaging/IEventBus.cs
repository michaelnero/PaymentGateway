using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Messaging
{
    public interface IEventBus
    {
        Task PublishAsync(Envelope<IEvent> @event);
        
        Task PublishAsync(IEnumerable<Envelope<IEvent>> events);
    }

    
}
