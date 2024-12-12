using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BSOFT.Domain.Interfaces;
using System.Net;

namespace BSOFT.Infrastructure.Repositories
{
    public class IPAddressService : IIPAddressService
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
        
    }
}