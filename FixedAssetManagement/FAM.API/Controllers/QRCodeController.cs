using MediatR;
using Microsoft.AspNetCore.Mvc;
using Core.Application.QRCode;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QRCodeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public QRCodeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("print")]
        public async Task<IActionResult> PrintQRCode([FromBody] PrintQRCodeCommand command)
        {
            if (string.IsNullOrEmpty(command.Content) || string.IsNullOrEmpty(command.PrinterName))
                return BadRequest("Content and Printer Name are required.");

            bool result = await _mediator.Send(command);

            if (result)
                return Ok("✅ QR Code sent to Zebra Printer successfully.");
            else
                return StatusCode(500, "❌ Failed to print QR Code.");
        }
    }
}
