using System;
using System.Threading.Tasks;
using Charges;
using Charges.Contracts.Commands;
using Infrastructure.EventSourcing;
using Infrastructure.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PaymentGateway.Contracts;

namespace PaymentGateway.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/charges")]
    public class ChargesController : ControllerBase
    {
        private readonly IEventSourcedRepository<Charge> _repository;
        private readonly ICommandBus _commands;
        private readonly ILogger<ChargesController> _logger;

        public ChargesController(IEventSourcedRepository<Charge> repository, ICommandBus commands, ILogger<ChargesController> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _commands = commands ?? throw new ArgumentNullException(nameof(commands));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(Guid id)
        {
            var entity = await _repository.FindAsync(id);
            if (entity is null)
            {
                _logger.LogWarning("A charge with id {Id} was not found.", id);
                return NotFound();
            }

            var amount = entity.Amount;
            var cardInfo = entity.CardInfo;

            var result = new GetChargeResponse
            {
                Id = entity.Id,
                CreatedOn = entity.CreatedOn,
                IsCompleted = (entity.ChargeState == ChargeState.Completed),
                IsSuccessful = (entity.PaymentState == PaymentState.Success),
                Amount = amount.Value,
                Currency = amount.Currency,
                CardNumber = cardInfo.CardNumber
            };
            
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] CreateChargeRequest request)
        {
            var chargeId = Guid.NewGuid();

            await _commands.SendAsync(new PlaceCharge
            {
                IdempotentKey = request.IdempotentKey,
                ChargeId = chargeId,
                Amount = request.Amount,
                Currency = request.Currency,
                CardNumber = request.CardNumber,
                Cvv = request.Cvv,
                ExpiryMonth = request.ExpiryMonth,
                ExpiryYear = request.ExpiryYear
            });

            return CreatedAtAction("Get", new { id = chargeId }, null);
        }
    }
}