using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PaymentGateway.Contracts;
using PaymentGateway.Model;
using PaymentGateway.Services;
using PaymentGateway.Util;

namespace PaymentGateway.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/charges")]
    public class ChargesController : ControllerBase
    {
        private readonly PaymentGatewayContext _db;
        private readonly IAcquiringBank _bank;
        private readonly ILogger _logger;

        public ChargesController(PaymentGatewayContext db, IAcquiringBank bank, ILogger<ChargesController> logger)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _bank = bank ?? throw new ArgumentNullException(nameof(bank));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(Guid id)
        {
            var entity = await _db.Charges.FindAsync(id);
            if (entity is null)
            {
                _logger.LogWarning("A charge with id {Id} was not found.", id);
                return NotFound();
            }

            var result = new GetChargeResponse(entity);
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Post([FromBody] CreateChargeRequest request)
        {
            // Note that explicitly checking ModelState is not required for controllers with the [ApiController]
            // attribute, so there is no code here for that here.

            var entity = new Charge
            {
                IdempotentKey = request.IdempotentKey,
                CreatedOn = DateTimeOffset.UtcNow,
                Status = ChargeStatus.Pending,
                Amount = request.Amount,
                Currency = request.Currency,
                Description = request.Description,
                CardNumber = request.CardNumber,
                Cvv = request.Cvv,
                ExpiryMonth = request.ExpiryMonth,
                ExpiryYear = request.ExpiryYear
            };

            _db.Charges.Add(entity);

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException exception) when (exception.IsViolationOfUniqueIndex())
            {
                // This handles the case where someone is double-posting a charge with the same IdempotentKey.

                _logger.LogWarning("A charge with idempotency key {IdempotentKey} attempted to double post.", request.IdempotentKey);
                return Conflict();
            }

            var acquiringBankRequest = new AcquiringBankRequest
            {
                Amount = entity.Amount,
                Currency = entity.Currency,
                CardNumber = entity.CardNumber,
                Cvv = entity.Cvv,
                ExpiryMonth = entity.ExpiryMonth,
                ExpiryYear = entity.ExpiryYear
            };

            var success = await _bank.TrySendAsync(acquiringBankRequest, out var bankChargeId);

            if (!success)
            {
                _logger.LogError("The acquiring bank for charge id {Id} rejected the payment.", entity.Id);
            }

            entity.Status = success ? ChargeStatus.Success : ChargeStatus.Failed;
            entity.BankChargeId = bankChargeId;

            await _db.SaveChangesAsync();

            var result = new GetChargeResponse(entity);
            return CreatedAtAction("Get", new { id = entity.Id }, result);
        }
    }
}