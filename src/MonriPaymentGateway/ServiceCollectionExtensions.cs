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

        services.AddOptions<MonriOptions>()
            .Configure(configureOptions)
            .Validate(ValidateOptions, "Monri payment gateway options are incomplete.");

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

    private static bool ValidateOptions(MonriOptions options)
    {
        return !string.IsNullOrWhiteSpace(options.PaymentUrl) &&
               !string.IsNullOrWhiteSpace(options.ApiKey) &&
               !string.IsNullOrWhiteSpace(options.MerchantId) &&
               !string.IsNullOrWhiteSpace(options.SuccessUrlBase);
    }
}
