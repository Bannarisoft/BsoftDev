namespace Core.Application.Common.Interfaces
{
    public interface IIPAddressService
    {
        string GetSystemIPAddress();  
        string GetUserBrowserDetails(string UserAgent);
        
    }
}