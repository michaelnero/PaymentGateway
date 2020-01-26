using System;
using System.Text.Json.Serialization;

namespace PaymentGatewayClient.Contracts
{
    public class Charge
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("idempotent_key")]
        public string IdempotentKey { get; set; } = null!;

        [JsonPropertyName("created_on")]
        public DateTimeOffset CreatedOn { get; set; }

        [JsonPropertyName("status")]
        public ChargeStatus Status { get; set; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; } = null!;

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("card_number")]
        public string CardNumber { get; set; } = null!;
    }
}