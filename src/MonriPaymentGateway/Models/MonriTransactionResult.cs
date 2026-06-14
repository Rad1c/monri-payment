namespace MonriPaymentGateway.Models;

public sealed record MonriTransactionResult(bool IsSuccess, string? Status);
