using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAssetTransfered;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetTransferIssue;
using Core.Domain.Entities.AssetMaster;
using Core.Domain.Events;
using FluentValidation;
using MediatR;

namespace Core.Application.AssetMaster.AssetTransferIssue.Command.UpdateAssetTransferIssue
{
    public class UpdateAssetTransferIssueCommandHandler  : IRequestHandler<UpdateAssetTransferIssueCommand, ApiResponseDTO<int>>
    {

        private readonly IAssetTransferCommandRepository _assetTransferCommandRepository;
        private readonly IAssetTransferQueryRepository _assetTransferQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IIPAddressService _ipAddressService;
        private readonly ITimeZoneService _timeZoneService;
        private readonly IValidator<UpdateAssetTransferIssueCommand> _validator;

        public UpdateAssetTransferIssueCommandHandler(
            IAssetTransferCommandRepository assetTransferCommandRepository,
            IAssetTransferQueryRepository assetTransferQueryRepository,
            IMapper mapper,
            IMediator mediator,
            IIPAddressService ipAddressService,
            ITimeZoneService timeZoneService,
            IValidator<UpdateAssetTransferIssueCommand> validator)
        {
            _assetTransferCommandRepository = assetTransferCommandRepository;
            _assetTransferQueryRepository = assetTransferQueryRepository;
            _mapper = mapper;
            _mediator = mediator;
            _ipAddressService = ipAddressService;
            _timeZoneService = timeZoneService;
            _validator = validator;
        }
            public async Task<ApiResponseDTO<int>> Handle(UpdateAssetTransferIssueCommand request, CancellationToken cancellationToken)
        {
                            // 🔹 Retrieve Existing Record from Query Repository
                var existingRecordDto = await _assetTransferQueryRepository.GetAssetTransferByIdAsync(request.AssetTransferHdr.Id);
                if (existingRecordDto == null)
                {
                    return new ApiResponseDTO<int>
                    {
                        IsSuccess = false,
                        Message = $"Asset Transfer Issue with ID {request.AssetTransferHdr.Id} not found."
                    };
                }

                // 🔹 Convert DTO to Domain Entity
                var assetTransferIssueHdr = _mapper.Map<Core.Domain.Entities.AssetMaster.AssetTransferIssueHdr>(request.AssetTransferHdr);                
                assetTransferIssueHdr.ModifiedDate = _timeZoneService.GetCurrentTime(_timeZoneService.GetSystemTimeZone());
                assetTransferIssueHdr.ModifiedBy = _ipAddressService.GetUserId();
                assetTransferIssueHdr.ModifiedByName = _ipAddressService.GetUserName();
                assetTransferIssueHdr.ModifiedIP = _ipAddressService.GetSystemIPAddress();

                
                // 🔹 Save Changes
                var result = await _assetTransferCommandRepository.UpdateAssetTransferAsync(assetTransferIssueHdr);

                if (result)
                {
                    return new ApiResponseDTO<int>
                    {
                        IsSuccess = true,
                        Message = "Asset Transfer updated successfully"
                      
                    };
                }
                return new ApiResponseDTO<int>
                {
                    IsSuccess = false,
                    Message = "Asset Transfer update failed"
                };           
        }

        // public Task<ApiResponseDTO<int>> Handle(UpdateAssetTransferIssueCommand request, CancellationToken cancellationToken)
        // {
        //     throw new NotImplementedException();
        // }

        //  public async Task<ApiResponseDTO<int>> Handle(UpdateAssetTransferIssueCommand request, CancellationToken cancellationToken)
        // {
        //     // 🔹 Validate the request
        //     var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        //     if (!validationResult.IsValid)
        //     {
        //         return new ApiResponseDTO<int>
        //         {
        //             IsSuccess = false,
        //             Message = "Validation failed",
        //             Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList()
        //         };
        //     }

        //     // 🔹 Retrieve Existing Record
            
        //     var existingRecord = await _assetTransferCommandRepository.GetAssetTransferByIdAsync(request.AssetTransferIssueHdrDto.Id);
        //     if (existingRecord == null)
        //     {
        //         return new ApiResponseDTO<int>
        //         {
        //             IsSuccess = false,
        //             Message = "Asset Transfer Issue not found."
        //         };
        //     }
        //     // 🔹 Get system details
        //     string currentIp = _ipAddressService.GetSystemIPAddress();
        //     int userId = _ipAddressService.GetUserId();
        //     string username = _ipAddressService.GetUserName();
        //     var systemTimeZoneId = _timeZoneService.GetSystemTimeZone();
        //     var currentTime = _timeZoneService.GetCurrentTime(systemTimeZoneId);

        //     // 🔹 Update Entity
        //     _mapper.Map(request.AssetTransferIssueHdrDto, existingRecord);
        //     existingRecord.ModifiedIP = currentIp;
        //     existingRecord.ModifiedDate = currentTime;
        //     existingRecord.ModifiedBy = userId;
        //     existingRecord.ModifiedByName = username;

        //     // 🔹 Save Changes
        //     var result = await _assetTransferCommandRepository.UpdateAssetTransferAsync(existingRecord);

        //     // 🔹 Publish Domain Event
        //     var domainEvent = new AuditLogsDomainEvent(
        //         actionDetail: "Update",
        //         actionCode: existingRecord.Id.ToString(),
        //         actionName: "Asset Transfer",
        //         details: $"Asset Transfer '{existingRecord.Id}' was updated.",
        //         module: "Asset Transfer"
        //     );

        //     await _mediator.Publish(domainEvent, cancellationToken);

        //     // 🔹 Return Response
        //     if (result > 0)
        //     {
        //         return new ApiResponseDTO<int>
        //         {
        //             IsSuccess = true,
        //             Message = "Asset Transfer updated successfully",
        //             Data = result
        //         };
        //     }
        //     return new ApiResponseDTO<int>
        //     {
        //         IsSuccess = false,
        //         Message = "Asset Transfer update failed"
        //     };
        // }
    }
}