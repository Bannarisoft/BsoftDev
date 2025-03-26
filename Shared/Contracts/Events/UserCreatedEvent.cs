namespace Contracts.Events
{
    public record UserCreatedEvent(Guid CorrelationId, Guid UserId, string Email);
}
