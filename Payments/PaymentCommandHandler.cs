using System;
using System.Threading.Tasks;
using Infrastructure.EventSourcing;
using Infrastructure.Messaging.Handling;
using Payments.Contracts.Commands;

namespace Payments
{
    public class PaymentCommandHandler :
        ICommandHandler<InitiatePayment>,
        ICommandHandler<CancelPayment>,
        ICommandHandler<UpdatePayment>,
        ICommandHandler<ConfirmPayment>,        
        ICommandHandler<RejectPayment>
        
    {
        private readonly IEventSourcedRepository<Payment> _repository;

        public PaymentCommandHandler(IEventSourcedRepository<Payment> repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task HandleAsync(InitiatePayment command)
        {
            var payment = await _repository.FindAsync(command.ChargeId);
            if (payment is null)
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

                // Payments get the same ID as the charge they are associated with.
                payment = new Payment(command.ChargeId, amount, cardInfo);
            }

            await _repository.SaveAsync(payment, command.Id.ToString());
        }

        public Task HandleAsync(CancelPayment command)
        {
            throw new NotImplementedException("Not implemented. For illustrative purposes only.");
        }

        public Task HandleAsync(UpdatePayment command)
        {
            throw new NotImplementedException("Not implemented. For illustrative purposes only.");
        }

        public async Task HandleAsync(ConfirmPayment command)
        {
            var payment = await _repository.GetAsync(command.ChargeId);

            payment.Confirm();

            await _repository.SaveAsync(payment, command.Id.ToString());
        }
        
        public async Task HandleAsync(RejectPayment command)
        {
            var payment = await _repository.GetAsync(command.ChargeId);

            payment.Reject();

            await _repository.SaveAsync(payment, command.Id.ToString());
        }
    }
}
