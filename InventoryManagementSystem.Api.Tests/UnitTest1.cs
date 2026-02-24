using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace InventoryManagementSystem.Api.Tests;

public class ApiSmokeTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ApiSmokeTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetWeatherForecast_ReturnsOkJson()
    {
        var response = await _client.GetAsync("/api/WeatherForecast");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Echo_InvalidModel_ReturnsBadRequestWithErrors()
    {
        var response = await _client.PostAsJsonAsync("/api/Diagnostics/echo", new { });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(content);
        Assert.Equal("Validation failed.", doc.RootElement.GetProperty("message").GetString());
        Assert.True(doc.RootElement.TryGetProperty("correlationId", out _));
        Assert.True(doc.RootElement.TryGetProperty("errors", out _));
    }

    [Fact]
    public async Task Echo_ValidModel_EchoesMessage()
    {
        var payload = new { message = "hello" };

        var response = await _client.PostAsJsonAsync("/api/Diagnostics/echo", payload);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(content);
        Assert.Equal("hello", doc.RootElement.GetProperty("message").GetString());
    }
}
