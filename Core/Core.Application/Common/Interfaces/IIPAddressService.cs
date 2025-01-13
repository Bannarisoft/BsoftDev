namespace Core.Application.Common.Interfaces
{
    public interface IIPAddressService
    {
        string GetSystemIPAddress();  
 		string GetUserIPAddress();    
        string GetUserAgent();
        string GetCurrentUserId();
        string GetUserId();
        string GetUserName();
        string GetUserOS(); 
               
    }
}