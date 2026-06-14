using System.Text.Json;

namespace MonriPaymentGateway.Models;

public sealed record MonriPaymentRequest
{
    public MonriCustomer? Customer { get; init; }
    public required string OrderNumber { get; init; }
    public required string OrderInfo { get; init; }
    public decimal Amount { get; init; }
    public required MonriCurrency Currency { get; init; }
    public string? Language { get; init; }
    public DateTime? Timestamp { get; init; }
    public JsonDocument? CustomParams { get; init; }
}
