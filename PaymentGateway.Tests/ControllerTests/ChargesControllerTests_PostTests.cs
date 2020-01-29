using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PaymentGateway.Contracts;
using PaymentGateway.Model;
using PaymentGateway.Services;

namespace PaymentGateway.Tests.ControllerTests
{
    public partial class ChargesControllerTests
    {
        [TestClass]
        public class PostTests
        {
            private Mock<IAcquiringBank> _bank;
            private PaymentGatewayContext _db;
            private HttpClient _client;

            public TestContext TestContext { get; set; }

            [TestInitialize]
            public void TestInitialize()
            {
                _bank = new Mock<IAcquiringBank>();

                _db = new PaymentGatewayContext(new DbContextOptionsBuilder<PaymentGatewayContext>().UseInMemoryDatabase(TestContext.TestName).Options);

                _client = _factory.CreateTestClient(TestContext.TestName, services =>
                {
                    services.AddSingleton(_bank.Object);
                    services.AddApiKeys(_defaultApiKey);
                });

                _client.DefaultRequestHeaders.Add("X-Api-Key", _defaultApiKey);
            }

            [TestMethod]
            public async Task Request_Without_ApiKey_Returns_UnAuthorized()
            {
                // Arrange

                // Remove the ApiKey header so that auth fails.
                _client.DefaultRequestHeaders.Remove("X-Api-Key");

                var request = new CreateChargeRequest
                {
                    IdempotentKey = "111",
                    Amount = 1.0m,
                    Currency = "USD",
                    Description = null,
                    CardNumber = "1234567890123452",
                    Cvv = "111",
                    ExpiryMonth = 1,
                    ExpiryYear = 2020
                };

                var serialized = JsonSerializer.Serialize(request);
                using var content = new StringContent(serialized, Encoding.UTF8, "application/json");

                // Act

                using var response = await _client.PostAsync("api/charges", content);

                // Assert

                Assert.IsNotNull(response);
                Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
            }

            [TestMethod]
            public async Task Request_With_Invalid_ApiKey_Returns_Unauthorized()
            {
                // Arrange

                // Remove the ApiKey header so that auth fails.
                _client.DefaultRequestHeaders.Remove("X-Api-Key");
                _client.DefaultRequestHeaders.Add("X-Api-Key", "invalid key");

                var request = new CreateChargeRequest
                {
                    IdempotentKey = "111",
                    Amount = 1.0m,
                    Currency = "USD",
                    Description = null,
                    CardNumber = "1234567890123452",
                    Cvv = "111",
                    ExpiryMonth = 1,
                    ExpiryYear = 2020
                };

                var serialized = JsonSerializer.Serialize(request);
                using var content = new StringContent(serialized, Encoding.UTF8, "application/json");

                // Act

                using var response = await _client.PostAsync("api/charges", content);

                // Assert

                Assert.IsNotNull(response);
                Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
            }

            [DataTestMethod]
            [DataRow("", 1.0, "USD", "", "1234567890123452", "111", 1, 2020, DisplayName = "IdempotentKey_Missing")]
            [DataRow("123", -1.0, "USD", "", "1234567890123452", "111", 1, 2020, DisplayName = "Amount_OutOfRange")]
            [DataRow("123", 1.0, "", "", "1234567890123452", "111", 1, 2020, DisplayName = "Currency_Missing")]
            [DataRow("123", 1.0, "JPN", "", "1234567890123452", "111", 1, 2020, DisplayName = "Currency_OutOfRange")]
            [DataRow("123", 1.0, "USD", "", "", "111", 1, 2020, DisplayName = "CardNumber_Missing")]
            [DataRow("123", 1.0, "USD", "", "1234", "111", 1, 2020, DisplayName = "CardNumber_TooShort")]
            [DataRow("123", 1.0, "USD", "", "1234567890123456", "111", 1, 2020, DisplayName = "CardNumber_Invalid")]
            [DataRow("123", 1.0, "USD", "", "1234567890123452", "", 1, 2020, DisplayName = "Cvv_Missing")]
            [DataRow("123", 1.0, "USD", "", "1234567890123452", "asdf", 1, 2020, DisplayName = "Cvv_Characters")]
            [DataRow("123", 1.0, "USD", "", "1234567890123452", "1", 1, 2020, DisplayName = "Cvv_TooShort")]
            [DataRow("123", 1.0, "USD", "", "1234567890123452", "12345", 1, 2020, DisplayName = "Cvv_TooLong")]
            [DataRow("123", 1.0, "USD", "", "1234567890123452", "111", 0, 2020, DisplayName = "ExpiryMonth_OutOfRangeMin")]
            [DataRow("123", 1.0, "USD", "", "1234567890123452", "111", 13, 2020, DisplayName = "ExpiryMonth_OutOfRangeMax")]
            [DataRow("123", 1.0, "USD", "", "1234567890123452", "111", 1, 1969, DisplayName = "ExpiryYear_OutOfRangeMin")]
            [DataRow("123", 1.0, "USD", "", "1234567890123452", "111", 1, 2101, DisplayName = "ExpiryYear_OutOfRangeMax")]
            public async Task Validate_InputValidation(string idempotentKey, double amount, string currency, string description, string cardNumber, string cvv, int expiryMonth, int expiryYear)
            {
                // Arrange

                var request = new CreateChargeRequest
                {
                    IdempotentKey = idempotentKey,
                    Amount = (decimal)amount,
                    Currency = currency,
                    Description = description,
                    CardNumber = cardNumber,
                    Cvv = cvv,
                    ExpiryMonth = expiryMonth,
                    ExpiryYear = expiryYear
                };

                var serialized = JsonSerializer.Serialize(request);
                using var content = new StringContent(serialized, Encoding.UTF8, "application/json");

                // Act

                using var response = await _client.PostAsync("api/charges", content);

                // Assert

                Assert.IsNotNull(response);
                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }

