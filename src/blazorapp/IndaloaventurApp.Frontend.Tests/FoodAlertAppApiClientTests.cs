namespace IndaloaventurApp.Frontend.Tests;

using System.Net;
using System.Text;
using System.Text.Json;
using IndaloaventurApp.Web.Client.Infrastructure.FoodAlerts;

public sealed class FoodAlertAppApiClientTests
{
    [Fact]
    public async Task GetAlertsAsync_UsesCategoryEndpointAndMapsItems()
    {
        var handler = new StubHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Get, request.Method);
            Assert.Equal("/api/food-alerts", request.RequestUri?.AbsolutePath);
            Assert.Equal("?code=general", request.RequestUri?.Query);

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(new[]
                {
                    new
                    {
                        id = "alert-1",
                        categoryCode = "general",
                        title = "Alerta general",
                        summary = "Resumen breve.",
                        publishedAt = new DateTime(2026, 6, 13, 10, 0, 0, DateTimeKind.Utc),
                        sourceUrl = "https://example.test/1"
                    }
                }), Encoding.UTF8, "application/json")
            });
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://localhost") };
        var sut = new FoodAlertAppApiClient(httpClient);

        var result = await sut.GetAlertsAsync("general");

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!);
        Assert.Equal("alert-1", result.Value![0].Id);
        Assert.Equal("general", result.Value[0].CategoryCode);
    }

    [Fact]
    public async Task GetAlertAsync_ReturnsNotFound_On404()
    {
        using var httpClient = new HttpClient(new StubHttpMessageHandler(_ => Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound))))
        {
            BaseAddress = new Uri("https://localhost")
        };

        var sut = new FoodAlertAppApiClient(httpClient);
        var result = await sut.GetAlertAsync("alert-404");

        Assert.False(result.IsSuccess);
        Assert.Equal("food_alerts.not_found", result.Error?.Code);
    }

    [Fact]
    public async Task GetAlertAsync_MapsDetailPayload()
    {
        var handler = new StubHttpMessageHandler(request =>
        {
            Assert.Equal("/api/food-alerts/alert-7", request.RequestUri?.AbsolutePath);

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(new
                {
                    id = "alert-7",
                    categoryCode = "general",
                    title = "Detalle alerta",
                    description = "Texto completo de detalle.",
                    publishedAt = new DateTime(2026, 6, 13, 10, 0, 0, DateTimeKind.Utc),
                    sourceUrl = "https://example.test/7"
                }), Encoding.UTF8, "application/json")
            });
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://localhost") };
        var sut = new FoodAlertAppApiClient(httpClient);

        var result = await sut.GetAlertAsync("alert-7");

        Assert.True(result.IsSuccess);
        Assert.Equal("Detalle alerta", result.Value?.Title);
        Assert.Equal("Texto completo de detalle.", result.Value?.Description);
    }
}
