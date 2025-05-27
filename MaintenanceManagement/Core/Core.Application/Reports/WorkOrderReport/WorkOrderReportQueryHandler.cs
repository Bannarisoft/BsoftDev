
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
        private readonly IDepartmentGrpcClient _departmentGrpcClient;
        public WorkOrderReportQueryHandler( IReportRepository reportQueryRepository, IMapper mapper, IDepartmentGrpcClient departmentGrpcClient)
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
            var departments = await _departmentGrpcClient.GetAllDepartmentAsync(); // ✅ Clean call

            // var departments = departmentResponse.Departments.ToList();
            var departmentLookup = departments.ToDictionary(d => d.DepartmentId, d => d.DepartmentName);

            var filteredWorkOrders = reportDto
                 .Where(p => departmentLookup.ContainsKey(p.DepartmentId))
                 .Select(p =>
                 {
                     p.Department = departmentLookup[p.DepartmentId];
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
    