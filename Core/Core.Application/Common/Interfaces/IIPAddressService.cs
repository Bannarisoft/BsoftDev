namespace Core.Application.Common.Interfaces
{
    public interface IIPAddressService
    {
        string GetSystemIPAddress();  
 		string GetUserIPAddress();    
        string GetUserAgent();
        string GetCurrentUserId();
        int GetUserId();
        string GetUserName();
        string GetUserOS(); 
        string GetUserBrowserDetails(string UserAgent);
        
    }
}