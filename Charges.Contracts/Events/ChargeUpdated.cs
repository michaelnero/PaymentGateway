using Infrastructure.EventSourcing;

namespace Charges.Contracts.Events
{
    public class ChargeUpdated : VersionedEvent
    {
        public decimal Amount { get; set; }

        public string Currency { get; set; }
    }
}
