using System;
using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;

namespace PaymentGatewayClient
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPaymentGatewayClient(this IServiceCollection services, string apiKey, Uri baseAddress)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            if (apiKey is null) throw new ArgumentNullException(nameof(apiKey));
            if (baseAddress is null) throw new ArgumentNullException(nameof(baseAddress));

            services.AddHttpClient<IChargesClient, ChargesClient>(c =>
            {
                c.BaseAddress = baseAddress;
                c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                c.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(new ProductHeaderValue("PaymentGatewayClient", "1.0.0")));
                c.DefaultRequestHeaders.Add("X-Api-Key", apiKey);
            });

            return services;
        }
    }
}
