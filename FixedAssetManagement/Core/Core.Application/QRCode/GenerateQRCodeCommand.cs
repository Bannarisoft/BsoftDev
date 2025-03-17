using MediatR;

namespace Core.Application.QRCode
{
    public class GenerateQRCodeCommand : IRequest<byte[]>
    {
        public string? Content { get; set; }
    }
}