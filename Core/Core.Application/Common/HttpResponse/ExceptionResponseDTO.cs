using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.HttpResponse
{
    public class ExceptionResponseDTO
    {
        public string Errors { get; set; } 
         public string Message { get; set; }
         public int Status  { get; set; }
    }
}