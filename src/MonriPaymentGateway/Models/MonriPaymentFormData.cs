using System.Text.Json;

namespace MonriPaymentGateway.Models;

public sealed record MonriPaymentFormData
{
    public string? MerchantId { get; init; }
    public string? Url { get; init; }
    public string? Digest { get; init; }
    public string? OrderNumber { get; init; }
    public string? OrderInfo { get; init; }
    public int Amount { get; init; }
    public string? Currency { get; init; }
    public DateTime? Timestamp { get; init; }
    public JsonDocument? CustomParams { get; init; }
    public string? ReferenceNumber { get; init; }
}
