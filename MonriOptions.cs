namespace MonriPaymentGateway;

public sealed class MonriOptions
{
    public bool Enabled { get; set; } = true;
    public string BaseUrl { get; set; } = string.Empty;
    public string PaymentUrl { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string MerchantId { get; set; } = string.Empty;
    public string SuccessUrlBase { get; set; } = string.Empty;
}
