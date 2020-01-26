using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaymentGateway.Util;

namespace PaymentGateway.Tests.UtilTests
{
    [TestClass]
    public class CardNumberUtilTests
    {
        [TestMethod]
        public void Null_String_Throws_Exception()
        {
            var input = (string) null;

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                CardNumberUtil.Mask(input);
            });
        }

        [DataTestMethod]
        [DataRow("", "")]
        [DataRow("123", "***")]
        [DataRow("1234", "****")]
        [DataRow("12345", "*2345")]
        [DataRow("1234567890123456", "************3456")]
        public void Validate_Scenarios(string input, string expected)
        {
            var result = CardNumberUtil.Mask(input);

            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result);
        }
    }
}
