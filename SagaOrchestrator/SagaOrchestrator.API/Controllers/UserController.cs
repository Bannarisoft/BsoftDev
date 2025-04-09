using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SagaOrchestrator.API.Models;
using SagaOrchestrator.Infrastructure.Services;
using SagaOrchestrator.Infrastructure.Services.UserServices;

namespace SagaOrchestrator.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        // private readonly OrchestratorService _orchestratorService;
        private readonly UserSagaService _userSagaService;

        public UserController(UserSagaService userSagaService)
        {
            _userSagaService = userSagaService;
        }
        [HttpPost("user")]
        public async Task<IActionResult> TriggerUser([FromBody] TriggerUserRequest request)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(token))
                return Unauthorized("Missing bearer token");

            var user = await _userSagaService.TriggerUserCreation(request.UserId, token);

            if (user == null)
                return NotFound($"User with ID {request.UserId} not found.");

            return Ok(new
            {
                message = "User creation process triggered successfully.",
                userId = user.UserId,
                userName = user.UserName,
                email = user.Email
            });
        }


        // [HttpPost("user")]
        // public async Task<IActionResult> TriggerUser(int userId)
        // {
        //     await _orchestratorService.TriggerUserCreation(userId);
        //     return Ok(new
        //     {
        //         message = "User creation process triggered successfully.",
        //         userId = userId

        //     });
        //     // await _orchestratorService.TriggerUserCreation(userId);
        //     // return Ok("User creation triggered.");
        // }
    }
}