using System;
using System.Configuration;
using System.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using AsyncOAuth;
using Withings.NET.Client;
using Withings.NET.Models;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var credentials = new WithingsCredentials();
credentials.SetCallbackUrl(Environment.GetEnvironmentVariable("WithingsCallbackUrl"));
credentials.SetConsumerProperties(
    Environment.GetEnvironmentVariable("WithingsConsumerKey"),
    Environment.GetEnvironmentVariable("WithingsConsumerSecret"));

var authenticator = new Authenticator(credentials);

app.MapGet("/", () => Results.Redirect("/api/oauth/authorize", permanent: true));

app.MapGet("/api/oauth/authorize", async () =>
{
    var requestToken = await authenticator.GetRequestToken();
    ConfigurationManager.AppSettings["RequestToken"] = JsonConvert.SerializeObject(requestToken);
    return Results.Redirect(authenticator.UserRequestUrl(requestToken));
});

app.MapGet("/api/oauth/callback", async (HttpContext context) =>
{
    var query = HttpUtility.ParseQueryString(context.Request.QueryString.Value ?? "");

    var userId = query["userid"];
    if (string.IsNullOrWhiteSpace(userId))
    {
        return Results.BadRequest("Missing required query parameter 'userid'.");
    }

    var verifier = query["oauth_verifier"];
    if (string.IsNullOrWhiteSpace(verifier))
    {
        return Results.BadRequest("Missing required query parameter 'oauth_verifier'.");
    }

    var requestTokenJson = ConfigurationManager.AppSettings["RequestToken"];
    if (string.IsNullOrWhiteSpace(requestTokenJson))
    {
        return Results.BadRequest("OAuth flow has not been started or request token is missing.");
    }

    var requestToken = JsonConvert.DeserializeObject<RequestToken>(requestTokenJson);
    if (requestToken == null)
    {
        return Results.BadRequest("Stored request token is invalid or could not be deserialized.");
    }

    ConfigurationManager.AppSettings["UserId"] = userId;

    var accessCredentials = await authenticator.ExchangeRequestTokenForAccessToken(
        requestToken,
        verifier);
    ConfigurationManager.AppSettings["OAuthToken"] = accessCredentials.Key;
    ConfigurationManager.AppSettings["OAuthSecret"] = accessCredentials.Secret;
    return Results.Json(accessCredentials);
});

app.MapGet("/api/withings/activity", async () =>
{
    var client = new WithingsClient(credentials);
    var activity = await client.GetActivityMeasures(
        DateTime.Parse("2017-01-01"),
        DateTime.Parse("2017-03-30"),
        ConfigurationManager.AppSettings["UserId"],
        ConfigurationManager.AppSettings["OAuthToken"],
        ConfigurationManager.AppSettings["OAuthSecret"]);
    return Results.Json(activity);
});

app.MapGet("/api/withings/dailyactivity", async () =>
{
    var client = new WithingsClient(credentials);
    var activity = await client.GetActivityMeasures(
        DateTime.Today.AddDays(-30),
        ConfigurationManager.AppSettings["UserId"],
        ConfigurationManager.AppSettings["OAuthToken"],
        ConfigurationManager.AppSettings["OAuthSecret"]);
    return Results.Json(activity);
});

app.MapGet("/api/withings/sleepsummary", async () =>
{
    var client = new WithingsClient(credentials);
    var activity = await client.GetSleepSummary(
        "2017-01-01",
        "2017-03-30",
        ConfigurationManager.AppSettings["OAuthToken"],
        ConfigurationManager.AppSettings["OAuthSecret"]);
    return Results.Json(activity);
});

app.MapGet("/api/withings/workouts", async () =>
{
    var client = new WithingsClient(credentials);
    var activity = await client.GetWorkouts(
        "2017-06-01",
        "2017-06-05",
        ConfigurationManager.AppSettings["OAuthToken"],
        ConfigurationManager.AppSettings["OAuthSecret"]);
    return Results.Json(activity);
});

app.MapGet("/api/withings/sleepmeasures", async () =>
{
    var client = new WithingsClient(credentials);
    var activity = await client.GetSleepMeasures(
        ConfigurationManager.AppSettings["UserId"],
        DateTime.Now.AddDays(-90),
        DateTime.Now.AddDays(-1),
        ConfigurationManager.AppSettings["OAuthToken"],
        ConfigurationManager.AppSettings["OAuthSecret"]);
    return Results.Json(activity);
});

app.MapGet("/api/withings/body", async () =>
{
    var client = new WithingsClient(credentials);
    var activity = await client.GetBodyMeasures(
        ConfigurationManager.AppSettings["UserId"],
        DateTime.Parse("2017-05-08"),
        DateTime.Parse("2017-05-10"),
        ConfigurationManager.AppSettings["OAuthToken"],
        ConfigurationManager.AppSettings["OAuthSecret"]);
    return Results.Json(activity);
});

app.MapGet("/api/withings/bodysince", async () =>
{
    var client = new WithingsClient(credentials);
    var activity = await client.GetBodyMeasures(
        ConfigurationManager.AppSettings["UserId"],
        DateTime.Parse("2017-05-08"),
        ConfigurationManager.AppSettings["OAuthToken"],
        ConfigurationManager.AppSettings["OAuthSecret"]);
    return Results.Json(activity);
});

app.MapGet("/api/withings/intraday", async () =>
{
    var client = new WithingsClient(credentials);
    var activity = await client.GetIntraDayActivity(
        ConfigurationManager.AppSettings["UserId"],
        DateTime.Now.AddDays(-90),
        DateTime.Now.AddDays(-1),
        ConfigurationManager.AppSettings["OAuthToken"],
        ConfigurationManager.AppSettings["OAuthSecret"]);
    return Results.Json(activity);
});

app.MapGet("/api/oauth/requesttoken", async () =>
{
    var token = await authenticator.GetRequestToken();
    return Results.Json(token);
});

app.Run("http://0.0.0.0:8080");
