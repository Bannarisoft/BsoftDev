
using Core.Application.Common.Interfaces.IUnit;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcServices.UserManagement;
using Microsoft.AspNetCore.Authorization;

[Authorize]
public class UnitGrpcService : UnitService.UnitServiceBase
 {
   private readonly IUnitQueryRepository _unitRepository;

    public UnitGrpcService(IUnitQueryRepository unitRepository)
    {
        _unitRepository = unitRepository;
    }

     public override async Task<UnitListResponse> GetAllUnit(Empty request, ServerCallContext context)
    {
        var (units, _) = await _unitRepository.GetAllUnitsAsync(1, 100, null);

        var response = new UnitListResponse();
        response.Units.AddRange(units.Select(d => new GrpcServices.UserManagement.UnitDto
        {
            UnitId = d.Id,
            UnitName = d.UnitName,
            ShortName = d.ShortName,
            UnitHeadName = d.UnitHeadName,
            OldUnitId = d.OldUnitId
        }));

        return response;
    } 
 }
