using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.External.IDepartment;
using Core.Application.Common.Interfaces.IReports;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.Reports.AssetTransferReport
{
    public class AssetTransferQueryHandler : IRequestHandler<AssetTransferQuery, ApiResponseDTO<List<AssetTransferDetailsDto>>>
    {
        private readonly IReportRepository _repository;
        private readonly IMapper _mapper;
        private readonly IDepartmentService _departmentService;
        private readonly IMediator _mediator;

        public AssetTransferQueryHandler(IReportRepository repository, IMapper mapper, IDepartmentService departmentService, IMediator mediator)
        {
            _repository = repository;
            _mapper = mapper;
            _departmentService = departmentService;
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<List<AssetTransferDetailsDto>>> Handle(AssetTransferQuery request, CancellationToken cancellationToken)
        {
            var fromDate = request.FromDate ?? throw new ArgumentNullException(nameof(request.FromDate));
            var toDate = request.ToDate ?? throw new ArgumentNullException(nameof(request.ToDate));

            // Fetch AssetTransfer report data from repository
            var assetTransfersReports = await _repository.AssetTransferReportAsync(fromDate, toDate);

            // Map to DTOs
            var assetTransfersReportDtos = _mapper.Map<List<AssetTransferDetailsDto>>(assetTransfersReports);

        
            // Log audit
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetAssetTransferReport",
                actionCode: "Get",
                actionName: assetTransfersReportDtos.Count.ToString(),
                details: "AssetTransfer report list fetched.",
                module: "AssetTransfer Reports"
            );

            await _mediator.Publish(domainEvent, cancellationToken);

            // Return API response
            return new ApiResponseDTO<List<AssetTransferDetailsDto>>
            {
                IsSuccess = true,
                Message = "Success",
                Data = assetTransfersReportDtos,
                TotalCount = assetTransfersReportDtos.Count
            };
        }
    }
}