using System;
using Infrastructure.Messaging;

namespace Payments.Contracts.Events
{
    public class FundsWithdrawSucceeded : IEvent
    {
        public Guid SourceId { get; set; }

        public Guid ChargeId { get; set; }

        public string ThirdPartyPaymentId { get; set; }
    }
}
