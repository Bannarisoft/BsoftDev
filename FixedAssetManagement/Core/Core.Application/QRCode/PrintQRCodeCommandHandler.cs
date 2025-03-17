using Infrastructure.Printing;
using MediatR;

namespace Core.Application.QRCode
{
    public class PrintQRCodeCommandHandler  : IRequestHandler<PrintQRCodeCommand, bool>
    {
        public async Task<bool> Handle(PrintQRCodeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 🔥 ZPL Format for QR Code (Zebra Label Printing)
                string zpl = $@"
^XA
^FO50,50^BQN,2,10
^FDLA,{request.Content}^FS
^XZ";

                // ✅ Send ZPL Data to the Zebra Printer
                SendToPrinter(request.PrinterName, zpl);
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error Printing QR Code: " + ex.Message);
                return false;
            }
        }

        private void SendToPrinter(string printerName, string zplData)
        {
            byte[] zplBytes = System.Text.Encoding.ASCII.GetBytes(zplData);
            RawPrinterHelper.SendToPrinter(printerName, zplBytes);
        }
    }
}