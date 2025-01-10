using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Microsoft.AspNetCore.Diagnostics;
using Core.Application.Common.HttpResponse;
using Microsoft.AspNetCore.Http;

namespace BSOFT.API.GlobalException
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext, 
            Exception exception, 
            CancellationToken cancellationToken)
        {
           var apiResponse = new ExceptionResponseDTO();

            
                if (exception is BadHttpRequestException badRequestException)
                 {
                     // Set status code to 400 for client-side errors
                     httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

                     apiResponse.Status = StatusCodes.Status400BadRequest;
                     apiResponse.Errors = badRequestException.Message;
                     apiResponse.Message = "Validation failed.";
                 }
                 
             

            await httpContext.Response
            .WriteAsJsonAsync(apiResponse, cancellationToken);

            return true;
        }
    }
}