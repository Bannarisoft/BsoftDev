namespace Core.Application.Common.Interfaces
{
   public interface ISmsService
{
    Task<bool> SendSmsAsync(string to, string message);
}
}