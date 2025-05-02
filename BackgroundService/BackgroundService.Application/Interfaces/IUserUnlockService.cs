

namespace BackgroundService.Application.Interfaces
{
    public interface IUserUnlockService
    {        
        Task<bool> UnlockUser(string username);
    }
}