using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts.Interfaces.External.IUser;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IReports;
using MediatR;

namespace Core.Application.Reports.WorkOderCheckListReport
{
    public class WorkOderCheckListReportQueryHandler : IRequestHandler<WorkOderCheckListReportQuery, ApiResponseDTO<List<WorkOderCheckListReportDto>>>
    {

        private readonly IReportRepository _workOrderCheckListQueryRepository;
        private readonly IMapper _mapper;   
        private readonly IIPAddressService _ipAddressService;    
        private readonly IUnitGrpcClient _unitGrpcClient;
        private readonly ICompanyGrpcClient _companyGrpcClient;


        public WorkOderCheckListReportQueryHandler(IReportRepository workOrderCheckListQueryRepository, IMapper mapper, IUnitGrpcClient unitGrpcClient, ICompanyGrpcClient companyGrpcClient, IIPAddressService ipAddressService)
        {
            _workOrderCheckListQueryRepository = workOrderCheckListQueryRepository;
            _mapper = mapper;

            _unitGrpcClient = unitGrpcClient;
            _companyGrpcClient = companyGrpcClient;
            _ipAddressService = ipAddressService;
        }

        public async Task<ApiResponseDTO<List<WorkOderCheckListReportDto>>> Handle(WorkOderCheckListReportQuery request, CancellationToken cancellationToken)
        {

            var requestReportEntities = await _workOrderCheckListQueryRepository.GetWorkOrderChecklistReportAsync(
            request.WorkOrderFromDate,
            request.WorkOrderToDate,
            request.MachineGroupId,
            request.MachineId,
            request.ActivityId
           );

            var requestReportDtos = _mapper.Map<List<WorkOderCheckListReportDto>>(requestReportEntities);
            var companyId = _ipAddressService.GetCompanyId();
            var unitId = _ipAddressService.GetUnitId();

            var companies = await _companyGrpcClient.GetAllCompanyAsync();
            var units = await _unitGrpcClient.GetAllUnitAsync();

            var companyLookup = companies.ToDictionary(c => c.CompanyId, c => c.CompanyName);
            var unitLookup = units.ToDictionary(u => u.UnitId, u => u.UnitName);

            foreach (var dto in requestReportDtos)
            {
                if (companyLookup.TryGetValue(dto.CompanyId, out var companyName))
                {
                    dto.CompanyName = companyName;
                }

                if (unitLookup.TryGetValue(dto.UnitId, out var unitName))
                {
                    dto.UnitName = unitName;
                }
            }
            // foreach (var dto in requestReportDtos)
            // {

            //     if (unitLookup.TryGetValue(dto.UnitId, out var unitName))
            //     {
            //         dto.UnitName = unitName;
            //     }
            // }
            return new ApiResponseDTO<List<WorkOderCheckListReportDto>>
            {
                IsSuccess = requestReportDtos != null && requestReportDtos.Any(),
                Message = requestReportDtos != null && requestReportDtos.Any()
                    ? "WorkOderCheckList report retrieved successfully."
                    : "No WorkOderCheckList requests found.",
                Data = requestReportDtos
            };
        }


    }
}