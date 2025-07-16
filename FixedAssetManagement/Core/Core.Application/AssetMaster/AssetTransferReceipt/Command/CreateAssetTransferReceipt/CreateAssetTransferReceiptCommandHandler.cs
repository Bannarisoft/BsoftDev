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
        private readonly IAssetTransferReceiptQueryRepository _iassettransferreceiptqueryrepository;
        private readonly IMediator _imediator;
        private readonly IMapper _imapper;
        private readonly IIPAddressService _ipAddressService;
        private readonly ITimeZoneService _timeZoneService;

        
        public CreateAssetTransferReceiptCommandHandler(IAssetTransferReceiptCommandRepository assetTransferReceiptCommandRepository, IMediator imediator, IMapper imapper, IIPAddressService ipAddressService, ITimeZoneService timeZoneService,IAssetTransferReceiptQueryRepository iassettransferreceiptqueryrepository)
        {
            _iassettransferreceiptcommandrepository=assetTransferReceiptCommandRepository;
            _imediator=imediator;
            _imapper=imapper;
            _ipAddressService=ipAddressService;
            _timeZoneService=timeZoneService;
            _iassettransferreceiptqueryrepository=iassettransferreceiptqueryrepository;

        }
        public async Task<ApiResponseDTO<int>> Handle(CreateAssetTransferReceiptCommand request, CancellationToken cancellationToken)
        {

            //Asset Location Mapping
            var ToLocationUpdate = await _iassettransferreceiptqueryrepository
                .GetByAssetTransferId(request.AssetTransferReceiptHdrDto.AssetTransferId);

              var ToCustodianId = ToLocationUpdate.ToCustodianId;
              var ToUnitId = ToLocationUpdate.ToUnitId;
              var ToDepartmentId = ToLocationUpdate.ToDepartmentId;

                     var assetLocation = request.AssetTransferReceiptHdrDto.AssetTransferReceiptDtl.Select(dto => {
                         var entity = _imapper.Map<Core.Domain.Entities.AssetMaster.AssetLocation>(dto);
                         entity.CustodianId = ToCustodianId;
                         entity.UnitId = ToUnitId;
                         entity.DepartmentId = ToDepartmentId;  
                         return entity;
                     }).ToList();

            // Fetching Current User
            string currentIp = _ipAddressService.GetSystemIPAddress();
            int userId = _ipAddressService.GetUserId();
            string username = _ipAddressService.GetUserName();
            var systemTimeZoneId = _timeZoneService.GetSystemTimeZone();
            var currentTime = _timeZoneService.GetCurrentTime(systemTimeZoneId);
            //Map the Command to Entity
             var assetTransferReceiptHdr = _imapper.Map<Core.Domain.Entities.AssetMaster.AssetTransferReceiptHdr>(request.AssetTransferReceiptHdrDto);
            // var assetTransferissueHdr = _imapper.Map<Core.Domain.Entities.AssetMaster.AssetTransferIssueHdr>(request.AssetTransferReceiptHdrDto.AssetTransferIssueHdr);
             
            
             assetTransferReceiptHdr.AuthorizedIP = currentIp;
             assetTransferReceiptHdr.AuthorizedDate = currentTime;
             assetTransferReceiptHdr.AuthorizedBy = userId;
             assetTransferReceiptHdr.AuthorizedByName = username;
             // Fetch existing receipt to check if it's an update or create operation
            var existingReceipt = await _iassettransferreceiptqueryrepository
                .GetByAssetReceiptId(assetTransferReceiptHdr.AssetTransferId);
            var result =  await _iassettransferreceiptcommandrepository.CreateAsync(assetTransferReceiptHdr,assetLocation);
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
                            Message = existingReceipt is null 
                                ? "Asset Transfer Receipt created successfully" 
                                : "Asset Transfer Receipt updated successfully",
                            Data = result
                        };
                                }
                 return new ApiResponseDTO<int>
                  {
                      IsSuccess = false,
                      Message = "Asset Transfer Receipt could not be processed"
                  };
        }
    }
}