using System;
using Infrastructure.Messaging;

namespace Payments.Contracts.Commands
{
    public class SendToAcquiringBank : ICommand
    {
        public SendToAcquiringBank()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        public Guid ChargeId { get; set; }

        public decimal Amount { get; set; }

        public string Currency { get; set; }

        public string CardNumber { get; set; }

        public string Cvv { get; set; }

        public int ExpiryMonth { get; set; }

        public int ExpiryYear { get; set; }
    }
}
