using System;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace PaymentGateway.Tests
{
    public class TestWebApplicationFactory : WebApplicationFactory<Startup>
    {
        public HttpClient CreateTestClient(string testName, Action<IServiceCollection> servicesConfiguration)
        {
            var instance = WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddInMemoryDatabase(testName);
                    servicesConfiguration(services);
                });
            });

            var client = instance.CreateClient();            
            return client;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseSetting("https_port", "5000");
        }
    }
}
