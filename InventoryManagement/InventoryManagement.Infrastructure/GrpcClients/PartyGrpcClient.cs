
using Grpc.Core;
using Microsoft.AspNetCore.Http;
using PartyProto = GrpcServices.Party.Party;
using PartyContractDto = Contracts.Dtos.Party.PartyDto;
using Contracts.Interfaces.External.IParty;

public sealed class PartyGrpcClient : IPartyGrpcClient
{
    private readonly PartyProto.PartyService.PartyServiceClient _client;
    private readonly IHttpContextAccessor _http;

    public PartyGrpcClient(PartyProto.PartyService.PartyServiceClient client, IHttpContextAccessor http)
    {
        _client = client;
        _http   = http;
    }

    private Metadata BuildAuth()
    {
        var token = _http.HttpContext?.Request?.Headers["Authorization"].ToString();
        if (string.IsNullOrWhiteSpace(token)) return new Metadata();
        if (!token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            token = $"Bearer {token}";
        return new Metadata { { "Authorization", token } };
    }

    public async Task<List<PartyContractDto>> GetAutoCompleteAsync(string? searchPattern = null, CancellationToken ct = default)
    {
        var req = new PartyProto.PartyRequest { Search = searchPattern ?? string.Empty };
        var res = await _client.GetPartyAutoCompleteAsync(req, headers: BuildAuth(), cancellationToken: ct);

        return res.Items.Select(x => new PartyContractDto
        {
            Id        = x.Id,
            PartyCode = x.PartyCode,
            PartyName = x.PartyName
        }).ToList();
    }

    public async Task<PartyContractDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        try
        {
            var res = await _client.GetPartyByIdAsync(new PartyProto.GetPartyByIdRequest { Id = id },
                                                      headers: BuildAuth(), cancellationToken: ct);
            return new PartyContractDto
            {
                Id        = res.Id,
                PartyCode = res.PartyCode,
                PartyName = res.PartyName
            };
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
        {
            return null;
        }
    }
}
