using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IAssetTransferReceipt;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetMaster.AssetTransferReceipt.Command.CreateAssetTransferReceipt
{
    public class CreateAssetTransferReceiptCommandHandler : IRequestHandler<CreateAssetTransferReceiptCommand, ApiResponseDTO<int>>
    {
        private readonly IAssetTransferReceiptCommandRepository _iassettransferreceiptcommandrepository;
        private readonly IMediator _imediator;
        private readonly IMapper _imapper;
        private readonly IIPAddressService _ipAddressService;
        private readonly ITimeZoneService _timeZoneService;

        
        public CreateAssetTransferReceiptCommandHandler(IAssetTransferReceiptCommandRepository assetTransferReceiptCommandRepository, IMediator imediator, IMapper imapper, IIPAddressService ipAddressService, ITimeZoneService timeZoneService)
        {
            _iassettransferreceiptcommandrepository=assetTransferReceiptCommandRepository;
            _imediator=imediator;
            _imapper=imapper;
            _ipAddressService=ipAddressService;
            _timeZoneService=timeZoneService;

        }
        public async Task<ApiResponseDTO<int>> Handle(CreateAssetTransferReceiptCommand request, CancellationToken cancellationToken)
        {
            string currentIp = _ipAddressService.GetSystemIPAddress();
            int userId = _ipAddressService.GetUserId();
            string username = _ipAddressService.GetUserName();
            var systemTimeZoneId = _timeZoneService.GetSystemTimeZone();
            var currentTime = _timeZoneService.GetCurrentTime(systemTimeZoneId);
            // :small_blue_diamond: Map Command to Entity
             var assetTransferReceiptHdr = _imapper.Map<Core.Domain.Entities.AssetMaster.AssetTransferReceiptHdr>(request.AssetTransferReceiptHdrDto);
             assetTransferReceiptHdr.AuthorizedIP = currentIp;
             assetTransferReceiptHdr.AuthorizedDate = currentTime;
             assetTransferReceiptHdr.AuthorizedBy = userId;
             assetTransferReceiptHdr.AuthorizedByName = username;
              var result =  await _iassettransferreceiptcommandrepository.CreateAsync(assetTransferReceiptHdr);
              //Domain Event
                  var domainEvent = new AuditLogsDomainEvent(
                      actionDetail: "Create",
                      actionCode: assetTransferReceiptHdr.Id.ToString(),
                      actionName: "Asset Transfer Receipt",
                      details: $"Asset Transfer Receipt '{assetTransferReceiptHdr.Id}' was created. ",
                      module:"Asset Transfer"
                  );
                  await _imediator.Publish(domainEvent, cancellationToken);
                  if (result > 0)
                  {
                        return new ApiResponseDTO<int>
                       {
                           IsSuccess = true,
                           Message = "Asset Transfer Receipt created successfully",
                           Data = result
                      };
                 }
                 return new ApiResponseDTO<int>
                  {
                      IsSuccess = false,
                      Message = "Asset Transfer Receipt not created"
                  };
        }
    }
}