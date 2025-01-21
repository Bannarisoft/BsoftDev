using System;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BSOFT.Infrastructure.Logging.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Request.EnableBuffering();

            try
            {
                // Log request details
                if (context.Request.Method == HttpMethods.Post || context.Request.Method == HttpMethods.Put)
                {
                    var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
                    _logger.LogInformation("Request Path: {Path}, Method: {Method}, Body: {Body}", 
                        context.Request.Path, context.Request.Method, requestBody);
                    context.Request.Body.Position = 0; // Reset stream position
                }

                await _next(context); // Call the next middleware

                // Log response status
                _logger.LogInformation("Response StatusCode: {StatusCode}, Path: {Path}", 
                    context.Response.StatusCode, context.Request.Path);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            int statusCode;
            string message;

            // Determine error details
            if (exception is NullReferenceException)
            {
                statusCode = StatusCodes.Status500InternalServerError;
                message = "A null reference occurred.";
                _logger.LogError(exception, "Error Code: {ErrorCode}, Message: {Message}, Path: {Path}",
                    statusCode, message, context.Request.Path);
            }
            else if (exception is DbUpdateException)
            {
                statusCode = StatusCodes.Status500InternalServerError;
                message = "A database update error occurred.";
                _logger.LogError(exception, "Error Code: {ErrorCode}, Message: {Message}, Path: {Path}",
                    statusCode, message, context.Request.Path);
            }
            else
            {
                statusCode = StatusCodes.Status500InternalServerError;
                message = "An unexpected error occurred.";
                _logger.LogError(exception, "Error Code: {ErrorCode}, Message: {Message}, Path: {Path}",
                    statusCode, message, context.Request.Path);
            }

            // Set the response
            var response = context.Response;
            response.ContentType = "application/json";
            response.StatusCode = statusCode;

            var errorResponse = new
            {
                StatusCode = statusCode,
                Message = message,
                Detailed = exception.Message // Optional: Hide in production
            };

            await response.WriteAsync(JsonSerializer.Serialize(errorResponse));
        }
    }
}
