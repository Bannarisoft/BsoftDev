using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Core.Application.QRCode
{
    public class PrintQRCodeCommand  : IRequest<bool>
    {
        public string? Content { get; set; } // Data inside QR Code
        public string? PrinterName { get; set; } // Zebra Printer Name
        public int LabelWidth { get; set; } = 300; // Default Label Width (adjust as needed)
        public int LabelHeight { get; set; } = 300; // Default Label Height
    }
}