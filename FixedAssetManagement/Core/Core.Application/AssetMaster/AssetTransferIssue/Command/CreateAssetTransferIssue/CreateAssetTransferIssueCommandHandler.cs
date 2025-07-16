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

namespace Core.Application.AssetMaster.AssetTransferIssue.Command.CreateAssetTransferIssue
{
    public class CreateAssetTransferIssueCommandHandler : IRequestHandler<CreateAssetTransferIssueCommand,ApiResponseDTO<int>>
    {
       private readonly  IAssetTransferCommandRepository _assetTransferCommandRepository;
        private readonly IMapper _mapper;  
        private readonly IMediator _Imediator;
         private readonly IIPAddressService _ipAddressService;
        private readonly ITimeZoneService _timeZoneService; 
        private readonly IValidator<CreateAssetTransferIssueCommand> _validator; 
      
    
        public CreateAssetTransferIssueCommandHandler(IAssetTransferCommandRepository assetTransferCommandRepository , IMapper mapper, IMediator Imediator, IIPAddressService ipAddressService, ITimeZoneService timeZoneService, IValidator<CreateAssetTransferIssueCommand> validator)
        {
            _assetTransferCommandRepository = assetTransferCommandRepository;
            _mapper = mapper;      
            _Imediator = Imediator;      
            _ipAddressService = ipAddressService;
            _timeZoneService = timeZoneService;
            _validator = validator;       

        }
     public async Task<ApiResponseDTO<int>> Handle(CreateAssetTransferIssueCommand request, CancellationToken cancellationToken)
        {
            // 🔹 Validate the request
                var validationResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                {
                    return new ApiResponseDTO<int>
                    {
                        IsSuccess = false,
                        Message = "Validation failed",
                        Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList()
                    };
                }
                
            string currentIp = _ipAddressService.GetSystemIPAddress();
            int userId = _ipAddressService.GetUserId(); 
            string username = _ipAddressService.GetUserName();
            var systemTimeZoneId = _timeZoneService.GetSystemTimeZone();
            var currentTime = _timeZoneService.GetCurrentTime(systemTimeZoneId); 
            // 🔹 Map Command to Entity
             var assetTransferIssueHdr = _mapper.Map<Core.Domain.Entities.AssetMaster.AssetTransferIssueHdr>(request.AssetTransferIssueHdrDto); 
             assetTransferIssueHdr.CreatedIP = currentIp;
             assetTransferIssueHdr.CreatedDate = currentTime;
             assetTransferIssueHdr.CreatedBy = userId;
             assetTransferIssueHdr.CreatedByName = username;
              var result =  await _assetTransferCommandRepository.CreateAssetTransferAsync(assetTransferIssueHdr);

            

              //Domain Event
                  var domainEvent = new AuditLogsDomainEvent(
                      actionDetail: "Create",
                      actionCode: assetTransferIssueHdr.Id.ToString(),
                      actionName: "Asset Transfer",
                      details: $"Asset Transfer '{assetTransferIssueHdr.Id}' was created. ",
                      module:"Asset Transfer"
                  );     

                  await _Imediator.Publish(domainEvent, cancellationToken);
                  if (result > 0)
                  {
                     
                        return new ApiResponseDTO<int>
                       {
                           IsSuccess = true,
                           Message = "Asset Transfer created successfully",
                           Data = result
                      };
                 }
                 return new ApiResponseDTO<int>
                  {
                      IsSuccess = false,
                      Message = "Asset Transfer not created",
                      Data = result
                  };
           
        }
        
    }
}