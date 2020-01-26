using System;

namespace Charges
{
    public class PaymentAmount : ICloneable
    {
        public decimal Value { get; set; }

        public string Currency { get; set; }

        public object Clone() => new PaymentAmount
        {
            Value = Value,
            Currency = Currency
        };
    }
}
