using AutoMapper;
using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetMasterGeneral;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneralById
{
    public class GetAssetMasterGeneralByIdQueryHandler : IRequestHandler<GetAssetMasterGeneralByIdQuery, ApiResponseDTO<AssetMasterDTO>>
    {
        private readonly IAssetMasterGeneralQueryRepository _assetMasterRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 
        private readonly IAssetDetailsQueryRepository _assetDetailsRepository;

        public GetAssetMasterGeneralByIdQueryHandler(IAssetMasterGeneralQueryRepository assetMasterRepository,  IMapper mapper, IMediator mediator, IAssetDetailsQueryRepository assetDetailsRepository)
        {
            _assetMasterRepository =assetMasterRepository;
            _mapper =mapper;
            _mediator = mediator;
            _assetDetailsRepository = assetDetailsRepository;
        }
        public async Task<ApiResponseDTO<AssetMasterDTO>> Handle(GetAssetMasterGeneralByIdQuery request, CancellationToken cancellationToken)
        {
          //  var assetMaster = await _assetMasterRepository.GetByIdAsync(request.Id);
          var (assetResult, locationResult, purchaseDetails, spec, warranty, amc, disposal, insurance) = await _assetDetailsRepository.GetAssetMasterByIdAsync(request.Id);
          var asset = _mapper.Map<AssetMasterDTO>(assetResult);

            if (assetResult?.AssetName != null)
             {
                 asset.AssetParent = _mapper.Map<AssetParentDTO>(assetResult);
             }

             if (locationResult != null)
             {
                 asset.AssetLocation = _mapper.Map<AssetLocationDTO>(locationResult);
             }
             if (purchaseDetails != null)
             {
             asset.AssetPurchaseDetails = _mapper.Map<List<AssetPurchaseDetailDTO>>(purchaseDetails);
             }
            if (spec != null)
             {
                 asset.AssetSpecification = _mapper.Map<List<AssetSpecDTO>>(spec);
             }
             if (warranty != null)
             {
                 asset.AssetWarranty = _mapper.Map<List<AssetWarrantyDTOById>>(warranty);
             }
             if (amc != null)
             {
                 asset.AssetAmc = _mapper.Map<List<AssetAMCDTOById>>(amc);
             }
             if (disposal != null)
             {
                 asset.AssetDisposal = _mapper.Map<AssetDisposalByIdDTO>(disposal);
             }
             if (insurance != null)
             {
                 asset.AssetInsurance = _mapper.Map<List<AssetInsuranceByIdDTO>>(insurance);
             }
             

            // string logoBase64 = null;
            //  if (!string.IsNullOrEmpty(assetMaster.AssetImage) && File.Exists(assetMaster.AssetImage))
            //  {
            //      byte[] imageBytes = await File.ReadAllBytesAsync(assetMaster.AssetImage);
            //      logoBase64 = Convert.ToBase64String(imageBytes);
            //  }

            // var assetMasterDto = _mapper.Map<AssetMasterGeneralDTO>(assetMaster);
            // assetMasterDto.AssetImageBase64 = logoBase64;

            if (asset is null)
            {                
                return new ApiResponseDTO<AssetMasterDTO>
                {
                    IsSuccess = false,
                    Message = "AssetName with ID {request.Id} not found."
                };   
            }       
            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetById",
                actionCode:"",        
                actionName: "",                
                details: $"Asset ",
                module:"AssetMasterGeneral"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<AssetMasterDTO>
            {
                IsSuccess = true,
                Message = "Success",
                Data = asset
            };       
        }      
    }
}