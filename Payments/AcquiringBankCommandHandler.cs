using System;
using System.Threading.Tasks;
using Infrastructure.Messaging;
using Infrastructure.Messaging.Handling;
using Microsoft.Extensions.Logging;
using Payments.Contracts.Commands;
using Payments.Contracts.Events;
using Payments.Services;

namespace Payments
{
    public class AcquiringBankCommandHandler : ICommandHandler<SendToAcquiringBank>
    {
        private readonly IAcquiringBank _bank;
        private readonly IEventBus _events;
        private readonly ILogger<AcquiringBankCommandHandler> _logger;

        public AcquiringBankCommandHandler(IAcquiringBank bank, IEventBus events, ILogger<AcquiringBankCommandHandler> logger)
        {
            _bank = bank ?? throw new ArgumentNullException(nameof(bank));
            _events = events ?? throw new ArgumentNullException(nameof(events));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task HandleAsync(SendToAcquiringBank command)
        {
            var request = new AcquiringBankRequest
            {
                Amount = command.Amount,
                Currency = command.Currency,
                CardNumber = command.CardNumber,
                Cvv = command.Cvv,
                ExpiryMonth = command.ExpiryMonth,
                ExpiryYear = command.ExpiryYear
            };

            if (await _bank.TrySendAsync(request, out var id))
            {
                await _events.PublishAsync(new FundsWithdrawSucceeded
                {
                    SourceId = command.ChargeId,
                    ChargeId = command.ChargeId,
                    ThirdPartyPaymentId = id
                });
            }
            else
            {
                _logger.LogWarning("Funds withdraw failed for charge {ChargeId}", command.ChargeId);

                await _events.PublishAsync(new FundsWithdrawFailed
                {
                    SourceId = command.ChargeId,
                    ChargeId = command.ChargeId,
                    ThirdPartyPaymentId = id
                });
            }
        }
    }
}
