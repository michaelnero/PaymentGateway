namespace Payments
{
    public class CreditCardInfo
    {
        public string CardNumber { get; set; }

        public string Cvv { get; set; }

        public int ExpiryMonth { get; set; }

        public int ExpiryYear { get; set; }
    }
}
