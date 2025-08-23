// Core.Application/Item/PutAway/Queries/GetPutAwayTargets/GetPutAwayTargetsQuery.cs
using MediatR;

namespace Core.Application.Item.PutAway.Queries.GetPutAwayTargets
{      public class GetPutAwayTargetsQuery : IRequest<List<PutAwayTargetLookupDto>>
    {
        public int WarehouseId { get; set; }          // required
        public int StorageTypeId { get; set; }        // MiscMaster.Id
        public string? SearchPattern { get; set; }    // optional filter (code/name contains)
    }
}
