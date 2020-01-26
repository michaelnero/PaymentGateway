using System;
using Infrastructure.Messaging;

namespace Payments.Contracts.Commands
{
    public class UpdatePayment : ICommand
    {
        public UpdatePayment()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        public Guid ChargeId { get; set; }

        public decimal Amount { get; set; }

        public string Currency { get; set; }
    }
}
