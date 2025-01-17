using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.HttpResponse
{
    public class ApiResponseDTO<T>
    {
         public bool IsSuccess { get; set; }
         public string Message { get; set; }
         public T Data { get; set; }
    }
}