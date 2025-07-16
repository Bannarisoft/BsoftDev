using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Core.Application.Common.Exceptions;
using Core.Application.Common.HttpResponse;

namespace FAM.Infrastructure.Logging.Middleware
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
            var traceId = context.TraceIdentifier;  // Generate a unique trace ID for each request            
            context.Request.EnableBuffering();
            try
            {
            // Log request details for POST, PUT, and GET requests
            if (context.Request.Method == HttpMethods.Post || 
                context.Request.Method == HttpMethods.Put || 
                context.Request.Method == HttpMethods.Get) // Include GET requests
                {
                var method = context.Request.Method;
                var path = context.Request.Path;

                if (method == HttpMethods.Get)
                {
                    _logger.LogInformation("TraceId: {TraceId}, Method: {Method}, Path: {Path}", traceId, method, path);
                }
                else
                {
                    context.Request.Body.Position = 0;
                    using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
                    var body = await reader.ReadToEndAsync();
                    context.Request.Body.Position = 0;

                    _logger.LogInformation("TraceId: {TraceId}, Method: {Method}, Path: {Path}, Body: {Body}", traceId, method, path, body);
                }
            }
            await _next(context);                    
        }
        catch (Exception ex)
            {
                // 🛑 Log and return standardized error
                _logger.LogError(ex, "Unhandled exception occurred. TraceId: {TraceId}", traceId);

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = ex switch
                {
                    EntityAlreadyExistsException => StatusCodes.Status409Conflict,
                    EntityNotFoundException => StatusCodes.Status404NotFound,
                    ExceptionRules => StatusCodes.Status400BadRequest,
                    SqlException => StatusCodes.Status503ServiceUnavailable,
                    DbUpdateException => StatusCodes.Status500InternalServerError,
                    NullReferenceException => StatusCodes.Status500InternalServerError,
                    _ => StatusCodes.Status500InternalServerError
                };

                var response =  new ApiResponseDTO<object>
                {
                    IsSuccess = false,
                    Message = ex.Message,                    
                   /*  Data = ex switch
                    {
                        EntityAlreadyExistsException => "Already exists.",
                        EntityNotFoundException => "Id not found.",
                        ExceptionRules => "Validation error.",
                        SqlException => "Database connection error.",
                        DbUpdateException => "Database update failed.",
                        NullReferenceException => "Null reference exception.",
                        _ => "An unexpected error occurred."
                    }, */
                    Errors = new List<string> { ex.Message },
                    StatusCode = context.Response.StatusCode
                };
                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
