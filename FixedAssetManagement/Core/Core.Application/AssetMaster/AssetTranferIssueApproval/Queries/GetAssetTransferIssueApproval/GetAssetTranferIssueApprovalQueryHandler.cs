using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts.Interfaces.External.IUser;
using Contracts.Interfaces.External.IWorkflow;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IAssetTransferIssueApproval;
using Core.Domain.Common;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetMaster.AssetTranferIssueApproval.Queries.GetAssetTransferIssueApproval
{
    public class GetAssetTranferIssueApprovalQueryHandler : IRequestHandler<GetAssetTranferIssueApprovalQuery,  ApiResponseDTO<List<AssetTransferIssueApprovalDto>>>
    {
        private readonly IAssetTransferIssueApprovalQueryRepository _assetTransferIssueQueryRepository;
        private readonly IMapper _mapper;        
        private readonly IMediator _mediator; 
        private readonly IDepartmentGrpcClient _departmentGrpcClient;
        private readonly IWorkflowGrpcClient _workflowGrpcClient;
        private readonly IIPAddressService _ipAddressService;

        public GetAssetTranferIssueApprovalQueryHandler(IAssetTransferIssueApprovalQueryRepository assetTransferIssueQueryRepository, IMapper mapper, IMediator mediator,
        IDepartmentGrpcClient departmentGrpcClient, IWorkflowGrpcClient workflowGrpcClient, IIPAddressService ipAddressService)
        {
            _assetTransferIssueQueryRepository = assetTransferIssueQueryRepository;
            _mapper = mapper;
            _mediator = mediator;
            _departmentGrpcClient = departmentGrpcClient;
            _workflowGrpcClient = workflowGrpcClient;
            _ipAddressService = ipAddressService;
        }

        public async Task<ApiResponseDTO<List<AssetTransferIssueApprovalDto>>> Handle(GetAssetTranferIssueApprovalQuery request, CancellationToken cancellationToken)
        {
           var (assetIssueTransfer, totalCount) = await _assetTransferIssueQueryRepository
                                                .GetAllPendingAssetTransferAsync(request.PageNumber, request.PageSize, request.SearchTerm, request.FromDate, request.ToDate);
            var assetIssueTransferList = _mapper.Map<List<AssetTransferIssueApprovalDto>>(assetIssueTransfer);
            // 🔥 Fetch departments using gRPC
            // var departments = await _departmentGrpcClient.GetAllDepartmentAsync();
            // var departmentLookup = departments.ToDictionary(d => d.DepartmentId, d => d.DepartmentName);
            
               var PendingTransfer = await _workflowGrpcClient.GetAllApprovalRequestByApprover(MiscEnumEntity.AssetTransfer,_ipAddressService.GetUserId());
            var TransferStatusDict = PendingTransfer.ToDictionary(u => u.ModuleTransactionId, u => u.CurrentStatus);

                 var filteredassetIssueTransfer = assetIssueTransferList
            .Where(p => TransferStatusDict.ContainsKey(p.Id))
            .ToList();
          


            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetAll",
                actionCode: "Get",        
                actionName: assetIssueTransferList.Count.ToString(),
                details: $"Asset Transfer Pending details was fetched.",
                module:"Asset Transfer Pending"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<List<AssetTransferIssueApprovalDto>>
            {
                IsSuccess = true,
                Message = "Success",
                Data = filteredassetIssueTransfer,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize                
            };   
        }
    }
}