using System;
using System.Runtime.InteropServices;

namespace Infrastructure.Printing
{
    public class RawPrinterHelper
    {
        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool OpenPrinter(string pPrinterName, out IntPtr hPrinter, IntPtr pDefault);

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool StartDocPrinter(IntPtr hPrinter, int level, ref DOCINFOA pDocInfo);

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool EndPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool WritePrinter(IntPtr hPrinter, byte[] pBuf, int cdBuf, out int pcWritten);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public class DOCINFOA
        {
            public string pDocName;
            public string pOutputFile;
            public string pDataType;
        }

        public static void SendToPrinter(string printerName, byte[] rawBytes)
        {
            IntPtr hPrinter;
            if (OpenPrinter(printerName, out hPrinter, IntPtr.Zero))
            {
                DOCINFOA docInfo = new DOCINFOA
                {
                    pDocName = "Raw Zebra Print",
                    pOutputFile = null,
                    pDataType = "RAW"
                };

                if (StartDocPrinter(hPrinter, 1, ref docInfo))
                {
                    if (StartPagePrinter(hPrinter))
                    {
                        WritePrinter(hPrinter, rawBytes, rawBytes.Length, out _);
                        EndPagePrinter(hPrinter);
                    }
                    EndDocPrinter(hPrinter);
                }
                ClosePrinter(hPrinter);
            }
        }
    }
}
