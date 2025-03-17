using MediatR;
using QRCoder;

namespace Core.Application.QRCode
{
    public class GenerateQRCodeCommandHandler : IRequestHandler<GenerateQRCodeCommand, byte[]>
    {
        public async Task<byte[]> Handle(GenerateQRCodeCommand request, CancellationToken cancellationToken)
        {
            using (var qrGenerator = new QRCodeGenerator())
            {
                using (var qrCodeData = qrGenerator.CreateQrCode(request.Content, QRCodeGenerator.ECCLevel.Q))
                {
                    var qrCode = new PngByteQRCode(qrCodeData); // ✅ Use PngByteQRCode
                    //return qrCode.GetGraphic(20); // ✅ Returns QR as PNG byte array
                    int qrSize = 300; // Adjust size here (e.g., 300x300 pixels)
                    return qrCode.GetGraphic(qrSize);
                }
            }
        }
    }
}
