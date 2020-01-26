using System.Threading.Tasks;

namespace Payments.Services
{
    public interface IAcquiringBank
    {
        Task<bool> TrySendAsync(AcquiringBankRequest request, out string id);
    }
}
