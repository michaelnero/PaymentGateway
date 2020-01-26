using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace PaymentGatewayClient
{
    internal class JsonHttpContent<T> : HttpContent where T : notnull
    {
        private readonly T _value;

        public JsonHttpContent(T value)
        {
            _value = value;

            Headers.ContentType = new MediaTypeHeaderValue("application/json");
        }

        protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            await JsonSerializer.SerializeAsync(stream, _value).ConfigureAwait(false);
        }

        protected override bool TryComputeLength(out long length)
        {
            length = -1;
            return false;
        }
    }
}
