using Contracts.Models.Email;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace BackgroundService.API.Controller
{
     [Route("api/email")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public EmailController(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        [HttpPost("send")]
        public async Task<IActionResult> PublishEmail([FromBody] EmailEventDto email)
        {
            await _publishEndpoint.Publish(email);
            return Ok("Email published");
        }
    }
}
    