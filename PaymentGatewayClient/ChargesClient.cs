using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using PaymentGatewayClient.Contracts;

namespace PaymentGatewayClient
{
    public class ChargesClient : IChargesClient
    {
        private readonly HttpClient _client;

        public ChargesClient(HttpClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<Charge?> FindAsync(Guid id)
        {
            using var response = await _client.GetAsync($"api/charges/?id={id}", HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            using var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

            var result = await JsonSerializer.DeserializeAsync<Charge>(stream).ConfigureAwait(false);
            return result;
        }

        public async Task<Charge> CreateAsync(CreateChargeRequest request)
        {
            if (request is null) throw new ArgumentNullException(nameof(request));

            using var content = new JsonHttpContent<CreateChargeRequest>(request);
            
            using var response = await _client.PostAsync("api/charges", content).ConfigureAwait(false);
            using var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            
            var result = await JsonSerializer.DeserializeAsync<Charge>(stream).ConfigureAwait(false);
            return result;
        }
    }
}
