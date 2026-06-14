using MonriPaymentGateway.Models;

namespace MonriPaymentGateway;

public interface IMonriPaymentClient
{
    /// <summary>
    /// Calculates the SHA-512 digest required by Monri for a payment request.
    /// </summary>
    /// <param name="orderNumber">The merchant order number sent to Monri.</param>
    /// <param name="amount">The payment amount in major currency units.</param>
    /// <param name="currency">The currency metadata used to convert the amount into minor units.</param>
    /// <returns>The lowercase hexadecimal SHA-512 digest.</returns>
    string CalculateDigest(string orderNumber, decimal amount, MonriCurrency currency);

    /// <summary>
    /// Converts an amount from major currency units into Monri minor units.
    /// </summary>
    /// <param name="amount">The payment amount in major currency units.</param>
    /// <param name="currency">The currency metadata containing the minor unit conversion factor.</param>
    /// <returns>The amount represented in minor currency units.</returns>
    int GetAmountInMinorUnits(decimal amount, MonriCurrency currency);

    /// <summary>
    /// Builds the form data needed to redirect a customer to Monri for payment.
    /// </summary>
    /// <param name="request">The payment request data used to create the Monri form payload.</param>
    /// <returns>The customer data and Monri payment form fields.</returns>
    MonriPaymentResponse CreatePayment(MonriPaymentRequest request);

    /// <summary>
    /// Builds the form data needed to initialize a Monri save-card payment flow.
    /// </summary>
    /// <param name="request">The payment request data used to create the Monri save-card form payload.</param>
    /// <returns>The customer data and Monri payment form fields.</returns>
    MonriPaymentResponse InitSaveCard(MonriPaymentRequest request);

    /// <summary>
    /// Creates a Monri transaction by charging a previously saved card token.
    /// </summary>
    /// <param name="request">The transaction request containing the saved PAN token and payment details.</param>
    /// <param name="cancellationToken">A token used to cancel the HTTP request.</param>
    /// <returns>The transaction result returned by Monri.</returns>
    Task<MonriTransactionResult> CreateTransactionWithPanTokenAsync(
        MonriTokenizedCardTransactionRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates the digest on a Monri return URL.
    /// </summary>
    /// <param name="requestUrl">The absolute or relative return URL received from Monri, including query string.</param>
    /// <returns><c>true</c> when the digest matches the configured API key and success URL base; otherwise, <c>false</c>.</returns>
    bool IsValidReturnDigest(string requestUrl);

    /// <summary>
    /// Validates a Monri callback authorization header against the callback request body.
    /// </summary>
    /// <param name="requestBody">The raw callback request body.</param>
    /// <param name="authorizationHeader">The Monri authorization header value using the WP3-callback scheme.</param>
    /// <returns><c>true</c> when the callback digest is valid; otherwise, <c>false</c>.</returns>
    bool IsValidCallback(string requestBody, string? authorizationHeader);

    /// <summary>
    /// Determines whether a Monri response code represents a successful payment response.
    /// </summary>
    /// <param name="responseCode">The response code received from Monri.</param>
    /// <returns><c>true</c> when the response code is the Monri success code; otherwise, <c>false</c>.</returns>
    bool IsSuccessResponse(string? responseCode);
}
