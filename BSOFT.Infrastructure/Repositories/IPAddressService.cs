using System.Net;
using System.Security.Claims;
using Core.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace BSOFT.Infrastructure.Repositories
{  public class IPAddressService : IIPAddressService
    {
         private readonly IHttpContextAccessor _httpContextAccessor;

        // Constructor
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
            var httpContext = _httpContextAccessor.HttpContext;            
            var userAgent = httpContext?.Request.Headers["User-Agent"].ToString();
            return userAgent != null ? GetBrowser(userAgent) : "Unknown Browser";
            //return _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString() ?? "Unknown User-Agent";
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
            var httpContext = _httpContextAccessor.HttpContext;
            var userAgent = httpContext?.Request.Headers["User-Agent"].ToString();
            return userAgent != null ? GetOperatingSystem(userAgent) : "Unknown OS";
            
        }
        private string GetOperatingSystem(string userAgent)
        {
            if (userAgent.Contains("Windows")) return "Windows";
            if (userAgent.Contains("Mac OS")) return "Mac OS";
            if (userAgent.Contains("Linux")) return "Linux";
            if (userAgent.Contains("Android")) return "Android";
            if (userAgent.Contains("iPhone")) return "iOS";
            return "Unknown OS";
        }

        private string GetBrowser(string userAgent)
        {
            if (userAgent.Contains("Firefox")) return "Firefox";
            if (userAgent.Contains("Chrome")) return "Chrome";
            if (userAgent.Contains("Safari") && !userAgent.Contains("Chrome")) return "Safari";
            if (userAgent.Contains("Edge")) return "Edge";
            if (userAgent.Contains("Opera") || userAgent.Contains("OPR")) return "Opera";
            return "Unknown Browser";
        }
    }
    
    
}


