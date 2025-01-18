using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Exceptions
{
   public class CustomException : Exception
    {
        public enum HttpStatus
        {
        BadRequest = 400,
        Unauthorized = 401,
        Forbidden = 403,
        NotFound = 404,
        Conflict = 409,
        InternalServerError = 500          
        }

        public int StatusCode { get; set; }
        public string[] ErrorMessages { get; set; }

        // Constructor for global exception handling
        public CustomException(string message, string[] errorMessages = null, HttpStatus statusCode = HttpStatus.BadRequest)
            : base(message)
        {
            StatusCode = (int)statusCode;
            ErrorMessages = errorMessages ?? Array.Empty<string>(); // Default to empty array
        }

        // Constructor for local exception handling with inner exception
        public CustomException(string message, Exception innerException, HttpStatus statusCode = HttpStatus.BadRequest)
            : base(message, innerException)
        {
            StatusCode = (int)statusCode;
            ErrorMessages = Array.Empty<string>(); // Default to empty array
        }
    }

    
}