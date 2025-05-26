using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts.Interfaces.External.IUser;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.ICostCenter;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.CostCenter.Queries.GetCostCenter
{
    public class GetCostCenterQueryHandler : IRequestHandler<GetCostCenterQuery, ApiResponseDTO<List<CostCenterDto>>>
    {
        private readonly ICostCenterQueryRepository _iCostCenterQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IDepartmentGrpcClient _departmentGrpcClient;
        private readonly IUnitGrpcClient _unitGrpcClient; // 👈 gRPC Inject here


        public GetCostCenterQueryHandler(ICostCenterQueryRepository iCostCenterQueryRepository, IMapper mapper, IMediator mediator, IDepartmentGrpcClient departmentService, IUnitGrpcClient unitGrpcClient)
        {
            _iCostCenterQueryRepository = iCostCenterQueryRepository;
            _mapper = mapper;
            _mediator = mediator;
            _departmentGrpcClient = departmentService;
            _unitGrpcClient = unitGrpcClient;
        }

        public async Task<ApiResponseDTO<List<CostCenterDto>>> Handle(GetCostCenterQuery request, CancellationToken cancellationToken)
        {
            var (costCenters, totalCount) = await _iCostCenterQueryRepository.GetAllCostCenterGroupAsync(request.PageNumber, request.PageSize, request.SearchTerm);
            var costCenterDtos = _mapper.Map<List<CostCenterDto>>(costCenters);

            // 🔥 Fetch departments using gRPC
            var departments = await _departmentGrpcClient.GetAllDepartmentAsync();
            var units = await _unitGrpcClient.GetAllUnitAsync();

            var departmentLookup = departments.ToDictionary(d => d.DepartmentId, d => d.DepartmentName);
            var unitLookup = units.ToDictionary(u => u.UnitId, u => u.UnitName);

            // 🔥 Map department & unit names with DataControl to costCenters
            // foreach (var dto in costCenterDtos)
            // {
            //     if (departmentLookup.TryGetValue(dto.DepartmentId, out var deptName))
            //         dto.DepartmentName = deptName;

            //     if (unitLookup.TryGetValue(dto.UnitId, out var unitName))
            //         dto.UnitName = unitName;
            // }

            var filteredCostCenterDtos = costCenterDtos
                    .Where(p => departmentLookup.ContainsKey(p.DepartmentId))
                    .Select(p => new CostCenterDto
                    {
                        DepartmentId = p.DepartmentId,
                        DepartmentName = departmentLookup[p.DepartmentId],
                        UnitId = p.UnitId,
                        UnitName = unitLookup.TryGetValue(p.UnitId, out var unitName) ? unitName : string.Empty,
                        CostCenterName = p.CostCenterName,
                    })
                    .ToList();

            // 📘 Log domain event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetCostCenter",
                actionCode: "Get",
                actionName: costCenters.Count().ToString(),
                details: "CostCenter details were fetched.",
                module: "CostCenter"
            );
            await _mediator.Publish(domainEvent, cancellationToken);

            // ✅ Return
            return new ApiResponseDTO<List<CostCenterDto>>
            {
                IsSuccess = true,
                Message = "Success",
                Data = filteredCostCenterDtos,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }



    }
}