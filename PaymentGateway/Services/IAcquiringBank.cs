using System.Threading.Tasks;

namespace PaymentGateway.Services
{
    public interface IAcquiringBank
    {
        Task<bool> TrySendAsync(AcquiringBankRequest request, out string id);
    }
}
