using System;
using System.Threading.Tasks;
using PaymentGatewayClient.Contracts;

namespace PaymentGatewayClient
{
    public interface IChargesClient
    {
        Task<Charge?> FindAsync(Guid id);

        Task<Charge> CreateAsync(CreateChargeRequest request);
    }
}
