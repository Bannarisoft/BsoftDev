using Core.Application.Common.Interfaces.IPartyMaster;
using Grpc.Core;
using PartyProto = GrpcServices.Party.Party;

public sealed class PartyGrpcService : PartyProto.PartyService.PartyServiceBase
{
    private readonly IPartyMasterQueryRepository _repo;
    public PartyGrpcService(IPartyMasterQueryRepository repo) => _repo = repo;

    public override async Task<PartyProto.PartyResponse> GetPartyAutoComplete(
        PartyProto.PartyRequest request, ServerCallContext context)
    {
        var term = (request.Search ?? string.Empty).Trim();
        var data = await _repo.GetPartyMasterAutoComplete(term);

        var resp = new PartyProto.PartyResponse();
        foreach (var p in data)
        {
            resp.Items.Add(new PartyProto.PartyDto
            {
                Id        = p.Id,
                PartyCode = p.PartyCode ?? string.Empty,
                PartyName = p.PartyName ?? string.Empty
            });
        }
        return resp;
    }

    public async Task<PartyProto.PartyDto> GetPartyById(
        PartyProto.GetPartyByIdRequest request, ServerCallContext context)
    {
        var p = await _repo.GetByIdPartyMasterAsync(request.Id);
        if (p is null)
            throw new RpcException(new Status(StatusCode.NotFound, $"Party {request.Id} not found."));

        return new PartyProto.PartyDto
        {
            Id        = p.Id,
            PartyCode = p.PartyCode ?? string.Empty,
            PartyName = p.PartyName ?? string.Empty
        };
    }

    // ✅ new:
    public override async Task<PartyProto.PartyResponse> GetPartiesByIds(
        PartyProto.GetPartiesByIdsRequest request, ServerCallContext context)
    {
        if (request.Ids == null || request.Ids.Count == 0)
            return new PartyProto.PartyResponse();

        var data = await _repo.GetByIdsAsync(request.Ids.Select(i => (int)i));

        var resp = new PartyProto.PartyResponse();
        foreach (var p in data)
        {
            resp.Items.Add(new PartyProto.PartyDto
            {
                Id        = p.Id,
                PartyCode = p.PartyCode ?? string.Empty,
                PartyName = p.PartyName ?? string.Empty
            });
        }
        return resp;
    }
}