            [TestMethod]
            public async Task Charge_Returned_When_Posted()
            {
                // Arrange

                var request = new CreateChargeRequest
                {
                    IdempotentKey = "111",
                    Amount = 1.0m,
                    Currency = "USD",
                    Description = null,
                    CardNumber = "1234567890123452",
                    Cvv = "111",
                    ExpiryMonth = 1,
                    ExpiryYear = 2020
                };

                var serialized = JsonSerializer.Serialize(request);
                using var content = new StringContent(serialized, Encoding.UTF8, "application/json");

                // Act

                using var response = await _client.PostAsync("api/charges", content);

                // Assert

                Assert.IsNotNull(response);
                Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

                using var responseStream = await response.Content.ReadAsStreamAsync();
                var responseCharge = await JsonSerializer.DeserializeAsync<GetChargeResponse>(responseStream);

                Assert.AreEqual(request.IdempotentKey, responseCharge.IdempotentKey);
                Assert.AreEqual(request.Amount, responseCharge.Amount);
                Assert.AreEqual(request.Currency, responseCharge.Currency);
                Assert.AreEqual(request.Description, responseCharge.Description);
                Assert.AreEqual("************3452", responseCharge.CardNumber);

                var location = response.Headers.Location;
                Assert.AreEqual($"/api/charges?id={responseCharge.Id}", location.PathAndQuery);
            }

            [DataTestMethod]
            [DataRow(false, ChargeStatus.Failed, DisplayName = "Bank_Fails")]
            [DataRow(true, ChargeStatus.Success, DisplayName = "Bank_Succeeds")]
            public async Task Charge_Written_To_Db_With_Bank_Status(bool bankReturnStatus, ChargeStatus expectedStatus)
            {
                // Arrange

                var s = "bankChargeId";
                _bank.Setup(o => o.TrySendAsync(It.IsAny<AcquiringBankRequest>(), out s)).ReturnsAsync(bankReturnStatus);

                var request = new CreateChargeRequest
                {
                    IdempotentKey = "111",
                    Amount = 1.0m,
                    Currency = "USD",
                    Description = null,
                    CardNumber = "1234567890123452",
                    Cvv = "111",
                    ExpiryMonth = 1,
                    ExpiryYear = 2020
                };

                var serialized = JsonSerializer.Serialize(request);
                using var content = new StringContent(serialized, Encoding.UTF8, "application/json");

                // Act

                using var response = await _client.PostAsync("api/charges", content);

                // Assert

                Assert.IsNotNull(response);
                Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

                using var responseStream = await response.Content.ReadAsStreamAsync();
                var responseCharge = await JsonSerializer.DeserializeAsync<GetChargeResponse>(responseStream);

                var dbCharge = await _db.Charges.FindAsync(responseCharge.Id);

                Assert.IsNotNull(dbCharge);

                Assert.AreEqual(request.IdempotentKey, dbCharge.IdempotentKey);
                Assert.AreEqual(request.Amount, dbCharge.Amount);
                Assert.AreEqual(request.Currency, dbCharge.Currency);
                Assert.AreEqual(request.Description, dbCharge.Description);
                Assert.AreEqual(request.CardNumber, dbCharge.CardNumber);
                Assert.AreEqual(request.Cvv, dbCharge.Cvv);
                Assert.AreEqual(request.ExpiryMonth, dbCharge.ExpiryMonth);
                Assert.AreEqual(request.ExpiryYear, dbCharge.ExpiryYear);
                Assert.AreEqual(expectedStatus, dbCharge.Status);
                Assert.AreEqual(s, dbCharge.BankChargeId);
            }
        }
    }
}
