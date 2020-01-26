using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PaymentGateway.Contracts
{
    public class CreateChargeRequest
    {
        [Required]
        [JsonPropertyName("idempotent_key")]
        public string IdempotentKey { get; set; }

        [Range(0.01, double.MaxValue)]
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [Required]
        [JsonPropertyName("currency")]        
        [RegularExpression("(USD|EUR)")]
        public string Currency { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [Required]
        [CreditCard]
        [JsonPropertyName("card_number")]
        public string CardNumber { get; set; }

        [Required]
        [RegularExpression("\\d{3,4}")]
        [JsonPropertyName("cvv")]
        public string Cvv { get; set; }

        [Range(1, 12)]
        [JsonPropertyName("expiry_month")]
        public int ExpiryMonth { get; set; }

        [Range(1970, 2100)]
        [JsonPropertyName("expiry_year")]
        public int ExpiryYear { get; set; }
    }
}
