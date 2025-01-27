using System;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;

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
            var traceId = context.TraceIdentifier; // Generate a unique trace ID for each request            
            context.Request.EnableBuffering();

            try
            {
                // Log request details
                if (context.Request.Method == HttpMethods.Post || context.Request.Method == HttpMethods.Put)
                {
                    var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
                    _logger.LogInformation("TraceId: {TraceId}, Request Path: {Path}, Method: {Method}, Body: {Body}", 
                        traceId, context.Request.Path, context.Request.Method, requestBody);
                    context.Request.Body.Position = 0; // Reset stream position
                }

                await _next(context); // Call the next middleware

                // Log response status
                _logger.LogInformation("TraceId: {TraceId}, Response StatusCode: {StatusCode}, Path: {Path}", 
                    traceId, context.Response.StatusCode, context.Request.Path);
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

            // Determine error details based on exception type
            switch (exception)
            {
                case DbUpdateException dbUpdateException:
                    statusCode = StatusCodes.Status500InternalServerError;
                    message = "A database update error occurred.";
                    _logger.LogError(dbUpdateException, "Error Code: {ErrorCode}, Message: {Message}, Path: {Path}",
                        statusCode, message, context.Request.Path);
                    break;

                case SqlException sqlException:
                    statusCode = StatusCodes.Status503ServiceUnavailable; // Indicate service unavailability
                    message = "Unable to connect to the database. Please try again later.";
                    _logger.LogError(sqlException, "Error Code: {ErrorCode}, Message: {Message}, Path: {Path}",
                        statusCode, message, context.Request.Path);
                    break;

                case NullReferenceException nullReferenceException:
                    statusCode = StatusCodes.Status500InternalServerError;
                    message = "A null reference occurred.";
                    _logger.LogError(nullReferenceException, "Error Code: {ErrorCode}, Message: {Message}, Path: {Path}",
                        statusCode, message, context.Request.Path);
                    break;

                default:
                    statusCode = StatusCodes.Status500InternalServerError;
                    message = "An unexpected error occurred.";
                    _logger.LogError(exception, "Error Code: {ErrorCode}, Message: {Message}, Path: {Path}",
                        statusCode, message, context.Request.Path);
                    break;
            }

            // Set the response
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var errorResponse = new
            {
                TraceId = context.TraceIdentifier,
                StatusCode = statusCode,
                Title = message,
                Detail = exception.Message // Optional: Mask this in production
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
        }
    }
}
