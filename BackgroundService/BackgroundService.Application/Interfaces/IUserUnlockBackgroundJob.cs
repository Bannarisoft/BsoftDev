
namespace BackgroundService.Application.Interfaces
{
    public interface IUserUnlockBackgroundJob
    {
        Task Execute(string userName);
    }
}