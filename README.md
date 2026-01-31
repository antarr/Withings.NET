
.NET Client for interacting with Withings API v2 (OAuth 2.0)

[![Build status](https://ci.appveyor.com/api/projects/status/lw9pd7gbdjgck3sq?svg=true)](https://ci.appveyor.com/project/atbyrd/withings-net)
[![Coverage Status](https://coveralls.io/repos/github/atbyrd/Withings.NET/badge.svg?branch=master)](https://coveralls.io/github/atbyrd/Withings.NET?branch=master)
[![codecov](https://codecov.io/gh/atbyrd/Withings.NET/branch/master/graph/badge.svg)](https://codecov.io/gh/atbyrd/Withings.NET)
[![Documentation Status](https://readthedocs.org/projects/withingsnet/badge/?version=latest)](http://withingsnet.readthedocs.io/en/latest/?badge=latest)

[![NuGet](https://img.shields.io/nuget/v/Nuget.Core.svg?style=plastic)](https://www.nuget.org/packages/Withings.NET)

## USAGE

### Authorization - Getting user authorization url
```csharp
var credentials = new WithingsCredentials();
credentials.SetClientProperties("client_id", "client_secret");
credentials.SetCallbackUrl("http://localhost:49294/callback");

var authenticator = new Authenticator(credentials);
var url = authenticator.GetAuthorizeUrl("state", "user.info,user.metrics");
// Redirect user to url
```

### Exchanging code for token
```csharp
var authResponse = await authenticator.GetAccessToken("authorization_code");
var accessToken = authResponse.AccessToken;
```

### Fetching Data
```csharp
var client = new WithingsClient();
var data = await client.GetActivityMeasures(DateTime.Now.AddDays(-1), DateTime.Now, "userid", accessToken);
```

## CHANGE LOG

Version: 3.0.0 |
Release Date: Current |
Breaking Changes |
Migrated to OAuth 2.0.
Removed OAuth 1.0 support.
Updated project to SDK style (netstandard2.0).
Renamed `ConsumerKey`/`ConsumerSecret` to `ClientId`/`ClientSecret`.
Methods in `WithingsClient` now accept `accessToken` instead of `token` and `secret`.
Removed `AsyncOAuth` dependency.
Added `Flurl.Http` 3.x+ support.

Version: 2.1.0 |
Release Date: April 03, 2017 |
New Features |
Get Ability To Get Body Measures

...
