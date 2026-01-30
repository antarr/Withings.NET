# Withings.NET - User Documentation
Withings.NET is a wrapper around the Withings Health Data API.

You can install the latest version using Nuget

`Install-Package Withings.NET`

## Details

#### OAuth Authentication
Withings.NET handles the OAuth 1.0a authentication flow required by the Withings API.

1.  **Setup Credentials**

    First, configure your Withings API credentials.
    ```csharp
    var credentials = new WithingsCredentials();
    credentials.SetCallbackUrl("YOUR_CALLBACK_URL");
    credentials.SetConsumerProperties("YOUR_CONSUMER_KEY", "YOUR_CONSUMER_SECRET");
    ```

2.  **Get Request Token**

    Instantiate the `Authenticator` and obtain a request token. This token is used to generate the user authorization URL.
    ```csharp
    var authenticator = new Authenticator(credentials);
    var requestToken = await authenticator.GetRequestToken();
    var authorizationUrl = authenticator.UserRequestUrl(requestToken);

    // Redirect the user to authorizationUrl
    ```

3.  **Exchange for Access Token**

    After the user authorizes your app, they will be redirected to your callback URL with `oauth_verifier` and `userid`. Use the verifier to exchange the request token for an access token.
    ```csharp
    // In your callback handler
    var verifier = Request.QueryString["oauth_verifier"];
    var accessToken = await authenticator.ExchangeRequestTokenForAccessToken(requestToken, verifier);

    // Store these securely
    var oauthToken = accessToken.Key;
    var oauthSecret = accessToken.Secret;
    var userId = Request.QueryString["userid"];
    ```

#### Accessing User Data
Once you have the `oauthToken`, `oauthSecret`, and `userId`, you can access the user's data using the `WithingsClient`.

1.  **Initialize Client**
    ```csharp
    var client = new WithingsClient(credentials);
    ```

2.  **Fetch Data**

    Call the available methods to retrieve data. For example, to get activity measures:
    ```csharp
    var activity = await client.GetActivityMeasures(
        DateTime.Parse("2017-01-01"),
        DateTime.Parse("2017-03-30"),
        userId,
        oauthToken,
        oauthSecret
    );
    ```

    Other available methods include `GetSleepSummary`, `GetWorkouts`, `GetBodyMeasures`, etc. Check the `WithingsClient` class for all available methods and their signatures.
