using AutoMapper;
using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetMasterGeneral;
using Core.Domain.Entities;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetMaster.AssetMasterGeneral.Commands.CreateAssetMasterGeneral
{
    public class CreateAssetMasterGeneralCommandHandler : IRequestHandler<CreateAssetMasterGeneralCommand, ApiResponseDTO<AssetMasterGeneralDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IAssetMasterGeneralCommandRepository _assetMasterGeneralRepository;
        private readonly IMediator _mediator;

        public CreateAssetMasterGeneralCommandHandler(IMapper mapper, IAssetMasterGeneralCommandRepository assetMasterGeneralRepository, IMediator mediator)
        {
            _mapper = mapper;
            _assetMasterGeneralRepository = assetMasterGeneralRepository;
            _mediator = mediator;    
        } 

        public async Task<ApiResponseDTO<AssetMasterGeneralDTO>> Handle(CreateAssetMasterGeneralCommand request, CancellationToken cancellationToken)
        {           

            // Fetch Company Name from DTO or Database
            var UnitName = request.UnitName;
            var assetGroupName = await _assetMasterGeneralRepository.GetAssetGroupNameById(request.AssetGroupId);
            var assetCategoryName = await _assetMasterGeneralRepository.GetAssetCategoryNameById(request.AssetSubCategoryId);

            if (string.IsNullOrWhiteSpace(UnitName) || string.IsNullOrWhiteSpace(assetGroupName) || string.IsNullOrWhiteSpace(assetCategoryName))
            {
                return new ApiResponseDTO<AssetMasterGeneralDTO>
                {
                    IsSuccess = false,
                    Message = "Invalid data: Company, Asset Group, or Asset SubCategory is missing."
                };
            }

            // Get latest AssetCode
            var latestAssetCode = await _assetMasterGeneralRepository.GetLatestAssetCode(request.CompanyId,request.UnitId, request.AssetGroupId, request.AssetSubCategoryId);

            // Extract sequence number
             int sequence = 1;
            if (!string.IsNullOrEmpty(latestAssetCode))
            {
                var parts = latestAssetCode.Split('/');
                if (parts.Length == 4 && int.TryParse(parts[3], out int lastSeq))
                {
                    sequence = lastSeq + 1;
                }
            }

            // Generate Asset Code
            var assetCode = $"{UnitName}-{assetGroupName}-{assetCategoryName}-{sequence}";

            var assetEntity = _mapper.Map<AssetMasterGenerals>(request);     
            assetEntity.AssetCode = assetCode; // Assign generated AssetCode       
            var result = await _assetMasterGeneralRepository.CreateAsync(assetEntity, cancellationToken);
            
            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Create",
                actionCode: assetEntity.AssetCode ?? string.Empty,
                actionName: assetEntity.AssetName ?? string.Empty,
                details: $"AssetMasterGeneral '{assetEntity.AssetName}' was created. Code: {assetEntity.AssetCode}",
                module:"AssetMasterGeneral"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            
            var assetMasterDTO = _mapper.Map<AssetMasterGeneralDTO>(result);
            if (assetMasterDTO.Id > 0)
            {
                return new ApiResponseDTO<AssetMasterGeneralDTO>{
                    IsSuccess = true, 
                    Message = "AssetMasterGeneral created successfully.",
                    Data = assetMasterDTO
                };
            }
            return  new ApiResponseDTO<AssetMasterGeneralDTO>{
                IsSuccess = false, 
                Message = "AssetMasterGeneral not created."
            };      
        }
    }
}