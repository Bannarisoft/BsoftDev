namespace Core.Application.Common.Interfaces
{
    public interface IIPAddressService
    {
        string GetSystemIPAddress();  
        string GetUserBrowserDetails(string UserAgent);
        string GenerateSessionId(int userId, string deviceDetails,string ipAddress);
        
    }
}