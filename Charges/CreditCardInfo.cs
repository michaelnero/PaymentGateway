using System;

namespace Charges
{
    public class CreditCardInfo : ICloneable
    {
        public string CardNumber { get; set; }

        public string Cvv { get; set; }

        public int ExpiryMonth { get; set; }

        public int ExpiryYear { get; set; }

        public object Clone() => new CreditCardInfo
        {
            CardNumber = CardNumber,
            Cvv = Cvv,
            ExpiryMonth = ExpiryMonth,
            ExpiryYear = ExpiryYear
        };
    }
}
