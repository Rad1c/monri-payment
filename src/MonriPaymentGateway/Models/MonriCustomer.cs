namespace MonriPaymentGateway.Models;

public sealed record MonriCustomer
{
    public string? FullName { get; init; }
    public string? Country { get; init; }
    public string? City { get; init; }
    public string? StreetName { get; init; }
    public string? StreetNumber { get; init; }
    public string? ZipCode { get; init; }
    public string? Email { get; init; }
    public string? PhoneNumber { get; init; }
}
