namespace Contracts.Events
{
    public interface IUserCreated
    {
        Guid CorrelationId { get; }
        Guid UserId { get; }
        string Email { get; }
        DateTime CreatedAt { get; }
    }


}
