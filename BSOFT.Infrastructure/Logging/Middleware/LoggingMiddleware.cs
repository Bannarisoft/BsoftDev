using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog;
using Serilog.Context;

namespace BSOFT.Infrastructure.Logging.Middleware
{
    public class LoggingMiddleware
    {
         private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
            _logger = Log.ForContext<LoggingMiddleware>();
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                 // Enrich log context with request information
                LogContext.PushProperty("RequestPath", context.Request.Path);
                LogContext.PushProperty("RequestId", context.TraceIdentifier);
                _logger.Information("Handling request: {Method} {Path}", context.Request.Method, context.Request.Path);
                await _next(context);
                _logger.Information("Finished handling request.");
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
                // _logger.Error(ex, "An exception occurred while processing the request.");
                // throw;
            }
        }
        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Log the exception
            Log.Error(exception, "An unhandled exception occurred.");

            // Set the response status code and content type
            var response = context.Response;
            response.ContentType = "application/json";
            response.StatusCode = (int)HttpStatusCode.InternalServerError;

            // Create error response
            var errorResponse = new
            {
                StatusCode = response.StatusCode,
                Message = "An unexpected error occurred. Please try again later.",
                Detailed = exception.Message // Optional: Remove in production
            };

            var errorJson = JsonSerializer.Serialize(errorResponse);
            await response.WriteAsync(errorJson);
        }
    }
}