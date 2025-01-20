using System.Net;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog.Context;

namespace BSOFT.API
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

         public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task Invoke(HttpContext context)
        {
        try
        {
            _logger.LogInformation("Processing request: {Method} {Path}", context.Request.Method, context.Request.Path);
            await _next(context);
             _logger.LogInformation("Response sent with StatusCode: {StatusCode}", context.Response.StatusCode);
        }
        catch (Exception ex)
        {
             LogContext.PushProperty("StatusCode", context.Response.StatusCode);
            _logger.LogError(ex, "An error occurred while processing the request.");
            throw;            
        }
    }
    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        int statusCode;
        string message;
        string[] errors;
        

        // if (ex is CustomException customEx)
        // {
        //     statusCode = customEx.StatusCode;
        //     message = customEx.Message;
        //     errors = customEx.ErrorMessages;
        // }
        // else if (ex is ValidationException validationEx)
        // {
        //     statusCode = StatusCodes.Status400BadRequest;
        //     message = validationEx.Message;
        //     errors = validationEx.ErrorMessages;
        // }

         if (ex is NullReferenceException)
        {
            statusCode = StatusCodes.Status500InternalServerError;
            message = "A null reference occurred.";
            errors = new[] { ex.Message };
            _logger.LogError(ex, "Database error occurred: {ErrorCode} - {Message}", (int)statusCode, ex.Message);
        }
        else if (ex is DbUpdateException)
        {
            statusCode = StatusCodes.Status500InternalServerError;
            message = "A database update error occurred.";
            errors = new[] { ex.Message };
            _logger.LogError(ex, "Database error occurred: {ErrorCode} - {Message}", (int)statusCode, ex.Message);
        }
        else
        {
            statusCode = StatusCodes.Status500InternalServerError;
            message = "An unexpected error occurred.";
            errors = new[] { ex.Message };
             _logger.LogError(ex, "Unexpected error occurred: {ErrorCode} - {Message}", (int)statusCode, ex.Message);
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var response = new
        {                       
            Message = message,
            Errors = errors,
            StatusCode = statusCode
        };
        
        await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
    }
  }
}
        
    