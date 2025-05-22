
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IReports;
using MediatR;

namespace Core.Application.Reports.WorkOrderReport
{
    public class WorkOrderReportQueryHandler  : IRequestHandler<WorkOrderReportQuery, ApiResponseDTO<List<WorkOrderReportDto>>>
    {
        
        private readonly IReportRepository _reportQueryRepository;
        private readonly IMapper _mapper;
        public WorkOrderReportQueryHandler( IReportRepository reportQueryRepository, IMapper mapper)
        {
            _reportQueryRepository = reportQueryRepository;
            _mapper = mapper;
        }      
        public async Task<ApiResponseDTO<List<WorkOrderReportDto>>> Handle(WorkOrderReportQuery request, CancellationToken cancellationToken)
        {
            var reportEntities = await _reportQueryRepository.WorkOrderReportAsync(request.FromDate,request.ToDate,request.RequestTypeId)?? new List<WorkOrderReportDto>();   
            var reportDto = _mapper.Map<List<WorkOrderReportDto>>(reportEntities)?? new List<WorkOrderReportDto>();
         
            return new ApiResponseDTO<List<WorkOrderReportDto>>
            {
            IsSuccess = reportDto.Any(),
            Message = reportDto.Any()
            ? "Work Order Report retrieved successfully."
            : "No Work Order Report found.",
            Data = reportDto
            };
        }
    }
}
    