using System;

namespace PaymentGateway.Util
{
    public static class CardNumberUtil
    {
        public static string Mask(string number)
        {
            if (number is null) throw new ArgumentNullException(nameof(number));
            if (0 == number.Length) return number;

            // This methods avoids intermediate allocations, leaving the only allocation being the resulting string.

            Span<char> result = stackalloc char[number.Length];

            var asteriskCount = (number.Length <= 4) ? number.Length : number.Length - 4;
            for (var i = 0; i < number.Length; i++)
            {
                result[i] = (i < asteriskCount) ? '*' : number[i];
            }

            return new string(result);
        }
    }
}
