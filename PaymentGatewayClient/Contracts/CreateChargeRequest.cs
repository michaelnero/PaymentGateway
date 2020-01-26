using System.Text.Json.Serialization;

namespace PaymentGatewayClient.Contracts
{
    public class CreateChargeRequest
    {
        [JsonPropertyName("idempotent_key")]
        public string IdempotentKey { get; set; } = null!;

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; } = null!;

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("card_number")]
        public string CardNumber { get; set; } = null!;

        [JsonPropertyName("cvv")]
        public string Cvv { get; set; } = null!;

        [JsonPropertyName("expiry_month")]
        public int ExpiryMonth { get; set; }

        [JsonPropertyName("expiry_year")]
        public int ExpiryYear { get; set; }
    }
}
