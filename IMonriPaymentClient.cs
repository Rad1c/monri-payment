using MonriPaymentGateway.Models;

namespace MonriPaymentGateway;

public interface IMonriPaymentClient
{
    string CalculateDigest(string orderNumber, decimal amount, MonriCurrency currency);
    int GetAmountInMinorUnits(decimal amount, MonriCurrency currency);
    MonriPaymentResponse CreatePayment(MonriPaymentRequest request);
    MonriPaymentResponse InitSaveCard(MonriPaymentRequest request);
    Task<MonriTransactionResult> CreateTransactionWithPanTokenAsync(
        MonriTokenizedCardTransactionRequest request,
        CancellationToken cancellationToken = default);
    bool IsValidReturnDigest(string requestUrl);
    bool IsValidCallback(string requestBody, string? authorizationHeader);
    bool IsSuccessResponse(string? responseCode);
}
