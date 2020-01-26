using System;
using Infrastructure.EventSourcing;

namespace Payments.Contracts.Events
{
    public class PaymentRejected : VersionedEvent
    {
        public Guid ChargeId { get; set; }
    }
}
