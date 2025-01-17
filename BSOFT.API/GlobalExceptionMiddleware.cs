using System.Net;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace BSOFT.API
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;

         public GlobalExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
    
        }
        public async Task Invoke(HttpContext context)
        {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
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
        }
        else if (ex is DbUpdateException)
        {
            statusCode = StatusCodes.Status500InternalServerError;
            message = "A database update error occurred.";
            errors = new[] { ex.Message };
        }
        else
        {
            statusCode = StatusCodes.Status500InternalServerError;
            message = "An unexpected error occurred.";
            errors = new[] { ex.Message };
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var response = new
        {
            message,
            errors,
            statusCode
        };

        await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
    }
  }
}
        
    