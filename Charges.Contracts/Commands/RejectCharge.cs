using System;
using Infrastructure.Messaging;

namespace Charges.Contracts.Commands
{
    public class RejectCharge : ICommand
    {
        public RejectCharge()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        public Guid ChargeId { get; set; }
    }
}
