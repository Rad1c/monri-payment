using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace MonriPaymentGateway;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMonriPaymentGateway(
        this IServiceCollection services,
        Action<MonriOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configureOptions);

        services.Configure(configureOptions);
        services.AddHttpClient<IMonriPaymentClient, MonriPaymentClient>((serviceProvider, httpClient) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<MonriOptions>>().Value;
            if (!string.IsNullOrWhiteSpace(options.BaseUrl))
            {
                httpClient.BaseAddress = new Uri(options.BaseUrl);
            }
        });

        return services;
    }
}
