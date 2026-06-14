using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MonriPaymentGateway;
using MonriPaymentGateway.Models;

var services = new ServiceCollection();

services.AddLogging();

services.AddMonriPaymentGateway(options =>
{
    options.BaseUrl = "https://ipgtest.monri.com/";
    options.PaymentUrl = "https://ipgtest.monri.com/payment/new";
    options.ApiKey = "demo-api-key";
    options.MerchantId = "demo-merchant-id";
    options.SuccessUrlBase = "https://localhost:5001/payments/monri/success";
});

using var serviceProvider = services.BuildServiceProvider();
var monriClient = serviceProvider.GetRequiredService<IMonriPaymentClient>();

var paymentRequest = new MonriPaymentRequest
{
    Customer = new MonriCustomer
    {
        FullName = "Demo Customer",
        Country = "RS",
        City = "Belgrade",
        StreetName = "Demo street",
        StreetNumber = "1",
        ZipCode = "11000",
        Email = "customer@example.com",
        PhoneNumber = "+381600000000"
    },
    OrderNumber = "ORDER-1001",
    OrderInfo = "Demo Monri payment",
    Amount = 25.99m,
    Currency = new MonriCurrency("EUR", 100),
    Language = "en",
    Timestamp = DateTime.UtcNow
};

var payment = monriClient.CreatePayment(paymentRequest);

Console.WriteLine("Monri payment form data");
Console.WriteLine($"Url: {payment.PaymentForm?.Url}");
Console.WriteLine($"MerchantId: {payment.PaymentForm?.MerchantId}");
Console.WriteLine($"OrderNumber: {payment.PaymentForm?.OrderNumber}");
Console.WriteLine($"Amount minor units: {payment.PaymentForm?.Amount}");
Console.WriteLine($"Currency: {payment.PaymentForm?.Currency}");
Console.WriteLine($"Digest: {payment.PaymentForm?.Digest}");
Console.WriteLine();

var responseCode = "0000";
Console.WriteLine($"Is success response '{responseCode}': {monriClient.IsSuccessResponse(responseCode)}");

var callbackBody = """{"order_number":"ORDER-1001","status":"approved"}""";
var callbackDigest = ToSha512Hash("demo-merchant-id" + callbackBody);
var callbackAuthorization = $"WP3-callback {callbackDigest}";

Console.WriteLine($"Is valid callback: {monriClient.IsValidCallback(callbackBody, callbackAuthorization)}");

static string ToSha512Hash(string dataToHash)
{
    var dataBytes = Encoding.UTF8.GetBytes(dataToHash);
    var hashBytes = SHA512.HashData(dataBytes);

    var digestBuilder = new StringBuilder();
    foreach (var b in hashBytes)
    {
        digestBuilder.Append(b.ToString("x2"));
    }

    return digestBuilder.ToString();
}
