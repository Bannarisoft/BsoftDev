using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts.Interfaces.External.IUser;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.External.IDepartment;
using Core.Application.Common.Interfaces.External.IUnit;
using Core.Application.Common.Interfaces.ICostCenter;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.CostCenter.Queries.GetCostCenter
{
    public class GetCostCenterQueryHandler : IRequestHandler<GetCostCenterQuery,ApiResponseDTO<List<CostCenterDto>>>
    {
        private readonly ICostCenterQueryRepository _iCostCenterQueryRepository;        
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IDepartmentGrpcClient _departmentGrpcClient;
        private readonly IUnitGrpcClient _unitGrpcClient;

        public GetCostCenterQueryHandler(ICostCenterQueryRepository iCostCenterQueryRepository, IMapper mapper, IMediator mediator, IDepartmentGrpcClient departmentService, IUnitGrpcClient unitGrpcClient)
        {
            _iCostCenterQueryRepository = iCostCenterQueryRepository;            
            _mapper = mapper;
            _mediator = mediator;
            _departmentGrpcClient = departmentService;
            _unitGrpcClient = unitGrpcClient;
        }

        // public async Task<ApiResponseDTO<List<CostCenterDto>>> Handle(GetCostCenterQuery request, CancellationToken cancellationToken)
        // {
        //       var (CostCenter, totalCount) = await _iCostCenterQueryRepository.GetAllCostCenterGroupAsync(request.PageNumber, request.PageSize, request.SearchTerm);
        //        var costCentersgrouplist = _mapper.Map<List<CostCenterDto>>(CostCenter);
        //      // 🔥 Fetch departments using HttpClientFactory
        //      var departments = await _departmentService.GetAllDepartmentAsync();
        //      var departmentLookup = departments.ToDictionary(d => d.DepartmentId, d => d.DepartmentName);
        //      var costCentersgrouplistDictionary = new Dictionary<int, CostCenterDto>();

        //       // 🔥 Map department names to CostCenter
        //     foreach (var data in costCentersgrouplist)
        //     {

        //         if (departmentLookup.TryGetValue(data.DepartmentId, out var departmentName) && departmentName != null)
        //         {
        //             data.DepartmentName = departmentName;
        //         }

        //         costCentersgrouplistDictionary[data.DepartmentId] = data;

        //     }

        //       // 🔥 Fetch UnitId using HttpClientFactory

        //     var units= await _unitService.GetUnitAutoCompleteAsync();
        //     var unitLookup = units.ToDictionary(d => d.UnitId, d => d.UnitName);
        //     var costCentersunitDictionary = new Dictionary<int, CostCenterDto>();
        //       // 🔥 Map Unit names to CostCenter
        //     foreach (var data in costCentersgrouplist)
        //     {

        //         if (unitLookup.TryGetValue(data.UnitId, out var UnitName) && UnitName != null)
        //         {
        //             data.UnitName = UnitName;
        //         }

        //         costCentersunitDictionary[data.UnitId] = data;

        //     }
          
        //      //Domain Event
        //         var domainEvent = new AuditLogsDomainEvent(
        //             actionDetail: "GetCostCenter",
        //             actionCode: "Get",        
        //             actionName: CostCenter.Count().ToString(),
        //             details: $"CostCenter details was fetched.",
        //             module:"CostCenter"
        //         );
        //         await _mediator.Publish(domainEvent, cancellationToken);
        //     return new ApiResponseDTO<List<CostCenterDto>> 
        //     { 
        //         IsSuccess = true, 
        //         Message = "Success", 
        //         Data = costCentersgrouplist ,
        //         TotalCount = totalCount,
        //         PageNumber = request.PageNumber,
        //         PageSize = request.PageSize
        //         };
        // }

        public async Task<ApiResponseDTO<List<CostCenterDto>>> Handle(GetCostCenterQuery request, CancellationToken cancellationToken)
        {
            var (costCenters, totalCount) = await _iCostCenterQueryRepository.GetAllCostCenterGroupAsync(request.PageNumber, request.PageSize, request.SearchTerm);
            var costCenterDtos = _mapper.Map<List<CostCenterDto>>(costCenters);

            // 🔥 Fetch lookups
            var departments = await _departmentGrpcClient.GetAllDepartmentAsync();
            var units = await _unitGrpcClient.GetUnitAutoCompleteAsync();

            var departmentLookup = departments.ToDictionary(d => d.DepartmentId, d => d.DepartmentName);
            var unitLookup = units.ToDictionary(u => u.UnitId, u => u.UnitName);

            // 🔁 Set DepartmentName and UnitName in one loop
            foreach (var dto in costCenterDtos)
            {
                if (departmentLookup.TryGetValue(dto.DepartmentId, out var deptName))
                    dto.DepartmentName = deptName;

                if (unitLookup.TryGetValue(dto.UnitId, out var unitName))
                    dto.UnitName = unitName;
            }

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
                Data = costCenterDtos,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        

    }
}