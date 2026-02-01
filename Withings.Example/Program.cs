using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
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
var session = new Dictionary<string, string>();

app.MapGet("/", () => Results.Redirect("/api/oauth/authorize", permanent: true));

app.MapGet("/api/oauth/authorize", async () =>
{
    var requestToken = await authenticator.GetRequestToken();
    session["RequestToken"] = JsonSerializer.Serialize(requestToken);
    return Results.Redirect(authenticator.UserRequestUrl(requestToken));
});

app.MapGet("/api/oauth/callback", async (HttpContext context) =>
{
    var query = context.Request.Query;

    var userId = (string)query["userid"];
    if (string.IsNullOrWhiteSpace(userId))
        return Results.BadRequest("Missing required query parameter 'userid'.");

    var verifier = (string)query["oauth_verifier"];
    if (string.IsNullOrWhiteSpace(verifier))
        return Results.BadRequest("Missing required query parameter 'oauth_verifier'.");

    if (!session.TryGetValue("RequestToken", out var requestTokenJson) || string.IsNullOrWhiteSpace(requestTokenJson))
        return Results.BadRequest("OAuth flow has not been started or request token is missing.");

    var requestToken = JsonSerializer.Deserialize<OAuthToken>(requestTokenJson);
    if (requestToken == null)
        return Results.BadRequest("Stored request token is invalid or could not be deserialized.");

    session["UserId"] = userId;

    var accessCredentials = await authenticator.ExchangeRequestTokenForAccessToken(
        requestToken,
        verifier);
    session["OAuthToken"] = accessCredentials.Key;
    session["OAuthSecret"] = accessCredentials.Secret;
    return Results.Json(accessCredentials);
});

app.MapGet("/api/withings/activity", async () =>
{
    var client = new WithingsClient(credentials);
    var activity = await client.GetActivityMeasures(
        DateTime.Parse("2017-01-01"),
        DateTime.Parse("2017-03-30"),
        session["UserId"],
        session["OAuthToken"],
        session["OAuthSecret"]);
    return Results.Json(activity);
});

app.MapGet("/api/withings/dailyactivity", async () =>
{
    var client = new WithingsClient(credentials);
    var activity = await client.GetActivityMeasures(
        DateTime.Today.AddDays(-30),
        session["UserId"],
        session["OAuthToken"],
        session["OAuthSecret"]);
    return Results.Json(activity);
});

app.MapGet("/api/withings/sleepsummary", async () =>
{
    var client = new WithingsClient(credentials);
    var activity = await client.GetSleepSummary(
        "2017-01-01",
        "2017-03-30",
        session["OAuthToken"],
        session["OAuthSecret"]);
    return Results.Json(activity);
});

app.MapGet("/api/withings/workouts", async () =>
{
    var client = new WithingsClient(credentials);
    var activity = await client.GetWorkouts(
        "2017-06-01",
        "2017-06-05",
        session["OAuthToken"],
        session["OAuthSecret"]);
    return Results.Json(activity);
});

app.MapGet("/api/withings/sleepmeasures", async () =>
{
    var client = new WithingsClient(credentials);
    var activity = await client.GetSleepMeasures(
        session["UserId"],
        DateTime.Now.AddDays(-90),
        DateTime.Now.AddDays(-1),
        session["OAuthToken"],
        session["OAuthSecret"]);
    return Results.Json(activity);
});

app.MapGet("/api/withings/body", async () =>
{
    var client = new WithingsClient(credentials);
    var activity = await client.GetBodyMeasures(
        session["UserId"],
        DateTime.Parse("2017-05-08"),
        DateTime.Parse("2017-05-10"),
        session["OAuthToken"],
        session["OAuthSecret"]);
    return Results.Json(activity);
});

app.MapGet("/api/withings/bodysince", async () =>
{
    var client = new WithingsClient(credentials);
    var activity = await client.GetBodyMeasures(
        session["UserId"],
        DateTime.Parse("2017-05-08"),
        session["OAuthToken"],
        session["OAuthSecret"]);
    return Results.Json(activity);
});

app.MapGet("/api/withings/intraday", async () =>
{
    var client = new WithingsClient(credentials);
    var activity = await client.GetIntraDayActivity(
        session["UserId"],
        DateTime.Now.AddDays(-90),
        DateTime.Now.AddDays(-1),
        session["OAuthToken"],
        session["OAuthSecret"]);
    return Results.Json(activity);
});

app.MapGet("/api/oauth/requesttoken", async () =>
{
    var token = await authenticator.GetRequestToken();
    return Results.Json(token);
});

app.Run("http://0.0.0.0:8080");
