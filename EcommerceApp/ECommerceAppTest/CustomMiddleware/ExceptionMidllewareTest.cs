using EcommerceApp.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using System.Net;

namespace ECommerceAppTest.CustomMiddleware
{
    public class ExceptionMidllewareTest
    {
        private TestServer? _server;
        private HttpClient? _client;
        [Fact]
        public async Task Middleware_Should_Return_InternalServerError_Response()
        {
            _server = new TestServer(new WebHostBuilder()
            .ConfigureServices(services =>
            {
            })
            .Configure(app =>
            {
                app.UseMiddleware<ExceptionMiddlware>();
                app.Run(context => 
                {
                    throw new InvalidOperationException("Simulated error");
                });
            }));

            _client = _server.CreateClient();
            // Arrange - Client sends request
            var request = new HttpRequestMessage(HttpMethod.Get, "/");
            request.Headers.Add("Accept", "application/json");

            // Act - Client receives response
            var response = await _client.SendAsync(request);

            // Assert - Validate response
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var errorResponse = JsonConvert.DeserializeAnonymousType(content, new
            {
                Status = 0,
                ErrorMessage = "",
                Path = ""
            });

            Assert.NotNull(errorResponse);
            Assert.Equal(StatusCodes.Status500InternalServerError, errorResponse.Status);
            Assert.Equal("An unexpected fault happened. Please try again later.", errorResponse.ErrorMessage);

        }
        [Fact]
        public async Task Middleware_Should_Return_NoError_Response()
        {
            _server = new TestServer(new WebHostBuilder()
            .ConfigureServices(services =>
            {
            })
            .Configure(app =>
            {
                app.UseMiddleware<ExceptionMiddlware>();
                app.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                    await context.Response.WriteAsync("No error occurred");
                });
            }));

            _client = _server.CreateClient();
            // Arrange - Client sends request
            var request = new HttpRequestMessage(HttpMethod.Get, "/");
            request.Headers.Add("Accept", "application/json");

            // Act - Client receives response
            var response = await _client.SendAsync(request);

            // Assert - Validate response
            var content = await response.Content.ReadAsStringAsync();
            Assert.NotNull(content);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("No error occurred", content);

        }
    }
}
