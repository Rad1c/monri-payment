namespace MonriPaymentGateway.Models;

public sealed record MonriTokenizedCardTransactionRequest
{
    public required string TransactionType { get; init; }
    public decimal Amount { get; init; }
    public required MonriCurrency Currency { get; init; }
    public required string IpAddress { get; init; }
    public required string OrderInfo { get; init; }
    public required string Address { get; init; }
    public required string City { get; init; }
    public required string Country { get; init; }
    public required string Email { get; init; }
    public required string FullName { get; init; }
    public required string Phone { get; init; }
    public required string PostalCode { get; init; }
    public required string OrderNumber { get; init; }
    public required string PanToken { get; init; }
    public required string Language { get; init; }
}
