using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MonriPaymentGateway.Models;

namespace MonriPaymentGateway;

public sealed class MonriPaymentClient : IMonriPaymentClient
{
    private const string CallbackScheme = "WP3-callback";
    private const string SuccessResponseCode = "0000";
    private const string ApprovedTransactionStatus = "approved";

    private readonly HttpClient _httpClient;
    private readonly MonriOptions _options;
    private readonly ILogger<MonriPaymentClient> _logger;

    public MonriPaymentClient(
        IOptions<MonriOptions> options,
        HttpClient httpClient,
        ILogger<MonriPaymentClient> logger)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(httpClient);
        ArgumentNullException.ThrowIfNull(logger);

        _options = options.Value;
        _httpClient = httpClient;
        _logger = logger;
    }

    public string CalculateDigest(string orderNumber, decimal amount, MonriCurrency currency)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(orderNumber);
        ArgumentNullException.ThrowIfNull(currency);

        var dataToHash = $"{_options.ApiKey}{orderNumber}{GetAmountInMinorUnits(amount, currency)}{currency.IsoCode}";

        return ToSha512Hash(dataToHash);
    }

    public int GetAmountInMinorUnits(decimal amount, MonriCurrency currency)
    {
        ArgumentNullException.ThrowIfNull(currency);

        return (int)(amount * currency.MinorUnitConversionFactor);
    }

    public MonriPaymentResponse CreatePayment(MonriPaymentRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return CreatePaymentResponse(request);
    }

    public MonriPaymentResponse InitSaveCard(MonriPaymentRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return CreatePaymentResponse(request);
    }

    public async Task<MonriTransactionResult> CreateTransactionWithPanTokenAsync(
        MonriTokenizedCardTransactionRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var payload = new
        {
            transaction = new
            {
                transaction_type = request.TransactionType,
                amount = GetAmountInMinorUnits(request.Amount, request.Currency),
                ip = request.IpAddress,
                order_info = request.OrderInfo,
                ch_address = request.Address,
                ch_city = request.City,
                ch_country = request.Country,
                ch_email = request.Email,
                ch_full_name = request.FullName,
                ch_phone = request.Phone,
                ch_zip = request.PostalCode,
                currency = request.Currency.IsoCode,
                digest = CalculateDigest(request.OrderNumber, request.Amount, request.Currency),
                order_number = request.OrderNumber,
                authenticity_token = _options.MerchantId,
                language = request.Language,
                pan_token = request.PanToken,
                moto = true
            }
        };

        using var response = await _httpClient.PostAsJsonAsync("v2/transaction", payload, cancellationToken);
        var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);

        _logger.LogInformation("Monri transaction response: {Response}", responseJson);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<TransactionResponseRoot>(cancellationToken);
        var status = result?.Transaction?.Status;

        return new MonriTransactionResult(
            string.Equals(ApprovedTransactionStatus, status, StringComparison.OrdinalIgnoreCase),
            status);
    }

    public bool IsValidReturnDigest(string requestUrl)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(requestUrl);

        var queryIndex = requestUrl.IndexOf('?');
        if (queryIndex < 0)
        {
            return false;
        }

        var normalizedRequest = string.Concat(_options.SuccessUrlBase, requestUrl.AsSpan(queryIndex));
        var requestWithoutDigest = GetRequestUrlWithoutDigest(normalizedRequest);
        var digestFromRequest = GetDigestFromRequestUrl(normalizedRequest);

        if (string.IsNullOrWhiteSpace(digestFromRequest))
        {
            return false;
        }

        var calculatedDigest = ToSha512Hash($"{_options.ApiKey}{requestWithoutDigest}");

        return string.Equals(calculatedDigest, digestFromRequest, StringComparison.OrdinalIgnoreCase);
    }

    public bool IsValidCallback(string requestBody, string? authorizationHeader)
    {
        if (string.IsNullOrWhiteSpace(requestBody) ||
            string.IsNullOrWhiteSpace(authorizationHeader) ||
            string.IsNullOrWhiteSpace(_options.MerchantId))
        {
            return false;
        }

        if (!authorizationHeader.StartsWith(CallbackScheme, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var providedDigest = authorizationHeader[CallbackScheme.Length..].Trim();
        if (string.IsNullOrWhiteSpace(providedDigest))
        {
            return false;
        }

        var recalculatedDigest = ToSha512Hash(_options.MerchantId + requestBody);

        return string.Equals(providedDigest, recalculatedDigest, StringComparison.OrdinalIgnoreCase);
    }

    public bool IsSuccessResponse(string? responseCode)
    {
        return string.Equals(responseCode, SuccessResponseCode, StringComparison.Ordinal);
    }

    private MonriPaymentResponse CreatePaymentResponse(MonriPaymentRequest request)
    {
        var paymentForm = new MonriPaymentFormData
        {
            MerchantId = _options.MerchantId,
            Url = _options.PaymentUrl,
            Digest = CalculateDigest(request.OrderNumber, request.Amount, request.Currency),
            OrderNumber = request.OrderNumber,
            OrderInfo = request.OrderInfo,
            Amount = GetAmountInMinorUnits(request.Amount, request.Currency),
            Currency = request.Currency.IsoCode,
            Timestamp = request.Timestamp,
            CustomParams = request.CustomParams,
            ReferenceNumber = request.OrderNumber
        };

        return new MonriPaymentResponse
        {
            FullName = request.Customer?.FullName,
            Country = request.Customer?.Country,
            City = request.Customer?.City,
            StreetName = request.Customer?.StreetName,
            StreetNumber = request.Customer?.StreetNumber,
            ZipCode = request.Customer?.ZipCode,
            Email = request.Customer?.Email,
            PhoneNumber = request.Customer?.PhoneNumber,
            DisplayLanguageCode = request.Language,
            PaymentForm = paymentForm
        };
    }

    private static string ToSha512Hash(string dataToHash)
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

    private static string GetRequestUrlWithoutDigest(string url)
    {
        var digestIndex = url.IndexOf("&digest", StringComparison.Ordinal);
        return digestIndex < 0 ? url : url[..digestIndex];
    }

    private static string? GetDigestFromRequestUrl(string url)
    {
        const string digestKey = "&digest=";
        var digestIndex = url.IndexOf(digestKey, StringComparison.Ordinal);

        return digestIndex < 0 ? null : url[(digestIndex + digestKey.Length)..];
    }

    private sealed class TransactionResponseRoot
    {
        [JsonPropertyName("transaction")]
        public TransactionInfo? Transaction { get; init; }
    }

    private sealed class TransactionInfo
    {
        [JsonPropertyName("status")]
        public string? Status { get; init; }
    }
}
