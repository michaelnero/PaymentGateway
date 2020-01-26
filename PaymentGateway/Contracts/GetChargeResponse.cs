using System;
using System.Text.Json.Serialization;

namespace PaymentGateway.Contracts
{
    public class GetChargeResponse
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("created_on")]
        public DateTimeOffset CreatedOn { get; set; }

        [JsonPropertyName("is_completed")]
        public bool IsCompleted { get; set; }

        [JsonPropertyName("is_successful")]
        public bool IsSuccessful { get; set; }

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