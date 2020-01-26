using System;
using Infrastructure.Messaging;

namespace Payments.Contracts.Commands
{
    public class RejectPayment : ICommand
    {
        public RejectPayment()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        public Guid ChargeId { get; set; }
    }
}
