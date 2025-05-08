

namespace BackgroundService.Application.Interfaces
{
    public interface IUserUnlockService
    {        
        Task UnlockUser(string username);
    }
}