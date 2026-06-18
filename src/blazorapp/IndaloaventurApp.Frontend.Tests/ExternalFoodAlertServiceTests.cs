namespace IndaloaventurApp.Frontend.Tests;

using System.Net;
using System.Text;
using System.Text.Json;
using IndaloaventurApp.Web.Infrastructure.FoodAlerts;

public sealed class ExternalFoodAlertServiceTests
{
    [Fact]
    public async Task GetAlertsAsync_NormalizesHtmlAndBuildsSummary()
    {
        var handler = new StubHttpMessageHandler(request =>
        {
            Assert.Equal("/api/Alerts/all", request.RequestUri?.AbsolutePath);
            Assert.Equal("?code=complementos", request.RequestUri?.Query);

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(new
                {
                    error = false,
                    mensaje = "ok",
                    data = new[]
                    {
                        new
                        {
                            id = 101,
                            fecha = "2026-06-13T10:00:00",
                            titulo = "Alerta <b>Complemento</b>",
                            url = "https://example.test/ca-1",
                            descripcion = "<p>Descripcion con <strong>HTML</strong> y texto adicional para superar claramente el limite de cien caracteres en el resumen mostrado.</p>"
                        }
                    }
                }), Encoding.UTF8, "application/json")
            });
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("http://redalertas.runasp.net") };
        var sut = new ExternalFoodAlertService(httpClient);

        var result = await sut.GetAlertsAsync("complementos");

        Assert.True(result.IsSuccess);
        var alert = Assert.Single(result.Value!);
        Assert.Equal("101", alert.Id);
        Assert.Equal("complementos", alert.CategoryCode);
        Assert.Equal("Alerta Complemento", alert.Title);
        Assert.DoesNotContain("<", alert.Summary);
        Assert.EndsWith("...", alert.Summary);
    }

    [Fact]
    public async Task GetAlertsAsync_ReturnsInvalidCategory_ForUnsupportedCode()
    {
        using var httpClient = new HttpClient(new StubHttpMessageHandler(_ => throw new InvalidOperationException("Should not call endpoint")))
        {
            BaseAddress = new Uri("http://redalertas.runasp.net")
        };

        var sut = new ExternalFoodAlertService(httpClient);
        var result = await sut.GetAlertsAsync("otra");

        Assert.False(result.IsSuccess);
        Assert.Equal("food_alerts.invalid_category", result.Error?.Code);
    }

    [Fact]
    public async Task GetAlertAsync_UsesDetailEndpointAndNormalizesDescription()
    {
        var handler = new StubHttpMessageHandler(request =>
        {
            Assert.Equal("/api/Alerts/alert-9", request.RequestUri?.AbsolutePath);

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(new
                {
                    id = 9,
                    fecha = "2026-06-13T10:00:00",
                    titulo = "Detalle alerta",
                    url = "https://example.test/alert-9",
                    descripcion = "<p>Primer bloque.</p><p>Segundo bloque.</p>"
                }), Encoding.UTF8, "application/json")
            });
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("http://redalertas.runasp.net") };
        var sut = new ExternalFoodAlertService(httpClient);

        var result = await sut.GetAlertAsync("alert-9");

        Assert.True(result.IsSuccess);
        Assert.Equal("9", result.Value?.Id);
        Assert.Contains("Primer bloque.", result.Value?.Description);
        Assert.Contains("Segundo bloque.", result.Value?.Description);
        Assert.DoesNotContain("<p>", result.Value?.Description);
    }
}
