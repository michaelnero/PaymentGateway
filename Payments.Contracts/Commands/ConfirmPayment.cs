using System;
using Infrastructure.Messaging;

namespace Payments.Contracts.Commands
{
    public class ConfirmPayment : ICommand
    {
        public ConfirmPayment()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        public Guid ChargeId { get; set; }
    }
}
