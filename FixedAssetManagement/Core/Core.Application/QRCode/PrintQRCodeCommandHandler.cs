using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.Application.QRCode;
using Infrastructure.Printing;
using MediatR;

namespace Application.Features.QRCode.Handlers
{
    public class PrintQRCodeCommandHandler : IRequestHandler<PrintQRCodeCommand, bool>
    {
        public async Task<bool> Handle(PrintQRCodeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Content) || string.IsNullOrEmpty(request.PrinterName))
                    throw new ArgumentException("Printer name and content are required.");

                // Convert content to ASCII bytes
                byte[] printData = Encoding.ASCII.GetBytes(request.Content);

                // Send to printer
                return RawPrinterHelper.SendToPrinter(request.PrinterName, printData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error Printing: {ex.Message}");
                return false;
            }
        }
    }
}
