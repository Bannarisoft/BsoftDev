using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Models.Maintenance;

namespace SagaOrchestrator.Application.Models
{
    public class DepartmentResponse
    {
         public int StatusCode { get; set; }
        public DepartmentDto? Data { get; set; }
    }
}