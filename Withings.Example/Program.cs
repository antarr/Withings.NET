using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Withings.NET.Client;
using Withings.NET.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMemoryCache();
var app = builder.Build();

app.UseSession();

var credentials = new WithingsCredentials();
credentials.SetCallbackUrl(Environment.GetEnvironmentVariable("WithingsCallbackUrl"));
credentials.SetConsumerProperties(
    Environment.GetEnvironmentVariable("WithingsConsumerKey"),
    Environment.GetEnvironmentVariable("WithingsConsumerSecret"));

var authenticator = new Authenticator(credentials);

var activityStartDate = new DateTime(2017, 1, 1);
var activityEndDate = new DateTime(2017, 3, 30);
var bodyStartDate = new DateTime(2017, 5, 8);
var bodyEndDate = new DateTime(2017, 5, 10);

app.MapGet("/", () => Results.Redirect("/api/oauth/authorize", permanent: true));

app.MapGet("/api/oauth/authorize", (HttpContext context) =>
{
    var state = Guid.NewGuid().ToString();
    context.Session.SetString("State", state);
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

    var storedState = context.Session.GetString("State");
    if (string.IsNullOrEmpty(storedState) || storedState != state)
         return Results.BadRequest("Invalid state.");

    var token = await authenticator.GetAccessToken(code);

    context.Session.SetString("AccessToken", token.AccessToken);
    context.Session.SetString("RefreshToken", token.RefreshToken);
    context.Session.SetString("UserId", token.UserId);

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

app.MapGet("/api/withings/dailyactivity", async (HttpContext context) =>
{
    var userId = context.Session.GetString("UserId");
    var accessToken = context.Session.GetString("AccessToken");

    if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(accessToken))
        return Results.Unauthorized();

    var client = new WithingsClient(credentials);
    var activity = await client.GetActivityMeasures(
        DateTime.Today.AddDays(-30),
        userId,
        accessToken);
    return Results.Json(activity);
});

app.MapGet("/api/withings/sleepsummary", async (HttpContext context) =>
{
    var accessToken = context.Session.GetString("AccessToken");
    if (string.IsNullOrEmpty(accessToken))
        return Results.Unauthorized();

    var client = new WithingsClient(credentials);
    var activity = await client.GetSleepSummary(
        "2017-01-01",
        "2017-03-30",
        accessToken);
    return Results.Json(activity);
});

app.MapGet("/api/withings/workouts", async (HttpContext context) =>
{
    var accessToken = context.Session.GetString("AccessToken");
    if (string.IsNullOrEmpty(accessToken))
        return Results.Unauthorized();

    var client = new WithingsClient(credentials);
    var activity = await client.GetWorkouts(
        "2017-06-01",
        "2017-06-05",
        accessToken);
    return Results.Json(activity);
});

app.MapGet("/api/withings/sleepmeasures", async (HttpContext context) =>
{
    var userId = context.Session.GetString("UserId");
    var accessToken = context.Session.GetString("AccessToken");

    if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(accessToken))
        return Results.Unauthorized();

    var client = new WithingsClient(credentials);
    var activity = await client.GetSleepMeasures(
        userId,
        DateTime.Now.AddDays(-90),
        DateTime.Now.AddDays(-1),
        accessToken);
    return Results.Json(activity);
});

app.MapGet("/api/withings/body", async (HttpContext context) =>
{
    var userId = context.Session.GetString("UserId");
    var accessToken = context.Session.GetString("AccessToken");

    if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(accessToken))
        return Results.Unauthorized();

    var client = new WithingsClient(credentials);
    var activity = await client.GetBodyMeasures(
        userId,
        DateTime.Parse("2017-05-08"),
        DateTime.Parse("2017-05-10"),
        accessToken);
    return Results.Json(activity);
});

app.MapGet("/api/withings/bodysince", async (HttpContext context) =>
{
    var userId = context.Session.GetString("UserId");
    var accessToken = context.Session.GetString("AccessToken");

    if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(accessToken))
        return Results.Unauthorized();

    var client = new WithingsClient(credentials);
    var activity = await client.GetBodyMeasures(
        userId,
        DateTime.Parse("2017-05-08"),
        accessToken);
    return Results.Json(activity);
});

app.MapGet("/api/withings/intraday", async (HttpContext context) =>
{
    var userId = context.Session.GetString("UserId");
    var accessToken = context.Session.GetString("AccessToken");

    if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(accessToken))
        return Results.Unauthorized();

    var client = new WithingsClient(credentials);
    var activity = await client.GetIntraDayActivity(
        userId,
        DateTime.Now.AddDays(-90),
        DateTime.Now.AddDays(-1),
        accessToken);
    return Results.Json(activity);
});

app.Run("http://0.0.0.0:8080");
