using System;
using System.Threading.Tasks;

namespace PaymentGateway.Services
{
    public class MockAcquiringBank : IAcquiringBank
    {
        private readonly bool _response;

        public MockAcquiringBank(bool response)
        {
            _response = response;
        }

        public Task<bool> TrySendAsync(AcquiringBankRequest request, out string id)
        {
            id = Guid.NewGuid().ToString();

            return Task.FromResult(_response);
        }
    }
}
