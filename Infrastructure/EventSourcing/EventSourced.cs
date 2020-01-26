using System;
using System.Collections.Generic;
using Infrastructure.Messaging;

namespace Infrastructure.EventSourcing
{
    public abstract class EventSourced : IEventSourced
    {
        private readonly Dictionary<Type, Action<IVersionedEvent>> _handlers;
        private readonly List<IVersionedEvent> _pendingEvents;

        protected EventSourced(Guid id)
        {
            Id = id;
            Version = -1;

            _handlers = new Dictionary<Type, Action<IVersionedEvent>>();
            _pendingEvents = new List<IVersionedEvent>();
        }

        public Guid Id { get; }

        public int Version { get; protected set; }

        public IEnumerable<IVersionedEvent> Events => _pendingEvents;

        protected void Handles<TEvent>(Action<TEvent> handler) where TEvent : IEvent
        {
            _handlers.Add(typeof(TEvent), @event => handler((TEvent)@event));
        }

        protected void LoadFrom(IEnumerable<IVersionedEvent> pastEvents)
        {
            foreach (var e in pastEvents)
            {
                _handlers[e.GetType()](e);
                Version = e.Version;
            }
        }

        protected void Update(VersionedEvent e)
        {
            e.SourceId = Id;
            e.Version = Version + 1;
            
            _handlers[e.GetType()](e);
            
            Version = e.Version;
            
            _pendingEvents.Add(e);
        }
    }
}
