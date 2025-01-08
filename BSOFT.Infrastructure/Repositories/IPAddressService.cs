using System.Net;
using Core.Application.Common.Interfaces;

namespace BSOFT.Infrastructure.Repositories
{  public class IPAddressService : IIPAddressService
    {
         public string GetSystemIPAddress()
        {
            string ipAddress = "127.0.0.1"; // Default to localhost.
            try
            {
                ipAddress = Dns.GetHostEntry(Dns.GetHostName())
                    .AddressList
                    .FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)?
                    .ToString() ?? ipAddress;
            }
            catch
            {
                
            }
            return ipAddress;
        }

        public string GetUserBrowserDetails(string userAgent)
        {
            string os = ExtractOS(userAgent);
            string systemName = Environment.MachineName; // Get the system/machine name
            string browserAndVersion = ExtractBrowserAndVersion(userAgent);
            return $"{os}/{systemName}/{browserAndVersion}";

        }

        private string ExtractOS(string userAgent)
        {
            if (userAgent.Contains("Windows NT")) return "WinNT";
            if (userAgent.Contains("Mac OS X")) return "MacOS";
            if (userAgent.Contains("Linux")) return "Linux";
            if (userAgent.Contains("Android")) return "Android";
            if (userAgent.Contains("iPhone") || userAgent.Contains("iPad")) return "iOS";
            return "UnknownOS";
        }
        private string ExtractBrowserAndVersion(string userAgent)
        {
         var match = System.Text.RegularExpressions.Regex.Match(userAgent,
        @"(Chrome|Firefox|Safari|Edge|MSIE|Trident)[/ ]([\d\.]+)");

    if (match.Success)
    {
        string browser = match.Groups[1].Value;
        string version = match.Groups[2].Value;

        // Normalize Trident/IE versions
        if (browser == "Trident")
        {
            browser = "IE";
            version = "11.0"; // Default for Trident
        }

        return $"{browser}/{version}";
    }

    return "UnknownBrowser/0.0";
    }   
    }
    
    
}


