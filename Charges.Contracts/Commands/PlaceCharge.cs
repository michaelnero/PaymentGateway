using System;
using Infrastructure.Messaging;

namespace Charges.Contracts.Commands
{
    public class PlaceCharge : ICommand
    {
        public PlaceCharge()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        public Guid ChargeId { get; set; }

        public string IdempotentKey { get; set; }

        public decimal Amount { get; set; }

        public string Currency { get; set; }

        public string CardNumber { get; set; }

        public string Cvv { get; set; }

        public int ExpiryMonth { get; set; }

        public int ExpiryYear { get; set; }
    }
}
