using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IUnit;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcServices.UserManagement;


namespace UserManagement.API.GrpcServices
{
    public class UnitGrpcService : UnitService.UnitServiceBase
    {
        private readonly IUnitQueryRepository _unitQueryRepository;
        public UnitGrpcService(IUnitQueryRepository unitQueryRepository)
        {
            _unitQueryRepository = unitQueryRepository;
        }
        public override async Task<UnitListResponse> GetAllUnit(Empty request, ServerCallContext context)
        {
            var (units, _) = await _unitQueryRepository.GetAllUnitsAsync(1, int.MaxValue, null);

            var response = new UnitListResponse();
            foreach (var unit in units)
            {
                response.Units.Add(new UnitDto
                {
                    UnitId = unit.Id,
                    UnitName = unit.UnitName,
                    ShortName = unit.ShortName,
                    UnitHeadName = unit.UnitHeadName,
                    OldUnitId = unit.OldUnitId
                });
            }
            return response;
        }

    }
}