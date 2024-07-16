using Microsoft.AspNetCore.Http;
using System.Runtime.InteropServices;

namespace EcommerceApp.Middleware
{
    public class ExceptionMiddlware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ExceptionMiddlware> logger;

        public ExceptionMiddlware(RequestDelegate next, ILogger<ExceptionMiddlware> logger)
        {
            this.next = next;
            this.logger = logger;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context); // Continue to the next middleware
            }
            catch (Exception ex)
            {
                // Handle the exception
                await HandleExceptionAsync(context, ex);
            }
        }
        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            logger.LogError(ex.Message, ex);

            var problemDetails = new
            {
                Status = (int)StatusCodes.Status500InternalServerError,
                ErrorMessage = "An unexpected fault happened. Please try again later.",
                Path = $"{context.Request.Method} {context.Request.Path}"
            };

            // Example: Return a JSON response
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)StatusCodes.Status500InternalServerError;
            return context.Response.WriteAsJsonAsync(problemDetails);
        }
    }
}
