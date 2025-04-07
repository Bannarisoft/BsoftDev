
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UserManagement.API.Controllers
{
    [Route("api/email")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly HttpClient _httpClient;

        public EmailController(IMediator mediator, IHttpClientFactory httpClientFactory)
        {
            _mediator = mediator;
            //_httpClient = httpClientFactory.CreateClient(); 
            _httpClient = httpClientFactory.CreateClient("BackgroundService");
        }      
        [HttpPost("send")]
         [AllowAnonymous]
        public async Task<IActionResult> SendEmailToBackgroundService([FromBody] SendEmailRequest request)
        {
            //var response = await _httpClient.PostAsJsonAsync("http://backgroundservice/api/email/send", request);
            //var response = await _httpClient.PostAsJsonAsync("http://localhost:5011/api/email/send", request);
            var response = await _httpClient.PostAsJsonAsync("api/email/send", request); // Notice relative URL
            return response.IsSuccessStatusCode
                ? Ok("Email request queued successfully")
                : StatusCode(500, "Error sending email");
        }
    }

}