namespace Core.Application.Common.Interfaces.Item.ItemDetail.Commands
{
    public interface IExecutionContext
    {
        int? CreatedBy { get; }
        string? CreatedByName { get; }
        string? CreatedIP { get; }
        string? CorrelationId { get; }
    }
}