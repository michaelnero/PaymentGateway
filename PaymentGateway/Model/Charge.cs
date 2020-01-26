using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaymentGateway.Model
{
    public class Charge
    {
        [Key]
        public Guid Id { get; set; }

        public string IdempotentKey { get; set; }

        public string BankChargeId { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public ChargeStatus Status { get; set; }

        [Column(TypeName = "money")]
        public decimal Amount { get; set; }

        [Required]
        public string Currency { get; set; }

        public string Description { get; set; }

        [Required]
        [CreditCard]
        public string CardNumber { get; set; }

        [Required]
        [RegularExpression("\\d{3,4}")]
        public string Cvv { get; set; }

        [Range(1, 12)]
        public int ExpiryMonth { get; set; }

        [Range(1970, 2100)]
        public int ExpiryYear { get; set; }
    }
}
