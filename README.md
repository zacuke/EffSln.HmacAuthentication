# HMAC Authentication

## Server-Side Authentication

### Configuration (.NET 9+ Minimal API)

**appsettings.json**
```json
{
  "HmacAuthentication": {
    "ExcludedPaths": ["/health", "/swagger"],
    "ApiKeys": {
      "sk_live_key1234": "whsec_prod_abcde",
      "sk_test_key1234": "whsec_test_abcde"
    }
  }
}
```

**Program.cs**
```csharp
builder.Services.Configure<HmacAuthenticationOptions>(builder.Configuration.GetSection("HmacAuthentication"));
builder.Services.AddHmacAuthentication();
```

## Client-Side Authentication

### Configuration (.NET 9+ Minimal API)

**appsettings.json**
```json
{
  "HmacAuthClient": {
    "ApiKey": "your-api-key",
    "ApiSecret": "your-api-secret"
  }
}
```

**Program.cs**
```csharp
builder.Services.Configure<HmacAuthClientOptions>(builder.Configuration.GetSection("HmacAuthClient"));

builder.Services.AddHttpClient("MyApiClient")
    .AddHmacAuthenticationHandler();
```

### Using the Client
```csharp
var client = _httpClientFactory.CreateClient("MyApiClient");
var response = await client.GetAsync("/api/endpoint");
```

