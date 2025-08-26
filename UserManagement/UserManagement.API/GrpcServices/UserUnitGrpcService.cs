using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IUnit;
using Grpc.Core;
using GrpcServices.UserManagement.UserUnit;

namespace UserManagement.API.GrpcServices
{
    public class UserUnitGrpcService : UserUnitService.UserUnitServiceBase
    {
        private readonly IUnitQueryRepository _unitQueryRepository;
        public UserUnitGrpcService(IUnitQueryRepository unitQueryRepository)
        {
            _unitQueryRepository = unitQueryRepository;
        }

        public override async Task<UserUnitListResponse> GetUserUnit(UserUnitRequest request, ServerCallContext context)
        {
            var (units, _) = await _unitQueryRepository.GetAllUnitsAsync(1, int.MaxValue, null);

            var response = new UserUnitListResponse();
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