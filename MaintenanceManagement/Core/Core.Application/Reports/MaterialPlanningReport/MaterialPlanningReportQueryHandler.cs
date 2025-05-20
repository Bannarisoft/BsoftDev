using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IReports;
using MediatR;

namespace Core.Application.Reports.MaterialPlanningReport
{
    public class MaterialPlanningReportQueryHandler : IRequestHandler<MaterialPlanningReportQuery, ApiResponseDTO<List<MaterialPlanningReportDto>>>
    {
        private readonly IReportRepository _reportQueryRepository;
        private readonly IMapper _mapper;
        public MaterialPlanningReportQueryHandler(IReportRepository reportQueryRepository, IMapper mapper)
        {
            _reportQueryRepository = reportQueryRepository;
            _mapper = mapper;
        }
        public async Task<ApiResponseDTO<List<MaterialPlanningReportDto>>> Handle(MaterialPlanningReportQuery request, CancellationToken cancellationToken)
        {
            var reportEntities = await _reportQueryRepository.MaterialPlanningReportAsync(request.FromDueDate, request.ToDueDate, request.MaintenanceCategory,
            request.MachineName, request.Activity, request.MaterialCode);

            var MaterialPlanningList = _mapper.Map<List<MaterialPlanningReportDto>>(reportEntities);
            
               return new ApiResponseDTO<List<MaterialPlanningReportDto>>
                {
                  IsSuccess = MaterialPlanningList != null && MaterialPlanningList.Any(),
                     Message = MaterialPlanningList != null && MaterialPlanningList.Any()
                     ? "Material Planning Report retrieved successfully."
                     : "No Material Planning Report found.",
                     Data = MaterialPlanningList ?? new List<MaterialPlanningReportDto>()
                };
        }
    }
}