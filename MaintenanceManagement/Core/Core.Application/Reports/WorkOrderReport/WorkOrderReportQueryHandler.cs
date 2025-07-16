
using AutoMapper;
using Contracts.Interfaces.External.IUser;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IReports;
using MediatR;

namespace Core.Application.Reports.WorkOrderReport
{
    public class WorkOrderReportQueryHandler  : IRequestHandler<WorkOrderReportQuery, ApiResponseDTO<List<WorkOrderReportDto>>>
    {
        
        private readonly IReportRepository _reportQueryRepository;
        private readonly IMapper _mapper;
        private readonly IDepartmentAllGrpcClient _departmentGrpcClient;
        public WorkOrderReportQueryHandler( IReportRepository reportQueryRepository, IMapper mapper, IDepartmentAllGrpcClient departmentGrpcClient)
        {
            _reportQueryRepository = reportQueryRepository;
            _mapper = mapper;
            _departmentGrpcClient = departmentGrpcClient;
        }      
        public async Task<ApiResponseDTO<List<WorkOrderReportDto>>> Handle(WorkOrderReportQuery request, CancellationToken cancellationToken)
        {
            var reportEntities = await _reportQueryRepository.WorkOrderReportAsync(request.FromDate,request.ToDate,request.RequestTypeId)?? new List<WorkOrderReportDto>();   
            var reportDto = _mapper.Map<List<WorkOrderReportDto>>(reportEntities)?? new List<WorkOrderReportDto>();


            // 🔥 Fetch departments using gRPC
            var departments = await _departmentGrpcClient.GetDepartmentAllAsync(); 
            var departmentLookup = departments.ToDictionary(d => d.DepartmentId, d => d.DepartmentName);

            var filteredWorkOrders = reportDto
                .Where(p => departmentLookup.ContainsKey(p.DepartmentId) || departmentLookup.ContainsKey(p.ProductionDepartmentId))
                .Select(p =>
                {
                    if (departmentLookup.TryGetValue(p.DepartmentId, out var deptName))
                        p.Department = deptName;

                    if (departmentLookup.TryGetValue(p.ProductionDepartmentId, out var prodDeptName))
                        p.ProductionDepartment = prodDeptName;

                    return p;
                })
                .ToList(); 
         
            return new ApiResponseDTO<List<WorkOrderReportDto>>
            {
            IsSuccess = reportDto.Any(),
            Message = reportDto.Any()
            ? "Work Order Report retrieved successfully."
            : "No Work Order Report found.",
            Data = filteredWorkOrders
            };
        }
    }
}
    