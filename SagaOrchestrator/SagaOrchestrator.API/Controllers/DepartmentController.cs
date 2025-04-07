using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SagaOrchestrator.API.Models;
using SagaOrchestrator.Infrastructure.Services.MaintenanceServices;

namespace SagaOrchestrator.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentController : ControllerBase
    {
        private readonly DepartmentSagaService _departmentSagaService;
        public DepartmentController(DepartmentSagaService departmentSagaService)
        {
            _departmentSagaService = departmentSagaService;
        }

        [HttpPost("department")]
        
        public async Task<IActionResult> TriggerDepartment([FromBody] TriggerDepartmentRequest request)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(token))
                return Unauthorized("Missing bearer token");

            var department = await _departmentSagaService.TriggerDepartmentCreation(request.DepartmentId, token);

            if (department == null)
                return NotFound($"Department with ID {request.DepartmentId} not found.");

            return Ok(new
            {
                message = "Department creation process triggered successfully.",
                departmentId = department.DepartmentId,
                departmentName = department.DepartmentName
            });
        }
        // public async Task<IActionResult> TriggerDepartment(int departmentId)
        // {
        //     await _departmentSagaService.TriggerDepartmentCreation(departmentId);
        //     return Ok(new
        //     {
        //         message = "Department creation process triggered successfully.",
        //         delegatementId = departmentId,
        //         departmentName = departmentId
        //     });

        // }

    }
}