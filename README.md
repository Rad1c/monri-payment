# MonriPaymentGateway

Client library for Monri payment gateway integrations.

## Features

- Builds Monri payment form payloads.
- Calculates SHA-512 digests for payment and return URLs.
- Validates `WP3-callback` callback authorization headers.
- Creates transactions with saved `pan_token` values.

## Registration

```csharp
services.AddMonriPaymentGateway(options =>
{
    options.BaseUrl = configuration["PaymentProviders:Monri:BaseUrl"]!;
    options.PaymentUrl = configuration["PaymentProviders:Monri:Url"]!;
    options.ApiKey = configuration["PaymentProviders:Monri:ApiKey"]!;
    options.MerchantId = configuration["PaymentProviders:Monri:MerchantId"]!;
    options.SuccessUrlBase = configuration["PaymentProviders:Monri:SuccessUrlBase"]!;
});
```

## Usage

```csharp
var request = new MonriPaymentRequest
{
    OrderNumber = "order-123",
    OrderInfo = "Invoice 123",
    Amount = 10.50m,
    Currency = new MonriCurrency("EUR", 100),
    Language = "en"
};

var response = monriPaymentClient.CreatePayment(request);
```
