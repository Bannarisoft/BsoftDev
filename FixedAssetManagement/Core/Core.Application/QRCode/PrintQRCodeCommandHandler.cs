using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.Application.QRCode;
using MediatR;

namespace Application.Features.QRCode.Handlers
{
    public class PrintQRCodeCommandHandler : IRequestHandler<PrintQRCodeCommand, bool>
    {
        public async Task<bool> Handle(PrintQRCodeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // ✅ Generate CPCL Command for Citizen Printer
                string cpcl = $@"
! 0 200 200 400 1
TEXT 4 0 50 50 QR Code:
B QR 50 100 M 2 U 6
{request.Content}
ENDQR
PRINT
";

                byte[] cpclBytes = Encoding.ASCII.GetBytes(cpcl);

                // ✅ Send to Citizen Printer
                return RawPrinterHelper.SendToPrinter(request.PrinterName, cpclBytes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error Printing: {ex.Message}");
                return false;
            }
        }
    }
}
