using AutoMapper;
using Contracts.Interfaces.External.IUser;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IMaintenanceRequest;
using Core.Application.Common.Interfaces.IReports;
using MediatR;

namespace Core.Application.Reports.MaintenanceRequestReport
{
    public class RequestReportQueryHandler : IRequestHandler<RequestReportQuery, ApiResponseDTO<List<RequestReportDto>>>
    {

        private readonly IReportRepository _maintenanceRequestQueryRepository;
        private readonly IMapper _mapper;
               private readonly IDepartmentGrpcClient _departmentGrpcClient;
        private readonly IUnitGrpcClient _unitGrpcClient;


        public RequestReportQueryHandler(IReportRepository maintenanceRequestQueryRepository, IMapper mapper, IDepartmentGrpcClient departmentService, IUnitGrpcClient unitGrpcClient)
        {
            _maintenanceRequestQueryRepository = maintenanceRequestQueryRepository;
            _mapper = mapper;
            _departmentGrpcClient = departmentService;
            _unitGrpcClient = unitGrpcClient;
         
        }

        public async Task<ApiResponseDTO<List<RequestReportDto>>> Handle(RequestReportQuery request, CancellationToken cancellationToken)
        {

            var requestReportEntities = await _maintenanceRequestQueryRepository.MaintenanceReportAsync(
            request.RequestFromDate,
            request.RequestToDate,
            request.RequestType,
            request.RequestStatus,
            request.DepartmentId);


            var requestReportDtos = _mapper.Map<List<RequestReportDto>>(requestReportEntities);
               if (requestReportDtos == null || !requestReportDtos.Any())
                {
                    return new ApiResponseDTO<List<RequestReportDto>>
                    {
                        IsSuccess = false,
                        Message = "No maintenance requests found.", 
                        Data = new List<RequestReportDto>()
                    };
                }
                          
            var departments = await _departmentGrpcClient.GetAllDepartmentAsync();
            var units = await _unitGrpcClient.GetAllUnitAsync();           
            var departmentLookup = departments.ToDictionary(d => d.DepartmentId, d => d.DepartmentName);
            var unitLookup = units.ToDictionary(u => u.UnitId, u => u.UnitName);

           
            foreach (var dto in requestReportDtos)
            {
                if (departmentLookup.TryGetValue(dto.DepartmentId, out var departmentName))
                {
                    dto.Department = departmentName;
                }

                if (unitLookup.TryGetValue(dto.UnitId, out var unitName))
                {
                    dto.UnitName = unitName;
                }
            }


           return new ApiResponseDTO<List<RequestReportDto>>
            {
                IsSuccess = true,
                Message = "Maintenance report retrieved successfully.",
                Data = requestReportDtos ?? new List<RequestReportDto>()
            };
        }




    }
}