using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using EcommerceApp.Middleware;
using Microsoft.AspNetCore.Builder;
using Newtonsoft.Json;
using System.Net;
using Microsoft.AspNetCore.Http;

namespace ECommerceAppTest.CustomMiddleware
{
    public class ExceptionMidllewareTest
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;
        public ExceptionMidllewareTest()
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
                 throw new InvalidOperationException("Simulated error");
            });
        }));

            _client = _server.CreateClient();
        }
        [Fact]
        public async Task Middleware_Should_Return_InternalServerError_Response()
        {
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
    }
}
