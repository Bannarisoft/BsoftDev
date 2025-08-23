using Core.Application.Item.PutAway.Queries.GetAllPutAwayRule;
using Core.Application.Item.PutAway.Queries.GetPutAwayTargets;

namespace Core.Application.Common.Interfaces.Item.PutAway
{
    public interface IPutAwayRuleQueryRepository
    {
        Task<(IEnumerable<PutAwayRuleListDto> rows, int total)> GetPagedAsync(int page, int size, string? search, CancellationToken ct = default);
        Task<PutAwayRuleDetailDto?> GetByIdAsync(int id, CancellationToken ct = default);
        //Task<PutAwayEvaluateResult?> EvaluateAsync(PutAwayEvaluateRequest req, CancellationToken ct = default);        
        //Task<List<PutAwayTargetLookupDto>> GetTargetsByMiscAsync(
        //int warehouseId, int storageTypeMiscId, string? searchPattern, CancellationToken ct = default);
    }
}