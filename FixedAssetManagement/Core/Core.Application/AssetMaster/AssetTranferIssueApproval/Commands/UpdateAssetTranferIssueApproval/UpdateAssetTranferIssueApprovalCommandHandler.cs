using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IAssetTransferIssueApproval;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetMaster.AssetTranferIssueApproval.Commands.UpdateAssetTranferIssueApproval
{
    public class UpdateAssetTranferIssueApprovalCommandHandler : IRequestHandler<UpdateAssetTranferIssueApprovalCommand, ApiResponseDTO<int>>
    {
       private readonly IAssetTransferIssueApprovalCommandRepository _assetTransferIssueApprovalCommandRepository;
        private readonly IMediator _imediator;
        private readonly IMapper _imapper;
        private readonly IIPAddressService _ipAddressService;
        private readonly ITimeZoneService _timeZoneService;
        public UpdateAssetTranferIssueApprovalCommandHandler(IAssetTransferIssueApprovalCommandRepository assetTransferIssueApprovalCommandRepository, IMediator imediator, IMapper imapper, IIPAddressService ipAddressService, ITimeZoneService timeZoneService)
        {
            _assetTransferIssueApprovalCommandRepository=assetTransferIssueApprovalCommandRepository;
            _imediator=imediator;
            _imapper=imapper;
            _ipAddressService=ipAddressService;
            _timeZoneService=timeZoneService;

        }

    public async Task<ApiResponseDTO<int>> Handle(UpdateAssetTranferIssueApprovalCommand request, CancellationToken cancellationToken)
        {
       
            // var transfers = await _assetTransferIssueApprovalCommandRepository.GetByIdsAsync(request.Id);
            
            // if (!transfers.Any()) 
            // {
            //     return new ApiResponseDTO<int> { IsSuccess = false, Message = "Asset transfer records not found." };
            // }

            // string currentIp = _ipAddressService.GetSystemIPAddress();
            // int userId = _ipAddressService.GetUserId();
            // string username = _ipAddressService.GetUserName();
            // var systemTimeZoneId = _timeZoneService.GetSystemTimeZone();
            // var currentTime = _timeZoneService.GetCurrentTime(systemTimeZoneId);

            // // Use AutoMapper to update only the status
            // foreach (var transfer in transfers)
            // {
            //     _imapper.Map(request, transfer);  // Maps only the required fields
            //     transfer.AuthorizedBy = userId;
            //     transfer.AuthorizedDate = currentTime;
            //     transfer.AuthorizedByName = username;
            //     transfer.AuthorizedIP = currentIp;
            // }
            // var result = await _assetTransferIssueApprovalCommandRepository.UpdateRangeAsync(transfers);

            // if (result <= 0) // No records updated
            // {
            //     return new ApiResponseDTO<int> { IsSuccess = false, Message = "Failed to update asset transfer records." };
            // }

            // // 🔹 Domain Event: Logging the update action
            // foreach (var transfer in transfers)
            // {
            //     var domainEvent = new AuditLogsDomainEvent(
            //         actionDetail: "Update",
            //         actionCode: transfer.Id.ToString(),
            //         actionName: transfer.Status,
            //         details: $"Asset transfer status updated to {transfer.Status}, Transfer ID: {transfer.Id}",
            //         module: "AssetTransferIssueApproval"
            //     );
            //     await _imediator.Publish(domainEvent, cancellationToken);
            // }
            // return new ApiResponseDTO<int> { IsSuccess = true, Message = "Asset transfer records updated successfully.", Data = result };

            var transfers = await _assetTransferIssueApprovalCommandRepository.GetByIdsAsync(request.Id);

            if (!transfers.Any())
            {
                return new ApiResponseDTO<int> { IsSuccess = false, Message = "Asset transfer records not found." };
            }

            string currentIp = _ipAddressService.GetSystemIPAddress();
            int userId = _ipAddressService.GetUserId();
            string username = _ipAddressService.GetUserName();
            var currentTime = _timeZoneService.GetCurrentTime(_timeZoneService.GetSystemTimeZone());

            // 🔹 Bulk Update in Single Query
            var result = await _assetTransferIssueApprovalCommandRepository.ExecuteBulkUpdateAsync(request.Id, request.Status, userId, currentTime, username, currentIp);

            if (result <= 0)
            {
                return new ApiResponseDTO<int> { IsSuccess = false, Message = "Failed to update asset transfer records." };
            }

            // 🔹 Publish Audit Log
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Update",
                actionCode: string.Join(",", request.Id),
                actionName: request.Status,
                details: $"Asset transfer status updated to {request.Status} for Transfer IDs: {string.Join(",", request.Id)}",
                module: "AssetTransferIssueApproval"
            );
            await _imediator.Publish(domainEvent, cancellationToken);

            return new ApiResponseDTO<int> { IsSuccess = true, Message = "Asset transfer records updated successfully.", Data = result };
        }
    }
}