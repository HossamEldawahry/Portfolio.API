using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Portfolio.API.Tests;

public class BasicApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public BasicApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(_ => { });
    }

    [Fact]
    public async Task Root_ReturnsRedirectOrNotFound()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var response = await client.GetAsync("/");
        Assert.True(response.StatusCode is HttpStatusCode.Redirect or HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Health_Endpoint_IsAvailable()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/health");
        Assert.True(response.StatusCode is HttpStatusCode.OK or HttpStatusCode.ServiceUnavailable);
    }
}
