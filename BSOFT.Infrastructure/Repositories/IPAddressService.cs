using System.Net;
using System.Security.Claims;
using Core.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace BSOFT.Infrastructure.Repositories
{  public class IPAddressService : IIPAddressService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public IPAddressService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
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
        public string GetUserIPAddress()
        {
            return _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";
        }

        public string GetUserAgent()
        {
            var userAgent = _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString() ?? "Unknown User-Agent";
            return GetBrowser(userAgent);
        }
 
        public string GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value ?? "Anonymous";
        } 

        public string GetUserId()
        {
             var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userId, out _) ? userId : "0";
        }

        public string GetUserName()
        {
            var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
            return userName ?? "Anonymous";
        }
           public string GetUserOS()
        {
            var userAgent = _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString() ?? "Unknown User-Agent";            
            return GetOperatingSystem(userAgent);
        }
        
        private static string GetOperatingSystem(string userAgent)
        {
            if (userAgent?.Contains("Windows") == true) return "Windows";
            if (userAgent?.Contains("Mac OS") == true) return "Mac OS";
            if (userAgent?.Contains("Linux") == true) return "Linux";
            if (userAgent?.Contains("Android") == true) return "Android";
            if (userAgent?.Contains("iPhone") == true) return "iOS";
            return "Unknown OS";
        }

        private static string GetBrowser(string userAgent)
        {
            if (userAgent?.Contains("Firefox") == true) return "Firefox";
            if (userAgent?.Contains("Chrome") == true) return "Chrome";
            if (userAgent?.Contains("Safari") == true && !userAgent.Contains("Chrome")) return "Safari";
            if (userAgent?.Contains("Edge") == true) return "Edge";
            if (userAgent?.Contains("Opera") == true || userAgent.Contains("OPR")) return "Opera";
            return "Unknown Browser";
        }
        
    }

}
