using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SagaOrchestrator.Infrastructure.Services;

namespace SagaOrchestrator.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly OrchestratorService _orchestratorService;

        public UserController(OrchestratorService orchestratorService)
        {
            _orchestratorService = orchestratorService;
        }

        [HttpPost("user")]
        public async Task<IActionResult> TriggerUser(int userId)
        {
            await _orchestratorService.TriggerUserCreation(userId);
            return Ok(new
            {
                message = "User creation process triggered successfully.",
                userId = userId
            });
            // await _orchestratorService.TriggerUserCreation(userId);
            // return Ok("User creation triggered.");
        }
    }
}