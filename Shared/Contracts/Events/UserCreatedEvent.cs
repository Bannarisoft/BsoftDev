namespace Contracts.Events
{
    // Renamed the parameter to avoid conflict with the record name
    public record UserCreatedEvent(Guid CorrelationId, Guid UserId, string Email);
}
