using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.QRCode;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FAM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QRCodeController  : ControllerBase
    {
        private readonly IMediator _mediator;

        public QRCodeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateQRCode([FromBody] GenerateQRCodeCommand command)
        {
            if (string.IsNullOrEmpty(command.Content))
                return BadRequest("Content is required to generate QR code.");

            var qrCodeBytes = await _mediator.Send(command);

            return File(qrCodeBytes, "image/png"); // ✅ Returns PNG image
        }
    }
}