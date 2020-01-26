using System;
using Infrastructure.Messaging;

namespace Payments.Contracts.Commands
{
    public class CancelPayment : ICommand
    {
        public CancelPayment()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        public Guid ChargeId { get; set; }
    }
}
