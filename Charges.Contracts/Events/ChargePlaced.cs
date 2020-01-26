using System;
using Infrastructure.EventSourcing;

namespace Charges.Contracts.Events
{
    public class ChargePlaced : VersionedEvent
    {
        public DateTimeOffset CreatedOn { get; set; }

        public decimal Amount { get; set; }

        public string Currency { get; set; }

        public string CardNumber { get; set; }

        public string Cvv { get; set; }

        public int ExpiryMonth { get; set; }

        public int ExpiryYear { get; set; }
    }
}
