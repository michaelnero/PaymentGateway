using System;
using System.Threading.Tasks;
using Infrastructure.Messaging.Handling;
using Infrastructure.Processes;
using Microsoft.Extensions.Logging;
using Payments.Contracts.Events;

namespace Payments
{
    public class PaymentProcessManagerRouter :
        IEventHandler<PaymentInitiated>,
        IEventHandler<FundsWithdrawSucceeded>,
        IEventHandler<FundsWithdrawFailed>
    {
        private readonly Func<IProcessManagerDataContext<PaymentProcessManager>> _contextFactory;
        private readonly ILogger<PaymentProcessManagerRouter> _logger;

        public PaymentProcessManagerRouter(Func<IProcessManagerDataContext<PaymentProcessManager>> contextFactory, ILogger<PaymentProcessManagerRouter> logger)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task HandleAsync(PaymentInitiated e)
        {
            using var context = _contextFactory();

            var pm = await context.FindAsync(o => o.ChargeId == e.ChargeId);
            if (pm is null)
            {
                pm = new PaymentProcessManager();
            }

            pm.Handle(e);

            await context.SaveAsync(pm);
        }

        public async Task HandleAsync(FundsWithdrawSucceeded e)
        {
            using var context = _contextFactory();

            var pm = await context.FindAsync(o => o.ChargeId == e.ChargeId);
            if (pm is null)
            {
                _logger.LogError("Failed to locate the payment process manager handling the charge with ID {Id}.", e.ChargeId);
                return;
            }

            pm.Handle(e);

            await context.SaveAsync(pm);
        }

        public async Task HandleAsync(FundsWithdrawFailed e)
        {
            using var context = _contextFactory();

            var pm = await context.FindAsync(o => o.ChargeId == e.ChargeId);
            if (pm is null)
            {
                _logger.LogError("Failed to locate the payment process manager handling the charge with ID {Id}.", e.ChargeId);
                return;
            }

            pm.Handle(e);

            await context.SaveAsync(pm);
        }
    }
}
