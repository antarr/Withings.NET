using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Withings.NET.Client;
using Withings.NET.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMemoryCache();
var app = builder.Build();

var credentials = new WithingsCredentials();
credentials.SetCallbackUrl(Environment.GetEnvironmentVariable("WithingsCallbackUrl"));
credentials.SetConsumerProperties(
    Environment.GetEnvironmentVariable("WithingsConsumerKey"),
    Environment.GetEnvironmentVariable("WithingsConsumerSecret"));

var authenticator = new Authenticator(credentials);
var session = new Dictionary<string, string>();

var activityStartDate = DateTime.Parse("2017-01-01");
var activityEndDate = DateTime.Parse("2017-03-30");
var bodyStartDate = DateTime.Parse("2017-05-08");
var bodyEndDate = DateTime.Parse("2017-05-10");

app.MapGet("/", () => Results.Redirect("/api/oauth/authorize", permanent: true));

app.MapGet("/api/oauth/authorize", () =>
{
    var state = Guid.NewGuid().ToString();
    session["State"] = state;
    var scope = "user.info,user.metrics,user.activity";
    var url = authenticator.GetAuthCodeUrl(scope, state);
    return Results.Redirect(url);
});

app.MapGet("/api/oauth/callback", async (HttpContext context) =>
{
    var query = context.Request.Query;

    var code = (string)query["code"];
    if (string.IsNullOrWhiteSpace(code))
        return Results.BadRequest("Missing required query parameter 'code'.");

    var state = (string)query["state"];
    if (string.IsNullOrWhiteSpace(state))
        return Results.BadRequest("Missing required query parameter 'state'.");

    if (!session.TryGetValue("State", out var storedState) || storedState != state)
         return Results.BadRequest("Invalid state.");

    var token = await authenticator.GetAccessToken(code);

    session["AccessToken"] = token.AccessToken;
    session["RefreshToken"] = token.RefreshToken;
    session["UserId"] = token.UserId;

    return Results.Json(token);
});

app.MapGet("/api/withings/activity", async (IMemoryCache cache) =>
{
    var start = DateTime.Parse("2017-01-01");
    var end = DateTime.Parse("2017-03-30");
    var userId = session["UserId"];
    var accessToken = session["AccessToken"];

    var key = $"activity_{userId}_{start:yyyyMMdd}_{end:yyyyMMdd}";

    var activity = await cache.GetOrCreateAsync(key, async entry =>
    {
        var client = new WithingsClient(credentials);
        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
        return await client.GetActivityMeasures(
            start,
            end,
            userId,
            accessToken);
    });

    return Results.Json(activity);
});

app.MapGet("/api/withings/dailyactivity", async () =>
{
    var client = new WithingsClient(credentials);
    var activity = await client.GetActivityMeasures(
        DateTime.Today.AddDays(-30),
        session["UserId"],
        session["AccessToken"]);
    return Results.Json(activity);
});

app.MapGet("/api/withings/sleepsummary", async () =>
{
    var client = new WithingsClient(credentials);
    var activity = await client.GetSleepSummary(
        "2017-01-01",
        "2017-03-30",
        session["AccessToken"]);
    return Results.Json(activity);
});

app.MapGet("/api/withings/workouts", async () =>
{
    var client = new WithingsClient(credentials);
    var activity = await client.GetWorkouts(
        "2017-06-01",
        "2017-06-05",
        session["AccessToken"]);
    return Results.Json(activity);
});

app.MapGet("/api/withings/sleepmeasures", async () =>
{
    var client = new WithingsClient(credentials);
    var activity = await client.GetSleepMeasures(
        session["UserId"],
        DateTime.Now.AddDays(-90),
        DateTime.Now.AddDays(-1),
        session["AccessToken"]);
    return Results.Json(activity);
});

app.MapGet("/api/withings/body", async () =>
{
    var client = new WithingsClient(credentials);
    var activity = await client.GetBodyMeasures(
        session["UserId"],
        bodyStartDate,
        bodyEndDate,
        session["AccessToken"]);
    return Results.Json(activity);
});

app.MapGet("/api/withings/bodysince", async () =>
{
    var client = new WithingsClient(credentials);
    var activity = await client.GetBodyMeasures(
        session["UserId"],
        bodyStartDate,
        session["AccessToken"]);
    return Results.Json(activity);
});

app.MapGet("/api/withings/intraday", async () =>
{
    var client = new WithingsClient(credentials);
    var activity = await client.GetIntraDayActivity(
        session["UserId"],
        DateTime.Now.AddDays(-90),
        DateTime.Now.AddDays(-1),
        session["AccessToken"]);
    return Results.Json(activity);
});

app.Run("http://0.0.0.0:8080");
