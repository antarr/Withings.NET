using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Withings.NET.Client;
using Withings.NET.Models;

// Define constant dates for API calls to avoid repeated parsing
var activityStart = new DateTime(2017, 01, 01);
var activityEnd = new DateTime(2017, 03, 30);
var bodyStart = new DateTime(2017, 05, 08);
var bodyEnd = new DateTime(2017, 05, 10);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMemoryCache();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

var credentials = new WithingsCredentials();
credentials.SetCallbackUrl(Environment.GetEnvironmentVariable("WITHINGS_CALLBACK_URL"));
credentials.SetClientProperties(
    Environment.GetEnvironmentVariable("WITHINGS_CLIENT_ID"),
    Environment.GetEnvironmentVariable("WITHINGS_CLIENT_SECRET"));

builder.Services.AddSingleton(credentials);
builder.Services.AddSingleton<Authenticator>();
builder.Services.AddSingleton<WithingsClient>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseSession();

app.MapGet("/", () => Results.Redirect("/api/oauth/authorize", permanent: true));

app.MapGet("/api/oauth/authorize", (HttpContext context, Authenticator authenticator) =>
{
    var state = Guid.NewGuid().ToString();
    context.Session.SetString("State", state);
    var scope = "user.info,user.metrics,user.activity";
    var url = authenticator.GetAuthCodeUrl(scope, state);
    return Results.Redirect(url);
});

app.MapGet("/api/oauth/callback", async (HttpContext context, Authenticator authenticator) =>
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

app.MapGet("/api/withings/activity", async (HttpContext context, WithingsClient client) =>
{
    var userId = context.Session.GetString("UserId");
    var accessToken = context.Session.GetString("AccessToken");

    if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(accessToken))
        return Results.Unauthorized();

    var activity = await client.GetActivityMeasures(
        activityStart,
        activityEnd,
        userId,
        accessToken);
    return Results.Json(activity);
});

app.MapGet("/api/withings/dailyactivity", async (HttpContext context, WithingsClient client, IMemoryCache cache) =>
{
    var userId = context.Session.GetString("UserId");
    var accessToken = context.Session.GetString("AccessToken");

    if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(accessToken))
        return Results.Unauthorized();

    var today = DateTime.Today;
    var cacheKey = $"dailyactivity_{userId}_{today:yyyyMMdd}";
    var activity = await cache.GetOrCreateAsync(cacheKey, async entry =>
    {
        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
        return await client.GetActivityMeasures(
            today.AddDays(-30),
            userId,
            accessToken);
    });
    return Results.Json(activity);
});

app.MapGet("/api/withings/sleepsummary", async (HttpContext context, WithingsClient client) =>
{
    var accessToken = context.Session.GetString("AccessToken");
    if (string.IsNullOrEmpty(accessToken))
        return Results.Unauthorized();

    var activity = await client.GetSleepSummary(
        "2017-01-01",
        "2017-03-30",
        accessToken);
    return Results.Json(activity);
});

app.MapGet("/api/withings/workouts", async (HttpContext context, WithingsClient client) =>
{
    var accessToken = context.Session.GetString("AccessToken");
    if (string.IsNullOrEmpty(accessToken))
        return Results.Unauthorized();

    var activity = await client.GetWorkouts(
        "2017-06-01",
        "2017-06-05",
        accessToken);
    return Results.Json(activity);
});

app.MapGet("/api/withings/sleepmeasures", async (HttpContext context, WithingsClient client) =>
{
    var userId = context.Session.GetString("UserId");
    var accessToken = context.Session.GetString("AccessToken");

    if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(accessToken))
        return Results.Unauthorized();

    var activity = await client.GetSleepMeasures(
        userId,
        DateTime.Now.AddDays(-90),
        DateTime.Now.AddDays(-1),
        accessToken);
    return Results.Json(activity);
});

app.MapGet("/api/withings/body", async (HttpContext context, WithingsClient client) =>
{
    var userId = context.Session.GetString("UserId");
    var accessToken = context.Session.GetString("AccessToken");

    if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(accessToken))
        return Results.Unauthorized();

    var activity = await client.GetBodyMeasures(
        userId,
        bodyStart,
        bodyEnd,
        accessToken);
    return Results.Json(activity);
});

app.MapGet("/api/withings/bodysince", async (HttpContext context, WithingsClient client) =>
{
    var userId = context.Session.GetString("UserId");
    var accessToken = context.Session.GetString("AccessToken");

    if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(accessToken))
        return Results.Unauthorized();

    var activity = await client.GetBodyMeasures(
        userId,
        bodyStart,
        accessToken);
    return Results.Json(activity);
});

app.MapGet("/api/withings/intraday", async (HttpContext context, WithingsClient client) =>
{
    var userId = context.Session.GetString("UserId");
    var accessToken = context.Session.GetString("AccessToken");

    if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(accessToken))
        return Results.Unauthorized();

    var activity = await client.GetIntraDayActivity(
        userId,
        DateTime.Now.AddDays(-90),
        DateTime.Now.AddDays(-1),
        accessToken);
    return Results.Json(activity);
});

app.Run();
