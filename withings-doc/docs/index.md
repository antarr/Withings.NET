# Withings.NET

Withings.NET is a .NET client for the [Withings Health Data API](https://developer.withings.com/).

## Installation

```
dotnet add package Withings.NET
```

Supports .NET 8.0, 9.0, and 10.0.

## Authentication

Withings uses OAuth 2.0 for authentication. You'll need a Client ID and Client Secret from the [Withings Developer Portal](https://developer.withings.com/).

### Setup

```csharp
using Withings.NET.Client;
using Withings.NET.Models;

var credentials = new WithingsCredentials();
credentials.SetClientProperties("your_client_id", "your_client_secret");
credentials.SetCallbackUrl("http://localhost:8585/callback");

var authenticator = new Authenticator(credentials);
```

### Authorization Flow

**Step 1:** Redirect the user to the Withings authorization page:

```csharp
var url = authenticator.GetAuthCodeUrl("user.info,user.metrics,user.activity", state);
// Redirect user to this URL
```

**Step 2:** After the user authorizes, Withings redirects to your callback URL with a `code` parameter. Exchange it for tokens:

```csharp
var token = await authenticator.GetAccessToken(code);
// token.AccessToken - use for API calls
// token.RefreshToken - save for later use
// token.UserId - the Withings user ID
// token.ExpiresIn - token lifetime in seconds
```

**Step 3:** Refresh the token when it expires:

```csharp
var newToken = await authenticator.RefreshAccessToken(token.RefreshToken);
```

Note: Withings refresh tokens are single-use. Always save the new refresh token from the response.

## Accessing Health Data

```csharp
var client = new WithingsClient(credentials);
```

All data methods return an `ExpandoObject` containing the raw API response.

### Activity Measures

```csharp
// By date range
var activity = await client.GetActivityMeasures(startDate, endDate, userId, accessToken);

// By single date
var daily = await client.GetActivityMeasures(date, userId, accessToken);
```

### Body Measures

```csharp
// By date range
var body = await client.GetBodyMeasures(userId, startDate, endDate, accessToken);

// Since last update
var recent = await client.GetBodyMeasures(userId, lastUpdateDate, accessToken);
```

### Sleep

```csharp
// Sleep summary (date strings: "yyyy-MM-dd")
var summary = await client.GetSleepSummary("2024-01-01", "2024-01-31", accessToken);

// Sleep measures (DateTime with unix timestamps)
var measures = await client.GetSleepMeasures(userId, startDate, endDate, accessToken);
```

### Workouts

```csharp
var workouts = await client.GetWorkouts("2024-01-01", "2024-01-31", accessToken);
```

### Intraday Activity

```csharp
var intraday = await client.GetIntraDayActivity(userId, startDate, endDate, accessToken);
```

### Heart

```csharp
// List ECG recordings
var heartList = await client.GetHeartList(startDate, endDate, accessToken);

// Get a specific ECG recording
var recording = await client.GetHeartRecording("signal_id", accessToken);
```

### User

```csharp
// List devices associated with the user
var devices = await client.GetDevices(accessToken);

// Get user goals
var goals = await client.GetGoals(accessToken);
```

### Webhook Subscriptions (Nudge)

```csharp
// Subscribe to notifications (appli: 1=Weight, 2=Temperature, 4=Blood Pressure, etc.)
await client.Subscribe("https://example.com/webhook", 1, accessToken);

// List active subscriptions
var subscriptions = await client.ListSubscriptions(1, accessToken);

// Get a specific subscription
var sub = await client.GetSubscription("https://example.com/webhook", 1, accessToken);

// Revoke a subscription
await client.RevokeSubscription("https://example.com/webhook", 1, accessToken);
```

## Error Handling

API errors throw `WithingsApiException` with the status code from the Withings API:

```csharp
try
{
    var token = await authenticator.GetAccessToken(code);
}
catch (WithingsApiException ex)
{
    Console.WriteLine($"Status: {ex.StatusCode}"); // e.g., 503
    Console.WriteLine(ex.Message); // "Withings API Error: 503"
}
```

See the [Withings API documentation](https://developer.withings.com/api-reference/) for status code definitions.
