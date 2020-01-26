using System;
using Infrastructure.EventSourcing;

namespace Payments.Contracts.Events
{
    public class PaymentCompleted : VersionedEvent
    {
        public Guid ChargeId { get; set; }
    }
}
