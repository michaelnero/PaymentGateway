using System;
using Infrastructure.Messaging;

namespace Charges.Contracts.Commands
{
    public class ConfirmCharge : ICommand
    {
        public ConfirmCharge()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        public Guid ChargeId { get; set; }
    }
}
