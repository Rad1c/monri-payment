# MonriPaymentGateway

Client library for Monri payment gateway integrations.

## Repository layout

- `src/MonriPaymentGateway` contains the NuGet package source.
- `samples/MonriPaymentGateway.Demo` contains a runnable console demo.

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

## Demo project

Run the sample console app from this repository:

```powershell
dotnet run --project .\samples\MonriPaymentGateway.Demo\MonriPaymentGateway.Demo.csproj
```

## Pack

```powershell
dotnet pack .\src\MonriPaymentGateway\MonriPaymentGateway.csproj -c Release -o .\artifacts
```

## Publish

Publishing runs from `.github/workflows/publish.yml` when a version tag is pushed.

Before the first publish, configure NuGet trusted publishing for this repository:

- Repository owner: `Rad1c`
- Repository: `monri-payment`
- Workflow file: `publish.yml`
- Environment: `release`

Create a GitHub environment named `release` and add an environment secret named `NUGET_USER` with your nuget.org username.

```powershell
git tag v1.0.0
git push origin v1.0.0
```
