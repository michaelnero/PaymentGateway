using System.Threading.Tasks;

namespace PaymentGateway.Authentication
{
    public interface IApiKeyStore
    {
        Task<ApiKey> FindAsync(string key);
    }
}
