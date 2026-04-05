# Withings.NET

.NET client for the [Withings Health Data API](https://developer.withings.com/) (OAuth 2.0).

[![NuGet](https://img.shields.io/nuget/v/Withings.NET.svg)](https://www.nuget.org/packages/Withings.NET)

## Requirements

- .NET 8.0, 9.0, or 10.0

## Installation

```
dotnet add package Withings.NET
```

## Quick Start

### 1. Configure Credentials

```csharp
var credentials = new WithingsCredentials();
credentials.SetClientProperties("your_client_id", "your_client_secret");
credentials.SetCallbackUrl("http://localhost:8585/callback");
```

### 2. OAuth 2.0 Authorization

```csharp
var authenticator = new Authenticator(credentials);

// Generate the authorization URL and redirect the user
var url = authenticator.GetAuthCodeUrl("user.info,user.metrics,user.activity", state);

// After the user authorizes, exchange the code for tokens
var token = await authenticator.GetAccessToken(authorizationCode);

// Refresh tokens when they expire
var newToken = await authenticator.RefreshAccessToken(token.RefreshToken);
```

### 3. Fetch Health Data

```csharp
var client = new WithingsClient(credentials);

// Activity measures
var activity = await client.GetActivityMeasures(startDate, endDate, userId, accessToken);

// Body measures
var body = await client.GetBodyMeasures(userId, startDate, endDate, accessToken);

// Sleep summary
var sleep = await client.GetSleepSummary("2024-01-01", "2024-01-31", accessToken);

// Sleep measures
var sleepData = await client.GetSleepMeasures(userId, startDate, endDate, accessToken);

// Workouts
var workouts = await client.GetWorkouts("2024-01-01", "2024-01-31", accessToken);

// Intraday activity
var intraday = await client.GetIntraDayActivity(userId, startDate, endDate, accessToken);

// Heart
var heartList = await client.GetHeartList(startDate, endDate, accessToken);
var recording = await client.GetHeartRecording(signalId, accessToken);

// User
var devices = await client.GetDevices(accessToken);
var goals = await client.GetGoals(accessToken);

// Webhook subscriptions
await client.Subscribe(callbackUrl, appli, accessToken);
var subscriptions = await client.ListSubscriptions(appli, accessToken);
await client.RevokeSubscription(callbackUrl, appli, accessToken);
```

## Development

### Running Unit Tests

```bash
dotnet test --filter "TestCategory!=E2E"
```

### Running E2E Tests

E2E tests run against the live Withings API. You need to bootstrap OAuth tokens first.

1. Create a `.env` file in the project root:

```
WITHINGS_CLIENT_ID=your_client_id
WITHINGS_CLIENT_SECRET=your_client_secret
WITHINGS_CALLBACK_URL=http://localhost:8585/callback
WITHINGS_REFRESH_TOKEN=
WITHINGS_USER_ID=
```

2. Run the bootstrap script to obtain tokens:

```bash
./scripts/bootstrap-token.sh
```

3. Run E2E tests:

```bash
dotnet test --filter "TestCategory=E2E" -f net10.0
```

The E2E test suite automatically saves new refresh tokens back to `.env` after each run (Withings refresh tokens are single-use).

## API Reference

| Class | Description |
|-------|-------------|
| `Authenticator` | OAuth 2.0 authorization and token management |
| `WithingsClient` | API client for health data endpoints |
| `OAuthToken` | Token response with access token, refresh token, and user ID |
| `WithingsApiException` | Exception thrown for non-zero API status codes |

## License

See [LICENSE](LICENSE) for details.
