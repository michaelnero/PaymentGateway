using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Messaging
{
    public static class EventBusExtensions
    {
        public static Task PublishAsync(this IEventBus bus, IEvent @event)
        {
            if (bus is null) throw new ArgumentNullException(nameof(bus));
            if (@event is null) throw new ArgumentNullException(nameof(@event));
            
            return bus.PublishAsync(new Envelope<IEvent>(@event));
        }

        public static Task PublishAsync(this IEventBus bus, IEnumerable<IEvent> events)
        {
            if (bus is null) throw new ArgumentNullException(nameof(bus));
            if (events is null) throw new ArgumentNullException(nameof(events));
            
            return bus.PublishAsync(events.Select(x => new Envelope<IEvent>(x)));
        }
    }
}
