using System;
using System.Text.Json.Serialization;
using PaymentGateway.Model;
using PaymentGateway.Util;

namespace PaymentGateway.Contracts
{
    public class GetChargeResponse
    {
        public GetChargeResponse()
        {

        }

        public GetChargeResponse(Charge charge)
        {
            if (charge is null) throw new ArgumentNullException(nameof(charge));

            Id = charge.Id;
            IdempotentKey = charge.IdempotentKey;
            CreatedOn = charge.CreatedOn;
            Status = charge.Status;
            Amount = charge.Amount;
            Currency = charge.Currency;
            Description = charge.Description;
            CardNumber = CardNumberUtil.Mask(charge.CardNumber);
        }

        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("idempotent_key")]
        public string IdempotentKey { get; set; }

        [JsonPropertyName("created_on")]
        public DateTimeOffset CreatedOn { get; set; }

        [JsonPropertyName("status")]
        public ChargeStatus Status { get; set; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("card_number")]
        public string CardNumber { get; set; }
    }
}