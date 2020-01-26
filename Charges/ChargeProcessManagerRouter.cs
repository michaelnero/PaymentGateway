using System;
using System.Threading.Tasks;
using Charges.Contracts.Events;
using Infrastructure.Messaging.Handling;
using Infrastructure.Processes;
using Microsoft.Extensions.Logging;
using Payments.Contracts.Events;

namespace Charges
{
    public class ChargeProcessManagerRouter :
        IEventHandler<ChargePlaced>,
        IEventHandler<ChargeUpdated>,
        IEventHandler<ChargeCancelled>,
        IEventHandler<PaymentCompleted>,
        IEventHandler<PaymentRejected>
    {
        private readonly Func<IProcessManagerDataContext<ChargeProcessManager>> _contextFactory;
        private readonly ILogger<ChargeProcessManagerRouter> _logger;

        public ChargeProcessManagerRouter(Func<IProcessManagerDataContext<ChargeProcessManager>> contextFactory, ILogger<ChargeProcessManagerRouter> logger)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task HandleAsync(ChargePlaced e)
        {
            using var context = _contextFactory();

            var pm = await context.FindAsync(o => o.ChargeId == e.SourceId);
            if (pm is null)
            {
                pm = new ChargeProcessManager();
            }

            pm.Handle(e);
            
            await context.SaveAsync(pm);
        }

        public async Task HandleAsync(ChargeUpdated e)
        {
            using var context = _contextFactory();

            var pm = await context.FindAsync(o => o.ChargeId == e.SourceId);
            if (pm is null)
            {
                _logger.LogError("Failed to locate the charge process manager handling the charge with ID {Id}.", e.SourceId);
                return;
            }

            pm.Handle(e);

            await context.SaveAsync(pm);
        }

        public async Task HandleAsync(ChargeCancelled e)
        {
            using var context = _contextFactory();

            var pm = await context.FindAsync(o => o.ChargeId == e.SourceId);
            if (pm is null)
            {
                _logger.LogError("Failed to locate the charge process manager handling the charge with ID {Id}.", e.SourceId);
                return;
            }

            pm.Handle(e);

            await context.SaveAsync(pm);
        }

        public async Task HandleAsync(PaymentCompleted e)
        {
            using var context = _contextFactory();

            var pm = await context.FindAsync(o => o.ChargeId == e.ChargeId);
            if (pm is null)
            {
                _logger.LogError("Failed to locate the charge process manager handling the charge with ID {Id}.", e.ChargeId);
                return;
            }

            pm.Handle(e);

            await context.SaveAsync(pm);
        }

        public async Task HandleAsync(PaymentRejected e)
        {
            using var context = _contextFactory();

            var pm = await context.FindAsync(o => o.ChargeId == e.ChargeId);
            if (pm is null)
            {
                _logger.LogError("Failed to locate the charge process manager handling the charge with ID {Id}.", e.ChargeId);
                return;
            }

            pm.Handle(e);

            await context.SaveAsync(pm);
        }
    }
}
