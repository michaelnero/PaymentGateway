using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.Authentication;
using PaymentGateway.Model;

namespace PaymentGateway.Tests
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RemoveService<T>(this IServiceCollection services)
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(T));
            if (null != descriptor)
            {
                services.Remove(descriptor);
            }

            return services;
        }

        public static void AddInMemoryDatabase(this IServiceCollection services, string testName)
        {
            RemoveService<DbContextOptions<PaymentGatewayContext>>(services);

            services.AddDbContext<PaymentGatewayContext>(options =>
            {
                options.UseInMemoryDatabase(testName);
            });
        }

        public static void AddApiKeys(this IServiceCollection services, params string[] keys)
        {
            RemoveService<IApiKeyStore>(services);

            var map = new Dictionary<string, ApiKey>();
            for (int i = 0; i < keys.Length; i++)
            {
                map[keys[i]] = new ApiKey(i, $"Owner {i}", true);
            }

            services.AddSingleton<IApiKeyStore>(new InMemoryApiKeyStore(map));
        }
    }
}
