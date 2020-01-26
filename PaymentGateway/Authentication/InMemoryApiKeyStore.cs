using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaymentGateway.Authentication
{
    public class InMemoryApiKeyStore : IApiKeyStore
    {
        private readonly Dictionary<string, ApiKey> _map;

        public InMemoryApiKeyStore(IDictionary<string, ApiKey> map)
        {
            if (map is null) throw new ArgumentNullException(nameof(map));

            _map = new Dictionary<string, ApiKey>(map);
        }

        public Task<ApiKey> FindAsync(string key)
        {
            if (key is null) throw new ArgumentNullException(nameof(key));

            _map.TryGetValue(key, out var entity);

            return Task.FromResult(entity);
        }
    }
}
