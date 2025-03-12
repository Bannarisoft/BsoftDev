using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.AssetMaster.AssetTranferIssueApproval.Queries.GetAssetTransferIssueApproval;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetTransferIssueApproval;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetMaster.AssetTranferIssueApproval.Queries.GetAssetTransferIssueById
{
    public class GetAssetTransferIssueByIdQueryHandler : IRequestHandler<GetAssetTransferIssueByIdQuery, ApiResponseDTO<List<AssetTransferIssueByIdDto>>> 
    {
        private readonly IAssetTransferIssueApprovalQueryRepository _assetTransferIssueQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public GetAssetTransferIssueByIdQueryHandler(IAssetTransferIssueApprovalQueryRepository assetTransferIssueQueryRepository, IMapper mapper, IMediator mediator)
        {
            _assetTransferIssueQueryRepository = assetTransferIssueQueryRepository;
            _mapper = mapper;
            _mediator = mediator;
        }

       public async Task<ApiResponseDTO<List<AssetTransferIssueByIdDto>>> Handle(GetAssetTransferIssueByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await _assetTransferIssueQueryRepository.GetByAssetTransferIdAsync(request.Id);

        // Check if data exists
        if (result is null || !result.Any())
        {
            return new ApiResponseDTO<List<AssetTransferIssueByIdDto>>
            {
                IsSuccess = false,
                Message = $"No records found for ID {request.Id}."
            };
        }

        // Map list of results
        var assetTransferIssueList = _mapper.Map<List<AssetTransferIssueByIdDto>>(result);

        // Domain Event Logging
        var domainEvent = new AuditLogsDomainEvent(
            actionDetail: "GetById",
            actionCode: "AssetTransferIssue",
            actionName: request.Id.ToString(),
            details: $"Asset transfer details for ID {request.Id} were fetched.",
            module: "AssetTransferIssue"
        );
        await _mediator.Publish(domainEvent, cancellationToken);

        return new ApiResponseDTO<List<AssetTransferIssueByIdDto>>
        {
            IsSuccess = true,
            Message = "Success",
            Data = assetTransferIssueList
        };
    }
    }
}