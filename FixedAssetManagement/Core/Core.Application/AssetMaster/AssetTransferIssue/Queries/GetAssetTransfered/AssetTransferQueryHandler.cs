using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts.Interfaces.External.IWorkflow;
using Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAssetTransfered;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetTransferIssue;
using Core.Domain.Common;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetMaster.AssetTransfer.Queries.GetAssetTransfered
{
    public class AssetTransferQueryHandler : IRequestHandler<AssetTransferQuery,  ApiResponseDTO<List<AssetTransferDto>>>
    {
        private readonly IAssetTransferQueryRepository _assetTransferQueryRepository;
        private readonly IMapper _mapper;        
        private readonly IMediator _mediator; 
        private readonly IWorkflowGrpcClient _workflowGrpcClient;
        public AssetTransferQueryHandler(IAssetTransferQueryRepository assetTransferQueryRepository, IMapper mapper, IMediator mediator, IWorkflowGrpcClient workflowGrpcClient)
        {
            _assetTransferQueryRepository = assetTransferQueryRepository;
            _mapper = mapper;
            _mediator = mediator;
            _workflowGrpcClient = workflowGrpcClient;
        }
         public  async Task<ApiResponseDTO<List<AssetTransferDto>>> Handle(AssetTransferQuery request, CancellationToken cancellationToken)        
        {
           // var (assetInsurance, totalCount) = await _assetInsuranceQueryRepository.GetAllAssetInsuranceAsync(request.PageNumber, request.PageSize, request.SearchTerm);
            var (assetTransferList, totalCount)  = await _assetTransferQueryRepository.GetAllAsync(request.PageNumber, request.PageSize, request.SearchTerm ,request.FromDate, request.ToDate);
          //  var totalCount = assetInsurance.Count;
            var AssetTransferList = _mapper.Map<List<AssetTransferDto>>(assetTransferList);

              var TransferStatus = await _workflowGrpcClient.GetAllApprovalRequestStatusAsync(MiscEnumEntity.AssetTransfer);
            var TransferStatusDict = TransferStatus.ToDictionary(u => u.ModuleTransactionId, u => u.CurrentStatus);

            foreach (var status in AssetTransferList)
            {
                if (TransferStatusDict.TryGetValue(status.Id, out var approvalstatus))
                {
                    status.ApprovalStatus = approvalstatus;
                }
                
            }

            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetAll",
                actionCode: "",        
                actionName: "",
                details: $"Asset Transfer    details was fetched.",
                module:"Asset Insurance"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<List<AssetTransferDto>>
            {
                IsSuccess = true,
                Message = "Success",
                Data = AssetTransferList,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize                
            };          
        }

      
        
    }
}