using AutoMapper;
using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetMasterGeneral;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneralById
{
    public class GetAssetMasterGeneralByIdQueryHandler : IRequestHandler<GetAssetMasterGeneralByIdQuery, ApiResponseDTO<AssetMasterGeneralDTO>>
    {
        private readonly IAssetMasterGeneralQueryRepository _assetMasterRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 

        public GetAssetMasterGeneralByIdQueryHandler(IAssetMasterGeneralQueryRepository assetMasterRepository,  IMapper mapper, IMediator mediator)
        {
            _assetMasterRepository =assetMasterRepository;
            _mapper =mapper;
            _mediator = mediator;
        }
        public async Task<ApiResponseDTO<AssetMasterGeneralDTO>> Handle(GetAssetMasterGeneralByIdQuery request, CancellationToken cancellationToken)
        {
            var assetMaster = await _assetMasterRepository.GetByIdAsync(request.Id);
            string logoBase64 = null;
             if (!string.IsNullOrEmpty(assetMaster.AssetImage) && File.Exists(assetMaster.AssetImage))
             {
                 byte[] imageBytes = await File.ReadAllBytesAsync(assetMaster.AssetImage);
                 logoBase64 = Convert.ToBase64String(imageBytes);
             }

            var assetMasterDto = _mapper.Map<AssetMasterGeneralDTO>(assetMaster);
            assetMasterDto.AssetImageBase64 = logoBase64;

            if (assetMaster is null)
            {                
                return new ApiResponseDTO<AssetMasterGeneralDTO>
                {
                    IsSuccess = false,
                    Message = "AssetName with ID {request.Id} not found."
                };   
            }       
            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetById",
                actionCode: assetMasterDto.AssetCode ?? string.Empty,        
                actionName: assetMasterDto.AssetName ?? string.Empty,                
                details: $"Asset '{assetMasterDto.AssetName}' was created. Code: {assetMasterDto.AssetCode}",
                module:"AssetMasterGeneral"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<AssetMasterGeneralDTO>
            {
                IsSuccess = true,
                Message = "Success",
                Data = assetMasterDto
            };       
        }      
    }
}