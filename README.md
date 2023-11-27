# Pointauc.Api

An official .NET library for interacting with https://pointauc.com.

Support author of https://pointauc.com [@kozjar](https://github.com/Kozjar)

[![](https://static.donationalerts.ru/uploads/qr/4936007/qr_b48bc5f13970efaf8cc3428d69932df4.png)](https://www.donationalerts.com/r/kozjar)

# Download

[![Nuget](https://img.shields.io/nuget/v/Pointauc.Api.svg?logo=nuget)](https://www.nuget.org/packages/Pointauc.Api)

# Example

1. You need to authorize with one of the existing integrations to create an account.
2. Navigate to the https://pointauc.com/settings.
3. Find “Personal Token” section at the bottom and copy that value.
4. You should put this token into Client initialization.

```CSharp
// Init client.
IPointaucClient client = new PointaucClient("token_value");

// Init Bid.
Bid bid = new Bid()
{
	Cost = 100,
	Message = "Test message",
};

// Make a bid.
await client.Bids(bid);
```
