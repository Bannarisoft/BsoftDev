using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.External.IUnit;
using Core.Application.Common.Interfaces.IReports;
using MediatR;

namespace Core.Application.Reports.MRS
{
    public class MRSReportQueryHandler : IRequestHandler<MRSReportQuery, ApiResponseDTO<List<MRSReportDto>>>
    {
        private readonly IReportRepository _repository;
        private readonly IMapper _mapper;
        private readonly IUnitService _unitService;
         private readonly IMediator _mediator;

        public Task<ApiResponseDTO<List<MRSReportDto>>> Handle(MRSReportQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}