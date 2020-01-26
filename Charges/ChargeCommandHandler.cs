using System;
using System.Threading.Tasks;
using Charges.Contracts.Commands;
using Infrastructure.EventSourcing;
using Infrastructure.Messaging.Handling;

namespace Charges
{
    public class ChargeCommandHandler :
        ICommandHandler<PlaceCharge>,
        ICommandHandler<ConfirmCharge>,
        ICommandHandler<RejectCharge>
    {
        private readonly IEventSourcedRepository<Charge> _repository;

        public ChargeCommandHandler(IEventSourcedRepository<Charge> repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task HandleAsync(PlaceCharge command)
        {
            var charge = await _repository.FindAsync(command.ChargeId);
            if (charge is null)
            {
                var amount = new PaymentAmount
                {
                    Value = command.Amount,
                    Currency = command.Currency
                };

                var cardInfo = new CreditCardInfo
                {
                    CardNumber = command.CardNumber,
                    Cvv = command.Cvv,
                    ExpiryMonth = command.ExpiryMonth,
                    ExpiryYear = command.ExpiryYear
                };

                charge = new Charge(command.ChargeId, amount, cardInfo);
            }
            else
            {
                charge.UpdateAmount(new PaymentAmount
                {
                    Value = command.Amount,
                    Currency = command.Currency
                });
            }

            await _repository.SaveAsync(charge, command.Id.ToString());
        }

        public async Task HandleAsync(ConfirmCharge command)
        {
            var charge = await _repository.GetAsync(command.ChargeId);

            charge.Confirm();

            await _repository.SaveAsync(charge, command.Id.ToString());
        }

        public async Task HandleAsync(RejectCharge command)
        {
            var charge = await _repository.GetAsync(command.ChargeId);

            charge.Reject();

            await _repository.SaveAsync(charge, command.Id.ToString());
        }
    }
}
