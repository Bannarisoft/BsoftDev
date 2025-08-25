namespace Core.Application.Item.PutAway.Commands.CreatePutAwayRule
{
    public sealed class CreatePutAwayStrategyRequest
    {
        public int StorageTypeId { get; set; }
        public int? TargetId { get; set; }
        public int PriorityId { get; set; }
    }
}