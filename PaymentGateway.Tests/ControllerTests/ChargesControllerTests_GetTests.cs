using System;
using System.Net;
using System.Net.Http;
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
        public class GetTests
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
            public async Task Charge_Not_Found_Returns_404()
            {
                using var response = await _client.GetAsync($"api/charges/?id={Guid.Empty}");

                Assert.IsNotNull(response);
                Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            }

            [TestMethod]
            public async Task Charge_Returned_When_Found()
            {
                // Arrange

                var charge = new Charge
                {
                    IdempotentKey = "IdempotentKey",
                    BankChargeId = "BankChargeId",
                    CreatedOn = DateTimeOffset.UtcNow,
                    Status = ChargeStatus.Success,
                    Amount = 123.456m,
                    Currency = "USD",
                    Description = "Description",
                    CardNumber = "CardNumber",
                    Cvv = "Cvv",
                    ExpiryMonth = 1,
                    ExpiryYear = 2020
                };

                _db.Charges.Add(charge);
                await _db.SaveChangesAsync();

                // Act

                using var response = await _client.GetAsync($"api/charges/?id={charge.Id}");

                // Assert

                Assert.IsNotNull(response);
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

                using var responseStream = await response.Content.ReadAsStreamAsync();
                var responseCharge = await JsonSerializer.DeserializeAsync<GetChargeResponse>(responseStream);

                Assert.AreEqual(charge.Id, responseCharge.Id);
                Assert.AreEqual(charge.IdempotentKey, responseCharge.IdempotentKey);
                Assert.AreEqual(charge.CreatedOn, responseCharge.CreatedOn);
                Assert.AreEqual(charge.Status, responseCharge.Status);
                Assert.AreEqual(charge.Amount, responseCharge.Amount);
                Assert.AreEqual(charge.Currency, responseCharge.Currency);
                Assert.AreEqual(charge.Description, responseCharge.Description);
                Assert.AreEqual("******mber", responseCharge.CardNumber);
            }
        }
    }
}
